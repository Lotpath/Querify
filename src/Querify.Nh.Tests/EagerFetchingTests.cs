using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate;
using NHibernate.SqlCommand;
using NHibernate.Tool.hbm2ddl;
using Sample.Domain;
using Sample.Persistence;
using Xunit;

namespace Querify.Nh.Tests
{   
    public class EagerFetchingTests : IDisposable
    {
        private readonly PersistenceFixture _fixture;

        public EagerFetchingTests()
        {
            _fixture = new PersistenceFixture();
            ConfigureFixture(_fixture);
        }

        [Fact]
        public void query_with_no_fetch_performs_extra_query_for_child_collection()
        {
            using (var session = _fixture.SessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                var repo = new NHibernateRepository(session);
                var contacts = repo.Advanced.Query<Contact>().Find().Items.ToList();
                var phones = contacts[0].PhoneNumbers.ToList();

                transaction.Commit();
            }

            Assert.Equal(2, _fixture.PreparedSqlStatements.Count);
        }

        [Fact]
        public void query_with_fetch_performs_single_query_for_parent_and_child_collection()
        {
            using (var session = _fixture.SessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                var repo = new NHibernateRepository(session);
                var contacts = repo.Advanced.Query<Contact>().FetchMany(x => x.PhoneNumbers).Find().Items.ToList();
                var phones = contacts[0].PhoneNumbers.ToList();

                transaction.Commit();
            }

            Assert.Equal(1, _fixture.PreparedSqlStatements.Count);
        }

        private void ConfigureFixture(PersistenceFixture fixture)
        {
            fixture.Create();

            using (var session = fixture.SessionFactory.OpenSession())
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

        public void Dispose()
        {
            _fixture.Drop();
        }
    }

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