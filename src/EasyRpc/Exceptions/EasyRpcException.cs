using System;

namespace EasyRpc.Exceptions
{
    public class EasyRpcException : Exception
    {
        public EasyRpcException(string message, Exception innerException = null)
         : base(message, innerException)
        {

        }
    }
}