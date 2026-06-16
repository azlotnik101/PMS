using Microsoft.EntityFrameworkCore;
using PMS.Data;
using PMS.Interfaces;
using PMS.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
const string AngularDevelopmentCorsPolicy = "AngularDevelopment";

builder.Services.AddDbContext<PmsDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IAuditLogService, AuditLogService>();
builder.Services.AddScoped<IParticipantService, ParticipantService>();
builder.Services.AddScoped<IQuestionnaireAssignmentService, QuestionnaireAssignmentService>();
builder.Services.AddScoped<IQuestionnaireService, QuestionnaireService>();
builder.Services.AddScoped<IQuestionResponseService, QuestionResponseService>();

builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy(AngularDevelopmentCorsPolicy, policy =>
    {
        policy.WithOrigins("http://localhost:4200", "http://127.0.0.1:4200")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(AngularDevelopmentCorsPolicy);
app.UseAuthorization();

app.MapControllers();

app.Run();
