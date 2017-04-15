namespace Tarantool.Client.Models
{
    public class IndexOptions
    {
        public bool Unique { get; set; }

        public uint Lsn { get; set; }
    }
}