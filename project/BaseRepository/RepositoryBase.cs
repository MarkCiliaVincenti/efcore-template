﻿using chatgptwriteproject.Context;
using chatgptwriteproject.DbFactories;
using chatgptwriteproject.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace chatgptwriteproject.BaseRepository
{
    public abstract class RepositoryBase<RCtx,WCtx,TEntity> : IRepository<TEntity> where TEntity : class where RCtx : DbContext where WCtx: DbContext
    {
        private readonly RCtx _readContext;
        private readonly WCtx _writeContext;

        public RepositoryBase(DbFactory<RCtx, WCtx> dbContext)
        {
            _readContext = dbContext.Context.Item1;
            _writeContext= dbContext.Context.Item2;
        }

        public void Add(TEntity entity)
        {
            _writeContext.Set<TEntity>().Add(entity);
        }

        public void Add(List<TEntity> entity)
        {
            _writeContext.Set<List<TEntity>>().Add(entity);
        }

        public void Delete(TEntity entity)
        {
            _writeContext.Set<TEntity>().Remove(entity);
        }

        public void Delete(List<TEntity> entity)
        {
            _writeContext.Set<List<TEntity>>().Remove(entity);
        }

        public async Task<IEnumerable<TEntity>> GetList()
        {
            var result = await _readContext.Set<TEntity>().AsNoTracking().ToListAsync();
            return result;
        }
        public IQueryable<TEntity> GetQuerable()
        {
            return _writeContext.Set<TEntity>().AsQueryable();
        }

        public void Update(TEntity entity)
        {
            _writeContext.Entry(entity).State = EntityState.Modified;
            _writeContext.Set<TEntity>().Update(entity);
        }

        public void Update(List<TEntity> entity)
        {
            foreach (var item in entity)
            {
                _writeContext.Entry(item).State = EntityState.Modified;
                _writeContext.Set<TEntity>().Update(item);
            }
        }

        public abstract IUnitOfWork GetUnitOfWork();
    }
}