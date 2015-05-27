using System;
using Sample.Domain;
using Sample.Domain.Acl;
using Sample.Domain.Services;
using Xunit;

namespace Querify.Nh.Tests
{
    public class PagingTests : IDisposable
    {
        private readonly PersistenceFixture _fixture;

        public PagingTests()
        {
            _fixture = new PersistenceFixture();
            ConfigureFixture(_fixture);
        }

        [Fact]
        public void query_with_fetch_many_and_paging_returns_page_of_parents()
        {
            var page1 = default(ContactsReviewModel);
            var page2 = default(ContactsReviewModel);

            using (var session = _fixture.SessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                var repo = new NHibernateRepository(session);
                var manager = new ContactManager(repo);

                page1 = manager.FetchContactsPage(1);
                page2 = manager.FetchContactsPage(2);
            }

            Assert.Equal(10, page1.Items.Count);
            Assert.Equal(1, page2.Items.Count);
        }

        private void ConfigureFixture(PersistenceFixture fixture)
        {
            fixture.Create();

            using (var session = fixture.SessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                for (var i = 0; i < 11; i++)
                {
                    var contact = new Contact {Name = "Joe Smith" + i};

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
                }

                transaction.Commit();
            }

            _fixture.PreparedSqlStatements.Clear();
        }

        public void Dispose()
        {
            _fixture.Drop();
        }
    }
}