using System.IO;

namespace Siscom.Agua.Api.Services.Extension
{
    public static class IOExtensions
    {
        public static void Rename(this FileInfo fileInfo, string newName)
        {
            fileInfo.MoveTo(fileInfo.Directory.FullName + "\\" + newName);
        }
    }
}
