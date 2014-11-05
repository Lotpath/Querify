using System;
using SubSpec;
using Xunit;

namespace Querify.Tests
{
    public class InMemoryRepositoryTests
    {
        [Specification]
        public void adding_and_getting_items()
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
        public void using_advanced_repository()
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
                        Id = 4,
                        Name = "Joe",
                        SomeDate = new DateTime(2014, 1, 1)
                    });
                    repo.Add(new AnEntity
                    {
                        Id = 5,
                        Name = "Jim",
                        SomeDate = new DateTime(2014, 1, 2)
                    });
                });

            "then entities can be retrieved using the advanced repository"
                .Assert(() =>
                    {
                        var match = repo.Advanced.Query<AnEntity>()
                            .FindOne(AnEntityFiltered.By.NameStartingWith("Jo"));
                        Assert.NotNull(match);
                    }
                );
        }
    }
}