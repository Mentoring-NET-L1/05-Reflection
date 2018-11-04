using System;
using System.Reflection;
using IoC.Sample;
using MyIoC;

namespace IoC.ConsoleApp
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                var container = new Container();
                container.AddAssembly(Assembly.Load("IoC.Sample"));

                var customerBLL = (CustomerBLL)container.CreateInstance(typeof(CustomerBLL));
                var customerBLL2 = container.CreateInstance<CustomerBLL2>();
                var logger = container.CreateInstance<Logger>();
            }
            catch(ContainerException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch(Exception)
            {
                Console.WriteLine("Unexpected error.");
            }

            Console.ReadLine();
        }
    }
}
