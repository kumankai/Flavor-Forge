/*
 * This service is for image handling such as saving, validating, and deleting
 */

namespace Flavor_Forge.Operations.Services.Service
{
    public interface IImageServices
    {
        Task<string> SaveImageAsync(IFormFile imageFile, string folderPath);
        bool ValidateImage(IFormFile imageFile, out string errorMessage);
        void DeleteImage(string imageUrl, string folderPath);
    }
}
