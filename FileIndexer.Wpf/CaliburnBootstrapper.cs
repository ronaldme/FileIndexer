using System;
using System.Collections.Generic;
using System.Windows;
using Caliburn.Micro;
using FileIndexer.Wpf.ViewModels;
using StructureMap;

namespace FileIndexer.Wpf
{
    public class CaliburnBootstrapper : BootstrapperBase
    {
        private Container container;

        public CaliburnBootstrapper()
        {
            Initialize();
            container = new Container(new AppRegistry());
        }

        protected override void BuildUp(object instance)
        {
            container.BuildUp(instance);
        }

        protected override object GetInstance(Type service, string key)
        {
            if (key == null) return container.GetInstance(service);
            return container.GetInstance(service, key);
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return (IEnumerable<object>)container.GetAllInstances(service);
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<SyncingViewModel>();
        }
    }
}
