using BuildNumberGenerator;
using BuildNumberGenerator.Interfaces;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddSingleton<IGenerator>(x => new Generator());
var app = builder.Build();
app.MapControllers();
app.UseMiddleware<AuthenticationHelper>();
app.UseMiddleware<AuthorizationHelper>();
app.Run();
