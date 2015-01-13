using FluentNHibernate.Mapping;
using Sample.Domain;

namespace Sample.Persistence
{
    public class ContactMap : ClassMap<Contact>
    {
        public ContactMap()
        {
            Id(x => x.Id).GeneratedBy.GuidComb();
            Map(x => x.Name);
            HasMany(x => x.PhoneNumbers)
                .ForeignKeyConstraintName("fk_contact_phone_2_contact")
                .Cascade.AllDeleteOrphan();
        }
    }
}
