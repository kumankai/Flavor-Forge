/*
 * This repository defines the services for image handling.
 * Specifically images saved in /wwwroot.
 */

using Flavor_Forge.Operations.Services.Service;

namespace Flavor_Forge.Operations.Services.Repository
{
    public class ImageRepository : IImageServices
    {
        private readonly string[] _allowedFileTypes = { "image/jpeg", "image/jpg", "image/png" };
        private const long _maxFileSize = 5 * 1024 * 1024; // 5MB

        // Validates image
        public bool ValidateImage(IFormFile imageFile, out string errorMessage)
        {
            errorMessage = string.Empty;

            // If file is null
            if (imageFile == null || imageFile.Length == 0)
            {
                errorMessage = "No image file provided.";
                return false;
            }

            // If file is too big
            if (imageFile.Length > _maxFileSize)
            {
                errorMessage = "Image file size must be less than 5MB.";
                return false;
            }

            // If wrong file type
            if (!_allowedFileTypes.Contains(imageFile.ContentType.ToLower()))
            {
                errorMessage = "Only JPG and PNG images are allowed.";
                return false;
            }

            return true;
        }

        //  Saves image to folderPath
        public async Task<string> SaveImageAsync(IFormFile imageFile, string folderPath)
        {
            // Creates a random file name
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(imageFile.FileName)}";
            // Specifices file path
            var filePath = Path.Combine(folderPath, fileName);
            // Creates the directory if not existing
            Directory.CreateDirectory(folderPath);

            // Saves the image to filePath
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            return $"/recipe-images/{fileName}"; // Return relative URL
        }

        // Deletes images from folderPath
        public void DeleteImage(string imageUrl, string folderPath)
        {
            // Grabs filename
            var fileName = Path.GetFileName(imageUrl);
            // Gets complete path
            var filePath = Path.Combine(folderPath, fileName);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }
}
