using Autofac;
using Autofac.Core;
using FluentNHibernate.Cfg;
using FluentNHibernate.Conventions.Helpers;
using FluentValidation;
using Inflector;
using NHibernate;
using NHibernate.Cfg;
using System.Data.SQLite;
using System.Reflection;
using TauCode.Cqrs.Autofac;
using TauCode.Cqrs.Commands;
using TauCode.Cqrs.Queries;
using TauCode.Cqrs.Validation;
using TauCode.IO;
using ISession = NHibernate.ISession;

namespace TauCode.Mq.NHibernate.Tests.App;

public static class AppHelper
{
    public static ISessionFactory BuildSessionFactory(
        Configuration configuration,
        Assembly mappingsAssembly)
    {
        return Fluently.Configure(configuration)
            .Mappings(m => m.FluentMappings.AddFromAssembly(mappingsAssembly)
                .Conventions.Add(ForeignKey.Format((p, t) =>
                {
                    if (p == null) return t.Name.Underscore() + "_id";

                    return p.Name.Underscore() + "_id";
                }))
                .Conventions.Add(LazyLoad.Never())
                .Conventions.Add(Table.Is(x => x.TableName.Underscore().ToUpper()))
                .Conventions.Add(ConventionBuilder.Property.Always(x => x.Column(x.Property.Name.Underscore())))
            )
            .BuildSessionFactory();
    }

    public static ContainerBuilder AddNHibernate(
        this ContainerBuilder containerBuilder,
        Configuration configuration,
        Assembly mappingsAssembly)
    {
        containerBuilder
            .Register(c => BuildSessionFactory(configuration, mappingsAssembly))
            .As<ISessionFactory>()
            .SingleInstance();

        containerBuilder
            .Register(c =>
            {
                var session = c.Resolve<ISessionFactory>().OpenSession();
                ((SQLiteConnection)session.Connection).BoostSQLiteInsertions();
                return session;
            })
            .As<ISession>()
            .InstancePerLifetimeScope();

        return containerBuilder;
    }

    public static Tuple<string, string> CreateSQLiteDatabase()
    {
        var tempDbFilePath = FileSystemHelper.CreateTempFilePath("zunit", ".sqlite");
        SQLiteConnection.CreateFile(tempDbFilePath);

        var connectionString = $"Data Source={tempDbFilePath};Version=3;";

        return Tuple.Create(tempDbFilePath, connectionString);
    }

    public static void BoostSQLiteInsertions(this SQLiteConnection sqLiteConnection)
    {
        if (sqLiteConnection == null)
        {
            throw new ArgumentNullException(nameof(sqLiteConnection));
        }

        using var command = sqLiteConnection.CreateCommand();
        command.CommandText = "PRAGMA journal_mode = WAL";
        command.ExecuteNonQuery();

        command.CommandText = "PRAGMA synchronous = NORMAL";
        command.ExecuteNonQuery();
    }

    public static ContainerBuilder AddCqrs(
        this ContainerBuilder containerBuilder,
        Assembly cqrsAssembly,
        Type commandHandlerDecoratorType)
    {
        if (containerBuilder == null)
        {
            throw new ArgumentNullException(nameof(containerBuilder));
        }

        if (cqrsAssembly == null)
        {
            throw new ArgumentNullException(nameof(cqrsAssembly));
        }

        if (commandHandlerDecoratorType == null)
        {
            throw new ArgumentNullException(nameof(commandHandlerDecoratorType));
        }

        // command dispatching
        containerBuilder
            .RegisterType<CommandDispatcher>()
            .As<ICommandDispatcher>()
            .InstancePerLifetimeScope();

        containerBuilder
            .RegisterType<ValidatingCommandDispatcher>()
            .As<IValidatingCommandDispatcher>()
            .InstancePerLifetimeScope();

        containerBuilder
            .RegisterType<AutofacCommandHandlerFactory>()
            .As<ICommandHandlerFactory>()
            .InstancePerLifetimeScope();

        // register API ICommandHandler decorator
        containerBuilder
            .RegisterAssemblyTypes(cqrsAssembly)
            .Where(t => t.IsClosedTypeOf(typeof(ICommandHandler<>)))
            .As(t => t.GetInterfaces()
                .Where(x => x.IsClosedTypeOf(typeof(ICommandHandler<>)))
                .Select(x => new KeyedService("commandHandler", x)))
            .InstancePerLifetimeScope();

        containerBuilder
            .RegisterGenericDecorator(
                commandHandlerDecoratorType,
                typeof(ICommandHandler<>),
                "commandHandler");

        // command validator source
        containerBuilder
            .RegisterInstance(new CommandValidatorSource(cqrsAssembly))
            .As<ICommandValidatorSource>()
            .SingleInstance();

        // validators
        containerBuilder
            .RegisterAssemblyTypes(cqrsAssembly)
            .Where(t => t.IsClosedTypeOf(typeof(AbstractValidator<>)))
            .AsSelf()
            .InstancePerLifetimeScope();

        // query handling
        containerBuilder
            .RegisterType<QueryRunner>()
            .As<IQueryRunner>()
            .InstancePerLifetimeScope();

        containerBuilder
            .RegisterType<ValidatingQueryRunner>()
            .As<IValidatingQueryRunner>()
            .InstancePerLifetimeScope();

        containerBuilder
            .RegisterType<AutofacQueryHandlerFactory>()
            .As<IQueryHandlerFactory>()
            .InstancePerLifetimeScope();

        containerBuilder
            .RegisterAssemblyTypes(cqrsAssembly)
            .Where(t => t.IsClosedTypeOf(typeof(IQueryHandler<>)))
            .AsImplementedInterfaces()
            .AsSelf()
            .InstancePerLifetimeScope();

        // query validator source
        containerBuilder
            .RegisterInstance(new QueryValidatorSource(cqrsAssembly))
            .As<IQueryValidatorSource>()
            .SingleInstance();

        return containerBuilder;
    }
}