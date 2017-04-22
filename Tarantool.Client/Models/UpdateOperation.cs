using System;
using System.Linq.Expressions;
using System.Reflection;

using MsgPack;
using MsgPack.Serialization;

using Tarantool.Client.Serialization.Attributes;

namespace Tarantool.Client.Models
{
    public abstract class UpdateOperation : IPackable
    {
        public uint FieldNo { get; set; }

        public UpdateOperationCode Operation { get; set; }

        public abstract void PackToMessage(Packer packer, PackingOptions options);
    }

    public class UpdateOperation<TField> : UpdateOperation
    {
        public TField Argument { get; set; }

        public uint Offset { get; set; }

        public uint Position { get; set; }

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


    public class UpdateOperation<T, TField> : UpdateOperation<TField>
    {
        public UpdateOperation(Expression<Func<T, TField>> field)
        {
            var body = field.Body as MemberExpression;
            if (body == null) throw new TarantoolException("Can only deal with properties, not methods.");
            var attr = body.Member.GetCustomAttribute<MessagePackMemberAttribute>();
            if (attr == null) throw new TarantoolException($"Cannot find MessagePackMemberAttribute for {field}.");
            FieldNo = (uint)attr.Id;
        }
    }
}

