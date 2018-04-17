using System.Collections.Generic;

namespace QuoteNetStandardShared
{
    public class Response
    {
        public Response()
        {
            Hotel = new List<int>();
        }

        public List<int> Hotel { get; set; }
    }
}