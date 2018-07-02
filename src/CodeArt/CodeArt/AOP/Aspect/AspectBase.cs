
namespace CodeArt.AOP
{
    public abstract class AspectBase : IAspect
    {
        public AspectBase() { }

        public abstract void Before();
        public abstract void After();
    }
}
