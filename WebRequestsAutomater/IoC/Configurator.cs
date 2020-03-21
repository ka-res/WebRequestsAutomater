using SimpleInjector;
using WebRequestsAutomater.Services.Implementations;
using WebRequestsAutomater.Services.Interfaces;

namespace WebRequestsAutomater.IoC
{
    public class Configurator
    {
        public static void RegisterComponents(Container container)
        {
            container.Register<IDataImporterService, DataImporterService>();
            container.Register<IVoterHttpService, VoterHttpService>();
            container.Verify();
        }
    }
}
