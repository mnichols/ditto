namespace Ditto.Internal
{
    public interface IInvoke
    {
        object GetValue(string propertyName, object source);
        void SetValue(AssignableValue assignableValue,object destination);
        object Copy(object source);
    }
}