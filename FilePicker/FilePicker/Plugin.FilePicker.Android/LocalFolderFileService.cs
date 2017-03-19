using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Java.IO;
using Plugin.FilePicker.Abstractions;
using File = System.IO.File;

namespace Plugin.FilePicker
{
    public class LocalFolderFileService : FilePickerImplementation
    {
        private readonly string _localFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

        public LocalFolderFileService() : base()
        {
        }

        public override async Task<bool> SaveFile(FileData fileToSave)
        {
            try
            {
                var savedFilePath = Path.Combine(_localFolderPath, fileToSave.FileName);

                if (File.Exists(savedFilePath)) return true;

                var fos = new FileOutputStream(savedFilePath);

                fos.Write(fileToSave.DataArray);
                fos.Close();

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }

        public override async Task<bool> SaveFile(FileData fileToSave, string destinationFolderName)
        {
            try
            {
                var nestedDirectoryInfo = Directory.CreateDirectory(Path.Combine(_localFolderPath, destinationFolderName));

                var savedFilePath = Path.Combine(nestedDirectoryInfo.FullName, fileToSave.FileName);

                if (File.Exists(savedFilePath)) return true;

                var fos = new FileOutputStream(savedFilePath);

                fos.Write(fileToSave.DataArray);
                fos.Close();

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }

        public override async Task<string> GetLocalPathAsync(string fileName) => await Task.FromResult(
            Path.Combine(_localFolderPath, fileName));
    }
}