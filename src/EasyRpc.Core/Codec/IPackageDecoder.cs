using System;
using System.Buffers;

namespace EasyRpc.Core.Codec
{
    public interface IPackageDecoder<out TPackageInfo> where TPackageInfo : class
    {
        TPackageInfo Decode(ReadOnlySpan<byte> span);
    }
}