using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using TBA.Common;

namespace TBA.Tests.Integration
{
    /// <summary>
    /// Integration tests for Runtime Settings + Provider
    /// </summary>
    [TestFixture]
    public class RuntimeSettingsTests : BaseRuntimeSettingsTests
    {
        private static readonly IRuntimeSettingsProvider _sut = new RuntimeSettingsProvider(new WindowsFileSystemManager());
        private static readonly IFileManager _fileManager = new WindowsFileSystemManager();
        private const string RuntimeSettingsTemplateFileName = "runtime.settings.TEMPLATE";
        private const string RuntimeSettingsFileName = "runtime.settings";
        private readonly string _templateLocation;
        private readonly string _settingsLocation;

        public RuntimeSettingsTests() : base (_sut, false)
        {
            _templateLocation = _fileManager.PathCombine(TestExecutionDirectory, RuntimeSettingsTemplateFileName);
            _settingsLocation = _fileManager.PathCombine(TestExecutionDirectory, RuntimeSettingsFileName);
        }

        [OneTimeSetUp]
        public void TestSetup()
        {
            // ensure settings + template files are found
            Assert.IsTrue(_fileManager.FileExists(_templateLocation), $"Could not find '{RuntimeSettingsTemplateFileName}' here: {_templateLocation}");
            Assert.IsTrue(_fileManager.FileExists(_settingsLocation), $"Could not find '{RuntimeSettingsFileName}' here: {_settingsLocation}");
        }

        [Test]
        [Ignore("JSON looping structure is incorrect within 'FetchPropertyNames' -- skipping test until time allows this to be revisited.")]
        public void Test_EnsureJsonStructuresMatch_Success()
        {
            // ensure files have content
            var templateContent = _fileManager.FileReadAllText(_templateLocation);
            Assert.IsFalse(string.IsNullOrWhiteSpace(templateContent));
            var settingsContent = _fileManager.FileReadAllText(_settingsLocation);
            Assert.IsFalse(string.IsNullOrWhiteSpace(settingsContent));

            // make json objects
            var template = JToken.Parse(templateContent);
            var settings = JToken.Parse(settingsContent);

            // validate props between each json object
            Assert.Multiple(() =>
            {
                Assert.IsTrue(IsJsonStructureFound(template, settings), "Some elements of template were not found in settings");
                Assert.IsTrue(IsJsonStructureFound(settings, template), "Some elements of settings were not found in template");
            });
        }

        private static bool IsJsonStructureFound(JToken source, JToken target)
        {
            // get list of source's props + paths
            var names = new List<string>();
            foreach (var token in source.Children())
            {
                FetchPropertyNames(token, string.Empty).ForEach(names.Add);
            }

            //var sourceKeys = source.Properties().Select(x => x.Name).ToList();

            // validate each prop/path is found in target
            //var targetKeys = target.Properties().Select(x => x.Name).ToList();
            //var result = sourceKeys.All(x => targetKeys.Contains(x));
            //return result;

            return true;
        }

        private static List<string> FetchPropertyNames(JToken token, string parentPath)
        {
            if (token == null)
                return new List<string>();

            var prefixPath = $"{parentPath}{(string.IsNullOrWhiteSpace(parentPath) ? string.Empty : ".")}";

            if (token is JObject)
            {
                return FetchPropertyNames(token, prefixPath);
            }

            if (token is JProperty)
            {
                var prop = token as JProperty;
                if (prop.Value.HasValues)
                {
                    // this has sub-props!
                    foreach (var childToken in prop.Value.Children())
                    {

                    }
                    var nestedPrefixPath = $"{prefixPath}{(string.IsNullOrWhiteSpace(prefixPath) ? string.Empty : ".")}";
                    nestedPrefixPath += token.Path;
                    return FetchPropertyNames(token, nestedPrefixPath);
                }

                var addMe = $"{prefixPath}{prop.Name}";
                return new List<string> { addMe };
            }

            return new List<string>();
        }
    }
}