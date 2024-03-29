﻿using Autofac;
using NHibernate;
using System.Data;
using TauCode.Mq.Autofac;

namespace TauCode.Mq.NHibernate;

public class NHibernateMessageHandlerContext : AutofacMessageHandlerContext
{
    #region Fields

    private ISession? _session;
    private ITransaction? _transaction;

    #endregion

    #region Constructor

    public NHibernateMessageHandlerContext(ILifetimeScope contextLifetimeScope)
        : base(contextLifetimeScope)
    {
    }

    #endregion

    #region Overridden

    public override Task BeginAsync(CancellationToken cancellationToken = default)
    {
        if (_session != null)
        {
            throw new InvalidOperationException("Session already open.");
        }

        _session = (ISession)this.GetService(typeof(ISession));
        _session.FlushMode = FlushMode.Commit;
        _transaction = _session.BeginTransaction(IsolationLevel.ReadCommitted);
        return Task.CompletedTask;
    }

    public override async Task EndAsync(CancellationToken cancellationToken = default)
    {
        if (_session == null)
        {
            throw new InvalidOperationException("Session not open.");
        }

        await _session.FlushAsync(cancellationToken);
        await _transaction!.CommitAsync(cancellationToken);

        _session = null;
        _transaction = null;
    }

    public override void Dispose()
    {
        _session?.Dispose();

        base.Dispose();
    }

    #endregion
}