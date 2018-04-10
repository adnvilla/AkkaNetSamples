using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QuoteShared
{
    public class Aggregator<T> : ReceiveActor
    {
        private IActorRef originalSender;
        private List<IActorRef> _refs;
        public string Name { get; set; }

        public Aggregator(string name)
        {
            Name = name;
            var worker1 = Context.ActorOf(Props.Create<QuoteWorkerActor>("QuoteWorkerActor" + name, 1), "QuoteWorkerActor" + name + 1);
            var worker2 = Context.ActorOf(Props.Create<QuoteWorkerActor>("QuoteWorkerActor" + name, 2), "QuoteWorkerActor" + name + 2);
            var worker3 = Context.ActorOf(Props.Create<QuoteWorkerActor>("QuoteWorkerActor" + name, 3), "QuoteWorkerActor" + name + 3);
            var worker4 = Context.ActorOf(Props.Create<QuoteWorkerActor>("QuoteWorkerActor" + name, 4), "QuoteWorkerActor" + name + 4);
            var worker5 = Context.ActorOf(Props.Create<QuoteWorkerActor>("QuoteWorkerActor" + name, 5), "QuoteWorkerActor" + name + 5);

            _refs = new List<IActorRef>();
            _refs.Add(worker1);
            _refs.Add(worker2);
            _refs.Add(worker3);
            _refs.Add(worker4);
            _refs.Add(worker5);

            // La operacion finalizara despues de 3 seg de inactividad
            // (mientras no se reciban nuevos mensajes)
            Context.SetReceiveTimeout(TimeSpan.FromSeconds(1));
            Receive<Request>(x =>
            {
                originalSender = Sender;

                // Dispatch
                foreach (var aref in _refs) aref.Tell(x);
                Become(Aggregating);
            });
        }

        private void Aggregating()
        {
            var replies = new List<T>();

            // Cuando ocurra el timeout, respondemos con lo que tengamos hasta el momento
            Receive<ReceiveTimeout>(_ =>
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("TimeOut");
                Console.ResetColor();
                ReplyAndStop(replies);
            });
            Receive<T>(x =>
            {
                if (_refs.Remove(Sender))
                {
                    // Aggregate
                    if (x is Response)
                    {
                        var r = x as Response;

                        if (r.Hotel.Any())
                        {
                            replies.Add(x);
                        }
                    }

                    Sender.Tell(PoisonPill.Instance);
                }
                if (_refs.Count == 0) ReplyAndStop(replies);
            });
        }

        private void ReplyAndStop(List<T> replies)
        {
            originalSender.Tell(new AggregatedReply<T>(Name, replies));
            Context.Stop(Self);
        }
    }
}