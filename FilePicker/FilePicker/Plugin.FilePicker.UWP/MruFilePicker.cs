using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Plugin.FilePicker.Abstractions;

namespace Plugin.FilePicker
{
    public class MruFilePicker : FilePickerImplementation
    {
        private readonly StorageItemMostRecentlyUsedList _mru = StorageApplicationPermissions.MostRecentlyUsedList;
        private string _mruToken;

        public override async Task<FileData> PickFile()
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker
            {
                ViewMode = Windows.Storage.Pickers.PickerViewMode.List,
                SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary
            };
            picker.FileTypeFilter.Add("*");

            var file = await picker.PickSingleFileAsync();

            if (file != null)
            {
                _mruToken = _mru.Add(file);

                return new FileData(file.Path, file.Name, () => File.OpenRead(file.Path));
            }

            return null;
        }

        public override async Task<bool> SaveFile(FileData fileToSave)
        {
            if (!_mru.ContainsItem(_mruToken)) return false;

            try
            {
                var mruFile = await _mru.GetFileAsync(_mruToken);

                await mruFile.CopyAsync(ApplicationData.Current.LocalFolder);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public override async Task<bool> SaveFile(FileData fileToSave, string destinationFolderName)
        {
            if (!_mru.ContainsItem(_mruToken)) return false;

            try
            {
                var mruFile = await _mru.GetFileAsync(_mruToken);

                var nestedFolder =
                    await ApplicationData.Current.LocalFolder.CreateFolderAsync(destinationFolderName,
                        CreationCollisionOption.OpenIfExists);

                await mruFile.CopyAsync(nestedFolder);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}