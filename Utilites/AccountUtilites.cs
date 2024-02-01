using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilites
{ 
    public static class AccountUtilites
    {
        private static string PhotoPath = Path.Combine(SharedFolderPaths.HostedFolderPath, "StaticFiles", "Photos");
        public static MemoryStream  GetCoverPhoto(string PhotoName)
        {
            var path = Path.Combine(PhotoPath, "CoverPhoto", PhotoName);
            if (!File.Exists(path))
                return null;
            return new MemoryStream(File.ReadAllBytes(path));
        }
        public static MemoryStream GetProfilehoto(string photoName)
        {
            var path = Path.Combine(PhotoPath, "ProfilePhoto", photoName);
            if (!File.Exists(path))
                return null;
            return new MemoryStream(File.ReadAllBytes(path));
        }
        public static List<MemoryStream> GetPostPhotos(List<string> photoNames)
        {
            var photoStreams = new List<MemoryStream>();
            var photoFolderPath = Path.Combine(PhotoPath, "PostPhoto");

            foreach (var photoName in photoNames)
            {
                var photoPath = Path.Combine(photoFolderPath, photoName);

                if (File.Exists(photoPath))
                {
                    var fileBytes = File.ReadAllBytes(photoPath);
                    var memoryStream = new MemoryStream(fileBytes);
                    photoStreams.Add(memoryStream);
                }
            }

            return photoStreams;
        }
        public static List<MemoryStream> GetQuestionPostPhotos(List<string> photoNames)
        {
            var photoStreams = new List<MemoryStream>();
            var photoFolderPath = Path.Combine(PhotoPath, "QuestionPhoto");

            foreach (var photoName in photoNames)
            {
                var photoPath = Path.Combine(photoFolderPath, photoName);

                if (File.Exists(photoPath))
                {
                    var fileBytes = File.ReadAllBytes(photoPath);
                    var memoryStream = new MemoryStream(fileBytes);
                    photoStreams.Add(memoryStream);
                }
            }

            return photoStreams;
        }



    }
}
