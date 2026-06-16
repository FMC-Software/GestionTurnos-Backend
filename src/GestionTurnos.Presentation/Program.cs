using GestionTurnos.Application.Abstraction;
using GestionTurnos.Application.Abstraction.Infrastructure;
using GestionTurnos.Application.Abstraction.Infrastructure.Auth;
using GestionTurnos.Application.Abstraction.Infrastructure.External_Interface;
using GestionTurnos.Application.Services;
using GestionTurnos.Infrastructure.BackgroundServices;
using GestionTurnos.Infrastructure.ExternalServices;
using GestionTurnos.Infrastructure.Persistance;
using GestionTurnos.Infrastructure.Persistance.Repository;
using GestionTurnos.Infrastructure.Persistence;
using GestionTurnos.Presentation.Authorization;
using GestionTurnos.Presentation.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);



builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
}); ;

builder.Services.AddOpenApi();
builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
builder.Services.AddScoped(typeof(BaseRepository<>), typeof(BaseRepository<>));
builder.Services.AddScoped<IBusinessRepository, BusinessRepository>();
builder.Services.AddScoped<IStaffRepository, StaffRepository>();
builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<IBranchRepository, BranchRepository>();
builder.Services.AddScoped<IPlanRepository, PlanRepository>();
builder.Services.AddScoped<IBusinessSubscriptionRepository, BusinessSubscriptionRepository>();
builder.Services.AddScoped<IScheduleRepository, ScheduleRepository>();
builder.Services.AddScoped<IServiceRepository, ServiceRepository>();
builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
builder.Services.AddScoped<ISysAdminRepository, SysAdminRepository>();

builder.Services.AddScoped<IAppointmentService, AppointmentService>();
builder.Services.AddScoped<IBusinessService, BusinessService>();
builder.Services.AddScoped<IStaffService, StaffService>();
builder.Services.AddScoped<IBranchService, BranchService>();
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<IPlanService, PlanService>();
builder.Services.AddScoped<IScheduleService, ScheduleService>();
builder.Services.AddScoped<IBusinessSubscriptionService, BusinessSubscriptionService>();
builder.Services.AddScoped<IServiceService, ServiceService>();
builder.Services.AddScoped<ISysAdminService, SysAdminService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();


builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITenantProvider, TenantProvider>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IEmailContentBuilder, EmailContentBuilder>();
builder.Services.AddScoped<IDolarService, DolarService>();

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
    options.AddPolicy(Policies.SysAdmin, policy => policy.RequireClaim(ClaimTypes.Role, "SysAdmin"));
    options.AddPolicy(Policies.SysAdminOrAdminOrRecepcionista, policy =>
    policy.RequireClaim(ClaimTypes.Role, "SysAdmin","Admin", "Recepcionista"));
    options.AddPolicy(Policies.SysAdminOrAdmin, policy =>
    policy.RequireClaim(ClaimTypes.Role, "SysAdmin", "Admin"));
    options.AddPolicy(Policies.AdminOrRecepcionista, policy =>
    policy.RequireClaim(ClaimTypes.Role, "Recepcionista", "Admin"));
});
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
       .AddJwtBearer(options =>
       {
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

builder.Services.AddHttpClient("DolarApi", client =>
{
    client.BaseAddress = new Uri(builder.Configuration[
"DolarHoy:Base_URL"]!);
});


builder.Services.AddCors(options => // NO LE DEN PELOTA A ESTO ES PARA PROBAR TODO EN EL FRONT Y BACK
{
    options.AddPolicy("ReactPolicy", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();



if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.UseCors("ReactPolicy"); // NO LE DEN PELOTA A ESTO ES PARA PROBAR TODO EN EL FRONT Y BACK
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
