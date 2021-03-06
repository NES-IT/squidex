﻿// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschränkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using Squidex.Infrastructure.MongoDb;

namespace Squidex.Infrastructure.UsageTracking
{
    public sealed class MongoUsageStore : MongoRepositoryBase<MongoUsage>, IUsageStore
    {
        public MongoUsageStore(IMongoDatabase database)
            : base(database)
        {
        }

        protected override string CollectionName()
        {
            return "Usages";
        }

        protected override Task SetupCollectionAsync(IMongoCollection<MongoUsage> collection)
        {
            return collection.Indexes.CreateOneAsync(
                new CreateIndexModel<MongoUsage>(Index.Ascending(x => x.Key).Ascending(x => x.Category).Ascending(x => x.Date)));
        }

        public Task TrackUsagesAsync(DateTime date, string key, string category, double count, double elapsedMs)
        {
            var id = $"{key}_{date:yyyy-MM-dd}_{category}";

            return Collection.UpdateOneAsync(x => x.Id == id && x.Category == category,
                Update
                    .Inc(x => x.TotalCount, count)
                    .Inc(x => x.TotalElapsedMs, elapsedMs)
                    .SetOnInsert(x => x.Id, id)
                    .SetOnInsert(x => x.Key, key)
                    .SetOnInsert(x => x.Date, date)
                    .SetOnInsert(x => x.Category, category),
                Upsert);
        }

        public async Task<IReadOnlyList<StoredUsage>> QueryAsync(string key, DateTime fromDate, DateTime toDate)
        {
            var entities = await Collection.Find(x => x.Key == key && x.Date >= fromDate && x.Date <= toDate).ToListAsync();

            return entities.Select(x => new StoredUsage(x.Category, x.Date, (long)x.TotalCount, (long)x.TotalElapsedMs)).ToList();
        }
    }
}
