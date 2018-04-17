using Akka.Actor;
using Akka.Configuration;
using System;
using System.Collections.Generic;

namespace QuoteNetCoreRemote
{
    class Program
    {
        static void Main(string[] args)
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
                        }
                        remote {
                            dot-netty.tcp {
		                        port = 8080
		                        hostname = localhost
                            }
                        }
                    }
                    ");

            List<int> r = new List<int>();

            using (ActorSystem.Create("QuoteRemote", config))
            {
                Console.ReadLine();
            }
        }
    }
}
