using Akka.Actor;
using Akka.Configuration;
using Akka.Routing;
using QuoteShared;
using System;

namespace Quote
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var config = ConfigurationFactory.ParseString(@"
                    akka {
                        log-config-on-start = on
                        stdout-loglevel = ERROR
                        loglevel = ERROR
                        actor {
                            provider = ""Akka.Remote.RemoteActorRefProvider, Akka.Remote""

                            debug {
                              receive = on
                              autoreceive = off
                              lifecycle = off
                              event-stream = off
                              unhandled = on
                            }

                            deployment {
                                /localactor {
                                    router = round-robin-pool
                                    nr-of-instances = 5
                                    resizer {
                                        enabled = on
                                        lower-bound = 1
                                        upper-bound = 10
                                    }
                                }
                                /remoteactor {
                                    router = round-robin-pool
                                    nr-of-instances = 5
                                    remote = ""akka.tcp://QuoteRemote@localhost:8080""
                                    resizer {
                                        enabled = on
                                        lower-bound = 1
                                        upper-bound = 10
                                    }
                                }
                            }
                        }
                        remote {
                            dot-netty.tcp {
		                        port = 8090
		                        hostname = localhost
                            }
                        }
                    }
                    ");

            do
            {
                using (var system = ActorSystem.Create("Quote", config))
                {
                    // DIspatcher && Aggregator
                    var local = system.ActorOf(
                        Props.Create<Aggregator<Response>>("Local").WithRouter(FromConfig.Instance), "localactor");
                    var remote =
                        system.ActorOf(Props.Create<Aggregator<Response>>("Remote").WithRouter(FromConfig.Instance),
                            "remoteactor");

                    //var f = local.Ask<AggregatedReply<Response>>(new Request { });

                    //f.ContinueWith(x =>
                    //{
                    //    Console.ForegroundColor = ConsoleColor.Cyan;
                    //    Console.WriteLine("Response Count " + x.Result.Replies.Count + " Hotels: " + x.Result.Replies.SelectMany(y => y.Hotel).Count());
                    //    Console.ResetColor();
                    //    //x.Result.Replies.ForEach(y => Console.WriteLine(y.Hotel));
                    //});

                    //var r = remote.Ask<AggregatedReply<Response>>(new Request { });

                    //r.ContinueWith(x =>
                    //{
                    //    Console.ForegroundColor = ConsoleColor.Cyan;
                    //    Console.WriteLine("Response Count " + x.Result.Replies.Count + " Hotels: " + x.Result.Replies.SelectMany(y => y.Hotel).Count());
                    //    Console.ResetColor();
                    //    //x.Result.Replies.ForEach(y => Console.WriteLine(y.Hotel));
                    //});

                    // Listener
                    var replyLocal = system.ActorOf<ResponseActor<AggregatedReply<Response>>>("replyLocal");
                    var replyRemote = system.ActorOf<ResponseActor<AggregatedReply<Response>>>("replyRemote");

                    remote.Tell(new Request(), replyRemote);

                    local.Tell(new Request(), replyLocal);

                    Console.WriteLine("......");
                    Console.ReadLine();

                    remote.Tell(PoisonPill.Instance);
                    local.Tell(PoisonPill.Instance);
                }
            } while (Console.ReadLine() != "q");
        }
    }
}