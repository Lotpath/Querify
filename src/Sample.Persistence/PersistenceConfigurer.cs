using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Cfg;

namespace Sample.Persistence
{
    public class PersistenceConfigurer
    {
        public Configuration Configuration { get; private set; }
        public ISessionFactory SessionFactory { get; private set; }

        public void Configure(IInterceptor interceptor)
        {
            if (SessionFactory != null)
            {
                return;
            }

            SessionFactory = Fluently
                .Configure()
                .Database(GetPersistenceConfigurer)
                .Mappings(m => m
                                   .FluentMappings.AddFromAssemblyOf<PersistenceConfigurer>()
                                   .Conventions.AddFromAssemblyOf<PersistenceConfigurer>())
                .ExposeConfiguration(cfg =>
                {
                    Configuration = cfg;
                    cfg.SetInterceptor(interceptor);
                })
                .BuildSessionFactory();
        }

        private IPersistenceConfigurer GetPersistenceConfigurer()
        {
            return MsSqlConfiguration
                .MsSql2012
                .ConnectionString(
                    c =>
                    c.Server(@".\SQLEXPRESS")
                     .Database("Querify")
                     .TrustedConnection())
                .AdoNetBatchSize(50);
        }
    }
}