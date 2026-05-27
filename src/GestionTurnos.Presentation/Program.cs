using GestionTurnos.Application.Abstraction;
using GestionTurnos.Application.Abstraction.Infrastructure;
using GestionTurnos.Application.Abstraction.Infrastructure.Auth;
using GestionTurnos.Application.Services;
using GestionTurnos.Infrastructure.BackgroundServices;
using GestionTurnos.Infrastructure.ExternalServices;
using GestionTurnos.Infrastructure.Persistance;
using GestionTurnos.Infrastructure.Persistance.Repository;
using GestionTurnos.Infrastructure.Persistence;
using GestionTurnos.Presentation.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);



builder.Services.AddControllers();

builder.Services.AddOpenApi();
builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
builder.Services.AddScoped(typeof(BaseRepository<>), typeof(BaseRepository<>));
builder.Services.AddScoped<IBusinessRepository, BusinessRepository>();
builder.Services.AddScoped<IStaffRepository, StaffRepository>();
builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<IBranchRepository, BranchRepository>();
builder.Services.AddScoped<IPlanRepository, PlanRepository>();
builder.Services.AddScoped<IBusinessSubscriptionRepository,BusinessSubscriptionRepository>();

builder.Services.AddScoped<IBusinessService, BusinessService>(); 
builder.Services.AddScoped<IStaffService, StaffService>();
builder.Services.AddScoped<IBranchService, BranchService>();
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<IPlanService, PlanService>();
builder.Services.AddScoped<IBusinessSubscriptionService, BusinessSubscriptionService>();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITenantProvider, TenantProvider>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IEmailContentBuilder, EmailContentBuilder>();

builder.Services.AddHostedService<SubscriptionWorker>();
builder.Services.AddScoped<SubscriptionProcessor>();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddHttpContextAccessor();

builder.Services.AddDbContext<FMCTurnosDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(Policies.Admin, policy => policy.RequireClaim(ClaimTypes.Role, "Admin"));
    options.AddPolicy(Policies.Recepcionista, policy => policy.RequireClaim(ClaimTypes.Role, "Recepcionista"));
    options.AddPolicy(Policies.Profesional, policy => policy.RequireClaim(ClaimTypes.Role, "Profesional"));
});
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
       .AddJwtBearer(options => {
           options.TokenValidationParameters = new TokenValidationParameters
           {
               ValidateIssuerSigningKey = true,
               IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
               ValidateIssuer = true,
               ValidIssuer = builder.Configuration["Jwt:Issuer"],
               ValidateAudience = true,
               ValidAudience = builder.Configuration["Jwt:Audience"],
               ValidateLifetime = true,   // rechaza tokens vencidos
               RoleClaimType = "role"
           };
       });
    

var app = builder.Build();



if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication(); 
app.UseAuthorization();

app.MapControllers();

app.Run();
