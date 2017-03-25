namespace Tarantool.Client.Models
{
    public enum TarantoolKey
    {
        Code = 0x00,
        Sync = 0x01,

        SchemaId = 0x05,

        Space = 0x10,
        Index = 0x11,
        Limit = 0x12,
        Offset = 0x13,
        Iterator = 0x14,

        Key = 0x20,
        Tuple = 0x21,
        Function = 0x22,
        UserName = 0x23,
        Expression = 0x27,
        UpsertOps = 0x28,

        Data = 0x30,
        Error = 0x31
    }
}