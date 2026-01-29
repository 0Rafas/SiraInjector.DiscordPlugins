using System;
using System.IO;
using System.Text;

namespace LoveStreakPlugin
{
    public class PluginInjector
    {
        private static readonly string[] DiscordPaths = new string[]
        {
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Discord"),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "discordcanary"),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "discordptb"),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Discord"),
        };

        public class InjectionResult
        {
            public bool Success { get; set; }
            public string Message { get; set; }
            public Exception Exception { get; set; }
        }

        public static InjectionResult ValidatePluginFile(string filePath)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(filePath))
                    return new InjectionResult 
                    { 
                        Success = false, 
                        Message = "Please select a plugin file first." 
                    };

                if (!File.Exists(filePath))
                    return new InjectionResult 
                    { 
                        Success = false, 
                        Message = "The selected file does not exist." 
                    };

                string extension = Path.GetExtension(filePath).ToLower();
                if (extension != ".js" && extension != ".ts")
                    return new InjectionResult 
                    { 
                        Success = false, 
                        Message = "Invalid file format. Plugin must be .js or .ts file." 
                    };

                FileInfo fileInfo = new FileInfo(filePath);
                if (fileInfo.Length > 10 * 1024 * 1024)
                    return new InjectionResult 
                    { 
                        Success = false, 
                        Message = "File size is too large (max 10MB)." 
                    };

                return new InjectionResult 
                { 
                    Success = true, 
                    Message = "File is valid." 
                };
            }
            catch (Exception ex)
            {
                return new InjectionResult 
                { 
                    Success = false, 
                    Message = "Error during file validation: " + ex.Message,
                    Exception = ex
                };
            }
        }

        private static string FindDiscordFolder()
        {
            foreach (var path in DiscordPaths)
            {
                if (Directory.Exists(path))
                    return path;
            }
            return null;
        }

        public static InjectionResult InjectPlugin(string pluginFilePath)
        {
            try
            {
                var validationResult = ValidatePluginFile(pluginFilePath);
                if (!validationResult.Success)
                    return validationResult;

                string discordPath = FindDiscordFolder();
                if (string.IsNullOrEmpty(discordPath))
                    return new InjectionResult 
                    { 
                        Success = false, 
                        Message = "Discord installation not found on this system." 
                    };

                string pluginsFolder = Path.Combine(discordPath, "plugins");
                if (!Directory.Exists(pluginsFolder))
                    Directory.CreateDirectory(pluginsFolder);

                string fileName = Path.GetFileName(pluginFilePath);
                string destinationPath = Path.Combine(pluginsFolder, fileName);

                if (File.Exists(destinationPath))
                {
                    File.Delete(destinationPath);
                }

                File.Copy(pluginFilePath, destinationPath, true);

                if (!File.Exists(destinationPath))
                    return new InjectionResult 
                    { 
                        Success = false, 
                        Message = "Failed to copy plugin file to Discord plugins folder." 
                    };

                return new InjectionResult 
                { 
                    Success = true, 
                    Message = "Plugin injected successfully!\n\n" +
                              "File: " + fileName + "\n" +
                              "Location: " + destinationPath + "\n\n" +
                              "Please restart Discord to apply the plugin." 
                };
            }
            catch (UnauthorizedAccessException)
            {
                return new InjectionResult 
                { 
                    Success = false, 
                    Message = "Access denied. Please run this application as administrator." 
                };
            }
            catch (IOException ex)
            {
                return new InjectionResult 
                { 
                    Success = false, 
                    Message = "File access error:\n" + ex.Message 
                };
            }
            catch (Exception ex)
            {
                return new InjectionResult 
                { 
                    Success = false, 
                    Message = "An error occurred during injection:\n" + ex.Message,
                    Exception = ex
                };
            }
        }
    }
}
