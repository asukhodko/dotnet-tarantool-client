using System;

namespace Tarantool.Client.Serialization.Attributes
{
    public class TarantoolFieldAttribute : Attribute
    {
        public TarantoolFieldAttribute(int fieldNumber)
        {
            FieldNumber = fieldNumber;
        }

        public int FieldNumber { get; }
    }
}
