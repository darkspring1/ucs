using Microsoft.Extensions.Configuration;
using System;
using Xunit;

namespace Rkl.Settings.UnitTests
{

    /// <summary>
    /// Объекты в этой Fixture, буду созданы один раз перед всеми тестами.
    /// </summary>
    public class SingletonFixture : IDisposable
    {
        public Settings Settings { get; }

        public SingletonFixture()
        {
            var config = new ConfigurationBuilder()
              .AddJsonFile("appsettings.integrationTests.json")
              .Build();

            Settings = new Settings(config);
        }

        public void Dispose()
        {
            
        }
    }


    [CollectionDefinition(Name)]
    public class SingletonCollection : ICollectionFixture<SingletonFixture>
    {
        public const string Name = "SingletonСollection";
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}
