﻿using System;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Adnc.Infra.Caching.StackExchange
{
    /// <summary>
    /// Default redis caching provider.
    /// </summary>
    public partial class DefaultRedisProvider : Adnc.Infra.Caching.IDistributedLocker
    {
        public (bool Success, string LockValue) Lock(string cacheKey, int timeoutSeconds = 5, bool autoDelay = true)
        {
            return _redisDb.Lock(cacheKey, timeoutSeconds, autoDelay);
        }

        public async Task<(bool Success, string LockValue)> LockAsync(string cacheKey, int timeoutSeconds = 5, bool autoDelay = true)
        {
            return await _redisDb.LockAsync(cacheKey, timeoutSeconds, autoDelay);
        }

        public bool SafedUnLock(string cacheKey, string lockValue)
        {
            return _redisDb.SafedUnLock(cacheKey, lockValue);
        }

        public async Task<bool> SafedUnLockAsync(string cacheKey, string lockValue)
        {
            return await _redisDb.SafedUnLockAsync(cacheKey, lockValue);
        }
    }
}
