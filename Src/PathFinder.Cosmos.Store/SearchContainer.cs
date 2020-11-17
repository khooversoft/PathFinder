using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using PathFinder.sdk.Models;
using PathFinder.sdk.Services.RecordAbstract;
using PathFinder.sdk.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Toolbox.Extensions;
using Toolbox.Tools;

namespace PathFinder.Cosmos.Store
{
    public class SearchContainer<T> : ISearchContainer<T> where T : IRecord
    {
        private readonly Container _container;
        private readonly ILogger _logger;
        public SearchContainer(Container container, ILogger logger)
        {
            _container = container;
            _logger = logger;
        }

        public async Task<IReadOnlyList<T>> List(QueryParameters queryParameters, CancellationToken token = default)
        {
            IReadOnlyList<string> tags = queryParameters.Tag
                ?.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                ?.Select(x => $"CONTAINS(n.Key, {x}, true)")
                ?.ToArray() ?? Array.Empty<string>();

            IReadOnlyList<string> restrictions = new string?[]
            {
                queryParameters.ToSqlIdSearch(),
                queryParameters.ToSqlRedirectUrlSearch(),
                queryParameters.ToSqlOwnerSearch(),
                !queryParameters.Tag.IsEmpty() ? $"exists{Construct("n in r.Tags", tags)}" : null,
            }.Where(x => x != null)
            .ToArray()!;

            string query = new string?[]
            {
                Construct("root r", restrictions),
                queryParameters.ToSqlOffset(),
                queryParameters.ToSqlLimit(),
            }.Func(x => string.Join(" ", x));

            return await Query(query, token: token);

            static string Construct(string from, IReadOnlyList<string> restrictions) => new string?[]
            {
                "select * from",
                from,
                restrictions.Count > 0 ? "where" : null,
                restrictions.Count > 1 ? "(" : null,
                string.Join(" or ", restrictions),
                restrictions.Count > 1 ? ")" : null,
            }
            .Where(x => x != null)
            .Func(x => string.Join(" ", x));
        }


        public async Task<IReadOnlyList<T>> Query(string sqlQuery, IEnumerable<KeyValuePair<string, string>>? parameters = null, CancellationToken token = default)
        {
            sqlQuery.VerifyNotEmpty(nameof(sqlQuery));
            parameters ??= Array.Empty<KeyValuePair<string, string>>();

            try
            {
                var list = new List<T>();

                _logger.LogTrace($"{nameof(Query)}: Query={sqlQuery.WithParameters(parameters)}");
                var queryDefinition = new QueryDefinition(sqlQuery);

                queryDefinition = parameters
                    .Select(x => queryDefinition.WithParameter(decorateKey(x.Key), x.Value))
                    .LastOrDefault()
                    ?? queryDefinition;

                using FeedIterator<T> feedIterator = _container.GetItemQueryIterator<T>(queryDefinition);

                while (feedIterator.HasMoreResults)
                {
                    foreach (T item in await feedIterator.ReadNextAsync())
                    {
                        list.Add(item);
                    }
                }

                _logger.LogTrace($"{nameof(Query)}: Query={sqlQuery.WithParameters(parameters)}, RecordCount={list.Count}");

                return list;
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"{nameof(Query)}: Error {ex.Message} for {sqlQuery}");
                return Array.Empty<T>();
            }

            static string decorateKey(string key) => key.StartsWith("@") ? key : "@" + key;
        }
    }
}