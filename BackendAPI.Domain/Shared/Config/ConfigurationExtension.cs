using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace Database.Config;

public static class ConfigurationExtension
{
    public static WebApplicationBuilder AddStageConfig(this WebApplicationBuilder builder)
    {
        var contentRootPath = builder.Environment.ContentRootPath;

        //var appSettingFolderPath = Path.Combine(contentRootPath, "..",  "appsetting.json");

        var appSettingFolderPath = Path.Combine(contentRootPath, "..", "appsetting.json");
        // var appSettingFolderPath = "C:\\DotNetTraining\\Mini App Management System\\AppManagementSystem\\appsetting.json";
        builder.Configuration.AddJsonFile(appSettingFolderPath, optional: false, reloadOnChange: true);


        //var contentRootPath = builder.Environment.ContentRootPath;

        //// Move up to the solution folder (one level up from Backendapi.AppManagementSystem)
        //var solutionRoot = Path.GetFullPath(Path.Combine(contentRootPath, ".."));

        //// Combine with the actual folder where appsettings.json is located
        //var appSettingsFilePath = Path.Combine(solutionRoot, ".Config", "appsetting.json");

        //// Print path to verify
        //Console.WriteLine("Loading appsettings from: " + appSettingsFilePath);

        //// Load configuration
        //builder.Configuration.AddJsonFile(appSettingsFilePath, optional: false, reloadOnChange: true);





        var config = (IConfigurationBuilder)builder.Configuration;
        var tempConfig = config.Build();
        var stage = tempConfig.GetSection("Stage")?.Value?.ToLower();

        if (string.IsNullOrEmpty(stage))
        {
            throw new Exception("The 'Stage' key is missing or empty in appsettings.json.");
        }

        var folderPath = Path.Combine(contentRootPath, "..");
        if (!Directory.Exists(folderPath))
        {
            throw new Exception($"Config folder must be created in the project with a custom-setting-{stage}.json file.");
        }

        var customSettingFilePath = Path.Combine(folderPath, $"custom-setting-{stage}.json");
        Console.WriteLine("Custom Setting JSON File Path: " + customSettingFilePath);

        builder.Configuration.AddJsonFile(customSettingFilePath, optional: false, reloadOnChange: true);

        return builder;
    }
}
