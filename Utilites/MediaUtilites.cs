using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilites
{
    public static class MediaUtilites
    {
        private static string FolderPath = SharedFolderPaths.HostedFolderPath; /*Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory())?.FullName);*/

        public static string ConverIformToPath(IFormFile formFile, string FileFolderPath)
        {
            if (formFile != null && formFile.Length > 0)
            {
                string fileExtension = Path.GetExtension(formFile.FileName);
                string newFileName = $"{Guid.NewGuid()}{fileExtension}";
                string newFilePath = Path.Combine(FolderPath, FileFolderPath, newFileName);

                using (var fileStream = new FileStream(newFilePath, FileMode.Create))
                {
                    formFile.CopyTo(fileStream);
                }

                return (Path.Combine(FileFolderPath, newFileName));
            }
            return string.Empty;

        }
        public static void DeleTeMediaPath(string mediaPath)
        {
            var fullPath = Path.Combine(FolderPath, mediaPath);
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
            return;

        }
    }
}
