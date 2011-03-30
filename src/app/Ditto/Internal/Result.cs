namespace Ditto.Internal
{
    public class Result
    {
        public static Result Unresolved=new Result(false,null);
        public bool IsResolved { get; private set; }
        public object Value { get; private set; }

        public Result(bool isResolved, object value)
        {
            IsResolved = isResolved;
            Value = value;
        }
    }
}