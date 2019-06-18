using System.Collections.Generic;

namespace EasyRpc.Core.Config
{
    public class ClientConfig
    {
        public int ClientId { get; set; }

        public string Name { get; set; }

        public string SecretKey { get; set; }

        public List<ChannelConfig> Channels { get; set; }
    }
}
