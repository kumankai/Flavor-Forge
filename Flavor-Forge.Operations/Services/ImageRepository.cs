namespace Flavor_Forge.Operations.Services
{
    public class ImageRepository : IImageServices
    {
        private readonly string[] _allowedFileTypes = { "image/jpeg", "image/jpg", "image/png" };
        private const long _maxFileSize = 5 * 1024 * 1024; // 5MB

        public bool ValidateImage(IFormFile imageFile, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (imageFile == null || imageFile.Length == 0)
            {
                errorMessage = "No image file provided.";
                return false;
            }

            if (imageFile.Length > _maxFileSize)
            {
                errorMessage = "Image file size must be less than 5MB.";
                return false;
            }

            if (!_allowedFileTypes.Contains(imageFile.ContentType.ToLower()))
            {
                errorMessage = "Only JPG and PNG images are allowed.";
                return false;
            }

            return true;
        }

        public async Task<string> SaveImageAsync(IFormFile imageFile, string folderPath)
        {
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(imageFile.FileName)}";
            var filePath = Path.Combine(folderPath, fileName);

            Directory.CreateDirectory(folderPath);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            return $"/recipe-images/{fileName}"; // Return relative URL
        }

        public void DeleteImage(string imageUrl, string folderPath)
        {
            var fileName = Path.GetFileName(imageUrl);
            var filePath = Path.Combine(folderPath, fileName);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }
}
