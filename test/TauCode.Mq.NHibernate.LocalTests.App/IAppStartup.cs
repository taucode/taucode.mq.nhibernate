using Autofac;

namespace TauCode.Mq.NHibernate.LocalTests.App;

public interface IAppStartup
{
    ILifetimeScope AutofacContainer { get; }
    string ConnectionString { get; }
    string TempDbFilePath { get; }
}