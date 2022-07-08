using BuildNumberGenerator;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddSingleton<IGenerator>(x => new Generator(new TimeProvider()));
var app = builder.Build();
app.MapControllers();
app.UseMiddleware<AuthenticationHelper>();
app.UseMiddleware<AuthorizationHelper>();
app.Run();
