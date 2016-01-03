using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Win32;
using System.IO.Compression;
using Common.Helpers;
using Microsoft.VisualBasic.Devices;

namespace GUIConfig.SupportInfo
{
#region SupportInfo File

    // This class will collect info (e.g. reg keys, log files, assembly info) and
    // create a zip file which then can be stored by the user locally

    public class SupportInfoFile 
    {
        private readonly List<SupportInfoItem> _items;       // List of items to be exported

        private string _tempFolder;                 // the temporary folder to collect output

        public string ErrorText { get; set; }       // return error string, if any

        // Initialize the class
        //
        public SupportInfoFile()
        {
            _items = new List<SupportInfoItem>();  // initialize list of items

            _tempFolder = GetTempDirectory();       // create the temp folder
        }

        // create the zip file and copy to the target location <filepath>
        //
        public bool CreateZipFile( string filepath)
        {
            var result = false;

            if (_items == null || !_items.Any())
            {
                ErrorText = "No items were added to report";
                return false;
            }

            try
            {
                foreach (var item in _items)            // create output for all items
                {
                    item.CreateOutput(_tempFolder);
                }

                if (File.Exists(filepath)) File.Delete(filepath);
                ZipFile.CreateFromDirectory(_tempFolder,filepath);
                result = true;
            }
            catch(Exception e)
            {
                ErrorText = "Error creating output: " + e;
            }
            finally
            {
                CleanUp();                              // do the cleanup          
            }
            return result;

        }

        //
        // Do cleanup after export: Delete all temp files
        //
        private void CleanUp()
        {
            if (Directory.Exists(_tempFolder))
            {
                DeleteDirectory(_tempFolder);
            }
            _tempFolder = string.Empty;

        }

        //
        // Add an item 
        //
        public void AddItem( SupportInfoItem item )
        {
            if (item != null)
            {
                _items?.Add(item);
            }
        }

        //
        // create a temp folder where the output will be collected
        // zip file will be created from this folder
        //
        private static string GetTempDirectory()
        {
            var path = Path.GetRandomFileName();
            path = Path.Combine(Path.GetTempPath(), path);
            Directory.CreateDirectory(path);

            return path;

        }

        //
        // delete a directory including all files and subdirectories
        //
        private static void DeleteDirectory(string targetDir)
        {
            var files = Directory.GetFiles(targetDir);
            var dirs = Directory.GetDirectories(targetDir);

            foreach (var file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (var dir in dirs)
            {
                DeleteDirectory(dir);
            }

            Directory.Delete(targetDir, false);
        }
    }

    #endregion

#region SupportInfo Item

    // Class to define items 
    //
    public class SupportInfoItem
    {
        public SupportInfoItemType Type { get; set; }   // item type
        public string Folder { get; set; }              // output folder name in zip file (empty = root)
        public string FileName { get; set; }            // output file name
        public string ItemPath { get; set; }            // file or folder path or reg key path

        private FileStream _outfile;

        //
        // create the ouput for the item
        //
        public void CreateOutput(string tempPath)
        {
            PrepareOutput(tempPath);                    // prepare the output folder / file

            switch (Type)                               // create output into temp folder according to item type
            {
                case SupportInfoItemType.AssemblyInfo:
                    OutputAssembly(tempPath);
                    break;

                case SupportInfoItemType.DataFile:
                    OutputDataFile(tempPath);
                    break;

                case SupportInfoItemType.RegistryKey:
                    OutputRegistryKey(tempPath);
                    break;

                case SupportInfoItemType.SystemInfo:
                    OutputSystemInfo(tempPath);
                    break;
            }

            if (_outfile == null) return;

            _outfile.Close();
            _outfile.Dispose();
            _outfile = null;
        }

        //
        // Prepare the output file or folder for the item
        // - if an output file is used the file is created (if needed) and opened for writing
        // - if an outpout folder is used the folder is created if needed
        //
        private void PrepareOutput(string tempPath)
        {
            // create folder if it does not exist
            if (!string.IsNullOrEmpty(Folder))
            {
                var path = Path.Combine(tempPath, Folder);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
            }

            // create file if it does not exist
            if (string.IsNullOrEmpty(FileName)) return;

            var filepath = Path.Combine(tempPath, FileName);
            _outfile = !File.Exists(filepath) ? File.Create(filepath) : File.Open(filepath, FileMode.Append);
        }

        //
        // create output for AssemblyInfo item: process an individual file or
        // content of a folder. Only Assemblys .dll and .exe are processed
        // subfolders are not processed
        //
        private void OutputAssembly(string tempPath)
        {
            if (tempPath == null) return;

            using (var sr = new StreamWriter(_outfile))
            {
                if (Directory.Exists(ItemPath))              // output an entire folder
                {
                    foreach (var file in Directory.EnumerateFiles(ItemPath, "*.*").Where(file =>
                        file.ToLower().EndsWith("dll", StringComparison.OrdinalIgnoreCase) ||
                        file.EndsWith("exe", StringComparison.OrdinalIgnoreCase)).ToList())
                    {
                        OutputAssemblyDetails(file, sr);
                    }
                }

                if (File.Exists(ItemPath))                  // output an individual file
                {
                    OutputAssemblyDetails(ItemPath, sr);
                 }
                sr.WriteLine();
            }
        }

        //
        // output the details for one assembly file into associated output stream
        //
        private static void OutputAssemblyDetails( string filename, TextWriter sr)
        {
            AssemblyName name = null;
            string errortext = null;

            try
            {
               name = AssemblyName.GetAssemblyName(filename);      
            }
            catch (FileNotFoundException)
            {
                errortext = "The file cannot be found";
            }

            catch (BadImageFormatException)
            {
                errortext = "The file is not an assembly.";
            }

            catch (FileLoadException)
            {
                errortext = "The assembly has already been loaded.";
            }

            var version = "n/a";
            if (name != null)
            {
                version = name.Version.ToString();
            }
            if (string.IsNullOrEmpty(errortext))
            {
                var fi = new FileInfo(filename);
                var size = fi.Length;
                var timestamp = fi.CreationTimeUtc;
                sr.WriteLine("AssemblyInfo [{0}]: Version {1} FileSize {2} bytes Timestamp {3} UTC", filename, version, size, timestamp);  
            }
            else
            {
               sr.WriteLine("AssemblyInfo [{0}]: Error: {1}", filename, errortext);  
            }
        }

        //
        // create output for DataFile: Copy individual file or content of a folder
        // into outpout folder. Subfolders are not copied.
        //
        private void OutputDataFile(string tempPath)
        {
            if (Directory.Exists(ItemPath))              // output an entire folder
            {
                foreach (var file in Directory.EnumerateFiles(ItemPath))
                {
                    var filename1 = Path.GetFileName(file);

                    if (filename1 != null) File.Copy(file, Path.Combine(tempPath, Folder, filename1));
                }
            }

            if (!File.Exists(ItemPath)) return;

            var filename = Path.GetFileName(ItemPath);
            if (filename != null) File.Copy(ItemPath, Path.Combine(tempPath, Folder, filename));
        }

        //
        // create output for a registry key
        //
        private void OutputRegistryKey(string tempPath)
        {
            if (tempPath == null) return;

            using (var sr = new StreamWriter(_outfile))
            {
               // determine the root key based on the full path of the reg key
                var root = ((((GetRegistryRootKey(@"HKEY_LOCAL_MACHINE\", Registry.LocalMachine) ??
                               GetRegistryRootKey(@"HKEY_CURRENT_USER\", Registry.CurrentUser)) ??
                              GetRegistryRootKey(@"HKEY_CLASSES_ROOT\", Registry.ClassesRoot)) ??
                             GetRegistryRootKey(@"HKEY_CURRENT_CONFIG\", Registry.CurrentConfig)) ??
                            GetRegistryRootKey(@"HKEY_PERFORMANCE_DATA\", Registry.PerformanceData)) ??
                           GetRegistryRootKey(@"HKEY_USERS\", Registry.Users);

                ReadRegValues(root,sr);

                sr.WriteLine();
            }

        }

        //
        // read the registry values for the key root recursively
        // write output into associated stream
        //
        private static void ReadRegValues(RegistryKey root, TextWriter sr)
        {
            if (root == null) return;

            sr.WriteLine("Registry Key: [{0}]", root);

            foreach (var child in root.GetSubKeyNames())
            {
                using (var childKey = root.OpenSubKey(child))
                {
                    ReadRegValues(childKey, sr);
                }
            }

            foreach (var value in root.GetValueNames())
            {
                sr.WriteLine("  -- {0}: <{1}>", value, root.GetValue(value));
            }

            sr.WriteLine("End of Registry Key [{0}]", root);
        }

        //
        // get the root RegistryKey for the ItemPath
        //
        private RegistryKey GetRegistryRootKey(string root, RegistryKey rootkey)
        {
            if (!ItemPath.StartsWith(root)) return null;

            var subkeyname = ItemPath.Remove(ItemPath.IndexOf(root, StringComparison.Ordinal), root.Length);
            return rootkey.OpenSubKey(subkeyname);
        }

        //
        // create output for a registry key
        //
        private void OutputSystemInfo(string tempPath)
        {
            if (tempPath == null) return;

            var computerInfo = new ComputerInfo();

            using (var sr = new StreamWriter(_outfile))
            {
                sr.WriteLine("Operating System: {0} Version: {1} Platform: {2}", computerInfo.OSFullName, computerInfo.OSVersion, computerInfo.OSPlatform);
                sr.WriteLine("UI Culture              : {0}", computerInfo.InstalledUICulture);
                sr.WriteLine("Total Physical Memory   : {0}", computerInfo.TotalPhysicalMemory);
                sr.WriteLine("Total Virtual Memory    : {0}", computerInfo.TotalVirtualMemory);
                sr.WriteLine("Available Virtual Memory: {0}", computerInfo.AvailableVirtualMemory);
                sr.WriteLine("Number of Processors    : {0}", Environment.ProcessorCount);
                sr.WriteLine("CLR Version             : {0}", Environment.Version);
                sr.WriteLine();

                var allDrives = DriveInfo.GetDrives();
                var num = 0;
                foreach (var d in allDrives.Where(d => d.IsReady && d.DriveType == DriveType.Fixed))
                {
                    sr.WriteLine("Drive{0}.Name        : {1}", num, d.Name);
                    sr.WriteLine("Drive{0}.VolumeLabel : {1}", num, d.VolumeLabel);
                    sr.WriteLine("Drive{0}.TotalSpace  : {1}", num, d.TotalSize.ToPrettySize(4));
                    sr.WriteLine("Drive{0}.FreeSpace   : {1}", num, d.AvailableFreeSpace.ToPrettySize(4));
                    var percentFree = Math.Round(100 * (double)d.TotalFreeSpace / d.TotalSize, 2);
                    sr.WriteLine("Drive{0}.PercentFree : {1}", num, percentFree.ToString(CultureInfo.InvariantCulture) + " %");
                    sr.WriteLine();
                    num++;
                }
            }
        }
    }

    #endregion

#region SupportInfo Item Types
    // item types that can be collected and exported
    //
    public enum SupportInfoItemType
    {
        RegistryKey,
        DataFile,
        AssemblyInfo,
        SystemInfo
    }

#endregion


}
