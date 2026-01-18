using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using static BackendAPI.Domain.Shared.Results;

namespace BackendAPI.Domain.Shared;

 public class BaseController (IOptions<CustomSettingModel> _setting) : ControllerBase
{
    

    public class EncryptedResponseDto
    {
        public string Data
        {
            get; set;
        }
    }


    protected IActionResult Execute<T>(Result<T> obj)
    {

        var responseType = obj.GetEnumRespType();

        dynamic result = EncryptResponse(obj);

        return responseType switch
        {
            EnumRespType.Success => Ok(result),
            EnumRespType.Error => BadRequest(result),
            EnumRespType.SystemError => BadRequest(result),
            EnumRespType.DuplicateRecord => BadRequest(result),
            EnumRespType.BadRequest => BadRequest(result),
            EnumRespType.Warning => BadRequest(result),
            EnumRespType.NotFound => NotFound(result),
            EnumRespType.ValidationError => BadRequest(result),
            EnumRespType.InvalidData => BadRequest(result),
            EnumRespType.None => throw new Exception("EnumRespType is none. pls check your logic."),
            _ => throw new Exception("Out of scope in Execute (BaseController). pls check your logic.")
        };
    }

    //Encryption 
    public EncryptedResponseDto EncryptResponse<T>(Result<T> obj)
    {
        var json = JsonConvert.SerializeObject(obj);
        var encrypted = Encrypt(json);
        return new EncryptedResponseDto { Data = encrypted };
    }




    //Decryption 
    protected T GetData<T>()
    {
        if (HttpContext.Items["DecryptedBody"] is null)
        {
            string message = "DecryptedBody is null. Ensure the controller is decorated with [ServiceFilter(typeof(DecryptRequestFilter))]. This filter is required to populate DecryptedBody from the encrypted payload.";
            throw new Exception(message);
        }

        if (_setting.Value.EnableEncryption)
        {
            string jsonStr = HttpContext.Items["DecryptedBody"]!.ToString()!;
            var obj = JsonConvert.DeserializeObject<T>(jsonStr,
                new JsonSerializerSettings { DateParseHandling = DateParseHandling.DateTimeOffset });

            if (obj is null)
                throw new Exception("DecryptedBody to object is null. pls check your logic.");
            return obj;
        }
        else
        {
            string str = HttpContext.Items["DecryptedBody"]!.ToString()!;
            JObject jObj = JObject.Parse(str);

            if (jObj.TryGetValue("ReqData", out JToken reqDataToken))
            {
                // CASE 1A: ReqData is a JSON object
                if (reqDataToken.Type == JTokenType.Object)
                {
                    return reqDataToken.ToObject<T>()!;
                }

                // CASE 1B: ReqData is a string containing JSON
                if (reqDataToken.Type == JTokenType.String)
                {
                    string innerJson = reqDataToken.ToString();
                    return JsonConvert.DeserializeObject<T>(innerJson)!;
                }

                throw new Exception("ReqData exists but is not a valid JSON object or JSON string.");
            }

            // CASE 2: Flat JSON (your new portal)
            var flatObj = jObj.ToObject<T>();
            if (flatObj is null)
                throw new Exception("Body to object is null. Check your logic.");

            return flatObj;
        }
    }

    #region 


    public  string Encrypt(string plainText)
    {
        var bytes = Encoding.UTF8.GetBytes(plainText);
        return Convert.ToBase64String(bytes);
    }



    #endregion


}
