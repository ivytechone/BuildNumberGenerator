using BuildNumberGenerator;
using IvyTech.Logging;
using Serilog;

// App level logging context
IvyTech.Logging.AppContext.SetAppName("BuildNum");
IvyTech.Logging.AppContext.SetVersion("1.0");

var builder = WebApplication.CreateBuilder(args);
var logger = DebugLogger.CreateLogger(builder.Configuration);

builder.Services.AddLogging(x => x.AddSerilog(logger));
builder.Services.AddControllers();
builder.Services.AddSingleton<IBuildNumberArchiverConfig>(x => builder.Configuration.GetSection("BuildNumberArchiverConfig").Get<BuildNumberArchiverConfig>());
builder.Services.AddSingleton<IStaticCertManagerConfig>(x => builder.Configuration.GetSection("StaticCertManager").Get<StaticCertManagerConfig>());
builder.Services.AddSingleton<ICertificateManager, StaticCertManager>();
builder.Services.AddSingleton<IBuildNumberArchiver, BuildNumberArchiver>();
builder.Services.AddSingleton<ITimeProvider, TimeProvider>();
builder.Services.AddSingleton<IGenerator, Generator>();
builder.Services.AddHostedService<BackgroundWorker>();
var app = builder.Build();
app.MapControllers();
app.UseIvyLogging();
app.UseMiddleware<AuthenticationHelper>();
app.UseMiddleware<AuthorizationHelper>();
app.Run();
