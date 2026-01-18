using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using BackendAPI.Domain.Shared;
using Microsoft.Extensions.Options;
using System.Text;

namespace BackendAPI.Domain.Filters;


public class DecryptionFilter : IAsyncActionFilter
{
    private readonly IOptions<CustomSettingModel> _settings;

    public DecryptionFilter(IOptions<CustomSettingModel> settings)
    {
        _settings = settings;
    }

    public class EncryptedRequestDto
{
    public string ReqData
    {
        get; set;
    }
}

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var httpContext = context.HttpContext;
        if (!_settings.Value.EnableEncryption)
        {
            httpContext.Items["DecryptedBody"] = context.ActionArguments["request"];
            await next();
            return;
        }

        var obj = context.ActionArguments["request"] as object;
        var result = obj.ToString();
        var encryptedDto = JsonConvert.DeserializeObject<EncryptedRequestDto>(result!,
                new JsonSerializerSettings { DateParseHandling = DateParseHandling.DateTimeOffset });

        var decryptedJson = Decrypt(encryptedDto.ReqData);

        var targetDto = JsonConvert.DeserializeObject<object>(decryptedJson);

        httpContext.Items["DecryptedBody"] = targetDto;

        await next();
    }



    public string Decrypt(string encrypted)
    {
        // Replace with your decryption logic
        var decryptedBytes = Convert.FromBase64String(encrypted);
        return Encoding.UTF8.GetString(decryptedBytes);
    }
}