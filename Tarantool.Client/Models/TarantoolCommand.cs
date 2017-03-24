namespace Tarantool.Client.Models
{
    public enum TarantoolCommand
    {
        // User command codes
        Select = 0x01,
        Insert = 0x02,
        Replace = 0x03,
        Update = 0x04,
        Delete = 0x05,
        Auth = 0x07,
        Eval = 0x08,
        Upsert = 0x09,
        Call = 0x0a,

        // Admin command codes
        Ping = 0x40
    }
}