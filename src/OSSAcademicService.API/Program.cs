using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OSSAcademicService.Application.Interfaces;
using OSSAcademicService.Application.Services;
using OSSAcademicService.Infrastructure.Data;
using OSSAcademicService.Infrastructure.Data.Repositories;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

var services = builder.Services;

services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "OSS Academic Service", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

var connectionString = builder.Configuration.GetConnectionString("AcademicDb");
services.AddDbContext<AcademicDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

services.AddScoped<IStudentProfileRepository, StudentProfileRepository>();
services.AddScoped<IStatusChangeRepository, StatusChangeRepository>();
services.AddScoped<IGraduationAuditRepository, GraduationAuditRepository>();
services.AddScoped<ICourseRepository, CourseRepository>();
services.AddScoped<ISemesterRepository, SemesterRepository>();
services.AddScoped<ITeachingTaskRepository, TeachingTaskRepository>();
services.AddScoped<IScheduleRepository, ScheduleRepository>();
services.AddScoped<ISelectionRepository, SelectionRepository>();
services.AddScoped<IScoreRepository, ScoreRepository>();
services.AddScoped<IExamRepository, ExamRepository>();
services.AddScoped<IClassroomRepository, ClassroomRepository>();
services.AddScoped<IBaseDataRepository, BaseDataRepository>();
services.AddScoped<IUnitOfWork, UnitOfWork>();

services.AddScoped<IStudentService, StudentService>();
services.AddScoped<ICourseService, CourseService>();
services.AddScoped<IScheduleService, ScheduleService>();
services.AddScoped<ISelectionService, SelectionService>();
services.AddScoped<IScoreService, ScoreService>();
services.AddScoped<IExamService, ExamService>();
services.AddScoped<IClassroomService, ClassroomService>();
services.AddScoped<IBaseDataService, BaseDataService>();

services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"] ?? ""))
    };
});

services.AddAuthorization();

services.AddCors(options =>
{
    options.AddPolicy("Default", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("Default");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AcademicDbContext>();
    await db.Database.EnsureCreatedAsync();
}

app.Run();