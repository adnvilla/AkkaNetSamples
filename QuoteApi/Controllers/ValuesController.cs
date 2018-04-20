using System;
using Akka.Actor;
using Akka.Routing;
using Microsoft.AspNetCore.Mvc;
using QuoteNetStandardShared;
using System.Collections.Generic;
using System.Linq;

namespace QuoteApi.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            var results = new List<string>();

            using (var system = ActorSystem.Create(Guid.NewGuid().ToString(), ApiActorSystem.Config))
            {
                // DIspatcher && Aggregator
                var remote =
                    system.ActorOf(Props.Create<Aggregator<Response>>(Guid.NewGuid().ToString()).WithRouter(FromConfig.Instance),
                        "remoteactor");

                var r = remote.Ask<AggregatedReply<Response>>(new Request { }).Result;

                results.Add("Response Count " + r.Replies.Count + " Hotels: " + r.Replies.SelectMany(y => y.Hotel).Count());
            }


            // DIspatcher && Aggregator
            //var remote =
            //    ApiActorSystem.ActorSystem.ActorOf(Props.Create<Aggregator<Response>>("Remote").WithRouter(FromConfig.Instance),
            //        "remoteactor");

            //var r = remote.Ask<AggregatedReply<Response>>(new Request { });

            //r.ContinueWith(x =>
            //{
            //    results.Add("Response Count " + x.Result.Replies.Count + " Hotels: " +
            //                x.Result.Replies.SelectMany(y => y.Hotel).Count());
            //});


            //TODO
            //    var r = ApiActorSystem.Remote.Ask<AggregatedReply<Response>>(new Request { }).Result;

            //results.Add("Response Count " + r.Replies.Count + " Hotels: " +
            //            r.Replies.SelectMany(y => y.Hotel).Count());

            return results;
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}