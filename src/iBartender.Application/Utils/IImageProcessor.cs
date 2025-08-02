using Microsoft.AspNetCore.Http;

namespace iBartender.Application.Utils
{
    public interface IImageProcessor
    {
        bool Validate(IFormFile file);

        byte[] ProcessProfilePhoto(IFormFile imageFile, int targetSize);

        byte[] ProcessPublicationPhoto(IFormFile imageFile);

        void SaveAsPng(byte[] imageData, string outputPath);
    }
}