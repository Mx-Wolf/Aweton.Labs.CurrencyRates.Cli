using System.Text;
using Aweton.Labs.CurrencyRates.Cli.Data;
using Aweton.Labs.CurrencyRates.Cli.Models;
using Aweton.Labs.CurrencyRates.Cli.Strategy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

await Host.CreateDefaultBuilder(args)
.ConfigureServices(ConfigureServices)
.Build().Services.GetRequiredService<CurrencyLoader>().Run();


void ConfigureServices(HostBuilderContext context, IServiceCollection services)
{
  Console.WriteLine(context.Configuration.GetConnectionString("MiceDb"));
  Console.WriteLine(context.Configuration.GetSection("ConnectionStrings").AsEnumerable().Aggregate(new StringBuilder(), (a, b) => { a.Append(b); return a; }));

  services.Configure<StarterSettings>(context.Configuration.GetSection("Starter"));
  services.Configure<MiceDbSettings>(context.Configuration.GetSection("MiceDb"));
  services.Configure<FetchWorkerSettings>(context.Configuration.GetSection("Fetch"));

  services.AddDbContextFactory<MiceDbContext>((services, options) =>
  {
    options.UseSqlServer("name=ConnectionStrings:MiceDb");
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
