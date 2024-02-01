using Business;
using Microsoft.AspNetCore.Mvc;
using Utilites;

namespace GAMAX.Services.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StaticFilesController : ControllerBase
    {
        private readonly string directoryPath /*= Directory.GetParent(Directory.GetCurrentDirectory())?.FullName*/;
        private readonly IWebHostEnvironment _env;
        public StaticFilesController(IWebHostEnvironment env)
        {
            _env = env;
            directoryPath = SharedFolderPaths.HostedFolderPath;
        }

        [HttpGet("download")]
        public IActionResult DownloadFile(string filePath)
        {
            var FullPath =Path.Combine(directoryPath, filePath);
            if (string.IsNullOrEmpty(FullPath) || !System.IO.File.Exists(FullPath))
            {
                return NotFound();
            }

            string fileExtension = Path.GetExtension(FullPath).ToLower();

            string contentType;
            if (IsImageExtension(fileExtension))
            {
                contentType = "image/" + fileExtension.Substring(1);
            }
            else if (IsVideoExtension(fileExtension))
            {
                contentType = "video/" + fileExtension.Substring(1);
            }
            else
            {
                return BadRequest("Invalid file extension."); 
            }

            // Read the file bytes
            byte[] fileBytes = System.IO.File.ReadAllBytes(FullPath);

            // Create a file content result
            var fileContentResult = new FileContentResult(fileBytes, contentType)
            {
                FileDownloadName = Path.GetFileName(FullPath) // Set the file name for downloading
            };

            return fileContentResult;
        }
        [HttpGet("downloadProfilePhoto")]
        public IActionResult downloadProfilePhoto(string UserID)
        {
            var filePath = Path.Combine(directoryPath, SharedFolderPaths.ProfilePhotos,UserID+ "Profile.jpg");
            if (string.IsNullOrEmpty(filePath) || !System.IO.File.Exists(filePath))
            {
                return NotFound(); 
            }

            // Get the file extension from the file path
            string fileExtension = Path.GetExtension(filePath).ToLower();

            // Set the content type based on the file extension
            string contentType;
            if (IsImageExtension(fileExtension))
            {
                contentType = "image/" + fileExtension.Substring(1);
            }
            else if (IsVideoExtension(fileExtension))
            {
                contentType = "video/" + fileExtension.Substring(1);
            }
            else
            {
                return BadRequest("Invalid file extension."); 
            }

            // Read the file bytes
            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);

            // Create a file content result
            var fileContentResult = new FileContentResult(fileBytes, contentType)
            {
                FileDownloadName = Path.GetFileName(filePath)
            };

            return fileContentResult;
        }
        [HttpGet("downloadCoverPhoto")]
        public IActionResult downloadCoverPhoto(string UserID)
        {
            var filePath = Path.Combine(directoryPath, SharedFolderPaths.CoverPhotos, UserID + "Cover.jpg");
            if (string.IsNullOrEmpty(filePath) || !System.IO.File.Exists(filePath))
            {
                return NotFound();
            }
            string fileExtension = Path.GetExtension(filePath).ToLower();

            string contentType;
            if (IsImageExtension(fileExtension))
            {
                contentType = "image/" + fileExtension.Substring(1);
            }
            else if (IsVideoExtension(fileExtension))
            {
                contentType = "video/" + fileExtension.Substring(1);
            }
            else
            {
                return BadRequest("Invalid file extension.");
            }

            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
            var fileContentResult = new FileContentResult(fileBytes, contentType)
            {
                FileDownloadName = Path.GetFileName(filePath) 
            };

            return fileContentResult;
        }

        private bool IsImageExtension(string fileExtension)
        {
            string[] imageExtensions = { ".jpg", ".jpeg", ".png", ".gif" }; // Add more image extensions as needed
            return imageExtensions.Contains(fileExtension);
        }

        private bool IsVideoExtension(string fileExtension)
        {
            string[] videoExtensions = { ".mp4", ".avi", ".mov", ".wmv" }; // Add more video extensions as needed
            return videoExtensions.Contains(fileExtension);
        }

    }
}
