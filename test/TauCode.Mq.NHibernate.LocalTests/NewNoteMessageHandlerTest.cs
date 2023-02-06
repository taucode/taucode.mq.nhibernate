using Autofac;
using EasyNetQ;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Globalization;
using TauCode.Extensions;
using TauCode.Mq.NHibernate.LocalTests.App;
using TauCode.Mq.NHibernate.LocalTests.App.Client;
using TauCode.Mq.NHibernate.LocalTests.App.Client.Messages.Notes;

namespace TauCode.Mq.NHibernate.LocalTests;

[TestFixture]
public class NewNoteMessageHandlerTest
{
    private TestFactory _factory;
    private ILifetimeScope _container;
    private IAppClient _appClient;
    private IMessageSubscriber _subscriber;

    [SetUp]
    public void SetUp()
    {
        Inflector.Inflector.SetDefaultCultureFunc = () => new CultureInfo("en-US");
        _factory = new TestFactory();
        var httpClient = this.CreateHttpClient();
        _appClient = new AppClient(httpClient);

        var startup = _container.Resolve<IAppStartup>();
        var migratorHelper = new FluentDbMigratorHelper("SQLite", startup.ConnectionString, typeof(Program).Assembly);
        migratorHelper.Migrate();

        _subscriber = _container.Resolve<IMessageSubscriber>();
        _subscriber.Start();
    }

    [TearDown]
    public void TearDown()
    {
        _subscriber.Dispose();
        _container.Dispose();
    }

    private HttpClient CreateHttpClient()
    {
        var httpClient = _factory
            .WithWebHostBuilder(builder => builder.UseSolutionRelativeContentRoot(@"test\TauCode.Mq.NHibernate.Tests.Integration"))
            .CreateClient();

        var testServer = _factory.Factories.Single().Server;

        var startup = (Startup)testServer.Services.GetService<IAppStartup>();
        _container = startup.AutofacContainer;

        return httpClient;
    }


    [Test]
    public async Task Publish_ValidNote_CanGetNote()
    {
        // Arrange
        using var bus = RabbitHutch.CreateBus("host=localhost");
        var message = new NewNoteMessage
        {
            CorrelationId = new Guid("3794470c-c02c-40af-921d-b9a2730160c0").ToString(),
            CreatedAt = "2020-01-01Z".ToUtcDateOffset().AddHours(1.5),
            UserId = "ak@m.net",
            Subject = "Ocean",
            Body = "Ready for.",
            Importance = ImportanceDto.Medium,
        };

        // Act
        bus.PubSub.Publish(message);

        await Task.Delay(300);

        var notes = await _appClient.GetUserNotes("ak@m.net");

        // Assert
        var note = notes.Single();
        Assert.That(note.Id, Is.Not.Null);
        Assert.That(note.UserId, Is.EqualTo("ak@m.net"));
        Assert.That(note.Subject, Is.EqualTo("Ocean"));
        Assert.That(note.Body, Is.EqualTo("Ready for."));
    }
}