﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MsgPack.Serialization;
using Tarantool.Client.Models.ClientMessages;
using Xunit;

namespace Tarantool.Client
{
    public class TarantoolSpace_ReplaceShould
    {
        public class MyTestEntity
        {
            [MessagePackMember(0)]
            public uint MyTestEntityId { get; set; }

            [MessagePackMember(1)]
            public string SomeStringField { get; set; }

            [MessagePackMember(2)]
            public int SomeIntField { get; set; }
        }

        [Fact]
        public async Task ReplaceEntity()
        {
            var tarantoolClient = TarantoolClient.Create("mytestuser:mytestpass@tarantool-host:3301");
            var testSpace = tarantoolClient.GetSpace<MyTestEntity>("test");

            try
            {
                await testSpace.InsertAsync(new MyTestEntity
                {
                    MyTestEntityId = 576,
                    SomeStringField = "Some name",
                    SomeIntField = 1700
                });

                var result = await testSpace.ReplaceAsync(new MyTestEntity
                {
                    MyTestEntityId = 576,
                    SomeStringField = "Some new name",
                    SomeIntField = 1776
                });

                Assert.Equal(1, result.Count);
                Assert.Equal(576u, result[0].MyTestEntityId);
                Assert.Equal("Some new name", result[0].SomeStringField);
                Assert.Equal(1776, result[0].SomeIntField);
            }
            finally
            {
                await testSpace.DeleteAsync(new List<object> { 576u });
            }
        }
    }
}