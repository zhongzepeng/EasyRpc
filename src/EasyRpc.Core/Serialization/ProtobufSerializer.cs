using System;
using System.Collections.Generic;
using System.Text;

namespace EasyRpc.Core.Serialization
{
    internal class ProtobufSerializer : ISerializer
    {
        public T Deserialize<T>(byte[] bytes)
        {
            throw new NotImplementedException();
        }

        public byte[] Serialize<T>(T instance)
        {
            throw new NotImplementedException();
        }
    }
}
