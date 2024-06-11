using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace NOS.Engineering.Challenge.Database
{
    public class CachedDatabase<TOut, TIn> : IDatabase<TOut, TIn>
    {
        private readonly IDatabase<TOut, TIn> _slowDatabase;
        private readonly IMemoryCache _cache;

        public CachedDatabase(IDatabase<TOut, TIn> slowDatabase, IMemoryCache cache)
        {
            _slowDatabase = slowDatabase;
            _cache = cache;
        }

        public async Task<TOut?> Create(TIn item)
        {
            return await _slowDatabase.Create(item);
        }

        public async Task<TOut?> Read(Guid id)
        {
            var cacheKey = $"Read_{id}";

            if (!_cache.TryGetValue(cacheKey, out TOut? item))
            {
                item = await _slowDatabase.Read(id);
                if (item != null)
                {
                    _cache.Set(cacheKey, item, TimeSpan.FromMinutes(1)); 
                }
            }

            return item;
        }

        public async Task<IEnumerable<TOut?>> ReadAll()
        {
            var cacheKey = "ReadAll";

            if (!_cache.TryGetValue(cacheKey, out IEnumerable<TOut?> items))
            {
                items = await _slowDatabase.ReadAll();
                if (items != null)
                {
                    _cache.Set(cacheKey, items, TimeSpan.FromMinutes(1)); 
                }
            }

            return items;
        }

        public async Task<TOut?> Update(Guid id, TIn item)
        {
            var updatedItem = await _slowDatabase.Update(id, item);
            if (updatedItem != null)
            {
                // Atualizar o cache após a atualização
                var cacheKey = $"Read_{id}";
                _cache.Set(cacheKey, updatedItem, TimeSpan.FromMinutes(1)); 
            }
            return updatedItem;
        }

        public async Task<Guid> Delete(Guid id)
        {
            var deletedId = await _slowDatabase.Delete(id);
            if (deletedId != Guid.Empty)
            {
                // Remover do cache após a exclusão
                var cacheKey = $"Read_{id}";
                _cache.Remove(cacheKey);
            }
            return deletedId;
        }
        public async Task<Content?> AddGenres(Guid id, IEnumerable<string> genres)
        {
            var content = await _slowDatabase.Read(id);

            if (content == null)
            {
                return null;
            }

            foreach (var genre in genres)
            {
                if (!content.GenreList.Contains(genre))
                {
                    content.GenreList.Add(genre);
                }
            }
            var updatedContent = await _slowDatabase.Update(id, content);
            return updatedContent;
        }


        public async Task<Content?> RemoveGenres(Guid id, IEnumerable<string> genres)
        {
            var content = await _slowDatabase.Read(id);

            if (content == null)
            {
                return null;
            }

            foreach (var genre in genres)
            {
                content.GenreList.Remove(genre);
            }

            var updatedContent = await _slowDatabase.Update(id, content);

            return updatedContent;
        }

        
    }
}
