using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using EasyNetQ;
using Messages;

namespace FileIndexer.Service
{
    public class FileIndexerService
    {
        private readonly IBus bus;

        public FileIndexerService(IBus bus)
        {
            this.bus = bus;
        }

        public void Start()
        {
            new Thread(x => 
            {
                bus.Respond<SyncRequest, SyncResponse>(Sync);
            }).Start();
        }

        public void Stop() => bus.Dispose();

        private SyncResponse Sync(SyncRequest request)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            var mainDirectory = request.MainDirectory;
            var syncDirectory = request.SyncDirectory;

            List<string> mainDirectoriesFullPath = Directory.GetDirectories(mainDirectory, "*", SearchOption.AllDirectories).ToList();
            var mainDirectories = GetDirectoryNames(mainDirectoriesFullPath, mainDirectory);
            
            if (request.SyncCommand == SyncCommands.WithDelete)
            {
                List<string> syncDirectoriesFullPath = Directory.GetDirectories(syncDirectory, "*", SearchOption.AllDirectories).ToList();
                var syncDirectories = GetDirectoryNames(syncDirectoriesFullPath, syncDirectory);
                DeleteFilesInSyncThatAreNotInMain(request.MainDirectory, request.SyncDirectory, syncDirectories, mainDirectories);
            }

            SyncFilesToMainDirectory(mainDirectories, syncDirectory, mainDirectory);

            stopWatch.Stop();
            return new SyncResponse { SyncTime = stopWatch.ElapsedMilliseconds };
        }

        private static void SyncFilesToMainDirectory(List<string> mainDirsMinStart, string syncDirectory, string mainDirectory)
        {
            foreach (var mainDir in mainDirsMinStart)
            {
                var path = syncDirectory + mainDir;
                FileInfo[] filesToCopy = new DirectoryInfo(mainDirectory + mainDir).GetFiles();

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);

                    foreach (FileInfo file in filesToCopy)
                    {
                        string temppath = Path.Combine(path, file.Name);
                        file.CopyTo(temppath, false);
                    }
                }
                else
                {
                    foreach (FileInfo file in filesToCopy)
                    {
                        var fileNameInSync = Path.Combine(path, file.Name);
                        if (!File.Exists(fileNameInSync))
                        {
                            string temppath = Path.Combine(path, file.Name);
                            file.CopyTo(temppath, false);
                        }
                    }
                }
            }
        }

        private static List<string> GetDirectoryNames(List<string> mainDirs, string mainDirectory)
        {
            return mainDirs.Select(
                mainDir => mainDir.Substring(mainDirectory.Length, mainDir.Length - mainDirectory.Length)).ToList();
        }

        private void DeleteFilesInSyncThatAreNotInMain(string mainDirectory, string syncDirectory, List<string> syncDirs, List<string> mainDirs)
        {
            foreach (var syncDir in syncDirs)
            {
                // delete directory if it does not exist in main directory
                if (!mainDirs.Contains(syncDir))
                {
                    Directory.Delete(syncDirectory + syncDir, true);
                    continue;
                }

                // location to check files in
                FileInfo[] syncFiles = new DirectoryInfo(syncDirectory + syncDir).GetFiles();
                FileInfo[] mainFiles =  new DirectoryInfo(mainDirectory + syncDir).GetFiles();

                foreach (FileInfo fileInfo in syncFiles)
                {
                    DeleteFileIfNotExistsInSyncDir(mainFiles, fileInfo);
                }
            }
        }

        private static void DeleteFileIfNotExistsInSyncDir(FileInfo[] mainFiles, FileInfo fileInfo)
        {
            var existsButShouldNot = true;

            foreach (FileInfo fileInfoToCompare in mainFiles)
            {
                if (fileInfoToCompare.Name == fileInfo.Name)
                {
                    existsButShouldNot = false;
                }
            }

            if (existsButShouldNot)
                fileInfo.Delete();
        }
    }
}