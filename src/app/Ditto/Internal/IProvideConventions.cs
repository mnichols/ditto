namespace Ditto.Internal
{
    public interface IProvideConventions
    {
        void Inject(IApplyConventions configuration);
    }
}