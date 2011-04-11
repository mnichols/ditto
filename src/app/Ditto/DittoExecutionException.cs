using System;

namespace Ditto
{
    [Serializable]
    public class DittoExecutionException:Exception
    {
        public DittoExecutionException(Exception innerException,string msg,params object[] args):base(string.Format(msg,args),innerException)
        {
        }

        public DittoExecutionException(string msg,params object[] args):base(string.Format(msg,args))
        {
        }
        // Constructor needed for serialization 
        // when exception propagates from a remoting server to the client.
        protected DittoExecutionException(System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context) { }
        public override string ToString()
        {
            return this.Message + this.StackTrace;
        }
    }
}