using MyIoC;

namespace IoC.Sample
{
    [ImportConstructor]
    public class CustomerBLL
    {
        public CustomerBLL(ICustomerDAL dal, Logger logger)
        {
            CustomerDAL = dal;
            Logger = logger;
        }

        public ICustomerDAL CustomerDAL { get; }

        public Logger Logger { get; }
    }

    public class CustomerBLL2
    {
        [Import]
        public ICustomerDAL CustomerDAL { get; set; }

        [Import]
        public Logger Logger { get; set; }
    }
}
