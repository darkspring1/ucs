using Microsoft.Extensions.Configuration;

namespace Rkl.Settings.UnitTests
{
    public class TestEnumSection : ConfigSection
    {
        public TestEnumSection(IConfiguration configuration, string keyPrefix) : base(configuration, keyPrefix)
        {
        }

        public TestEnum CaseSensetiveValue => GetEnum<TestEnum>("CaseSensetiveValue", ignoreCase:false);
        public TestEnum CaseInSensetiveValue => GetEnum<TestEnum>("CaseInSensetiveValue", ignoreCase: true);

        public TestEnum UnknowValue => GetEnum<TestEnum>("UnknowValue", ignoreCase: true);
    }
}
