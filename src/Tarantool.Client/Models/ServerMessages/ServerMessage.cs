using System.Diagnostics;
using System.IO;
using MsgPack;

namespace Tarantool.Client.Models.ServerMessages
{
    public class ServerMessage
    {
        public ServerMessage(byte[] messageBytes)
        {
            using (var unpacker = Unpacker.Create(new MemoryStream(messageBytes)))
            {
                var header = unpacker.ReadItem()?.AsDictionary();
                Debug.Assert(header != null);
                foreach (var key in header.Keys)
                {
                    var value = header[key];
                    switch ((TarantoolKey)key.AsInt32())
                    {
                        case TarantoolKey.Code:
                            Code = value.AsInt32();
                            break;
                        case TarantoolKey.Sync:
                            RequestId = value.AsUInt64();
                            break;
                        case TarantoolKey.SchemaId:
                            SchemaId = value.AsInt32();
                            break;
                    }
                }
                var body = unpacker.ReadItem()?.AsDictionary();
                if (body != null)
                    foreach (var key in body.Keys)
                    {
                        var value = body[key];
                        switch ((TarantoolKey)key.AsInt32())
                        {
                            case TarantoolKey.Data:
                                Body = value;
                                break;
                            case TarantoolKey.Error:
                                ErrorMessage = value.AsString();
                                IsError = true;
                                break;
                        }
                    }
            }
        }

        public bool IsError { get; }

        public string ErrorMessage { get; }

        public MessagePackObject Body { get; }

        public int SchemaId { get; }

        public ulong RequestId { get; }

        public int Code { get; }
    }
}