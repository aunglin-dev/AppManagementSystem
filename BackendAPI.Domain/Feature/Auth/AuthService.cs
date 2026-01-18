using BackendAPI.Domain.Shared;
using Database.AppDbContextModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using static BackendAPI.Domain.Feature.Auth.AuthController;
using static BackendAPI.Domain.Shared.Results;

namespace BackendAPI.Domain.Feature.Auth;

public class AuthService(ApplicationDbContext _context, ILogger<AuthService> _logger) : IAuthService
{

    public record CreateUserResponse(bool IsSuccess);


    public async Task<Result<CreateUserResponse>> CreateUser(CreateUSerRequest request)
    {
        try
        {
            bool userExists = await _context.Users
                .AnyAsync(x => x.Username == request.UserName);

            if (userExists)
            {
                return Result<CreateUserResponse>
                    .DuplicateRecordError("Username already exists");
            }

            Helper.CreatePasswordHash(request.Password,out string passwordHash, out string passwordSalt);

            var user = new User
            {
                UserId = Guid.NewGuid(),
                Username = request.UserName,
                Email = request.Email,
                PasswordHash = passwordHash, 
                PasswordSalt = passwordSalt,
                Role = request.Role,
                IsActive = true,
                LastLoginAt = null,
                DepartmentId = request.DeparmentId,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = request.CreatedUserId,
                UpdatedAt = DateTime.UtcNow,
                UpdatedBy = null
            };

            _context.Users.Add(user);

            bool isSuccess = await _context.SaveChangesAsync() > 0;

            var response = new CreateUserResponse(isSuccess);

            return Result<CreateUserResponse>.Success(response);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred at CreateUser");

            return Result<CreateUserResponse>
                .SystemError("Unexpected system error occurred");
        }
    }

}
