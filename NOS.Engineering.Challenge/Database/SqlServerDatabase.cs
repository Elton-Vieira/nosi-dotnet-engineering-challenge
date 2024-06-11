using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NOS.Engineering.Challenge.Models;

namespace NOS.Engineering.Challenge.Database
{
    public class SqlServerDatabase : IDatabase<Content, ContentDto>
    {
        private readonly DbContext _dbContext;
        private readonly IMapper<Content?, ContentDto> _mapper;

        public SqlServerDatabase(DbContext dbContext, IMapper<Content?, ContentDto> mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<Content?> Create(ContentDto item)
        {
            var content = _mapper.Map(Guid.NewGuid(), item);
            _dbContext.Set<Content>().Add(content);
            await _dbContext.SaveChangesAsync();
            return content;
        }

        public Task<Content?> Read(Guid id)
        {
            return _dbContext.Set<Content>().FindAsync(id);
        }

        public Task<IEnumerable<Content?>> ReadAll()
        {
            return Task.FromResult<IEnumerable<Content?>>(_dbContext.Set<Content>().ToList());
        }

        public async Task<Content?> Update(Guid id, ContentDto item)
        {
            var content = await _dbContext.Set<Content>().FindAsync(id);
            if (content == null)
                return null;

            content = _mapper.Patch(content, item);
            await _dbContext.SaveChangesAsync();
            return content;
        }

        public async Task<Guid> Delete(Guid id)
        {
            var content = await _dbContext.Set<Content>().FindAsync(id);
            if (content == null)
                return Guid.Empty;

            _dbContext.Set<Content>().Remove(content);
            await _dbContext.SaveChangesAsync();
            return id;
        }

        public async Task<Content?> AddGenres(Guid id, IEnumerable<string> genres)
        {
            var content = await _dbContext.Set<Content>().FindAsync(id);
            if (content == null)
                return null;

            foreach (var genre in genres)
            {
                content.GenreList.Add(genre);
            }

            await _dbContext.SaveChangesAsync();
            return content;
        }

        public async Task<Content?> RemoveGenres(Guid id, IEnumerable<string> genres)
        {
            var content = await _dbContext.Set<Content>().FindAsync(id);
            if (content == null)
                return null;

            foreach (var genre in genres)
            {
                content.GenreList.Remove(genre);
            }

            await _dbContext.SaveChangesAsync();
            return content;
        }
    }
}
