namespace Ditto
{
    public abstract class AbstractGlobalConventionConfiguration:IConfigureGlobalConventions
    {
        /// <summary>
        /// Injected conventions container
        /// </summary>
        /// <value>The conventions.</value>
// ReSharper disable VirtualMemberNeverOverriden.Global
        public virtual IGlobalConventionContainer Conventions { get; set; }
// ReSharper restore VirtualMemberNeverOverriden.Global
        public abstract void Configure();
    }
}