using FluentNHibernate.Mapping;
using Sample.Domain;

namespace Sample.Persistence
{
    public class ContactPhoneMap : ClassMap<ContactPhone>
    {
        public ContactPhoneMap()
        {
            Id(x => x.Id).GeneratedBy.GuidComb();
            Map(x => x.Type);
            Map(x => x.Number);
        }
    }
}