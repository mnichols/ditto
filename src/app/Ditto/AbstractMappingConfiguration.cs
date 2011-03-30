namespace Ditto
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class AbstractMappingConfiguration:IConfigureMapping
    {
        
        /// <summary>
        /// Injected cfg
        /// </summary>
        /// <value>The CFG.</value>
        public IContainDestinationConfiguration Cfg { get; set; }
        public abstract void Configure();
    }
}