using Autofac;
using NHibernate;
using System;
using System.Data;
using TauCode.Mq.Autofac;

namespace TauCode.Mq.NHibernate
{
    public class NHibernateMessageHandlerContext : AutofacMessageHandlerContext
    {
        private ISession _session;
        private ITransaction _transaction;

        public NHibernateMessageHandlerContext(ILifetimeScope contextLifetimeScope)
            : base(contextLifetimeScope)
        {
        }

        public override void Begin()
        {
            if (_session != null)
            {
                throw new InvalidOperationException("Session already open.");
            }

            _session = (ISession)this.GetService(typeof(ISession));
            _session.FlushMode = FlushMode.Commit;
            _transaction = _session.BeginTransaction(IsolationLevel.ReadCommitted);
        }

        public override void End()
        {
            if (_session == null)
            {
                throw new InvalidOperationException("Session not open.");
            }

            _session.Flush();
            _transaction.Commit();

            _session = null;
            _transaction = null;
        }

        public override void Dispose()
        {
            _session?.Dispose();

            base.Dispose();
        }
    }
}
