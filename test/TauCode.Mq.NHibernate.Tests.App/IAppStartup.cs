using Autofac;

namespace TauCode.Mq.NHibernate.Tests.App;

public interface IAppStartup
{
    ILifetimeScope AutofacContainer { get; }
    string ConnectionString { get; }
    string TempDbFilePath { get; }
}