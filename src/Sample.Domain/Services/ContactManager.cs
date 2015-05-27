using System;
using System.Linq;
using Querify;
using Sample.Domain.Acl;

namespace Sample.Domain.Services
{
    public class ContactManager
    {
        private readonly IRepository _repository;

        public ContactManager(IRepository repository)
        {
            _repository = repository;
        }

        public ContactsReviewModel FetchContactsPage(int page)
        {
            var result = FetchContacts(q => q.Find(null, page));

            return ContactsReviewModel.CreateFrom(result);
        }

        private PagedResult<Contact> FetchContacts(Func<IQueryable<Contact>, PagedResult<Contact>> fetcher)
        {
            var result = _repository
                .Advanced.Query<Contact>()
                .OrderByDescending(x => x.Name);

            return fetcher(result);
        }  
    }
}
