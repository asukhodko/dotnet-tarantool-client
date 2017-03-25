using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tarantool.Client.Models;
using Tarantool.Client.Models.ClientMessages;
using Xunit;

namespace Tarantool.Client
{
    public class TarantoolClient_RequestShould
    {
        [Fact]
        public async Task EvaluateScalars()
        {
            var tarantoolClient =
                new TarantoolClient("mytestuser:mytestpass@tarantool-host:3301");

            var result = await tarantoolClient.RequestAsync(new EvalRequest
            {
                Expression = "return 12345, 23456, 34567"
            });

            Assert.Equal(new[] { 12345, 23456, 34567 }, result.Select(x => x.AsInt32()));
        }

        [Fact]
        public async Task EvaluateScalarsFromArguments()
        {
            var tarantoolClient =
                new TarantoolClient("mytestuser:mytestpass@tarantool-host:3301");

            var result = await tarantoolClient.RequestAsync(new EvalRequest
            {
                Expression = "return ...",
                Args = new List<object> { 912345, 923456, 934567 }
            });

            Assert.Equal(new[] { 912345, 923456, 934567 }, result.Select(x => x.AsInt32()));
        }

        [Fact]
        public async Task SelectAll()
        {
            var tarantoolClient =
                new TarantoolClient("mytestuser:mytestpass@tarantool-host:3301");
            var testSpaceId = (await tarantoolClient.FindSpaceByNameAsync("test"))[0].AsUInt32();

            var result = await tarantoolClient.RequestAsync(new SelectRequest
            {
                SpaceId = testSpaceId,
                Iterator = Iterator.All
            });

            Assert.True(result.Count >= 3);
            Assert.Equal(new[] { "1", "Roxette" }, result[0].AsList().Select(x => x.ToObject().ToString()).ToArray());
            Assert.Equal(new[] { "2", "Scorpions", "2015" },
                result[1].AsList().Select(x => x.ToObject().ToString()).ToArray());
            Assert.Equal(new[] { "3", "Ace of Base", "1993" },
                result[2].AsList().Select(x => x.ToObject().ToString()).ToArray());
        }


        [Fact]
        public async Task SelectBy1()
        {
            var tarantoolClient =
                new TarantoolClient("mytestuser:mytestpass@tarantool-host:3301");
            var testSpaceId = (await tarantoolClient.FindSpaceByNameAsync("test"))[0].AsUInt32();

            var result = await tarantoolClient.RequestAsync(new SelectRequest
            {
                SpaceId = testSpaceId,
                Key = new List<dynamic> { 1 }
            });

            Assert.Equal(1, result.Count);
            Assert.Equal(new[] { "1", "Roxette" }, result[0].AsList().Select(x => x.ToObject().ToString()).ToArray());
        }

        [Fact]
        public async Task SelectBy3()
        {
            var tarantoolClient =
                new TarantoolClient("mytestuser:mytestpass@tarantool-host:3301");
            var testSpaceId = (await tarantoolClient.FindSpaceByNameAsync("test"))[0].AsUInt32();

            var result = await tarantoolClient.RequestAsync(new SelectRequest
            {
                SpaceId = testSpaceId,
                Key = new List<dynamic> { 3 }
            });

            Assert.Equal(1, result.Count);
            Assert.Equal(new[] { "3", "Ace of Base", "1993" },
                result[0].AsList().Select(x => x.ToObject().ToString()).ToArray());
        }

        [Fact]
        public async Task Insert()
        {
            var tarantoolClient =
                new TarantoolClient("mytestuser:mytestpass@tarantool-host:3301");
            var testSpaceId = (await tarantoolClient.FindSpaceByNameAsync("test"))[0].AsUInt32();
            await tarantoolClient.RequestAsync(new DeletetRequest
            {
                SpaceId = testSpaceId,
                Key = new List<object> { 99 }
            });

            try
            {
                var result = await tarantoolClient.RequestAsync(new InsertRequest
                {
                    SpaceId = testSpaceId,
                    Tuple = new List<object> { 99, "Some name", 1900 }
                });

                Assert.Equal(1, result.Count);
                Assert.Equal(new[] { "99", "Some name", "1900" },
                    result[0].AsList().Select(x => x.ToObject().ToString()).ToArray());
            }
            finally
            {
                await tarantoolClient.RequestAsync(new DeletetRequest
                {
                    SpaceId = testSpaceId,
                    Key = new List<object> { 99 }
                });
            }
        }

        [Fact]
        public async Task Delete()
        {
            var tarantoolClient =
                new TarantoolClient("mytestuser:mytestpass@tarantool-host:3301");
            var testSpaceId = (await tarantoolClient.FindSpaceByNameAsync("test"))[0].AsUInt32();
            await tarantoolClient.RequestAsync(new InsertRequest
            {
                SpaceId = testSpaceId,
                Tuple = new List<object> { 88, "Some name", 1800 }
            });

            var result = await tarantoolClient.RequestAsync(new DeletetRequest
            {
                SpaceId = testSpaceId,
                Key = new List<object> { 88 }
            });

            Assert.Equal(1, result.Count);
            Assert.Equal(new[] { "88", "Some name", "1800" },
                result[0].AsList().Select(x => x.ToObject().ToString()).ToArray());
        }

        [Fact]
        public async Task Replace()
        {
            var tarantoolClient =
                new TarantoolClient("mytestuser:mytestpass@tarantool-host:3301");
            var testSpaceId = (await tarantoolClient.FindSpaceByNameAsync("test"))[0].AsUInt32();

            try
            {
                await tarantoolClient.RequestAsync(new InsertRequest
                {
                    SpaceId = testSpaceId,
                    Tuple = new List<object> { 77, "Some name", 1700 }
                });

                var result = await tarantoolClient.RequestAsync(new ReplaceRequest
                {
                    SpaceId = testSpaceId,
                    Tuple = new List<object> { 77, "Some new name", 1777 }
                });

                Assert.Equal(1, result.Count);
                Assert.Equal(new[] { "77", "Some new name", "1777" },
                    result[0].AsList().Select(x => x.ToObject().ToString()).ToArray());
            }
            finally
            {
                await tarantoolClient.RequestAsync(new DeletetRequest
                {
                    SpaceId = testSpaceId,
                    Key = new List<object> { 77 }
                });
            }
        }

        [Fact]
        public async Task CallSomeFunction()
        {
            var tarantoolClient =
                new TarantoolClient("mytestuser:mytestpass@tarantool-host:3301");

            var result = await tarantoolClient.RequestAsync(new CallRequest
            {
                FunctionName = "some_function"
            });

            Assert.True(result.Count == 1);
            Assert.Equal("ok", result[0].AsString());
        }

        [Fact]
        public async Task Update()
        {
            var tarantoolClient =
                new TarantoolClient("mytestuser:mytestpass@tarantool-host:3301");
            var testSpaceId = (await tarantoolClient.FindSpaceByNameAsync("test"))[0].AsUInt32();

            try
            {
                await tarantoolClient.RequestAsync(new InsertRequest
                {
                    SpaceId = testSpaceId,
                    Tuple = new List<object> { 66, "Some name", 1600 }
                });

                var result = await tarantoolClient.RequestAsync(new UpdateRequest
                {
                    SpaceId = testSpaceId,
                    Key = new List<object>{66},
                    UpdateUperations = new []
                    {
                        new UpdateOperation<int>
                        {
                            Operation = UpdateOperationCode.Assign,
                            FieldNo = 2,
                            Argument = 1666
                        } 
                    }
                });

                Assert.Equal(1, result.Count);
                Assert.Equal(new[] { "66", "Some name", "1666" },
                    result[0].AsList().Select(x => x.ToObject().ToString()).ToArray());
            }
            finally
            {
                await tarantoolClient.RequestAsync(new DeletetRequest
                {
                    SpaceId = testSpaceId,
                    Key = new List<object> { 66 }
                });
            }
        }

        [Fact]
        public async Task UpsertAsInsert()
        {
            var tarantoolClient =
                new TarantoolClient("mytestuser:mytestpass@tarantool-host:3301");
            var testSpaceId = (await tarantoolClient.FindSpaceByNameAsync("test"))[0].AsUInt32();

            try
            {
                await tarantoolClient.RequestAsync(new UpsertRequest
                {
                    SpaceId = testSpaceId,
                    Tuple = new List<object>{ 55, "Some name", 1550 },
                    UpdateUperations = new []
                    {
                        new UpdateOperation<int>
                        {
                            Operation = UpdateOperationCode.Assign,
                            FieldNo = 2,
                            Argument = 1555
                        } 
                    }
                });

                var result = await tarantoolClient.RequestAsync(new SelectRequest
                {
                    SpaceId = testSpaceId,
                    Key = new List<object> { 55 }
                });
                Assert.Equal(1, result.Count);
                Assert.Equal(new[] { "55", "Some name", "1550" },
                    result[0].AsList().Select(x => x.ToObject().ToString()).ToArray());
            }
            finally
            {
                await tarantoolClient.RequestAsync(new DeletetRequest
                {
                    SpaceId = testSpaceId,
                    Key = new List<object> { 55 }
                });
            }
        }

        [Fact]
        public async Task UpsertAsUpdate()
        {
            var tarantoolClient =
                new TarantoolClient("mytestuser:mytestpass@tarantool-host:3301");
            var testSpaceId = (await tarantoolClient.FindSpaceByNameAsync("test"))[0].AsUInt32();

            try
            {
                await tarantoolClient.RequestAsync(new InsertRequest
                {
                    SpaceId = testSpaceId,
                    Tuple = new List<object> { 44, "Some name", 1400 }
                });

                await tarantoolClient.RequestAsync(new UpsertRequest
                {
                    SpaceId = testSpaceId,
                    Tuple = new List<object> { 44, "Some name", 1440 },
                    UpdateUperations = new[]
                    {
                        new UpdateOperation<int>
                        {
                            Operation = UpdateOperationCode.Assign,
                            FieldNo = 2,
                            Argument = 1444
                        }
                    }
                });

                var result = await tarantoolClient.RequestAsync(new SelectRequest
                {
                    SpaceId = testSpaceId,
                    Key = new List<object> { 44 }
                });
                Assert.Equal(1, result.Count);
                Assert.Equal(new[] { "44", "Some name", "1444" },
                    result[0].AsList().Select(x => x.ToObject().ToString()).ToArray());
            }
            finally
            {
                await tarantoolClient.RequestAsync(new DeletetRequest
                {
                    SpaceId = testSpaceId,
                    Key = new List<object> { 44 }
                });
            }
        }

    }
}