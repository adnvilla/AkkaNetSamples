using System;
using System.Collections.Generic;
using System.Threading;
using Akka.Actor;

namespace QuoteShared
{
    public class QuoteWorkerActor : ReceiveActor
    {
        public QuoteWorkerActor(string name, int provider)
        {
            Console.WriteLine(name + provider);

            Receive<Request>(x =>
            {
                var sender = Sender;

                Random r = new Random(DateTime.Now.Millisecond + provider);
                
                if (r.Next(1, DateTime.Now.Millisecond) % 3 != 0)
                {
                    var responseCount = r.Next(0, 100);

                    var response = new Response
                    {
                        Hotel = new List<int>()
                    };

                    for (var count = 1; count <= responseCount; count++)
                    {
                        response.Hotel.Add(r.Next(1000,2000));
                    }

                    var t = r.Next(200, 3000);

                    Console.WriteLine(name + provider + "Wait " + t);

                    Thread.Sleep(t);

                    sender.Tell(response);

                    Console.WriteLine("Response " + name + provider + " count " +responseCount);
                }
                else
                {
                    sender.Tell(new Response());
                    Console.WriteLine("Response " + name + provider + " count " + 0);
                }
            });
        }
    }
}
