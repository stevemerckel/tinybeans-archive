using Ninject.Modules;

namespace TBA.Common
{
    /// <summary>
    /// IoC bindings using Ninject
    /// </summary>
    public class Ioc : NinjectModule
    {
        public override void Load()
        {
            Bind<IAppLogger>().To<SerilogAppLogger>().InSingletonScope();
            Bind<IRuntimeSettings>().To<RuntimeSettings>().InSingletonScope();
            Bind<ITinybeansApiHelper>().To<TinybeansApiHelper>().InThreadScope();
        }
    }
}