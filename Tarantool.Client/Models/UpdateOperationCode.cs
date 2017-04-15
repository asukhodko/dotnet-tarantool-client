using Tarantool.Client.Serialization.Attributes;

namespace Tarantool.Client.Models
{
    public enum UpdateOperationCode
    {
        [StringValue("+")]
        Addition,

        [StringValue("-")]
        Subtraction,

        [StringValue("&")]
        BitwiseAnd,

        [StringValue("^")]
        BitwiseXor,

        [StringValue("|")]
        BitwiseOr,

        [StringValue("#")]
        Delete,

        [StringValue("!")]
        Insert,

        [StringValue("=")]
        Assign,

        [StringValue(":")]
        Splice
    }
}