using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Png;


namespace iBartender.Application.Utils
{
    public class ImageProcessor : IImageProcessor
    {
        private bool CheckSignature(byte[] fileBytes)
        {
            // Проверка сигнатур
            // JPEG
            if (fileBytes[0] == 0xFF && fileBytes[1] == 0xD8 && fileBytes[2] == 0xFF)
                return true;

            // PNG
            if (fileBytes[0] == 0x89 && fileBytes[1] == 0x50 && fileBytes[2] == 0x4E &&
                fileBytes[3] == 0x47 && fileBytes[4] == 0x0D && fileBytes[5] == 0x0A &&
                fileBytes[6] == 0x1A && fileBytes[7] == 0x0A)
                return true;

            // GIF
            if (fileBytes[0] == 0x47 && fileBytes[1] == 0x49 && fileBytes[2] == 0x46)
                return true;

            // BMP
            if (fileBytes[0] == 0x42 && fileBytes[1] == 0x4D)
                return true;

            return false;
        }

        public bool Validate(IFormFile file)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".bmp" };

            if (file == null || file.Length == 0)
            {
                return false;
            }

            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (string.IsNullOrEmpty(fileExtension) || !allowedExtensions.Contains(fileExtension))
            {
                return false;
            }

            //using (var stream = file.OpenReadStream())
            //{
            //    var headers = new byte[8];
            //    stream.Read(headers, 0, headers.Length);
            //    stream.Position = 0;
            //
            //    if (!IsImage(headers))
            //    {
            //        return false;
            //    }
            //}

            return true;
        }

        public byte[] ProcessProfilePhoto(IFormFile imageFile, int targetSize)
        {
            using var image = Image.Load(imageFile.OpenReadStream());

            int originalSquareSize = Math.Min(image.Width, image.Height);
            var cropRectangle = new Rectangle(
                x: (image.Width - originalSquareSize) / 2,
                y: (image.Height - originalSquareSize) / 2,
                width: originalSquareSize,
                height: originalSquareSize);

            image.Mutate(x => x.Crop(cropRectangle));

            image.Mutate(x => x.Resize(targetSize, targetSize));

            using var memoryStream = new MemoryStream();
            image.Save(memoryStream, new PngEncoder());

            return memoryStream.ToArray();
        }
        
        public byte[] ProcessPublicationPhoto(IFormFile imageFile)
        {
            using var image = Image.Load(imageFile.OpenReadStream());
            using var memoryStream = new MemoryStream();
            image.Save(memoryStream, new PngEncoder());

            return memoryStream.ToArray();
        }
        
        public void SaveAsPng(byte[] imageData, string outputPath)
        {
            // Создаем все директории в пути, если их нет
            var directory = Path.GetDirectoryName(outputPath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.WriteAllBytes(outputPath, imageData);
        }
    }
}
