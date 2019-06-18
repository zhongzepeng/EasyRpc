﻿using Newtonsoft.Json;
using System.Text;

namespace EasyRpc.Core.Model
{
    public class AuthenticationModel
    {
        public int ClientId { get; set; }

        public string SecretKey { get; set; }
    }
}
