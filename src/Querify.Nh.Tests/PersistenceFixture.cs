using System.Collections.Generic;
using NHibernate;
using NHibernate.SqlCommand;
using NHibernate.Tool.hbm2ddl;
using Sample.Persistence;

namespace Querify.Nh.Tests
{
    public class PersistenceFixture
    {
        private PersistenceConfigurer _configurer;
        private SchemaExport _schemaExport;

        public PersistenceFixture()
        {
            _configurer = new PersistenceConfigurer();
            _configurer.Configure(new LoggingInterceptor(this));
            _schemaExport = new SchemaExport(_configurer.Configuration);
            PreparedSqlStatements = new List<string>();
        }

        public void Create()
        {
            _schemaExport.Create(true, true);
        }

        public void Drop()
        {
            _schemaExport.Drop(true, true);
        }

        public ISessionFactory SessionFactory { get { return _configurer.SessionFactory; } }

        private class LoggingInterceptor : EmptyInterceptor
        {
            private readonly PersistenceFixture _fixture;

            public LoggingInterceptor(PersistenceFixture fixture)
            {
                _fixture = fixture;
            }

            public override SqlString OnPrepareStatement(SqlString sql)
            {
                if (sql.ToString().ToLower().StartsWith("select"))
                {
                    _fixture.PreparedSqlStatements.Add(sql.ToString());
                }
                return base.OnPrepareStatement(sql);
            }
        }

        public IList<string> PreparedSqlStatements { get; private set; }
    }
}