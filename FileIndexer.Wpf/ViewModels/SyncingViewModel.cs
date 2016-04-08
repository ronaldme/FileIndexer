using System;
using System.Configuration;
using Caliburn.Micro;
using EasyNetQ;
using FileIndexer.Helpers;
using Messages;

namespace FileIndexer.Wpf.ViewModels
{
    public class SyncingViewModel : Screen
    {
        private readonly IBus bus = RabbitHutch.CreateBus(ConfigurationManager.AppSettings["easynetq"]);
        private string mainPath;
        private string syncPath;
        private string lastSync;

        public SyncingViewModel()
        {
            MainPath = ConfigurationManager.AppSettings["mainPath"];
            SyncPath = ConfigurationManager.AppSettings["syncPath"];
        }

        public string MainPath
        {
            get { return mainPath; }
            set
            {
                if (mainPath != value)
                {
                    mainPath = value;
                    NotifyOfPropertyChange();
                }
            }
        }

        public string SyncPath
        {
            get { return syncPath; }
            set
            {
                if (syncPath != value)
                {
                    syncPath = value;
                    NotifyOfPropertyChange();
                }
            }
        }

        public string LastSync
        {
            get { return lastSync;}
            set
            {
                if (lastSync != value)
                {
                    lastSync = value;
                    NotifyOfPropertyChange();
                }
            }
        }

        public bool WithDelete { get; set; }

        public void SelectMainPath() => MainPath = GetFolderBrowserResult();
        public void SelectSyncPath() => SyncPath = GetFolderBrowserResult();

        public string GetFolderBrowserResult()
        {
            Gat.Controls.OpenDialogView openDialog = new Gat.Controls.OpenDialogView();
            Gat.Controls.OpenDialogViewModel vm = (Gat.Controls.OpenDialogViewModel)openDialog.DataContext;

            vm.IsDirectoryChooser = true;
            vm.StartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            bool? result = vm.Show();

            return result == true ?
                vm.SelectedFilePath :
                null;
        }

        public void Sync()
        {
            if (UserInputValidation.ValidInputFormat(MainPath, SyncPath, WithDelete))
            {
                var request = RequestBuilder.GetSyncRequest(MainPath + " " + SyncPath + (WithDelete ? " -d" : ""));
                var response = bus.Request<SyncRequest, SyncResponse>(request);

                LastSync = $"{DateTime.Now} which took: {response.SyncTime}ms";
            }
        }
    }
}