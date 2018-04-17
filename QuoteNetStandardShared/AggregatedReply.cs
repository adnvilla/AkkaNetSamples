using System.Collections.Generic;

namespace QuoteNetStandardShared
{
    public class AggregatedReply<T>
    {
        public string Name { get; set; }

        public List<T> Replies { get; set; }

        public AggregatedReply(string name, List<T> replies)
        {
            Name = name;
            Replies = replies;
        }
    }
}