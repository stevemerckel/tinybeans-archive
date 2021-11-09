using System;
using System.Net.Http;
using System.Threading.Tasks;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using TBA.Common;

namespace TBA.Tests.Integration
{
    /// <summary>
    /// Integration tests for the Tinybeans API
    /// </summary>
    [TestFixture]
    public sealed class TinybeansApiTests : BaseTinybeansApiTests
    {
        private static readonly IFileManager _fileManager;
        private static readonly IRuntimeSettingsProvider _runtimeSettingsProvider;
        private static readonly ITinybeansJsonHelper _jsonHelper;
        private static readonly ITinybeansApiHelper _sut;

        /// <remarks>
        /// Initializes the static objects for later consumption by the default ctor
        /// </remarks>
        static TinybeansApiTests() 
        {
            _fileManager = new WindowsFileSystemManager();
            _runtimeSettingsProvider = new RuntimeSettingsProvider(_fileManager);
            _jsonHelper = new TinybeansJsonHelper(DefaultMocks.MockLogger);
            _sut = new TinybeansApiHelper(DefaultMocks.MockLogger, _runtimeSettingsProvider, _jsonHelper, _fileManager);
        }

        /// <summary>
        /// Default ctor
        /// </summary>
        public TinybeansApiTests() : base(_sut)
        {
        }

        [Test]
        //[GatewayTimeoutInconclusive]
        [RetryOnGatewayTimeout(3)]
        public async Task TestGatewayTimeout()
        {
            const string BaseUrl = "https://localhost:44343/";
            //const string Endpoint = "api/";
            const string Endpoint = "api/gt";

            Console.WriteLine("Hitting URL: " + BaseUrl);
            HttpClient client = null;
            try
            {
                client = new HttpClient
                {
                    BaseAddress = new Uri(BaseUrl)
                };
                Console.WriteLine("Endpoint = " + Endpoint);
                var response = await client.GetAsync(Endpoint);
                Console.WriteLine("response = " + response);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
            }
            finally
            {
                client?.Dispose();
            }

            Console.WriteLine("All done");

            //return true;
        }

        //[TearDown]
        public void TestTeardown()
        {
            if (TestContext.CurrentContext.Result.Outcome == ResultState.Success)
                return; // nothing to do

            // look for potential 504 (gateway timeout) errors, and mark as "inconclusive" instead
            var errorMessage = TestContext.CurrentContext.Result.Message;
            if (errorMessage.Contains("504") 
                || errorMessage.Contains("Gateway Timeout", System.StringComparison.InvariantCultureIgnoreCase))
            {
                Assert.Inconclusive("504 HIT !!!");
            }
        }
    }
}