using BuildNumberGenerator;
using IvyTech.Logging;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
var logger = DebugLogger.CreateLogger(builder.Configuration);
builder.Services.AddLogging(x => x.AddSerilog(logger));
builder.Services.AddSingleton<ICertificateManager>(x => new StaticCertManager(builder.Configuration.GetSection("StaticCertManager").Get<StaticCertManagerConfig>()));
builder.Services.AddControllers();
builder.Services.AddSingleton<IGenerator>(x => new Generator(new TimeProvider()));
var app = builder.Build();
app.MapControllers();
app.UseIvyLogging();
app.UseMiddleware<AuthenticationHelper>();
app.UseMiddleware<AuthorizationHelper>();
app.Run();
