using System.Collections.Generic;
using System.Linq;
using NHibernate;
using NHibernate.SqlCommand;
using NHibernate.Tool.hbm2ddl;
using Sample.Domain;
using Sample.Persistence;
using Xunit;

namespace Querify.Nancy.Tests
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
                _fixture.PreparedSqlStatements.Add(sql.ToString());
                return base.OnPrepareStatement(sql);
            }
        }

        public IList<string> PreparedSqlStatements { get; private set; }
    }

    public class Test
    {
        [DebugOnlyFact]
        public void query_with_no_fetch()
        {
            var fixture = new PersistenceFixture();

            using (var session = fixture.SessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                var repo = new NHibernateRepository(session);
                var contacts = repo.Advanced.Query<Contact>().Find().Items.ToList();
                var phones = contacts[0].PhoneNumbers.ToList();

                transaction.Commit();
            }

            Assert.Equal(2, fixture.PreparedSqlStatements.Count);
        }

        [DebugOnlyFact]
        public void query_with_fetch()
        {
            var fixture = new PersistenceFixture();

            using (var session = fixture.SessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                var repo = new NHibernateRepository(session);
                var contacts = repo.Advanced.Query<Contact>().FetchMany(x => x.PhoneNumbers).Find().Items.ToList();
                var phones = contacts[0].PhoneNumbers.ToList();

                transaction.Commit();
            }

            Assert.Equal(1, fixture.PreparedSqlStatements.Count);
        }
    }

    public class PersistenceConfigurerTests : IUseFixture<PersistenceFixture>
    {
        private PersistenceFixture _fixture;

        [DebugOnlyFact]
        public void bootstrap_database()
        {
            using (var session = _fixture.SessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                var contact = new Contact();
                contact.Name = "Joe Smith";

                contact.PhoneNumbers.Add(new ContactPhone
                    {
                        Type = "Home",
                        Number = "555-555-5555"
                    });

                contact.PhoneNumbers.Add(new ContactPhone
                    {
                        Type = "Work",
                        Number = "554-555-5554"
                    });

                session.Save(contact);

                transaction.Commit();
            }
        }

        public void SetFixture(PersistenceFixture fixture)
        {
            _fixture = fixture;
            _fixture.Create();
        }
    }
}