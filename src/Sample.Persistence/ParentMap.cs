using FluentNHibernate.Mapping;
using Sample.Domain;

namespace Sample.Persistence
{
    public class ParentMap : ClassMap<Parent>
    {
        public ParentMap()
        {
            Id(x => x.Id).GeneratedBy.Assigned();
            Map(x => x.Name);
            HasOne(x => x.Firstborn)
                .ForeignKey("fk_child_2_parent")
                .Cascade.All();
        }   
    }

    public class ChildMap : ClassMap<Child>
    {
        public ChildMap()
        {
            Id(x => x.Id).GeneratedBy.Assigned();
            Map(x => x.Name);
            HasMany(x => x.Grandkids)
                .ForeignKeyConstraintName("fk_grandchild_2_child")
                .Cascade.AllDeleteOrphan();
        }   
    }

    public class GrandchildMap : ClassMap<Grandchild>
    {
        public GrandchildMap()
        {
            Id(x => x.Id).GeneratedBy.Assigned();
            Map(x => x.Name);
        }   
    }
}