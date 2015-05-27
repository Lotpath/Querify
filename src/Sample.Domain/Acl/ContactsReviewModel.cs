using System;
using System.Collections.Generic;
using System.Linq;
using Querify;

namespace Sample.Domain.Acl
{
    public class ContactsReviewModel : PagedResponse<ContactsReviewModel.Item>
    {
        public class Item
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public IList<ContactPhone> PhoneNumbers { get; set; } 
        }

        internal static ContactsReviewModel CreateFrom(PagedResult<Contact> result)
        {
            var model = new ContactsReviewModel
                {
                    TotalItems = result.TotalItems,
                    TotalPages = result.TotalPages,
                    PageSize = result.PageSize,
                    Items = result.Items.Select(x => new Item
                        {
                            Id = x.Id,
                            Name = x.Name,
                            PhoneNumbers = x.PhoneNumbers
                        }).ToList()
                };

            return model;
        }
    }
}