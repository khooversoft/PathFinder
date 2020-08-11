﻿using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using PathFinder.Cosmos.Store.Application;
using PathFinder.sdk.Services.RecordAbstract;
using PathFinder.sdk.Store;
using System;
using System.Threading;
using System.Threading.Tasks;
using Toolbox.Extensions;
using Toolbox.Tools;

namespace PathFinder.Cosmos.Store
{
    public class StoreRepository : IStoreContainer
    {
        private const string _recordText = "Record";
        private readonly ICosmosPathFinderOption _storeOption;
        private readonly CosmosClient _cosmosClient;
        private readonly ILogger<StoreRepository> _logger;
        private readonly ILoggerFactory _loggerFactory;

        public StoreRepository(ICosmosPathFinderOption storeOption, CosmosClient cosmosClient, ILoggerFactory loggerFactory, ILogger<StoreRepository> logger)
        {
            storeOption.VerifyNotNull(nameof(storeOption)).Verify();
            cosmosClient.VerifyNotNull(nameof(cosmosClient));
            loggerFactory.VerifyNotNull(nameof(loggerFactory));
            logger.VerifyNotNull(nameof(logger));

            _storeOption = storeOption;
            _cosmosClient = cosmosClient;
            _loggerFactory = loggerFactory;
            _logger = logger;
        }

        public Task<IRecordContainer<T>> Create<T>(TimeSpan? defaultTimeToLive = null, string? partitionKey = null, CancellationToken token = default)
            where T : IRecord =>
            Create<T>(GetContainerName<T>(), defaultTimeToLive, partitionKey, token);

        public async Task<IRecordContainer<T>> Create<T>(string containerName, TimeSpan? defaultTimeToLive = null, string? partitionKey = null, CancellationToken token = default)
            where T : IRecord
        {
            containerName.VerifyNotEmpty(nameof(containerName));
            partitionKey = partitionKey.ToNullIfEmpty() ?? Constants.DefaultPartitionKey;

            _logger.LogTrace($"{nameof(Create)}: Create databaseName={_storeOption.DatabaseName}");
            Database database = await _cosmosClient.CreateDatabaseIfNotExistsAsync(_storeOption.DatabaseName, cancellationToken: token);

            _logger.LogTrace($"{nameof(Create)}: Create container, DatabaseName={_storeOption.DatabaseName}, ContainerName={containerName}, PartitionKey={partitionKey}, DefaultTTL={defaultTimeToLive}");
            ContainerResponse containerResponse = await database.CreateContainerIfNotExistsAsync(new ContainerProperties
            {
                Id = containerName,
                PartitionKeyPath = partitionKey,
                DefaultTimeToLive = (int?)defaultTimeToLive?.TotalSeconds,
            }, cancellationToken: token);

            return new RecordContainer<T>(containerResponse.Container, _loggerFactory.CreateLogger<T>());
        }

        public IRecordContainer<T> Get<T>() where T : class, IRecord => Get<T>(GetContainerName<T>());

        public IRecordContainer<T> Get<T>(string containerName) where T : class, IRecord
        {
            containerName.VerifyNotEmpty(nameof(containerName));

            Database database = _cosmosClient.GetDatabase(_storeOption.DatabaseName);
            Container container = database.GetContainer(containerName);

            return new RecordContainer<T>(container, _loggerFactory.CreateLogger<T>());
        }

        public async Task<bool> Delete(string containerName, CancellationToken token = default)
        {
            containerName.VerifyNotEmpty(nameof(containerName));

            Database database = _cosmosClient.GetDatabase(_storeOption.DatabaseName);
            Container container = database.GetContainer(containerName);

            _logger.LogTrace($"{nameof(Delete)}: Delete container, , DatabaseName={_storeOption.DatabaseName}, ContainerName={containerName}");
            await container.DeleteContainerAsync(cancellationToken: token);

            return true;
        }

        private string GetContainerName<T>() => typeof(T).Name
            .Func(x => x.EndsWith(_recordText) ? x.Substring(0, x.Length - _recordText.Length) : x);
    }
}
