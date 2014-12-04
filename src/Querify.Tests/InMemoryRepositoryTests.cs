using System;
using System.Linq;
using SubSpec;
using Xunit;

namespace Querify.Tests
{
    public class InMemoryRepositoryTests
    {
        [Specification]
        public void adding_and_getting_items_with_manually_assigned_ids()
        {
            var repo = default(InMemoryRepository);

            "Given an in memory repository"
                .Context(() =>
                    {
                        repo = new InMemoryRepository(cfg =>
                            {
                                cfg.ConfigureIdFor<AnEntity, int>(manuallyAssignedIds: true);
                            });
                    });

            "when adding a new entity"
                .Do(() =>
                    {
                        repo.Add(new AnEntity
                            {
                                Id = 4,
                                Name = "Joe",
                                SomeDate = new DateTime(2014, 1, 1)
                            });
                    });

            "then the entity can be retrieved by id"
                .Assert(() =>
                    Assert.NotNull(repo.Get<AnEntity>(4))
                );
        }

        [Specification]
        public void using_advanced_repository_to_fetch_items_with_auto_generated_ids()
        {
            var repo = default(InMemoryRepository);

            "Given an in memory repository"
                .Context(() =>
                {
                    repo = new InMemoryRepository();
                });

            "when adding a new entity"
                .Do(() =>
                {
                    repo.Add(new AnEntity
                    {
                        Name = "Joe",
                        SomeDate = new DateTime(2014, 1, 1)
                    });
                    repo.Add(new AnEntity
                    {
                        Name = "Jim",
                        SomeDate = new DateTime(2014, 1, 2)
                    });
                });

            "then entities can be retrieved using the advanced repository"
                .Assert(() =>
                    {
                        var match = repo.Advanced.Query<AnEntity>()
                            .FindOne(AnEntityFiltered.By.NameStartingWith("Jo")).Value;
                        Assert.NotNull(match);
                    }
                );

            "then retrieved entities have auto assigned id values"
                .Assert(() =>
                    {
                        var match = repo.Advanced.Query<AnEntity>()
                            .Find();
                        Assert.True(match.Items.All(x => x.Id != default(int)));
                    }
                );
        }

        [Specification]
        public void adding_entity_with_guid_id_and_manual_id_generation_does_not_overwrite_assigned_id_value()
        {
            var repo = default(InMemoryRepository);
            var id = default(Guid);
            var entity = default(EntityWithGuidId);

            "Given an in memory repository with an entity configured for manual id assignment"
                .Context(() =>
                {
                    repo = new InMemoryRepository(cfg =>
                    {
                        cfg.ConfigureIdFor<EntityWithGuidId, Guid>(manuallyAssignedIds: true);
                    });

                    id = Guid.NewGuid();

                    entity = new EntityWithGuidId
                    {
                        Id = id,
                        Name = "Joe",
                    };

                });

            "when adding a new entity"
                .Do(() =>
                    {
                        repo.Add(entity);
                    });

            "then a new entity id is assigned and the entity can be retrieved by the assigned id"
                .Assert(() =>
                    Assert.NotNull(repo.Get<EntityWithGuidId>(id))
                );
        }

        [Specification]
        public void adding_entity_with_guid_id_and_auto_id_generation_overwrites_assigned_id_value()
        {
            var repo = default(InMemoryRepository);
            var id = default(Guid);
            var entity = default(EntityWithGuidId);

            "Given an in memory repository with default id generator configuration"
                .Context(() =>
                {
                    repo = new InMemoryRepository();

                    id = Guid.NewGuid();

                    entity = new EntityWithGuidId
                    {
                        Id = id,
                        Name = "Joe",
                    };

                });

            "when adding a new entity with a manually assigned Guid id value"
                .Do(() =>
                {
                    repo.Add(entity);
                });

            "then the entity id is overwritten and cannot be retrieved by the originally assigned value"
                .Assert(() =>
                    Assert.Null(repo.Get<EntityWithGuidId>(id))
                );

            "then the entity can be retrieved by querying but has a new assigned value"
                .Assert(() =>
                    {
                        var match = repo.Advanced.Query<EntityWithGuidId>().Find();
                        Assert.True(match.Items.All(x => x.Id != id));
                    });
        }

        [Specification]
        public void can_configure_entity_with_id_named_something_other_than_id()
        {
            var repo = default(InMemoryRepository);
            var entity = default(EntityWithIntegerIdNotCalledId);

            "Given an in memory repository with an entity with id value named 'Identifier'"
                .Context(() =>
                {
                    repo = new InMemoryRepository(cfg =>
                    {
                        cfg.ConfigureIdFor<EntityWithIntegerIdNotCalledId, int>(x => x.Identifier, (t, i) => t.Identifier = i);
                    });

                    entity = new EntityWithIntegerIdNotCalledId
                    {
                        Name = "Joe",
                    };

                });

            "when adding a new entity"
                .Do(() =>
                {
                    repo.Add(entity);
                });

            "then the entity can be retrieved by querying and has an autogenerated id value assigned"
                .Assert(() =>
                    {
                        var match = repo.Advanced.Query<EntityWithIntegerIdNotCalledId>().Find();
                        Assert.True(match.Items.All(x => x.Identifier != default(int)));
                    }
                );
        }

        [Specification]
        public void retrieving_entity_type_where_no_entities_of_that_type_exist()
        {
            var repo = default(InMemoryRepository);
            var entity = default(AnEntity);

            "Given an in memory repository with no entities"
                .Context(() =>
                {
                    repo = new InMemoryRepository(cfg =>
                    {
                        cfg.ConfigureIdFor<EntityWithIntegerIdNotCalledId, int>(x => x.Identifier, (t, i) => t.Identifier = i);
                    });
                });

            "when attempting to retrieve an entity by id"
                .Do(() =>
                    {
                        entity = repo.Get<AnEntity>(1);
                    });

            "then returned value is null"
                .Assert(() =>
                    {
                        Assert.Null(entity);
                    });
        }
    }
}