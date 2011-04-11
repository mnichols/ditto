using System;

namespace Ditto
{
    [Serializable]
    public class DittoConfigurationException : Exception
    {
        public DittoConfigurationException(string msg, params object[] args)
            : base(string.Format(msg, args))
        {
        }
        // Constructor needed for serialization 
        // when exception propagates from a remoting server to the client.
        protected DittoConfigurationException(System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context) { }
        public override string ToString()
        {
            return this.Message + this.StackTrace;
        }
    }
}