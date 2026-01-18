
using static BackendAPI.Domain.Shared.Results;

namespace BackendAPI.Domain.Feature.Auth
{
    public interface IAuthService
    {
        Task<Result<AuthService.CreateUserResponse>> CreateUser(AuthController.CreateUSerRequest request);
    }
}