namespace Ditto.Internal
{
    public interface ICreateValueAssignment
    {
        IAssignValue Create(object destination, IDescribeMappableProperty destinationProperty);
    }
}