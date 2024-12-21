using Expo.Core.Entities;
using Expo.Data.DAL;
using Expo.Data.Repositories.Abstracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Expo.Data.Repositories.Concretes
{
    public class Repository<T> : IRepositroy<T> where T : BaseEntitiy, new()
    {
        private readonly ExpoContext _context;
        public Repository(ExpoContext expoContext)
        {
            _context = expoContext;
        }
        private DbSet<T> Table { get => _context.Set<T>(); }

        public async Task<int> Save()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task AddAsync(T entity)
        {
             await Table.AddAsync(entity);
        }
        
        public async Task AddRangeAsync(IList<T> entities)
        {
            await Table.AddRangeAsync(entities);
        }

        public async Task HardDeleteAsync(T entity)
        {
            await Task.Run(() => Table.Remove(entity));
           
        }

        public async Task HardDeleteByIdAsync(int id)
        {
            T? entity =  await Task.Run(() => Table.Where(x=>x.Id == id).FirstOrDefaultAsync());
            if(entity != null) { 
            await Task.Run(()=> Table.Remove(entity)); 
            }
        }



        public async Task<T> UpdateAsync(T entity)
        {
            await Task.Run(() => Table.Update(entity));
            return entity;
        }
    
        public async Task<int> CountAsync(Expression<Func<T, bool>>? pradicate = null)
        {
            var query = Table.AsQueryable();
            if (pradicate is not null)
            {
                query = query.Where(pradicate);
            }
            return await query.AsNoTracking().CountAsync();
        }

        public  IQueryable<T> Find(Expression<Func<T, bool>> pradicate, bool ebanleTracking = false)
        {
            var query = Table.Where(pradicate);
            if (!ebanleTracking)
            {
                query = query.AsNoTracking();
            }
            return query;
        }

        public async Task<IList<T>> GetAllAsync(Expression<Func<T, bool>>? pradicate = null, Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, bool enableTracing = false)
        {
            IQueryable<T> queryable = Table;
            if(!enableTracing) queryable = queryable.AsNoTracking();
            if(include is not null) queryable = include(queryable);
            if(pradicate is not null) queryable = queryable.Where(pradicate);
            if(orderBy is not null)
                return await orderBy(queryable).ToListAsync();

            return await queryable.ToListAsync();

        }

        public async Task<IList<T>> GetAllByPagingAsync(Expression<Func<T, bool>>? pradicate = null, Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, bool enableTracing = false, int currentPage = 1, int pageSize = 3)
        {
            IQueryable<T> queryable = Table;
            if (!enableTracing) queryable = queryable.AsNoTracking();
            if (include is not null) queryable = include(queryable);
            if (pradicate is not null) queryable = queryable.Where(pradicate);
            if (orderBy is not null)
                return await orderBy(queryable).Skip((currentPage - 1) * pageSize).Take(pageSize).ToListAsync();

            return await queryable.Skip((currentPage - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        public async Task<T> GetAsync(Expression<Func<T, bool>> pradicate, Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null, bool enableTracing = true)
        {
            IQueryable<T> queryable = Table;
            if (!enableTracing) queryable = queryable.AsNoTracking();
            if (include is not null) queryable = include(queryable);
            
 
            return await queryable.Where(pradicate).FirstOrDefaultAsync();
        }

    }
}
