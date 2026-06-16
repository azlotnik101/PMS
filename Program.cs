using Microsoft.EntityFrameworkCore;
using PMS.Data;
using PMS.Interfaces;
using PMS.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<PmsDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IAuditLogService, AuditLogService>();
builder.Services.AddScoped<IParticipantService, ParticipantService>();
builder.Services.AddScoped<QuestionnaireRequestApplier>();
builder.Services.AddScoped<QuestionnaireResponseFactory>();
builder.Services.AddScoped<QuestionnaireSynchronizer>();
builder.Services.AddScoped<IQuestionnaireAssignmentService, QuestionnaireAssignmentService>();
builder.Services.AddScoped<IQuestionnaireService, QuestionnaireService>();
builder.Services.AddScoped<IQuestionResponseService, QuestionResponseService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
