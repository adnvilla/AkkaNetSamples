
using Akka.Actor;
using Akka.Configuration;
using Akka.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using QuoteNetStandardShared;

namespace QuoteApi
{
    public class ApiActorSystem
    {
        private static ActorSystem _actorSystem;

        static ApiActorSystem()
        {
            Config = ConfigurationFactory.ParseString(@"
                    akka {
                        log-config-on-start = on
                        stdout-loglevel = DEBUG
                        loglevel = DEBUG
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
        }

        public static Config Config { get; set; }

        private static object _obj = new object();
        public static ActorSystem ActorSystem
        {
            get
            {
                if (_actorSystem == null)
                {
                    lock (_obj)
                    {
                        if (_actorSystem == null)
                        {
                            Initialize();
                        }
                    }
                }

                return _actorSystem;
            }
        }

        private static IActorRef _remote;


        private static object _objr = new object();
        public static IActorRef Remote
        {
            get
            {
                if (_remote == null)
                {
                    lock (_objr)
                    {
                        if (_remote == null)
                        {
                            InitializeRemote();
                        }
                    }
                }

                return _remote;
            }
        }

        private static void InitializeRemote()
        {
            _remote = ApiActorSystem.ActorSystem.ActorOf(Props.Create<Aggregator<Response>>("Remote").WithRouter(FromConfig.Instance), "remoteactor");
        }

        private static void Initialize()
        {
            _actorSystem = ActorSystem.Create("Quote", Config);
        }
    }
}
