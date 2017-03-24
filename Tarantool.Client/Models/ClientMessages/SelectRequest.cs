using System;
using System.Collections.Generic;
using MsgPack;

namespace Tarantool.Client.Models.ClientMessages
{
    public class SelectRequest : ClientMessageBase
    {
        public SelectRequest()
            : base(TarantoolCommand.Select)
        {
            Iterator = Iterator.Eq;
            Limit = int.MaxValue;
            Key = new List<object>();
        }

        public uint SpaceId { get; set; }
        public uint IndexId { get; set; }
        public uint Limit { get; set; }
        public uint Offset { get; set; }
        public Iterator Iterator { get; set; }
        public List<object> Key { get; set; }

        protected override void PackBody(Packer packer)
        {
            packer.PackMapHeader(6);

            packer.Pack((byte)TarantoolKey.Space);
            packer.Pack(SpaceId);

            packer.Pack((byte)TarantoolKey.Index);
            packer.Pack(IndexId);

            packer.Pack((byte)TarantoolKey.Limit);
            packer.Pack(Limit);

            packer.Pack((byte)TarantoolKey.Offset);
            packer.Pack(Offset);

            packer.Pack((byte)TarantoolKey.Iterator);
            packer.Pack((uint)Iterator);

            packer.Pack((byte)TarantoolKey.Key);
            packer.PackArrayHeader(Key.Count);
            foreach (var arg in Key)
            {
                var type = arg.GetType();
                if(type == typeof(int))
                    packer.Pack((int)arg);
                else if(type == typeof(uint))
                    packer.Pack((uint)arg);

                else if(type == typeof(long))
                    packer.Pack((long)arg);
                else if(type == typeof(ulong))
                    packer.Pack((ulong)arg);

                else if(type == typeof(string))
                    packer.Pack((string)arg);

                else if(type == typeof(DateTime))
                    packer.Pack((DateTime)arg);

                else if(type == typeof(byte))
                    packer.Pack((byte)arg);

                else if(type == typeof(bool))
                    packer.Pack((bool)arg);

                else if(type == typeof(float))
                    packer.Pack((float)arg);
                else if(type == typeof(double))
                    packer.Pack((double)arg);

                else
                    packer.Pack(arg);
            }
        }
    }
}