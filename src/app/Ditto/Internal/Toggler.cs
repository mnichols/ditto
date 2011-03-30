using System;
using System.Linq.Expressions;

namespace Ditto.Internal
{
    public class Toggler<TDest> : IToggle<TDest>
    {
        private readonly Expression<Func<TDest, object>> property;
        private ISourcedDestinationConfiguration<TDest> cfg;

        public Toggler(ISourcedDestinationConfiguration<TDest> cfg, Expression<Func<TDest, object>> property)
        {
            this.cfg = cfg;
            this.property = property;
        }

        public void By<TTrue, TFalse>()
        {
            cfg.UsingValue<TTrue>(true, property);
            cfg.UsingValue<TFalse>(false, property);
        }
    }
}