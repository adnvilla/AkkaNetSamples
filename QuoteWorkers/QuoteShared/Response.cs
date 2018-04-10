using System.Collections.Generic;

namespace QuoteShared
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