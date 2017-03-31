namespace Tarantool.Client.Models
{
    public class IndexPart
    {
        public IndexPart(int fieldNumber, IndexedFieldType type)
        {
            FieldNumber = fieldNumber;
            Type = type;
        }

        public int FieldNumber { get; }
        public IndexedFieldType Type { get; }
    }
}