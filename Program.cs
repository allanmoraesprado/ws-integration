using BLL;
using System.ServiceProcess;

namespace IntegradorSSW
{
    public static class Program
    {
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new Integrador()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
