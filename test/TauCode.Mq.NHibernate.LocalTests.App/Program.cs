using System.Globalization;
using Autofac.Extensions.DependencyInjection;

namespace TauCode.Mq.NHibernate.LocalTests.App;

public class Program
{
    public static void Main(string[] args)
    {
        Inflector.Inflector.SetDefaultCultureFunc = () => new CultureInfo("en-US");

        var host = CreateHostBuilder(args).Build();
        var startup = (IAppStartup)host.Services.GetService(typeof(IAppStartup));

        var migratorHelper = new FluentDbMigratorHelper("SQLite", startup.ConnectionString, typeof(Program).Assembly);
        migratorHelper.Migrate();

        using IMessageSubscriber messageSubscriber = host.Services.GetService<IMessageSubscriber>();
        messageSubscriber.Start();

        host.Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .UseServiceProviderFactory(new AutofacServiceProviderFactory())
            .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
}