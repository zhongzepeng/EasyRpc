using System.IO.Pipelines;
using System.Threading.Tasks;

namespace EasyRpc.Core.Codec
{
    public interface IPackageEncoder<in TPackageInfo> where TPackageInfo : class
    {
        Task<int> EncodeAsync(PipeWriter writer, TPackageInfo package);
    }
}