
namespace CodeArt.AOP
{
    public abstract class AspectInitializerBase : IAspect
    {
        public AspectInitializerBase() { }

        public void Before()
        {
            Init();
        }

        public void After() { }

        public abstract void Init();
    }
}
