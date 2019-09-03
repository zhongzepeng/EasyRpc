using System;
using System.Threading.Tasks;

namespace EasyRpc.Test.Common
{
    public class UserService : IUserService
    {
        public T GetUser<T>(int id)
        {
            return default;
        }

        public string GetUserNameById(TestParam id)
        {
            return "username";
        }

        public string GetUserNameById(int id)
        {
            return "username";
        }

        public Task<string> GetUserNameByIdAsync(int id)
        {
            return Task.FromResult("username");
        }

    }
}
