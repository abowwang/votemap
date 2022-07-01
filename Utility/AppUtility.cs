using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using System.Text.Json;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace VoteMap.Core.Utility
{
    public interface IAppUtility
    {
        string ReadAppSetting(string field);
        Task<bool> CopyStream2Data(Stream stream, string fileName);
        string GetDataPath(string fileName);
        string ConvertObj2String<T>(T input);
        T ConvertString2Obj<T>(string input) where T : new();
    }

    public class AppUtility : IAppUtility
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private static readonly String projectPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private static readonly String dataPath = @"Data";

        #region ReadAppSetting
        public string ReadAppSetting(string field)
        {
            var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", false)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();
            return config[field];
        }
        #endregion

        #region CopyStream2Data 
        public async Task<bool> CopyStream2Data(Stream stream, string fileName)
        {
            string destPath = GetDataPath(fileName);

            try
            {
                using (var fileStream = new FileStream(destPath, FileMode.Create, FileAccess.Write))
                {
                    await stream.CopyToAsync(fileStream);
                }
            }
            catch (Exception e)
            {
                logger.Debug(e.StackTrace);
                logger.Error(e.Message);
                return false;
            }
            return true;
        }
        #endregion

        #region GetDataPath
        public string GetDataPath(string fileName)
        {
            if (!Directory.Exists(Path.Combine(projectPath, dataPath)))
            {
                Directory.CreateDirectory(Path.Combine(projectPath, dataPath));
            }
            string destPath = Path.Combine(projectPath, dataPath, fileName);
            return destPath;
        }
        #endregion

        #region ConvertObj2String
        public string ConvertObj2String<T>(T input)
        {
            return JsonSerializer.Serialize(input, new JsonSerializerOptions { Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping });
        }
        #endregion

        #region ConvertString2Obj
        public T ConvertString2Obj<T>(string input) where T : new()
        {
            return JsonSerializer.Deserialize<T>(input, new JsonSerializerOptions { IgnoreNullValues = true });
        }
        #endregion
    }
}
