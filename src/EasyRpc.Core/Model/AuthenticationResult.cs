using EasyRpc.Core.Config;
using System.Collections.Generic;

namespace EasyRpc.Core.Model
{
    public class AuthenticationResult
    {
        public bool Success { get; set; }

        public string Message { get; set; }

        public List<ChannelConfig> Channels { get; set; }
    }
}
