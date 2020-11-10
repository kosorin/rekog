using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Rekog.Core
{
    public class Alphabet
    {
        private static readonly Func<char, bool> AllComparer = x => !char.IsControl(x) || x == '\t';

        private readonly Func<char, bool> _comparer;

        public Alphabet() : this(false)
        {
        }

        public Alphabet(bool caseSensitive) : this("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz", caseSensitive)
        {
        }

        public Alphabet(IEnumerable<char> characters) : this(characters, false)
        {
        }

        public Alphabet(IEnumerable<char> characters, bool caseSensitive)
        {
            (_comparer, Characters) = CreateComparer(characters, caseSensitive);
            CaseSensitive = caseSensitive;
        }

        public List<char> Characters { get; }

        public bool CaseSensitive { get; }

        public bool Contains(char character)
        {
            return _comparer(character);
        }

        private static (Func<char, bool>, List<char>) CreateComparer(IEnumerable<char> rawCharacters, bool caseSensitive)
        {
            if (!caseSensitive)
            {
                rawCharacters = rawCharacters.SelectMany(x => new[] { char.ToLower(x), char.ToUpper(x) });
            }
            rawCharacters = rawCharacters
                .Distinct()
                .Where(AllComparer);
            var characters = rawCharacters.ToList();

            Func<char, bool> comparer;
            if (characters.Any())
            {
                var parameter = Expression.Parameter(typeof(char));
                var conditions = characters.Select(x => Expression.Equal(parameter, Expression.Constant(x, typeof(char)))).Cast<Expression>();
                var body = conditions.Skip(1).Aggregate(conditions.First(), (acumulate, condition) => Expression.Or(acumulate, condition));
                var lambda = Expression.Lambda(body, parameter);
                comparer = (Func<char, bool>)lambda.Compile();
            }
            else
            {
                comparer = AllComparer;
            }

            return (comparer, characters);
        }
    }
}
