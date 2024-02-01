using DomainModels.Models;
using Utilites;

namespace Business.Helper
{
    public static class PostHelper
    {
        public static AllPostsModel ConvertToPaths(UploadPost post, string photoFolderPath, string videoFolderPath)
        {
            var model = mappingModel(post);
            if (model.Photos != null)
            {
                List<string> photoPaths = new List<string>();
                foreach (var photo in model.Photos)
                {
                    var path = MediaUtilites.ConverIformToPath(photo, photoFolderPath);
                    if (path != null)
                        photoPaths.Add(path);

                }
                model.PhotosPath = photoPaths;
            }

            if (model.Vedios != null)
            {
                List<string> videoPaths = new List<string>();
                foreach (var video in model.Vedios)
                {
                    var path = MediaUtilites.ConverIformToPath(video, videoFolderPath);
                    if (path != null)
                        videoPaths.Add(path);
                }
                model.VediosPath = videoPaths;
            }

            return model;
        }
        public static AllPostsModel mappingModel(UploadPost post)
        {
            var model = new AllPostsModel()
            {
                Answer = post.Answer,
                Description = post.Description,
                Photos = post.Photos,
                Question = post.Question,
                TimeCreated = DateTime.UtcNow,
                Title = post.Title,
                Type = post.Type,
                Vedios = post.Vedios,
            };
            return model;
        }

    }
}
