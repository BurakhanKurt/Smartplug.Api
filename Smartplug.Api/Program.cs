using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Smartplug.Api.Application.Jwt;
using Smartplug.Application.Jwt;
using Smartplug.Application.Scoket;
using Smartplug.Application.Services;
using Smartplug.Application.Settings;
using Smartplug.Domain.Entities;
using Smartplug.Persistence;
using Smartplug.Persistence.Seeds;

var builder = WebApplication.CreateBuilder(args);


var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

if (environment.Equals("Local"))
{
    builder.Configuration.AddJsonFile("appsettings.json", true, true);
}
else
{
    builder.Configuration.AddJsonFile($"appsettings.{environment}.json", optional: false, reloadOnChange: true);
}

builder.Configuration.AddJsonFile("jwtSettings.json");
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));

builder.Services.AddDbContext<SmartplugDbContext>(options =>
{
    options.UseNpgsql(Environment.GetEnvironmentVariable("ConnectionString") ??
                      builder.Configuration.GetConnectionString("localDb"));
});

builder.Services.AddIdentity<Users, Role>(options =>
    {
        options.Tokens.PasswordResetTokenProvider = "passwordReset";
        options.SignIn.RequireConfirmedAccount = false;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequiredUniqueChars = 1;
        options.Password.RequiredLength = 8;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireDigit = true;
        options.User.RequireUniqueEmail = false;
        options.Tokens.PasswordResetTokenProvider = TokenOptions.DefaultEmailProvider;
        options.Tokens.EmailConfirmationTokenProvider = "TokenProvider";
    })
    .AddEntityFrameworkStores<SmartplugDbContext>()
    //.AddErrorDescriber<MultilanguageIdentityErrorDescriber>()
    .AddDefaultTokenProviders()
    //.AddTokenProvider("TokenProvider", typeof(VerifyProvider<Users>))
    //.AddTokenProvider<PasswordResetTokenProvider<Users>>("passwordReset")
    .AddUserStore<UserStore<Users, Role, SmartplugDbContext, Guid>>()
    .AddRoleStore<RoleStore<Role, SmartplugDbContext, Guid>>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters.RoleClaimType = "role";
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
        ValidateLifetime = true
    };
});

builder.Services.AddScoped<JwtGenerator>();
builder.Services.AddTransient<IUserAccessor, UserAccessor>();
builder.Services.AddSwaggerGen(setup =>
{
    //string xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    //string xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    //setup.IncludeXmlComments(xmlPath);
    // Include 'SecurityScheme' to use JWT Authentication
    var jwtSecurityScheme = new OpenApiSecurityScheme
    {
        BearerFormat = "JWT",
        Name = "JWT Authentication",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        Description = "Put **_ONLY_** your JWT Bearer token on textbox below!",

        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };

    setup.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);

    setup.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { jwtSecurityScheme, Array.Empty<string>() }
    });
});
const string myAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(myAllowSpecificOrigins, builder =>
        builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMediatR(cfg => { cfg.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies()); });
builder.Services.AddSignalR();

var app = builder.Build();


RoleSeed.SeedRoles(app.Services).Wait();
DefaultSeedUsers.SeedDefaultUser(app.Services).Wait();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || true)
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();
//app.UseMiddleware<JwtMiddleware>();
// WebSocket Middleware
// app.UseWebSockets();
// app.Use(async (context, next) =>
// {
//     if (context.Request.Headers["Upgrade"] == "websocket")
//     {
//         var socket = await context.WebSockets.AcceptWebSocketAsync();
//         var plugService = context.RequestServices.GetRequiredService<IPlugService>();
//         await plugService.HandleAsync(context); // WebSocket ba�lant�s�n� y�nlendir
//     }
//     else
//     {
//         await next.Invoke();
//     }
// });
app.MapControllers();
app.UseRouting();

app.MapHub<PlugHub>("/plugHub");

app.MapGet("TEST/plug-on", ([FromServices]IHubContext<PlugHub> hubContext) =>
{
    hubContext.Clients.All.SendAsync("PlugStatus", "on");
    return Results.Ok("Plug is on");
});

app.MapGet("TEST/plug-off", ([FromServices]IHubContext<PlugHub> hubContext) =>
{
    hubContext.Clients.All.SendAsync("PlugStatus", "off");
    return Results.Ok("Plug is off");
});

app.Run();