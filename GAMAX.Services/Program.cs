using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NETCore.MailKit.Extensions;
using NETCore.MailKit.Infrastructure.Internal;
using GAMAX.Services.Services;
using GAMAX.Services.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using GAMAX.Services.MiddleWare;
using DataBase.Core;
using DataBase.EF;
using DataBase.Core.Models.Authentication;
using Business;
using GAMAX.Services.Hubs;
using Business.Services;
using Business.Implementation;
using Utilites;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

SharedFolderPaths.HostedFolderPath = builder.Environment.ContentRootPath;
SharedFolderPaths.ProfilePhotos = builder.Configuration.GetValue<string>("SharedFolder:ProfilePhotos");
SharedFolderPaths.CoverPhotos = builder.Configuration.GetValue<string>("SharedFolder:CoverPhotos");
SharedFolderPaths.PostPhotos = builder.Configuration.GetValue<string>("SharedFolder:PostPhotos");
SharedFolderPaths.PostVideos = builder.Configuration.GetValue<string>("SharedFolder:PostVideos");
SharedFolderPaths.QuestionVideos = builder.Configuration.GetValue<string>("SharedFolder:QuestionVideos");
SharedFolderPaths.QuestionPhotos = builder.Configuration.GetValue<string>("SharedFolder:QuestionPhotos");
SharedFolderPaths.CommentsPhotos = builder.Configuration.GetValue<string>("SharedFolder:CommentsPhotos");
SharedFolderPaths.CommentsVideos = builder.Configuration.GetValue<string>("SharedFolder:CommentsVideos");
SharedFolderPaths.ChatPhotos = builder.Configuration.GetValue<string>("SharedFolder:ChatPhotos");
SharedFolderPaths.ChatVideos = builder.Configuration.GetValue<string>("SharedFolder:ChatVideos");
SharedFolderPaths.orginUrl= builder.Configuration.GetValue<string>("AllowedOrigin");
SharedFolderPaths.BackendUrl = builder.Configuration.GetValue<string>("BackendUrl");
SharedFolderPaths.validIssuer = builder.Configuration.GetValue<string>("JWT:Issuer");
SharedFolderPaths.validAudience = builder.Configuration.GetValue<string>("JWT:Audience");
SharedFolderPaths.key = builder.Configuration.GetValue<string>("JWT:Key");
var allowedOrigin = builder.Configuration.GetValue<string>("AllowedOrigin");

builder.Services.AddControllers();
builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
);



builder.Services.AddHttpContextAccessor();
builder.Services.Configure<JWT>(builder.Configuration.GetSection("JWT"));
builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
builder.Services.AddTransient<IMailingService, MailingService>();
builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAcountService, AcountService>();
builder.Services.AddScoped<IPostService, PostService>();
builder.Services.AddScoped<ICommentServices, CommentServices>();
builder.Services.AddScoped<IReactServices, ReactsServices>();
builder.Services.AddScoped<INotificationServices,NotificationServices>();
builder.Services.AddScoped<IRoitServices, RoitServices>();
builder.Services.AddScoped<IChatServices, ChatServices>();
builder.Services.AddSingleton<SignalRActions>();
builder.Services.AddSingleton<UserConnectionManager>();
builder.Services.AddSingleton<HubContextNotify>();

builder.Services.AddMailKit(config =>
{
    var configuration = builder.Configuration;
    config.UseMailKit(configuration.GetSection("Email").Get<MailKitOptions>());
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(o =>
    {
        o.RequireHttpsMetadata = false;
        o.SaveToken = false;
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidIssuer = builder.Configuration["JWT:Issuer"],
            ValidAudience = builder.Configuration["JWT:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"])),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAnyOrigin",
        builder =>
        {
            builder.WithOrigins(allowedOrigin)
                   .AllowAnyMethod()
                   .AllowAnyHeader()
                   .AllowCredentials();
        });
});
builder.Services.AddSignalR();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();





var app = builder.Build();

////TODO What is this ?
//var apiProjectPath = Directory.GetCurrentDirectory();
//var solutionPath = Directory.GetParent(apiProjectPath)?.FullName;
//var photosFolderPath = Path.Combine(solutionPath, "StaticFiles");

//// Configure the static files middleware
//app.UseStaticFiles(new StaticFileOptions
//{
//    FileProvider = new PhysicalFileProvider(photosFolderPath),
//    RequestPath = "/ProfilePhotos"
//});

// Define the routes that should skip token validation
var routesToSkipTokenValidation = new List<string>
{
    "/api/Auth/register",
    "/api/Auth/verify",
    "/api/Auth/token",
    "/api/Auth/login",
    "/api/Auth/refreshToken",
    "/api/Auth/revokeToken",
    "/api/Auth/ResendConfirmMail",
    "/api/Auth/ResetPasswordCode",
    "/api/Auth/UpdatePassword",
    "/api/StaticFiles/download",
    "/api/StaticFiles/downloadProfilePhoto",
    "/api/StaticFiles/downloadCoverPhoto"
};

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("AllowAnyOrigin");
app.UseWhen(context => !routesToSkipTokenValidation.Contains(context.Request.Path.Value), builder =>
{
    builder.UseMiddleware<TokenValidationMiddleware>();
});


app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseEndpoints(endpoints =>{endpoints.MapHub<SingalRHub>("/SingalRHub");});
app.Run();
