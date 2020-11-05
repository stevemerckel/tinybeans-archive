using Ninject;
using System;
using TBA.Common;

namespace TBA.ConsoleApp
{
    class Program
    {
        private static readonly IKernel _kernel = new StandardKernel(new Ioc());

        static void Main(string[] args)
        {
            var logger = _kernel.Get<IAppLogger>();
            logger.Info("Hello World!");

            // init object
            // fetch journal ids --> grab first
            // grab entries by date using journal id above


            Console.ReadLine();
        }
    }
}
