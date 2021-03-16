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

            Console.WriteLine("EOP");
            Console.ReadLine();
        }
    }
}