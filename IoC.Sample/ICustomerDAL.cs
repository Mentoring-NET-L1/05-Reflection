using MyIoC;

namespace IoC.Sample
{
    public interface ICustomerDAL
    {
    }

    [Export(typeof(ICustomerDAL))]
    public class CustomerDAL : ICustomerDAL
    { }

    //[Export(typeof(ICustomerDAL))]
    public class CustomerDAL2 : ICustomerDAL
    { }
}
