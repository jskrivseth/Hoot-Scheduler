using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scheduler
{
    static class Utilities
    {
        public static void CreateMyDocumentsShortcut(string name, string linkDir)
        {
            String path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            IWshRuntimeLibrary.WshShellClass wshShell = new IWshRuntimeLibrary.WshShellClass();

            IWshRuntimeLibrary.IWshShortcut newShortcut;

            newShortcut = (IWshRuntimeLibrary.IWshShortcut)wshShell.CreateShortcut(path + "\\" + name + ".lnk");
            newShortcut.TargetPath = linkDir;
            newShortcut.Save();
        }

        public static void CreateDesktopShortcut(string name, string linkDir)
        {
            String desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            IWshRuntimeLibrary.WshShellClass wshShell = new IWshRuntimeLibrary.WshShellClass();

            IWshRuntimeLibrary.IWshShortcut newShortcut;

            newShortcut = (IWshRuntimeLibrary.IWshShortcut)wshShell.CreateShortcut(desktopPath + "\\" + name + ".lnk");
            newShortcut.TargetPath = linkDir;
            newShortcut.Save();
        }

        public static System.IO.FileInfo[] GetDocuments(string physicalPath, string type)
        {
            System.IO.DirectoryInfo directory = new System.IO.DirectoryInfo(physicalPath);

            if (directory.Exists)
            {
                return directory.GetFiles(type);
            }
            else
            {
                throw new System.IO.DirectoryNotFoundException(physicalPath);
            }
        } 
    }
}
