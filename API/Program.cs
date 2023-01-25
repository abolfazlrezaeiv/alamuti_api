using API;
using application;
using Infrastructure;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddNewtonsoftJson(options =>
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

builder.Services.AddControllers();
builder.Services.Configure<IISOptions>(builder.Configuration);
builder.Services.AddSignalR(e => { e.MaximumReceiveMessageSize = 102400000;});
builder.Services.AddHttpContextAccessor();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Alamuti", Version = "v1" });
    c.AddSecurityDefinition("BearerAuth", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = JwtBearerDefaults.AuthenticationScheme.ToLowerInvariant(),
        In = ParameterLocation.Header,
        Name = "Authorization",
        BearerFormat = "JWT",
        Description = "JWT Authorization header using the Bearer scheme."
    });
    c.OperationFilter<AuthResponsesOperationFilter>();

});
builder.Services.AddCors(options =>
{
    options.AddPolicy("Open", builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});
builder.Services.RegisterInfrastructerServices(builder.Configuration);
builder.Services.RegisterApplicationServices(builder.Configuration);
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddSwaggerGen(c =>
{
    c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
});
builder.Services.Configure<JwtConfig>(builder.Configuration.GetSection("JwtConfig"));
var key = Encoding.ASCII.GetBytes(builder.Configuration["JwtConfig:Secret"]);


Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;


var tokenValidationParameters = new TokenValidationParameters
{
    ValidateIssuerSigningKey = true,
    IssuerSigningKey = new SymmetricSecurityKey(key),
    ValidateIssuer = false,
    ValidateAudience = false,
    ValidateLifetime = true,
    RequireExpirationTime = false,
    ClockSkew = TimeSpan.Zero
};

builder.Services.AddSingleton(tokenValidationParameters);
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
   
})
    .AddJwtBearer(jwt =>
    {

        jwt.SaveToken = true;
        jwt.TokenValidationParameters = tokenValidationParameters;
    });

builder.Services.AddIdentityCore<IdentityUser>(
    options =>
    {
        options.SignIn.RequireConfirmedAccount = true;
        options.User.RequireUniqueEmail = false;
        options.SignIn.RequireConfirmedEmail = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 4;
    }).AddEntityFrameworkStores<AlamutDbContext>();




var app = builder.Build();

app.UseDeveloperExceptionPage();
app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Alamuti v1"));
/*if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Alamuti v1"));
}*/

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseCors("Open");

app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<ChatHub>("/chat");
    endpoints.MapControllers();
   
});

app.Run();




internal class AuthResponsesOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var attributes = context.MethodInfo.DeclaringType?.GetCustomAttributes(true)
                            .Union(context.MethodInfo.GetCustomAttributes(true));

        if (attributes!.OfType<IAllowAnonymous>().Any())
        {
            return;
        }

        var authAttributes = attributes!.OfType<IAuthorizeData>();

        if (authAttributes.Any())
        {
            operation.Responses["401"] = new OpenApiResponse { Description = "Unauthorized" };

            if (authAttributes.Any(att => !String.IsNullOrWhiteSpace(att.Roles) || !String.IsNullOrWhiteSpace(att.Policy)))
            {
                operation.Responses["403"] = new OpenApiResponse { Description = "Forbidden" };
            }

            operation.Security = new List<OpenApiSecurityRequirement>
                {
                    new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Id = "BearerAuth",
                                    Type = ReferenceType.SecurityScheme
                                }
                            },
                            Array.Empty<string>()
                        }
                    }
                };
        }
    }
}