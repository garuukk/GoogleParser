using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Svyaznoy.Core.Model
{
    public class InventoryCache
    {
        private readonly Dictionary<string, Dictionary<Type, List<CacheEntity>>> _cache = new Dictionary<string, Dictionary<Type, List<CacheEntity>>>();

        private const int _defaultTime = 300000;

        private int _cacheTimeout = _defaultTime;

        private int _cleanTimeOut = _defaultTime;
        private DateTime _lastCleanTime = DateTime.Now;
        private bool _enabled;

        /// <summary>
        /// ms
        /// </summary>
        public int CacheTimeout
        {
            get { return _cacheTimeout; }
            set
            {
                _cacheTimeout = value;
                if (_cacheTimeout < 0)
                {
                    _cacheTimeout = 0;
                }

                if (_cacheTimeout > _defaultTime)
                {
                    _cleanTimeOut = _cacheTimeout;
                }
            }
        }

        public bool Enabled
        {
            get { return _enabled; }
            set
            {
                _enabled = value;
                if (!_enabled)
                {
                    _cache.Clear();
                }
            }
        }

        private CacheEntity GetCacheEntity<T>(IInventory inventory, Expression<Func<T, bool>> where = null)
        {
            if (!_cache.ContainsKey(inventory.ConnectionString))
            {
                return null;
            }

            var typeCache = _cache[inventory.ConnectionString];

            if (typeCache != null && typeCache.ContainsKey(typeof(T)))
            {
                var entities = typeCache[typeof(T)];

                if (entities == null)
                {
                    return null;
                }

                var whereString = where == null ? null : where.ToString();

                var entity = entities.FirstOrDefault(e => e.WherePredicate == whereString);

                return entity;
            }

            return null;
        }

        private void CleanOldCacheEntities()
        {
            _lastCleanTime = DateTime.Now;
            lock (_cache)
            {
                foreach (var cache in _cache)
                {
                    var typeDictionary = cache.Value;
                    if (typeDictionary != null)
                    {
                        foreach (var type in typeDictionary)
                        {
                            if (type.Value.HasValues())
                            {
                                var entitiesList = type.Value;
                                if (entitiesList != null)
                                {
                                    for (int i = 0; i < entitiesList.Count; i++)
                                    {
                                        var entity = entitiesList[i];
                                        if (IsOld(entity))
                                        {
                                            entitiesList.RemoveAt(i);
                                            i--;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private CacheEntity CreateCacheEntity<T>(IInventory inventory, Expression<Func<T, bool>> where = null)
        {
            var entity = GetCacheEntity(inventory, where: where);

            if (entity == null)
            {
                lock (_cache)
                {
                    entity = GetCacheEntity(inventory, where: where);
                    if (entity == null)
                    {
                        if (!_cache.ContainsKey(inventory.ConnectionString))
                        {
                            _cache[inventory.ConnectionString] = new Dictionary<Type, List<CacheEntity>>();
                        }

                        var typeCache = _cache[inventory.ConnectionString];

                        if (!typeCache.ContainsKey(typeof (T)))
                        {
                            typeCache[typeof(T)] = new List<CacheEntity>();
                        }

                        var entities = typeCache[typeof(T)];

                        var whereString = where == null ? null : where.ToString();

                        entity = entities.FirstOrDefault(e => e.WherePredicate == whereString);

                        if (entity == null)
                        {
                            entity = new CacheEntity()
                            {
                                WherePredicate = whereString,
                            };
                            entities.Add(entity);
                        }

                        return entity;
                    }
                }
            }
            
            return entity;
        }

        private bool IsOld(CacheEntity entity)
        {
            return entity != null && (DateTime.Now - entity.DateTime).TotalMilliseconds > CacheTimeout;
        }

        public IEnumerable<T> Cache<T>(IQueryable<T> source, IInventory inventory, Expression<Func<T, bool>> where = null)
            where T : class, Svyaznoy.Core.Model.IEntity
        {
            if (!Enabled || CacheTimeout <= 0)
            {
                var q = source;

                if (where != null)
                {
                    q = q.Where(where);
                }

                return q;
            }
            else
            {
                if ((DateTime.Now - _lastCleanTime).TotalMilliseconds > _cleanTimeOut)
                {
                    CleanOldCacheEntities();
                }

                var entity = GetCacheEntity(inventory, @where) ?? CreateCacheEntity(inventory, @where);

                if (entity.Values == null || IsOld(entity))
                {
                    entity.Values = source.Where(where).ToList().AsEnumerable();
                }

                return entity.Values as IEnumerable<T>;
            }
        }

        private class CacheEntity
        {
            public DateTime DateTime { get; set; }

            public string WherePredicate { get; set; }

            public Object Values { get; set; }
        }
    }

    public static class CacheExtension
    {
        public readonly static InventoryCache InventoryCache = new InventoryCache();

        public static IEnumerable<T> Cache<T>(this IQueryable<T> source, IInventory inventory, Expression<Func<T, bool>> where = null)
            where T : class, Svyaznoy.Core.Model.IEntity
        {
            return InventoryCache.Cache(source, inventory, where: where);
        }
    }
}