using Topshelf;

namespace TaxiBackend
{    
    public class Program
    {
        public static void Main()
        {
            HostFactory.Run(x => //1
            {
                x.Service<TransitBackendService>(s => //2
                {
                    s.ConstructUsing(name => new TransitBackendService()); //3
                    s.WhenStarted(tc => tc.Start()); //4
                    s.WhenStopped(tc => tc.Stop()); //5
                });
                x.RunAsLocalSystem(); //6

                x.SetDescription("TransitService"); //7
                x.SetDisplayName("TransitService"); //8
                x.SetServiceName("TransitService"); //9
            }); //10
        }
    }
}