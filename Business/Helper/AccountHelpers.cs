using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilites;

namespace Business.Helper
{
    public static class AccountHelpers
    {
        private static string parentFolder = SharedFolderPaths.HostedFolderPath;/* Directory.GetParent(Directory.GetCurrentDirectory()).FullName;*/
        public static string GetDefaultProfilePohot(Guid id)
        {
            var sourceFile = Path.Combine(parentFolder, SharedFolderPaths.ProfilePhotos);
            CopyAndRenamePhoto(sourceFile, sourceFile, "Profile.jpg", id.ToString() + "Profile.jpg");
            return Path.Combine(SharedFolderPaths.ProfilePhotos, id.ToString() + "Profile.jpg");
        }
        public static string GetDefaultCoverPohot(Guid id)
        {
            var sourceFile = Path.Combine(parentFolder, SharedFolderPaths.CoverPhotos);
            CopyAndRenamePhoto(sourceFile, sourceFile, "Cover.jpg", id.ToString() + "Cover.jpg");
            return Path.Combine(SharedFolderPaths.CoverPhotos, id.ToString() + "Cover.jpg");
        }
        public static string IformToProfilePath(IFormFile formFile, Guid userId)
        {
            if (formFile != null && formFile.Length > 0)
            {
                string newFileName = $"{userId}Profile.jpg";
                string newFilePath = Path.Combine(parentFolder, SharedFolderPaths.ProfilePhotos, newFileName);

                using (var fileStream = new FileStream(newFilePath, FileMode.Create))
                {
                    formFile.CopyTo(fileStream);
                }

                return Path.Combine(SharedFolderPaths.ProfilePhotos, newFileName);
            }
            return string.Empty;

        }
        public static string IformToCoverPath(IFormFile formFile, Guid userId)
        {
            if (formFile != null && formFile.Length > 0)
            {
                string newFileName = $"{userId}Cover.jpg";
                string newFilePath = Path.Combine(parentFolder, SharedFolderPaths.CoverPhotos, newFileName);

                using (var fileStream = new FileStream(newFilePath, FileMode.Create))
                {
                    formFile.CopyTo(fileStream);
                }
                return Path.Combine(SharedFolderPaths.CoverPhotos, newFileName);
            }
            return string.Empty;

        }

        public static void CopyAndRenamePhoto(string sourceFolderPath, string destinationFolderPath, string photoName, string newPhotoName)
        {
            try
            {
                string sourceFilePath = Path.Combine(sourceFolderPath, photoName);
                string destinationDirectoryPath = Path.Combine(destinationFolderPath);
                string destinationFilePath = Path.Combine(destinationDirectoryPath, newPhotoName);

                if (File.Exists(sourceFilePath))
                {
                    File.Copy(sourceFilePath, destinationFilePath, true);

                }
            }
            catch (Exception ex)
            {
            }
        }

    }
}
