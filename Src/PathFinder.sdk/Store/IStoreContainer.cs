﻿using PathFinder.sdk.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PathFinder.sdk.Store
{
    public interface IStoreContainer
    {
        Task<IRecordContainer<T>> Create<T>(TimeSpan? defaultTimeToLive = null, string? partitionKey = null, CancellationToken token = default) where T : IRecord;
        Task<IRecordContainer<T>> Create<T>(string containerName, TimeSpan? defaultTimeToLive = null, string? partitionKey = null, CancellationToken token = default) where T : IRecord;
        Task<bool> Delete(string containerName, CancellationToken token = default);
        IRecordContainer<T> Get<T>() where T : class, IRecord;
        IRecordContainer<T> Get<T>(string containerName) where T : class, IRecord;
    }
}