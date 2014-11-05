using System;
using System.Collections.Generic;
using System.Linq;
using FizzWare.NBuilder;
using Querify.Advanced;
using SubSpec;
using Xunit;

namespace Querify.Tests
{
    public class QueryableExtensionsTests
    {
        private readonly IList<AnEntity> _source;

        public QueryableExtensionsTests()
        {
            _source = Builder<AnEntity>.CreateListOfSize(200).Build();
        }

        [Specification]
        public void advanced_fetch_all_retrieves_all_items()
        {
            var queryable = default(IQueryable<AnEntity>);
            var result = default(IList<AnEntity>);

            "Given a queryable".Context(() =>
                {
                    queryable = _source.AsQueryable();
                });

            "when performing an advanced fetch-all"
                .Do(() =>
                    {
                        result = queryable.Advanced().FetchAll().ToList();
                    });

            "then all items are retrieved"
                .Assert(() =>
                        Assert.Equal(_source.Count, result.Count));
        }

        [Specification]
        public void advanced_fetch_all_with_expression_retrieves_all_matching_items()
        {
            var queryable = default(IQueryable<AnEntity>);
            var result = default(IList<AnEntity>);

            "Given a queryable".Context(() =>
                {
                    queryable = _source.AsQueryable();
                });

            "when performing an advanced fetch-all with a filter"
                .Do(() =>
                    {
                        result = queryable.Advanced()
                                          .FetchAll(AnEntityFiltered.By.NameStartingWith("Name1")).ToList();
                    });

            "then all items are retrieved"
                .Assert(() =>
                        Assert.Equal(_source.Count(x => x.Name.StartsWith("Name1")), result.Count));
        }

        [Specification]
        public void find_one_or_throw_with_existing_item()
        {
            var queryable = default(IQueryable<AnEntity>);
            var result = default(AnEntity);

            "Given a queryable".Context(() =>
                {
                    queryable = _source.AsQueryable();
                });

            "when performing a find-one-or-throw where a match exists"
                .Do(() =>
                    {
                        result = queryable.FindOneOrThrow(AnEntityFiltered.By.IdEqualTo(4)).Value;
                    });

            "then a matching result is returned"
                .Assert(() =>
                        Assert.NotNull(result));

            "then the returned result is an object with the expected id"
                .Assert(() =>
                        Assert.Equal(4, result.Id));
        }

        [Specification]
        public void find_one_or_throw_with_no_matching_item()
        {
            var queryable = default(IQueryable<AnEntity>);
            var exception = default(Exception);

            "Given a queryable".Context(() =>
                {
                    queryable = _source.AsQueryable();
                });

            "when performing a find-one-or-throw where no match exists"
                .Do(() => exception = Record.Exception(() =>
                    {
                        var r =
                            queryable.FindOneOrThrow(AnEntityFiltered.By.IdEqualTo(_source.Max(x => x.Id) + 1)).Value;
                    }));

            "then an exception is thrown"
                .Assert(() =>
                        Assert.NotNull(exception));

            "then the thrown exception is a NoMatchFoundException"
                .Assert(() =>
                        Assert.IsType<NoMatchFoundException>(exception));
        }

        [Specification]
        public void find_one_with_existing_item()
        {
            var queryable = default(IQueryable<AnEntity>);
            var result = default(AnEntity);

            "Given a queryable".Context(() =>
                {
                    queryable = _source.AsQueryable();
                });

            "when performing a find-one where a match exists"
                .Do(() =>
                    {
                        result = queryable.FindOne(AnEntityFiltered.By.IdEqualTo(4)).Value;
                    });

            "then a matching result is returned"
                .Assert(() =>
                        Assert.NotNull(result));

            "then the returned result is an object with the expected id"
                .Assert(() =>
                        Assert.Equal(4, result.Id));
        }

        [Specification]
        public void find_one_with_no_matching_item()
        {
            var queryable = default(IQueryable<AnEntity>);
            var result = default(AnEntity);

            "Given a queryable".Context(() =>
                {
                    queryable = _source.AsQueryable();
                });

            "when performing a find-one where no match exists"
                .Do(() =>
                    result = queryable.FindOne(AnEntityFiltered.By.IdEqualTo(_source.Max(x => x.Id) + 1)).Value);

            "then no matching result is returned"
                .Assert(() =>
                        Assert.Null(result));
        }

        [Specification]
        public void find_with_page_size_exceeding_max_page_size()
        {
            var queryable = default(IQueryable<AnEntity>);
            var exception = default(Exception);

            "Given a queryable".Context(() =>
                {
                    queryable = _source.AsQueryable();
                });

            "when performing a find with a page size larger than the max page size"
                .Do(() => exception = Record.Exception(() =>
                    {
                        queryable.Find(pageSize: QueryableExtensions.MaxPageSize + 1);
                    }));

            "then an exception is thrown"
                .Assert(() =>
                        Assert.NotNull(exception));

            "then the thrown exception is a ArgumentOutOfRangeException"
                .Assert(() =>
                        Assert.IsType<ArgumentOutOfRangeException>(exception));
        }

        [Specification]
        public void find_with_no_filter_returns_one_page_of_results()
        {
            var queryable = default(IQueryable<AnEntity>);
            var result = default(PagedResult<AnEntity>);

            "Given a queryable".Context(() =>
                {
                    queryable = _source.AsQueryable();
                });

            "when performing a find with no expression and default paging configuration"
                .Do(() => result = queryable.Find());

            "then one page of results is returned"
                .Assert(() =>
                        Assert.Equal(QueryableExtensions.DefaultPageSize, result.Items.Count()));

            "then total item count equals count of all available items"
                .Assert(() =>
                        Assert.Equal(_source.Count, result.TotalItems));
        }
    }
}