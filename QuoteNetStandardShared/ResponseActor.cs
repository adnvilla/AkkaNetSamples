using Akka.Actor;
using System;
using System.Linq;

namespace QuoteNetStandardShared
{
    public class ResponseActor<T> : ReceiveActor
    {
        public ResponseActor()
        {
            Receive<T>(x =>
            {
                if (x is AggregatedReply<Response>)
                {
                    var r = x as AggregatedReply<Response>;

                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine(r.Name + "Hotels " + r.Replies.SelectMany(y => y.Hotel).Count());
                    Console.WriteLine();
                    Console.ResetColor();
                }
            });
        }
    }
}