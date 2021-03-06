Tarantool .NET client
=====================

.NET client (connector/driver) for Tarantool database (https://tarantool.org/) with ORM support.
Works both with .NET Core and .NET Framework.

[Get dotnet TarantoolClient package on NuGet](https://www.nuget.org/packages/TarantoolClient)

```powershell
PM> Install-Package TarantoolClient
```
[![Latest stable](https://img.shields.io/nuget/v/TarantoolClient.svg)](https://www.nuget.org/packages/TarantoolClient)

Working with Tarantool spaces like collections
----------------------------------------------

The examples assume to these usings:
```C#
using Tarantool.Client;
using Tarantool.Client.Models;
using MsgPack.Serialization;
```
Create TarantoolClient instance:
```C#
var tarantoolClient = TarantoolClient.Create("tarantool://user:pass@tarantool-host:3301");
```

### Defining the model
```C#
public class MyTestEntity
{
    [MessagePackMember(0)]
    public uint MyTestEntityId { get; set; }

    [MessagePackMember(1)]
    public string SomeStringField { get; set; }

    [MessagePackMember(2)]
    public int SomeIntField { get; set; }

    [MessagePackMember(3)]
    [MessagePackDateTimeMember(DateTimeConversionMethod = DateTimeMemberConversionMethod.UnixEpoc)]
    public DateTime SomeDateTimeField { get; set; }
}
```
### Map space and indexes with model
```C#
var testSpace = tarantoolClient.GetSpace<MyTestEntity>("test");
var testSpacePrimaryIndex = testSpace.GetIndex<IndexKey<uint>>("primary");
```

### Select by key
```C#
var result = await testSpacePrimaryIndex.SelectAsync(new IndexKey<uint>(3)); // where primary indexed key = 3
```

### Select from secondary index by key
```C#
var testSpaceSecondaryIndex = testSpace.GetIndex<IndexKey<string>>("secondaryIndexName");
var result = await testSpaceSecondaryIndex.SelectAsync(new IndexKey<string>("some value"));
```

### Insert
```C#
await testSpace.InsertAsync(new MyTestEntity
{
    MyTestEntityId = 198,
    SomeStringField = "Some name",
    SomeIntField = 1900
});
```

### Update
```C#
await testSpacePrimaryIndex.UpdateAsync(
    new IndexKey<uint>(166), // key
    new UpdateDefinition<MyTestEntity>()
    .AddOpertation(
        x => x.SomeIntField,
        UpdateOperationCode.Assign,
        1666));
```

### Delete
```C#
await testSpacePrimaryIndex.DeleteAsync(new IndexKey<uint>(166));
```

### Replace
```C#
await testSpace.ReplaceAsync(new MyTestEntity
{
    MyTestEntityId = 176,
    SomeStringField = "Some new name",
    SomeIntField = 1776
});
```

### Upsert
```C#
await testSpace.UpsertAsync(
    new MyTestEntity
    {
        MyTestEntityId = 544,
        SomeStringField = "Some name",
        SomeIntField = 1440
    },
    new UpdateDefinition<MyTestEntity>()
    .AddOpertation(
        x => x.SomeIntField,
        UpdateOperationCode.Assign,
        1444));
```

Core Tarantool operations
-------------------------
The examples assume to these usings:
```C#
using Tarantool.Client;
using Tarantool.Client.Models;
using Tarantool.Client.Models.ClientMessages;
```
Create TarantoolClient instance:
```C#
var tarantoolClient = TarantoolClient.Create("tarantool://user:pass@tarantool-host:3301");
```

### Selecting from space by key
```C#
var spaceId = (await tarantoolClient.FindSpaceByNameAsync("testspace")).SpaceId;
var rows = await tarantoolClient.SelectAsync(new SelectRequest
{
    SpaceId = spaceId,
    Key = new object[] { 3 } // find row where Id = 3
});
```
or with object mapping to custom type:
```C#
var rows = await tarantoolClient.SelectAsync<MyTestEntity>(new SelectRequest
{
    SpaceId = spaceId,
    Key = new object[] { 3 } // find row where Id = 3
});
```

### Selecting all rows from space
```C#
var spaceId = (await tarantoolClient.FindSpaceByNameAsync("testspace")).SpaceId;
var rows = await tarantoolClient.SelectAsync<MyEntity>(new SelectRequest
{
    SpaceId = spaceId,
    Iterator = Iterator.All
});
```

### Selecting by secondary index
```C#
var spaceId = (await tarantoolClient.FindSpaceByNameAsync("testspace")).SpaceId;
var indexId = (await tarantoolClient.FindIndexByNameAsync(spaceId, "indexname")).IndexId;
var rows = await tarantoolClient.SelectAsync(new SelectRequest
{
    SpaceId = spaceId,
    IndexId = indexId,
    Key = new object[] { "some data" } // find rows where indexed field = "some data"
});
```

### Inserting data
```C#
var spaceId = (await tarantoolClient.FindSpaceByNameAsync("testspace")).SpaceId;
await tarantoolClient.InsertAsync(new InsertRequest
{
    SpaceId = spaceId,
    Tuple = new object[] { 99, "Some string", 1900 }
});
```
or with object mapping to custom type:
```C#
await tarantoolClient.InsertAsync(new InsertRequest<MyTestEntity>
{
    SpaceId = testSpaceId,
    Tuple = new MyTestEntity
    {
        MyTestEntityId = 99,
        SomeStringField = "Some name",
        SomeIntField = 1900
    }
});
```

### Updating data
```C#
var spaceId = (await tarantoolClient.FindSpaceByNameAsync("testspace")).SpaceId;
await tarantoolClient.UpdateAsync(new UpdateRequest
{
    SpaceId = spaceId,
    Key = new object[] { 66 }, // update row where Id = 66
    UpdateOperations = new []
    {
        new UpdateOperation<int>
        {
            Operation = UpdateOperationCode.Assign,
            FieldNo = 2, // updating field with position=2 in tuple
            Argument = 1666 // new value
        } 
    }
});
```

### Deleting data
```C#
var spaceId = (await tarantoolClient.FindSpaceByNameAsync("testspace")).SpaceId;
await tarantoolClient.DeleteAsync(new DeleteRequest
{
    SpaceId = spaceId,
    Key = new object[] { 88 } // delete row where Id = 88
});
```

### Replace operation
```C#
var spaceId = (await tarantoolClient.FindSpaceByNameAsync("testspace")).SpaceId;
await tarantoolClient.ReplaceAsync(new ReplaceRequest
{
    SpaceId = spaceId,
    Tuple = new object[] { 77, "Some new name", 1777 } // find row with Id = 77 and replace it
});
```
or with object mapping to custom type:
```C#
await tarantoolClient.ReplaceAsync(new ReplaceRequest<MyTestEntity>
{
    SpaceId = testSpaceId,
    Tuple = new MyTestEntity
    {
        MyTestEntityId = 77,
        SomeStringField = "Some new name",
        SomeIntField = 1777
    }
});
```

### Upsert operation
```C#
var spaceId = (await tarantoolClient.FindSpaceByNameAsync("testspace")).SpaceId;
await tarantoolClient.UpsertAsync(new UpsertRequest
{
    SpaceId = spaceId,
    Tuple = new object[] { 55, "Some name", 1550 }, // find row with Id = 55 and insert if not exists
    UpdateOperations = new[]
    {
        new UpdateOperation<int> // if row with Id = 55 exist then perform this update operation
        {
            Operation = UpdateOperationCode.Assign,
            FieldNo = 2,
            Argument = 1555
        }
    }
});
```

### Call database LUA functions
```C#
var result = await tarantoolClient.CallAsync(new CallRequest
{
    FunctionName = "some_function", // execute this database stored LUA funtion
    Args = new object[] {1, 2, 3} // pass these arguments to function
});
```

### Eval operation
```C#
var result = await tarantoolClient.EvalAsync(new EvalRequest
{
    Expression = "return ...", // any tarantool expression
    Args = new object[] { 912345, 923456, 934567 } // arguments if needed for expression
});
```
Or with Tarantool.Client.Extensions:
```C#
var result = await tarantoolClient.EvalAsync("return ...", 912345, 923456, 934567);
```

DDL-operations
--------------
```C#
using Tarantool.Client.Extensions;
```
### Create space
```C#
await tarantoolClient.CreateSpaceAsync("new_space_name");
```

### Drop space
```C#
await tarantoolClient.DropSpaceAsync("new_space_name");
```

### Create index
```C#
await tarantoolClient.CreateIndexAsync("some_space",
    "index_name",
    IndexType.Hash,
    new IndexPart(0, IndexedFieldType.Unsigned));
```

### Drop index
```C#
await tarantoolClient.DropIndexAsync("some_space", "index_name");
```

Other examples
--------------
You can find more examples in unit tests available with TarantoolClient source code.
