using BackendAPI.Domain.Filters;
using BackendAPI.Domain.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

namespace BackendAPI.Domain.Feature.Auth;


[ApiController]
[Route("api/auth")]
[ServiceFilter(typeof(DecryptionFilter))]

public class AuthController (IAuthService _service, IOptions<CustomSettingModel> _setting) : BaseController(_setting)
{
    public record CreateUSerRequest(string UserName, string Email, string Password,string Role, int DeparmentId, Guid CreatedUserId);

    [HttpPost("Register")]
    public async Task<IActionResult> Create([FromBody] object request)
    {
        
        var loginRequest = GetData<CreateUSerRequest>();
        var accessToken = Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
        if (loginRequest is null)
            return BadRequest("Invalid or missing login data.");

        var result = await _service.CreateUser(loginRequest);
        return Execute(result);
    }




}
