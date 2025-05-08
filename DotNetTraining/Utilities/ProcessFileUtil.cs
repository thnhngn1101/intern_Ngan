namespace BPMaster.Utilities
{
    public class Config
    {
        public static string tempFolderName = "temp";
        public static string rootPath = Environment.GetEnvironmentVariable("STORAGE_ROOT");
        public static string tempPath = rootPath + "/" + tempFolderName;

    }
    public class ProcessFileUtil
    {
        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static byte[]? ReadAllBytes(string PhysicalPath)
        {
            try
            {              
                byte[] data = File.ReadAllBytes(PhysicalPath);
                return data;
            }
            catch
            {
                return null;
            }       
        }
        public static bool RemoveIfExist(string relativeFilePath)
        {
            try
            {
                if (File.Exists(relativeFilePath))
                {
                    File.Delete(relativeFilePath);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
           
        }
        public static string GetPhysicalPath(string relativeFilePath)
        {
            string absolutePath = Config.rootPath + "/" + relativeFilePath;
            return absolutePath.Replace("//", "/");
        }
        public static string GetTempPath()
        {
            return Config.tempPath;
        }
        public static bool IsFileExists(string relativeFilePath)
        {
            string absolutePath = Config.rootPath + "/" + relativeFilePath;
            try
            {
                if (File.Exists(absolutePath.Replace("//", "/")))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {

            }

            return false;
        }
        public async Task CopyFileAsync(string sourcePath, string destinationPath)
        {
            using (Stream source = File.Open(sourcePath, FileMode.CreateNew))
            {
                using (Stream destination = File.Create(destinationPath))
                {
                    await source.CopyToAsync(destination);
                }
            }
        }
        public static bool SaveAsAttachmentUpload(IFormFile item, string FilePhysicalPath)
        {
            // Define the path to save the file
            try 
            {
                var dirPhysicalPath = Path.GetDirectoryName(FilePhysicalPath);

                if (!Directory.Exists(dirPhysicalPath))
                {
                    Directory.CreateDirectory(dirPhysicalPath);
                }

                using (var stream = System.IO.File.Create(FilePhysicalPath))
                {
                    item.CopyTo(stream);
                }

                return true;
            }
            catch
            {
                return false;
            }         
        }
        public static bool ByteArrayToFile(string filePath, byte[] byteArray)
        {
            try
            {
                if (!Directory.Exists(Path.GetDirectoryName(filePath)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                }
                using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    fs.Write(byteArray, 0, byteArray.Length);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught in process: {0}", ex);
                return false;
            }
        }
        //Set to public when debug then change to protected after done
        protected static string SaveAsDebug(string FilePhysicalPath, IFormFile item, Boolean isEncrypt = true)
        {
            string DraftPath = Config.tempPath;
            // FilePhysicalPath must have '/' before
            FilePhysicalPath = Config.rootPath + FilePhysicalPath;
            var fileName = DateTime.Now.ToString("ddMMyyyyHHmmssfff") + (new Random()).Next(1000, 99999).ToString();
            string pathDraft = DraftPath + "/" + fileName;
            try
            {

                if (!Directory.Exists(DraftPath))
                {
                    Directory.CreateDirectory(DraftPath);
                }
                var dirPhysicalPath = Path.GetDirectoryName(FilePhysicalPath);
                if (!Directory.Exists(dirPhysicalPath))
                {
                    Directory.CreateDirectory(dirPhysicalPath);
                }

                if (!isEncrypt)
                {
                    using (var stream = System.IO.File.Create(FilePhysicalPath))
                    {
                        item.CopyTo(stream);
                    }
                    return "true";
                }

                System.IO.FileStream fileStream = System.IO.File.Create(pathDraft, (int)item.Length);
                // Initialize the bytes array with the stream length and then fill it with data
                BinaryReader reader = new BinaryReader(item.OpenReadStream());
                byte[] bytesInStream = reader.ReadBytes((int)item.Length);
                fileStream.Write(bytesInStream, 0, bytesInStream.Length);
                fileStream.Close();
                using (FileStream fsSource = new FileStream(pathDraft, FileMode.Open, FileAccess.Read))
                {
                    int iMask = 7;
                    byte[] buffer = new byte[16 * 1024];
                    int read;
                    while ((read = fsSource.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        for (int i = 0; i < read; i++)
                        {
                            buffer[i] = Convert.ToByte(buffer[i] ^ iMask);
                        }

                        // Write the byte array to the other FileStream.
                        using (FileStream fsNew = new FileStream(FilePhysicalPath, FileMode.Append, FileAccess.Write))
                        {
                            fsNew.Write(buffer, 0, read);
                        }
                    }
                }
                return "true";
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
            finally
            {
                File.Delete(pathDraft);
            }
        }

    }
}
