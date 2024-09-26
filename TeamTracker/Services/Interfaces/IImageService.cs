namespace TeamTracker.Services.Interfaces
{
    public interface IImageService
    {
        public Task<byte[]> ConvertFileToByteArrayAsync(IFormFile file);
        public string ConvertByteArrayToFile(byte[] fileData, string extension);
    }
}


// 1. services, dependency injection, and interfaces
// 2. create our IImageService interface
// 3. handle our incoming image file
// 4. add suffixes (size)
// 5. show a default image if none is uploaded
// 6. convert the file data
// 7. The image to the database
// 8. change the image in the view using JavaScript
// 9. register our IImageService
