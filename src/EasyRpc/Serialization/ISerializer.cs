using System;

namespace EasyRpc.Serialization
{
    public interface ISerializer
    {
        byte[] Serialize<T>(T obj);

        T Deserialize<T>(byte[] bytes);

        object Deserialize(byte[] bytes, Type type);
    }
}