using System.Threading.Tasks;

namespace Plugin.FilePicker.Abstractions
{
    /// <summary>
    /// Interface for FilePicker
    /// </summary>
    public interface IFilePicker
    {
        Task<FileData> PickFile ();

        Task<string> GetLocalPathAsync(string fileName);
        Task<bool> SaveFile (FileData fileToSave);
        Task<bool> SaveFile(FileData fileToSave, string destinationFolderName);

        void OpenFile (string fileToOpen);

        void OpenFile (FileData fileToOpen);
    }
}