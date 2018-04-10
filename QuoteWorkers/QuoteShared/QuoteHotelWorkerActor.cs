using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;

namespace QuoteShared
{
    public class QuoteHotelWorkerActor : ReceiveActor
    {
        private IActorRef originalSender;
        private List<IActorRef> refs;

        public QuoteHotelWorkerActor(List<IActorRef> refs)
        {
            this.refs = refs;

            Console.WriteLine("QuoteHotelWorkerActor");

            Receive<Request>(x =>
            {
                originalSender = Sender;
                if (0 == x.Provider)
                {
                    var top = Context.ActorOf(Props.Create<Aggregator<Response>>(this.refs), "AggregatorAll");

                    Console.WriteLine("Send Workers ");
                    var f = top.Ask<AggregatedReply<Response>>(new Request {Provider = 1});

                    f.ContinueWith(y =>
                    {
                        //Console.WriteLine("Response Count " + y.Result.Replies.Count + " Hotels: " + y.Result.Replies.SelectMany(z => z.Hotel).Count());
                        //x.Result.Replies.ForEach(y => Console.WriteLine(y.Hotel));

                        var response = new Response
                        {
                            Hotel = y.Result.Replies.SelectMany(z => z.Hotel).ToList()
                        };

                        originalSender.Tell(response);
                    });

                    f.Wait();
                }
                else
                {
                    Sender.Tell(new Response());
                }
            });
        }
    }
}
