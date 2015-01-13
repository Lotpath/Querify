using Xunit;

namespace Querify.Nancy.Tests
{
#if DEBUG
    public class DebugOnlyFactAttribute : FactAttribute
    {
        public DebugOnlyFactAttribute()
        {
            if (!System.Diagnostics.Debugger.IsAttached)
            {
                _skip = "Test is only configured to run with an attached debugger";
            }
        }

        private string _skip;
        public override string Skip
        {
            get { return _skip; }
            set { _skip = value; }
        }
    }
#else
    public class DebugOnlyFactAttribute : System.Attribute
    {        
    }
#endif
}
