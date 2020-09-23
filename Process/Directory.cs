using System;
using System.IO;

namespace Service.Process {
    static class DirectoryProcess {
        public static DirectoryInfo CreateDirectory(string newFolder) {
            try {
                DirectoryInfo dirRoot;
                string path = Path.Combine(Directory.GetCurrentDirectory(), @"c:/backup");

                if (!Directory.Exists(path)) {
                    dirRoot = Directory.CreateDirectory(path);
                } else {
                    dirRoot = new DirectoryInfo(path);
                }

                DirectoryInfo dir;
                path = Path.Combine(dirRoot.FullName, newFolder.ToLower());

                if (!Directory.Exists(path)) {
                    dir = Directory.CreateDirectory(path);
                } else {
                    dir = new DirectoryInfo(path);
                }

                Console.WriteLine("Directory Created successfully.");

                return dir;
            } catch (Exception ex) {
                Console.WriteLine("Error creating directory" + ex.Message);
                throw new Exception("Error creating directory. {newFolder}");
            }
        }
    }
}