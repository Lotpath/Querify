using System;
using System.Linq.Expressions;

namespace Querify.Tests
{
    public class AnEntityFiltered : Specification<AnEntity>
    {
        private readonly int? _id;
        private readonly string _nameStartingWith;
        private readonly string _nameLike;
        private readonly DateTime? _onOrAfter;
        private readonly DateTime? _before;

        private AnEntityFiltered(int? id, string nameStartingWith, string nameLike, DateTime? onOrAfter,
                                 DateTime? before)
        {
            _id = id;
            _nameStartingWith = nameStartingWith;
            _nameLike = nameLike;
            _onOrAfter = onOrAfter;
            _before = before;
        }

        public static AnEntityFiltered By
        {
            get { return new AnEntityFiltered(null, null, null, null, null); }
        }

        public AnEntityFiltered IdEqualTo(int id)
        {
            return new AnEntityFiltered(id, _nameStartingWith, _nameLike, _onOrAfter, _before);
        }

        public AnEntityFiltered NameStartingWith(string nameStartingWith)
        {
            return new AnEntityFiltered(_id, nameStartingWith, _nameLike, _onOrAfter, _before);
        }

        public AnEntityFiltered NameLike(string nameLike)
        {
            return new AnEntityFiltered(_id, _nameStartingWith, nameLike, _onOrAfter, _before);
        }

        public AnEntityFiltered OnOrAfter(DateTime onOrAfter)
        {
            return new AnEntityFiltered(_id, _nameStartingWith, _nameLike, onOrAfter, _before);
        }

        public AnEntityFiltered Before(DateTime before)
        {
            return new AnEntityFiltered(_id, _nameStartingWith, _nameLike, _onOrAfter, before);
        }

        public override Expression<Func<AnEntity, bool>> IsSatisfiedBy()
        {
            var expr = Expressions.Apply<AnEntity>(() => _id.HasValue, null, x => x.Id == _id.Value);
            expr = Expressions.Apply(() => !string.IsNullOrEmpty(_nameStartingWith), expr,
                                     x => x.Name.ToLowerInvariant().StartsWith(_nameStartingWith.ToLowerInvariant()));
            expr = Expressions.Apply(() => !string.IsNullOrEmpty(_nameLike), expr,
                                     x => x.Name.ToLowerInvariant().Contains(_nameLike.ToLowerInvariant()));
            expr = Expressions.Apply(() => _onOrAfter.HasValue, expr, x => x.SomeDate >= _onOrAfter.Value);
            expr = Expressions.Apply(() => _before.HasValue, expr, x => x.SomeDate < _before.Value);
            return expr;
        }
    }
}