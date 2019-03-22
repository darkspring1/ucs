using Microsoft.Extensions.Configuration;

namespace Rkl.Settings.UnitTests
{
    public class TestArraySection : ConfigSection
    {
        public TestArraySection(IConfiguration configuration, string keyPrefix) : base(configuration, keyPrefix)
        {
        }

        public string[] StringArray => GetArray(nameof(StringArray));

        public int[] IntArray => GetArray(nameof(IntArray), x => int.Parse(x));        
    }
}
