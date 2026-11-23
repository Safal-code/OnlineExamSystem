using Microsoft.EntityFrameworkCore;
using OnlineExamApi.Data;
using OnlineExamApi.Endpoints;
using OnlineExamApi.Services;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ExamDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("OnlineExamConnection"));
});

builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<TestService>();
builder.Services.AddScoped<CurrentUserService>();
builder.Services.AddScoped<RequestUserService>();
builder.Services.AddScoped<QuestionService>();
builder.Services.AddScoped<ExamService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

//taking above configs and build app
WebApplication app = builder.Build();

//app.UseHttpsRedirection(); 
app.UseCors("Frontend");

app.MapAuthEndpoints();
app.MapTestEndpoints();
app.MapQuestionEndpoints();
app.MapExamEndpoints();

app.Run();
