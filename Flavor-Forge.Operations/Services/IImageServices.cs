namespace Flavor_Forge.Operations.Services
{
    public interface IImageServices
    {
        Task<string> SaveImageAsync(IFormFile imageFile, string folderPath);
        bool ValidateImage(IFormFile imageFile, out string errorMessage);
        void DeleteImage(string imageUrl, string folderPath);
    }
}
