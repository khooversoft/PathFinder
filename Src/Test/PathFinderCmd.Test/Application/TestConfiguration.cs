using PathFinderCmd.Application;

namespace PathFinderCmd.Test.Application
{
    public class TestConfiguration
    {
        public const string TestConfigResourceId = "PathFinderCmd.Test.Application.TestConfig.json";

        public TestConfiguration() { }

        public string[] BuildArgs(params string[] args) => TestTools.GetArguments(TestConfigResourceId, args);

        internal IOption GetOption(params string[] args) => new OptionBuilder()
            .SetArgs(BuildArgs(args))
            .Build();
    }
}
