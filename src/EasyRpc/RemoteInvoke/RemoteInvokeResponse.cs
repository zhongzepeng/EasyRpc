namespace EasyRpc.RemoteInvoke
{
    public class RemoteInvokeResponse
    {
        public bool Success { get; set; }

        public string Message { get; set; }

        public byte[] Data { get; set; }

        public static RemoteInvokeResponse CreateSuccess(byte[] data)
        {
            return new RemoteInvokeResponse
            {
                Success = true,
                Message = string.Empty,
                Data = data
            };
        }

        public static RemoteInvokeResponse CreateFail(string message)
        {
            return new RemoteInvokeResponse
            {
                Message = message
            };
        }

    }
}