using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace testing_asp_web_api_nunit.Controllers
{
    public class TopicController : ApiController
    {
        static List<Topic> _topicsCollection;
        static TopicController()
        {
            string filePath = HttpContext.Current.Server.MapPath("~/App_Data/topics.json");
            var topicsFileContent = File.ReadAllText(filePath);
            _topicsCollection = JsonConvert.DeserializeObject<List<Topic>>(topicsFileContent);
        }

        // GET api/topics
        public HttpResponseMessage Get()
        {
            return Request.CreateResponse(HttpStatusCode.OK,
                _topicsCollection.Select(t => new
                {
                    id = t.id,
                    topic = t.topic
                }));
        }

        // GET api/topic/2
        public HttpResponseMessage Get(int id)
        {
            var topic = _topicsCollection.SingleOrDefault(t => t.id == id);           
            if (topic == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, new {
                    message = "The topic you requested was not found"
                });
            }
            return Request.CreateResponse(HttpStatusCode.OK, topic);
        }

        [Route("api/topic/{id}/{name}")]
        // GET api/topic/2
        public HttpResponseMessage Get(int id, string name)
        {
            var tutorials = _topicsCollection.Where(t => t.id == id)
                                     .SelectMany(t => t.tutorials)
                                     .Where(t => t.name == name)
                                     .ToList();

            if (tutorials.Count == 0)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    message = "The tutorial  you requested was not found"
                });
            }
           
            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                id = id,
                tutorials = tutorials
            });
        }

        // POST api/values
        public HttpResponseMessage Post(Topic topic)
        {
            topic.id = _topicsCollection.Last().id + 1;            
            _topicsCollection.Add(topic);
            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                id = topic.id,
                url = Request.RequestUri.AbsoluteUri + "/" + topic.id
            });
        }
    }
}
