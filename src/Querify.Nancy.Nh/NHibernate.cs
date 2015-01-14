using NHibernate;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.TinyIoc;

namespace Querify
{
    public static class NHibernate
    {
        public static void PerRequest(TinyIoCContainer container, NancyContext context)
        {
            var session = container.Resolve<ISessionFactory>().OpenSession();
            container.Register(session);
            container.Register<IRepository>(new NHibernateRepository(session));
            session.BeginTransaction();
            context.Items["NhSession"] = session;
        }

        public static void OnStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            pipelines.AfterRequest += ctx => CommitSession(ctx);
            pipelines.OnError += (ctx, ex) => RollbackSession(ctx);
        }

        private static AfterPipeline CommitSession(NancyContext context)
        {
            var session = (ISession)context.Items["NhSession"];
            session.Transaction.Commit();
            return null;
        }

        private static Response RollbackSession(NancyContext context)
        {
            var session = (ISession)context.Items["NhSession"];
            session.Transaction.Rollback();
            return null;
        }
    }
}
