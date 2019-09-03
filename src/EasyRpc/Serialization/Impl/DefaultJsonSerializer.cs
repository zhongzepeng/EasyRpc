using System;
using System.Text;
using System.Text.Json;

namespace EasyRpc.Serialization.Impl
{
    public class DefaultJsonSerializer : ISerializer
    {
        private readonly Encoding defaultEncoding = Encoding.UTF8;
        public T Deserialize<T>(byte[] bytes)
        {
            var jsonStr = defaultEncoding.GetString(bytes);

            return JsonSerializer.Deserialize<T>(jsonStr);
        }

        public byte[] Serialize<T>(T obj)
        {
            var jsonStr = JsonSerializer.Serialize<T>(obj);
            return defaultEncoding.GetBytes(jsonStr);
        }

        public object Deserialize(byte[] bytes, Type type)
        {
            var jsonStr = defaultEncoding.GetString(bytes);

            return JsonSerializer.Deserialize(jsonStr, type);
        }

    }
}