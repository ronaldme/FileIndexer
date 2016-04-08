using Caliburn.Micro;
using StructureMap;
using StructureMap.Pipeline;

namespace FileIndexer.Wpf
{
    public class AppRegistry : Registry
    {
        public AppRegistry()
        {
            For<IWindowManager>().Use<WindowManager>().LifecycleIs<SingletonLifecycle>();
            For<IEventAggregator>().Use<EventAggregator>().LifecycleIs<SingletonLifecycle>();

            Scan(scanner =>
            {
                scanner.AssemblyContainingType<AppRegistry>();
                scanner.LookForRegistries();
                scanner.WithDefaultConventions();
            });
        }
    }
}