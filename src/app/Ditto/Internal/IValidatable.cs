namespace Ditto.Internal
{
    public interface IValidatable
    {
        MissingProperties Validate();
        void Assert();
    }
}