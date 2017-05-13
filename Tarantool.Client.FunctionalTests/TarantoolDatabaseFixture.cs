using System;
using System.Diagnostics;

using Xunit;

namespace Tarantool.Client
{
    public class TarantoolDatabaseFixture : IDisposable
    {
        public TarantoolDatabaseFixture()
        {
            Build();
            Up();
        }

        private void Build()
        {
            var process = Process.Start(new ProcessStartInfo { FileName = "docker-compose", Arguments = "build" });
            process.WaitForExit();
            Assert.Equal(0, process.ExitCode);
        }

        private void Up()
        {
            var process = Process.Start(new ProcessStartInfo { FileName = "docker-compose", Arguments = "up -d" });
            process.WaitForExit();
            Assert.Equal(0, process.ExitCode);
        }

        private void Down()
        {
            var process = Process.Start(new ProcessStartInfo { FileName = "docker-compose", Arguments = "down -v" });
            process.WaitForExit();
            Assert.Equal(0, process.ExitCode);
        }

        public void Dispose()
        {
            Down();
        }
    }

    [CollectionDefinition("Tarantool database collection")]
    public class TarantoolDatabaseCollection : ICollectionFixture<TarantoolDatabaseFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}
