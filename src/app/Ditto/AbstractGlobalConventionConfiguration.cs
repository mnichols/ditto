namespace Ditto
{
    public abstract class AbstractGlobalConventionConfiguration:IConfigureGlobalConventions
    {
        /// <summary>
        /// Injected conventions container
        /// </summary>
        /// <value>The conventions.</value>
        public virtual IGlobalConventionContainer Conventions { get; set; }
        public abstract void Configure();
    }
}