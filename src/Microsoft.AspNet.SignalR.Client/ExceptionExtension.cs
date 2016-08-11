using System;

namespace Microsoft.AspNet.SignalR
{
    public static class ExceptionExtension
    {
        public static Exception Unwrap(this Exception ex)
        {
            return ex;
        }
    }
}
