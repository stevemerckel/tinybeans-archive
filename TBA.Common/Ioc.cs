using Ninject.Modules;

namespace TBA.Common
{
    /// <summary>
    /// IoC bindings using Ninject
    /// </summary>
    public class Ioc : NinjectModule
    {
        /// <inheritdoc />
        public override void Load()
        {
            Bind<IAppLogger>().To<ConsoleAppLogger>().InSingletonScope(); // todo: swap for SerilogAppLogger when that is finished
            Bind<IRuntimeSettingsProvider>().To<RuntimeSettingsProvider>().InSingletonScope();
            Bind<ITinybeansApiHelper>().To<TinybeansApiHelper>().InThreadScope();
        }
    }
}