using System.Text;
using Aweton.Labs.CurrencyRates.Cli.Data;
using Aweton.Labs.CurrencyRates.Cli.Models;
using Aweton.Labs.CurrencyRates.Cli.Strategy;
using Aweton.Labs.XorString.BusinessRules;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

await Host.CreateDefaultBuilder(args)
.ConfigureAppConfiguration(ConfigureApp)
.ConfigureServices(ConfigureServices)
.Build().Services.GetRequiredService<CurrencyLoader>().Run();

void ConfigureApp(HostBuilderContext context, IConfigurationBuilder builder){
  builder.AddCommandLine(args);
}

void ConfigureServices(HostBuilderContext context, IServiceCollection services)
{
  
  services.AddXorStrings(context.Configuration);
  services.Configure<StarterSettings>(context.Configuration.GetSection("Starter"));
  services.Configure<MiceDbSettings>(context.Configuration.GetSection("MiceDb"));
  services.Configure<FetchWorkerSettings>(context.Configuration.GetSection("Fetch"));

  services.AddDbContextFactory<MiceDbContext>((services, options) =>
  {
    SqlConnectionStringBuilder csb = new SqlConnectionStringBuilder(context.Configuration.GetConnectionString("MiceDb"));
    IMiceRunInfo mi = services.GetRequiredService<IXorStrings>().Run();
    csb.UserID = mi.UserId;
    csb.Password = mi.Token;
    Console.WriteLine(csb.ConnectionString);
    options.UseSqlServer(csb.ConnectionString);
  });

  services.AddHttpClient(FetchWorker.HttpClientName, (provider, http) =>
  {
    http.BaseAddress = new Uri(provider.GetRequiredService<IOptions<FetchWorkerSettings>>().Value.Url);
  });

  services
  .AddTransient<CurrencyLoader>()
  .AddTransient<IRegistarWorker, RegistrarWorker>()
  .AddTransient<IRegistarPersister, RegistarPersister>()
  .AddTransient<IFetchWorker, FetchWorker>()
  .AddTransient<ICurrencyCodesWorker, CurrencyCodesWorker>()
  .AddTransient<IStarterDataWorker, StarterDataWorker>()
  .AddSingleton<IEqualityComparer<CurrencyRate>, RateNaturalKeyComparer>()
  .AddSingleton<IMiceClock, MiceClock>()
  .AddSingleton<IValCursSerializer, ValCursSerializer>()
  ;
}
