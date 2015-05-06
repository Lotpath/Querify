using System;
using NHibernate;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.TinyIoc;

namespace Querify
{
    public static class NHibernate
    {
        [Obsolete("Use OnConfigureRequestContainer - PerRequest will be removed in a future version")]
        public static void PerRequest(TinyIoCContainer container, NancyContext context)
        {
            OnConfigureRequestContainer(container, context);
        }

        [Obsolete("Use OnApplicationStartup - OnStartup will be removed in a future version")]
        public static void OnStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            OnApplicationStartup(container, pipelines, false);
        }

        public static void OnConfigureRequestContainer(TinyIoCContainer container, NancyContext context)
        {
            var session = container.Resolve<ISessionFactory>().OpenSession();
            container.Register(session);
            container.Register<IRepository>(new NHibernateRepository(session));
            session.BeginTransaction();
            context.Items["NhSession"] = session;
        }

        public static void OnApplicationStartup(TinyIoCContainer container, IPipelines pipelines, bool convertNoMatchFoundExceptionTo404 = true)
        {
            pipelines.AfterRequest += ctx => CommitSession(ctx);
            pipelines.OnError += (ctx, ex) => RollbackSession(ctx);

            if (convertNoMatchFoundExceptionTo404)
            {
                pipelines.OnError += (ctx, ex) =>
                    {
                        var exception = ex as NoMatchFoundException;
                        return exception != null ? ConvertNoMatchFoundExceptionTo404(exception) : null;
                    };
            }
        }

        private static Response ConvertNoMatchFoundExceptionTo404(NoMatchFoundException ex)
        {
            return new NotFoundResponse
                {
                    StatusCode = HttpStatusCode.NotFound,
                    ReasonPhrase = ex.Message,
                };
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
