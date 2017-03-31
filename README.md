Tarantool .NET client
=====================

.NET client (connector/driver) for Tarantool database (https://tarantool.org/).
Works both with .NET Core and .NET Framework (>= 4.6.1).

[Get dotnet TarantoolClient package on NuGet](https://www.nuget.org/packages/TarantoolClient)

```powershell
PM> Install-Package TarantoolClient
```
[![Latest stable](https://img.shields.io/nuget/v/TarantoolClient.svg)](https://www.nuget.org/packages/TarantoolClient)

Basic tarantool operations
---------------
Next examples assume to these usings:
```C#
using Tarantool.Client;
using Tarantool.Client.Models;
using Tarantool.Client.Models.ClientMessages;
```
Create TarantoolClient instance:
```C#
var tarantoolClient = new TarantoolClient("tarantool://user:pass@tarantool-host:3301");
```

### Selecting from space by key
```C#
var spaceId = (await tarantoolClient.FindSpaceByNameAsync("testspace"))[0].AsUInt32();
var rows = await tarantoolClient.RequestAsync(new SelectRequest
{
    SpaceId = spaceId,
    Key = new object[] { 3 } // find row where Id = 3
});
```

### Selecting all rows from space
```C#
var spaceId = (await tarantoolClient.FindSpaceByNameAsync("testspace"))[0].AsUInt32();
var rows = await tarantoolClient.RequestAsync(new SelectRequest
{
    SpaceId = spaceId,
    Iterator = Iterator.All
});
```

### Selecting by secondary index
```C#
var spaceId = (await tarantoolClient.FindSpaceByNameAsync("testspace"))[0].AsUInt32();
var indexId = (await tarantoolClient.FindIndexByNameAsync(spaceId, "indexname"))[0].AsUInt32();
var rows = await tarantoolClient.RequestAsync(new SelectRequest
{
    SpaceId = spaceId,
    IndexId = indexId,
    Key = new object[] { "some data" } // find rows where indexed field = "some data"
});
```

### Inserting data
```C#
var spaceId = (await tarantoolClient.FindSpaceByNameAsync("testspace"))[0].AsUInt32();
await tarantoolClient.RequestAsync(new InsertRequest
{
    SpaceId = spaceId,
    Tuple = new object[] { 99, "Some string", 1900 }
});
```

### Updating data
```C#
var spaceId = (await tarantoolClient.FindSpaceByNameAsync("testspace"))[0].AsUInt32();
await tarantoolClient.RequestAsync(new UpdateRequest
{
    SpaceId = spaceId,
    Key = new object[] { 66 }, // update row where Id = 66
    UpdateUperations = new []
    {
        new UpdateOperation<int>
        {
            Operation = UpdateOperationCode.Assign,
            FieldNo = 2, // updating field with index=2 in tuple
            Argument = 1666 // new value
        } 
    }
});
```

### Deleting data
```C#
var spaceId = (await tarantoolClient.FindSpaceByNameAsync("testspace"))[0].AsUInt32();
await tarantoolClient.RequestAsync(new DeletetRequest
{
    SpaceId = spaceId,
    Key = new object[] { 88 } // delete row where Id = 88
});
```

### Replace operation
```C#
var spaceId = (await tarantoolClient.FindSpaceByNameAsync("testspace"))[0].AsUInt32();
await tarantoolClient.RequestAsync(new ReplaceRequest
{
    SpaceId = spaceId,
    Tuple = new object[] { 77, "Some new name", 1777 } // find row with Id = 77 and replace it
});
```

### Upsert operation
```C#
var spaceId = (await tarantoolClient.FindSpaceByNameAsync("testspace"))[0].AsUInt32();
await tarantoolClient.RequestAsync(new UpsertRequest
{
    SpaceId = spaceId,
    Tuple = new object[] { 55, "Some name", 1550 }, // find row with Id = 55 and insert if not exists
    UpdateUperations = new[]
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
var result = await tarantoolClient.RequestAsync(new CallRequest
{
    FunctionName = "some_function", // execute this database stored LUA funtion
    Args = new object[] {1, 2, 3} // pass these arguments to function
});
```

### Eval operation
```C#
var result = await tarantoolClient.RequestAsync(new EvalRequest
{
    Expression = "return ...", // any tarantool expression
    Args = new object[] { 912345, 923456, 934567 } // arguments if needed for expression
});
```
Or with Tarantool.Client.Extensions:
```C#
var result = await tarantoolClient.EvalAsync("return ...", 912345, 923456, 934567);
```
### Other examples
You can find more examples in unit tests available with TarantoolClient source code.

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

### Drop space
```C#
await tarantoolClient.DropIndexAsync("some_space", "index_name");
```

High-level operations and ORM
-----------------------------

At this time package contains only low-level application interface (API)
for operations with Tarantool database.
Object-Relational Mapping (ORM) for Tarantool is under development.

