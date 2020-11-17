using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using PathFinder.sdk.Models;
using PathFinder.sdk.Services.RecordAbstract;
using PathFinder.sdk.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Toolbox.Extensions;
using Toolbox.Services;
using Toolbox.Tools;

namespace PathFinder.Cosmos.Store
{
    public class RecordContainer<T> : IRecordContainer<T> where T : IRecord
    {
        private readonly ILogger<T> _logger;
        private readonly Container _container;

        private static readonly HashSet<int> _validStatusCode = new HashSet<int>(new[]
        {
            (int)HttpStatusCode.OK,
            (int)HttpStatusCode.Created,
            (int)HttpStatusCode.NotFound,
            (int)HttpStatusCode.NoContent,
        });

        public RecordContainer(Container container, ILogger<T> logger)
        {
            container.VerifyNotNull(nameof(container));

            _container = container;
            _logger = logger;

            Search = new SearchContainer<T>(_container, logger);
        }

        public string ContainerName => _container.Id;

        public ISearchContainer<T> Search { get; }

        public Task<ETag> Set(T item, CancellationToken token = default) => Set(new Record<T>(item));

        public async Task<ETag> Set(Record<T> record, CancellationToken token = default)
        {
            record.VerifyNotNull(nameof(record));
            record.Prepare();

            _logger.LogTrace($"{nameof(Set)}: {nameof(_container.UpsertItemAsync)} item, {Json.Default.Serialize(record.Value)}");

            var option = new ItemRequestOptions
            {
                IfMatchEtag = record.ETag!,
            };

            ItemResponse<T> itemResponse = await _container.UpsertItemAsync(record.Value, requestOptions: option, cancellationToken: token);

            itemResponse.VerifyAssert(x => IsValid(x.StatusCode), x =>
            {
                string msg = $"{nameof(Set)}: Failed to {nameof(_container.UpsertItemAsync)}, StatusCode={x.StatusCode}";
                _logger.LogError(msg);
                return msg;
            });

            return (ETag)itemResponse.ETag;
        }

        public async Task<Record<T>?> Get(string id, string? partitionKey = null, CancellationToken token = default)
        {
            id = id
                .VerifyNotEmpty(nameof(id))
                .ToLowerInvariant();

            try
            {
                ItemResponse<T> itemResponse = await _container.ReadItemAsync<T>(id, new PartitionKey(partitionKey ?? id), cancellationToken: token);

                itemResponse.VerifyAssert(x => IsValid(x.StatusCode), x =>
                {
                    string msg = $"{nameof(Get)}: Failed to {nameof(_container.ReadItemAsync)} in getting id={id}, StatusCode={x.StatusCode}";
                    _logger.LogError(msg);
                    return msg;
                });

                return itemResponse.StatusCode == HttpStatusCode.OK ? new Record<T>(itemResponse.Resource, (ETag)itemResponse.ETag) : default;
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                _logger.LogWarning($"{nameof(Get)}: Record does not exit, id={id}");
                return default;
            }
        }

        public async Task<bool> Delete(string id, string? eTag = null, string? partitionKey = null, CancellationToken token = default)
        {
            id = id
                .VerifyNotEmpty(nameof(id))
                .ToLowerInvariant();

            _logger.LogTrace($"{nameof(Delete)}: Deleting {id}, eTag={eTag}");

            var option = new ItemRequestOptions
            {
                IfMatchEtag = eTag
            };

            try
            {
                ItemResponse<T> itemResponse = await _container.DeleteItemAsync<T>(id, new PartitionKey(partitionKey ?? id), requestOptions: option, cancellationToken: token);

                itemResponse.VerifyAssert(x => IsValid(x.StatusCode), x =>
                {
                    string msg = $"{nameof(Delete)}: Failed to {nameof(_container.DeleteItemAsync)}, StatusCode={x.StatusCode}";
                    _logger.LogError(msg);
                    return msg;
                });

                return itemResponse.StatusCode == HttpStatusCode.NoContent ? true : false;
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                _logger.LogWarning($"{nameof(Get)}: Record does not exit, id={id}");
                return default;
            }
        }

        public async Task<bool> Exist(string id, CancellationToken token = default)
        {
            id.VerifyNotEmpty(nameof(id)).ToLowerInvariant();

            KeyValuePair<string, string>[] parameters = new[]
            {
                new KeyValuePair<string, string>("id", id)
            };

            var result = await Search.Query($"select * from ROOT r where r.id = \"@id\"", parameters, token);

            return result.Count > 0;
        }

        private bool IsValid(HttpStatusCode httpStatusCode) => _validStatusCode.Contains((int)httpStatusCode);
    }
}
