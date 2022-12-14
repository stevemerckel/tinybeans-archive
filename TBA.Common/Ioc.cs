using System;
using Ninject.Modules;

namespace TBA.Common
{
    /// <summary>
    /// IoC bindings for POCOs using Ninject
    /// </summary>
    public class Ioc : NinjectModule
    {
        /// <inheritdoc />
        public override void Load()
        {
            Bind<IAppLogger>().To<ConsoleAppLogger>().InSingletonScope(); // todo: swap for SerilogAppLogger when that is finished
            Bind<IRuntimeSettingsProvider>().To<RuntimeSettingsProvider>().InSingletonScope();
            Bind<ITinybeansJsonHelper>().To<TinybeansJsonHelper>();
            Bind<ITinybeansApiHelper>().To<TinybeansApiHelper>();
            Bind<IFileManager>().To<WindowsFileSystemManager>().InSingletonScope();

            // fetch runtime location, include in ctor of IJournalManager implementation
            var runtimePath = Environment.CurrentDirectory;
            Bind<IJournalManager>()
                .To<JournalManager>()
                .WithConstructorArgument("rootForRepo", runtimePath);
        }
    }
}