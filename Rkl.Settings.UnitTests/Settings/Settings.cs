using Microsoft.Extensions.Configuration;

namespace Rkl.Settings.UnitTests
{
    public class Settings : ConfigSection
    {
        public Settings(IConfiguration configuration) : base(configuration)
        {
        }

        public TestEnumSection TestEnum => GetSection<TestEnumSection>(nameof(TestEnumSection));

        public TestArraySection TestArray => GetSection<TestArraySection>(nameof(TestArraySection));
    }
}
