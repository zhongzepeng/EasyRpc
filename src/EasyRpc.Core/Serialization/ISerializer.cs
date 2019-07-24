namespace EasyRpc.Core.Serialization
{
    public interface ISerializer
    {
        byte[] Serialize<T>(T instance);

        T Deserialize<T>(byte[] bytes);

    }
}
