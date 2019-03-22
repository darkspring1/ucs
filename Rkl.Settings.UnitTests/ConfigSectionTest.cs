using Xunit;

namespace Rkl.Settings.UnitTests
{
    [Collection(SingletonCollection.Name)]
    public class ConfigSectionTest
    {
        const string TRAIT = "Rkl.Settings.Unit";

        private readonly Settings _settings;

        public ConfigSectionTest(SingletonFixture fixture)
        {
            _settings = fixture.Settings;
        }

        [Fact]
        [Trait("Category", TRAIT)]
        public void EnumTest()
        {
            Assert.Equal(TestEnum.CaseSensetiveValue, _settings.TestEnum.CaseSensetiveValue);
            Assert.Equal(TestEnum.CaseInSensetiveValue, _settings.TestEnum.CaseInSensetiveValue);

            var exceptionFlag = false;
            try
            {
                var unknown = _settings.TestEnum.UnknowValue;
            }
            catch
            {
                exceptionFlag = true;
            }

            Assert.True(exceptionFlag);
        }

        
        [Fact]
        [Trait("Category", TRAIT)]
        public void ArrayTest()
        {
            Assert.Equal("TestItem", _settings.TestArray.StringArray[0]);
            Assert.Equal(10, _settings.TestArray.IntArray[0]);
        }
    }
}
