using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NHibernate.Cfg;
using TauCode.Cqrs.NHibernate;
using TauCode.Mq.EasyNetQ;
using TauCode.Mq.NHibernate.Tests.App.Core.Handlers.Notes;
using TauCode.Mq.NHibernate.Tests.App.Domain.Notes;
using TauCode.Mq.NHibernate.Tests.App.Persistence.Repositories;

namespace TauCode.Mq.NHibernate.Tests.App
{
    public class Startup : IAppStartup
    {
        private readonly string _tempDbFilePath;
        private readonly string _connectionString;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            var tuple = AppHelper.CreateSQLiteDatabase();
            _tempDbFilePath = tuple.Item1;
            _connectionString = tuple.Item2;
        }

        public string ConnectionString => _connectionString;
        public string TempDbFilePath => _tempDbFilePath;

        public IConfiguration Configuration { get; }
        public ILifetimeScope AutofacContainer { get; private set; }
        public string RabbitMQConnectionString => "host=localhost";

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //var cqrsAssembly = typeof(CoreBeacon).Assembly;
            services
                .AddControllers()
                .AddApplicationPart(typeof(Startup).Assembly)
                .AddNewtonsoftJson(options => options.UseCamelCasing(false));
        }

        private Configuration CreateConfiguration()
        {
            var configuration = new Configuration();
            configuration.Properties.Add("connection.connection_string", _connectionString);
            configuration.Properties.Add("connection.driver_class", "NHibernate.Driver.SQLite20Driver");
            configuration.Properties.Add("connection.provider", "NHibernate.Connection.DriverConnectionProvider");
            configuration.Properties.Add("dialect", "NHibernate.Dialect.SQLiteDialect");

            return configuration;
        }

        public void ConfigureContainer(ContainerBuilder containerBuilder)
        {
            var configuration = this.CreateConfiguration();

            containerBuilder
                .AddNHibernate(configuration, this.GetType().Assembly)
                .AddCqrs(this.GetType().Assembly, typeof(TransactionalCommandHandlerDecorator<>));

            containerBuilder
                .RegisterType<NHibernateNoteRepository>()
                .As<INoteRepository>()
                .InstancePerLifetimeScope();

            containerBuilder
                .RegisterInstance(this)
                .As<IAppStartup>()
                .SingleInstance();

            containerBuilder
                .Register(context => new NHibernateMessageHandlerContextFactory(context.Resolve<ILifetimeScope>()))
                .As<IMessageHandlerContextFactory>()
                .SingleInstance();

            containerBuilder
                .Register(context => context.Resolve<IMessageHandlerContextFactory>().CreateContext())
                .As<IMessageHandlerContext>()
                .InstancePerLifetimeScope();

            containerBuilder
                .RegisterType<NewNoteHandler>()
                .AsSelf()
                .InstancePerLifetimeScope();

            containerBuilder
                .Register(context =>
                {
                    var subscriber = new EasyNetQMessageSubscriber(
                        context.Resolve<IMessageHandlerContextFactory>(),
                        this.RabbitMQConnectionString);

                    subscriber.Subscribe(typeof(NewNoteHandler));

                    return subscriber;
                })
                .As<IMessageSubscriber>()
                .SingleInstance();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            this.AutofacContainer = app.ApplicationServices.GetAutofacRoot();

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
