using Microsoft.AspNetCore.SignalR;
namespace Authentication.PL.Helpers
{
    public static class DocumentSettings
    {
        private static string GetFolderPath(string folderName)
        {
            // Get the full folder path
            string folderPath = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\Files\\{folderName}");

            // Ensure the folder exists
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            return folderPath;
        }

        private static string GetSafeFileName(string fileName)
        {
            // Generate a unique file name and remove invalid characters
            string safeFileName = $"{Guid.NewGuid()}_{Path.GetFileName(fileName)}";
            return string.Concat(safeFileName.Split(Path.GetInvalidFileNameChars()));
        }

        public static async Task<string> UploadAsync(IFormFile file, string folderName)
        {
            try
            {
                // Get folder and file paths
                string folderPath = GetFolderPath(folderName);
                string fileName = GetSafeFileName(file.FileName);
                string fullPath = Path.Combine(folderPath, fileName);

                // Create the file stream and copy the file
                using (var fileStream = new FileStream(fullPath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                // Return relative file path
                return Path.Combine($"Files/{folderName}", fileName);
            }
            catch (Exception ex)
            {
                // Log or handle the error appropriately
                throw new InvalidOperationException("Error uploading file", ex);
            }
        }

        public static async Task<string> UpdateFileAsync(IFormFile newFile, string oldFilePath, string folderName)
        {
            try
            {
                // Delete the old file if it exists
                DeleteFile(oldFilePath);

                // Upload the new file
                return await UploadAsync(newFile, folderName);
            }
            catch (Exception ex)
            {
                // Log or handle the error appropriately
                throw new InvalidOperationException("Error updating file", ex);
            }
        }

        public static bool DeleteFile(string filePath)
        {
            try
            {
                // Get full path to the file
                string fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", filePath);

                // Check if the file exists and delete it
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                // Log or handle the error appropriately
                throw new InvalidOperationException("Error deleting file", ex);
            }
        }
    }
}

