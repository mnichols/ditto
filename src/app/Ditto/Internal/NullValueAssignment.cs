using System;

namespace Ditto.Internal
{
    public class NullValueAssignment:IAssignValue
    {
        public void SetValue(Result value)
        {
            //no op
        }
    }
}