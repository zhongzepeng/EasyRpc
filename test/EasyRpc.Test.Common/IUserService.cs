using System.Threading.Tasks;
using EasyRpc.Services;

namespace EasyRpc.Test.Common
{
    public interface IUserService : IRpcService
    {
        string GetUserNameById(TestParam id);

        string GetUserNameById(int id);

        Task<string> GetUserNameByIdAsync(int id);

        T GetUser<T>(int id);

        void Update(int id);

        Task UpdateAsync(int id);
    }
}