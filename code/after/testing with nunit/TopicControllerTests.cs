using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Hosting;
using testing_asp_web_api_nunit.Controllers;

namespace testing_with_nunit
{
    [TestFixture]
    public class TopicControllerTests
    {
        [SetUp]
        public void SetUp()
        {

        }

        [Test]
        public void TopicController_Ctor_WhenCreatingAnInstanceOfController_TopicsWillBeInitialized()
        {
            var wasGetTopicsInitialized = false;
            TopicController.GetTopicsCollection = () =>
            {
                wasGetTopicsInitialized = true;
                return new List<Topic>();
            };
            TopicController.Init();
            var controller = new TopicController();
            Assert.IsTrue(wasGetTopicsInitialized);
        }

        [Test]
        public void TopicController_Ctor_WhenCreatingAnInstanceTwice_TopicsWillBeInitializedOnlyOnce()
        {
            var callCount = 0;
            TopicController.GetTopicsCollection = () =>
            {
                callCount++;
                return new List<Topic>();
            };
            TopicController.Init();
            new TopicController();
            new TopicController();
            Assert.AreEqual(1, callCount);
        }

        [Test]
        public void TopicController_Get_WhenTopicsCollectionIsInitializedWith2Topics_WillReturn2Topics()
        {
            TopicController.GetTopicsCollection = () =>
            {
                return new List<Topic>
                {
                    new Topic {topic = "ASP.NET Core", id = 1 },
                    new Topic {topic = "Docker for .NET Developers", id = 2 }
                };
            };
            TopicController.Init();
            var controller = new TopicController();
            SetUpHttpRequestParameters(controller);
            var response = controller.Get();
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            IEnumerable<dynamic> topics;
            Assert.IsTrue(response.TryGetContentValue(out topics));
            var topicsArray = topics.ToArray();
            var expandoDict0 = (IDictionary<string, object>)topicsArray[0];
            Assert.AreEqual("ASP.NET Core", expandoDict0["topic"]);
            Assert.AreEqual(1, expandoDict0["id"]);
            var expandoDict1 = (IDictionary<string, object>)topicsArray[1];
            Assert.AreEqual("Docker for .NET Developers", expandoDict1["topic"]);
            Assert.AreEqual(2, expandoDict1["id"]);
        }

        [Test]
        public void TopicController_Get_WhenTheIdRequestedExists_WillReturnATopic()
        {
            TopicController.GetTopicsCollection = () =>
            {
                return new List<Topic>
                {
                    new Topic {topic = "ASP.NET Core", id = 1 },
                    new Topic {topic = "Docker for .NET Developers", id = 2 }
                };
            };
            TopicController.Init();
            var controller = new TopicController();
            SetUpHttpRequestParameters(controller);
            var response = controller.Get(1);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Topic topic;
            Assert.IsTrue(response.TryGetContentValue(out topic));
            Assert.AreEqual("ASP.NET Core", topic.topic);
            Assert.AreEqual(1, topic.id);
        }


        [Test]
        public void TopicController_Get_WhenTheIdRequestedDoesNotExists_WillReturnA404()
        {
            TopicController.GetTopicsCollection = () =>
            {
                return new List<Topic>
                {
                    new Topic {topic = "ASP.NET Core", id = 1 },
                    new Topic {topic = "Docker for .NET Developers", id = 2 }
                };
            };
            TopicController.Init();
            var controller = new TopicController();
            SetUpHttpRequestParameters(controller);
            var response = controller.Get(3);
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
            ExpandoObject expando;
            Assert.IsTrue(response.TryGetContentValue(out expando));
            var expandoDict = (IDictionary<string, object>)expando;
            Assert.AreEqual("The topic you requested was not found", expandoDict["message"]);
        }


        [Test]
        public void TopicController_Get_WhenTheIdAndNameRequestedExists_WillReturnATopic()
        {
            TopicController.GetTopicsCollection = () =>
            {
                return new List<Topic>
                {
                    new Topic {topic = "ASP.NET Core", id = 1, tutorials = (new List<Tutorial>
                    {
                        new Tutorial
                        {
                            name = "ASP.NET Core on Ubuntu",
                            type = "video",
                            url = "http://www.learninghabits.co.za/#/topics/ubuntu"
                        }
                    }).ToArray() },
                    new Topic {topic = "Docker for .NET Developers", id = 2 }
                };
            };
            TopicController.Init();
            var controller = new TopicController();
            SetUpHttpRequestParameters(controller);
            var response = controller.Get(1, "ASP.NET Core on Ubuntu");
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            ExpandoObject expando;
            Assert.IsTrue(response.TryGetContentValue(out expando));
            var expandoDict = (IDictionary<string, object>)expando;
            Assert.AreEqual(1, expandoDict["id"]);
            var tutorials = expandoDict["tutorials"] as List<Tutorial>;
            Assert.IsNotNull(tutorials);
            Assert.AreEqual(1, tutorials.Count);
            Assert.AreEqual("ASP.NET Core on Ubuntu", tutorials[0].name);
        }

        [Test]
        public void TopicController_Get_WhenTheIdOrNameRequestedDoesNotExists_WillReturnA404()
        {
            TopicController.GetTopicsCollection = () =>
            {
                return new List<Topic>
                {
                    new Topic {topic = "ASP.NET Core", id = 1 },
                    new Topic {topic = "Docker for .NET Developers", id = 2 }
                };
            };
            TopicController.Init();
            var controller = new TopicController();
            SetUpHttpRequestParameters(controller);
            var response = controller.Get(3, "Some Random Name");
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
            ExpandoObject expando;
            Assert.IsTrue(response.TryGetContentValue(out expando));
            var expandoDict = (IDictionary<string, object>)expando;
            Assert.AreEqual("The tutorial  you requested was not found", expandoDict["message"]);
        }

        [Test]
        public void TopicController_Post_WhenTheTopicIsAddedSuccessfully_WillReturnANavigationProperty()
        {
            TopicController.GetTopicsCollection = () =>
            {
                return new List<Topic>
                {
                    new Topic {topic = "ASP.NET Core", id = 1 },
                    new Topic {topic = "Docker for .NET Developers", id = 2 }
                };
            };
            TopicController.Init();
            var controller = new TopicController();
            SetUpHttpRequestParameters(controller);
            var response = controller.Post(new Topic
            {
                topic = "Visual Studio on a Mac",
                tutorials = (new List<Tutorial> { }).ToArray()
            });
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            ExpandoObject expando;
            Assert.IsTrue(response.TryGetContentValue(out expando));
            var expandoDict = (IDictionary<string, object>)expando;
            Assert.AreEqual(3, expandoDict["id"]);
            Assert.AreEqual("http://localhost/api/Topic/3", expandoDict["url"]);
        }

        private void SetUpHttpRequestParameters(TopicController controller)
        {
            controller.Request = new HttpRequestMessage(HttpMethod.Post, "http://localhost/api/Topic");
            controller.Request.Properties[HttpPropertyKeys.HttpConfigurationKey] = new HttpConfiguration();
        }
    }
}
