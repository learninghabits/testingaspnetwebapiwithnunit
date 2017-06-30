using NUnit.Framework;
using testing_asp_web_api_nunit.Controllers;

namespace testing_with_nunit
{
    [TestFixture]
    public class RootControllerTests
    {
        [Test]
        public void RootController_Get_WillReturnString()
        {
            var controller = new RootController();
            var message = controller.Get();
            Assert.AreEqual("API is ready to receive requests", message);
        }
    }
}
