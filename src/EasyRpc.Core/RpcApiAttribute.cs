using System;

namespace EasyRpc.Core
{
    public class RpcApiAttribute : Attribute
    {
        public string Role { get; set; }
    }
}
