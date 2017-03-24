using MsgPack;
using Tarantool.Client.Helpers;

namespace Tarantool.Client.Models
{
    public abstract class UpdateOperation : IPackable
    {
        public UpdateOperationCode Operation { get; set; }

        public uint FieldNo { get; set; }

        public abstract void PackToMessage(Packer packer, PackingOptions options);
    }

    public class UpdateOperation<T> : UpdateOperation
    {
        public uint Position { get; set; }

        public uint Offset { get; set; }

        public T Argument { get; set; }

        public override void PackToMessage(Packer packer, PackingOptions options)
        {
            packer.PackArrayHeader(Operation == UpdateOperationCode.Splice ? 5 : 3);
            packer.PackString(StringValueAttribute.GetStringValue(Operation));
            packer.Pack(FieldNo);
            if (Operation == UpdateOperationCode.Splice)
            {
                packer.Pack(Position);
                packer.Pack(Offset);
            }
            packer.Pack(Argument);
        }
    }
}