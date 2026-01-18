using BackendAPI.Domain.Feature.Auth;
using BackendAPI.Domain.Shared;
using Database.AppDbContextModels;
using Database.Config;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BackendAPI.Domain;

public static class FeatureManger
{
    private static void AddServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IAuthService,AuthService>();
        builder.Services.AddHttpContextAccessor();

    }


    public static void BackendAPIDomin(this WebApplicationBuilder builder)
    {
        builder.AddStageConfig();

        builder.Services.Configure<CustomSettingModel>(builder.Configuration);


        //  Entity Framework DbContext with SQL Server
        builder.Services.AddDbContext<ApplicationDbContext>(opt =>
        {
            opt.UseSqlServer(
                builder.Configuration["DbConnection"]);
        }, ServiceLifetime.Transient, ServiceLifetime.Transient);

        builder.AddServices();


        //  Register DecryptRequestFilter for MVC ServiceFilter attribute
        builder.Services.AddScoped<BackendAPI.Domain.Filters.DecryptionFilter>();
    }

}
