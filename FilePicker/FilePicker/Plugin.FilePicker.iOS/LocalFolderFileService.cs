using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Plugin.FilePicker.Abstractions;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;

namespace Plugin.FilePicker
{
    public class LocalFolderFileService : FilePickerImplementation
    {
        public override async Task<FileData> PickFile()
        {
            var storageStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Storage);
            if (storageStatus != PermissionStatus.Granted)
            {
                var result = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Storage);
                storageStatus = result[Permission.Storage];
            }

            if (storageStatus == PermissionStatus.Granted)
            {
                var file = await CrossMedia.Current.PickPhotoAsync();

                return new FileData(file.Path, GetPickedFileName(file), file.GetStream);
            }
            else
                CrossPermissions.Current.OpenAppSettings();

            return null;

            string GetPickedFileName(MediaFile file) => file.Path.Substring(
                file.Path.LastIndexOf("/", StringComparison.Ordinal) + 1);
        }

        private readonly string _localFolderPath =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "..", "Library");

        public override Task<bool> SaveFile(FileData fileToSave)
        {
            try
            {
                var savedFilePath = Path.Combine(_localFolderPath, fileToSave.FileName);

                if (File.Exists(savedFilePath)) return Task.FromResult(true);

                File.WriteAllBytes(savedFilePath, fileToSave.DataArray);

                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return Task.FromResult(false);
            }
        }

        public override Task<bool> SaveFile(FileData fileToSave, string destinationFolderName)
        {
            try
            {
                var nestedDirectoryPath = Path.Combine(_localFolderPath, destinationFolderName);

                Directory.CreateDirectory(nestedDirectoryPath);

                var savedFilePath = Path.Combine(nestedDirectoryPath, fileToSave.FileName);

                if (File.Exists(savedFilePath)) return Task.FromResult(true);

                File.WriteAllBytes(savedFilePath, fileToSave.DataArray);

                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return Task.FromResult(false);
            }
        }
    }
}