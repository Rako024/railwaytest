using Expo.Core.Entities;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Expo.Data.Repositories.Abstracts
{
    public interface IRepositroy<T> where T : BaseEntitiy, new()
    {

        //Reading
        Task<IList<T>> GetAllAsync(Expression<Func<T, bool>>? pradicate = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            bool enableTracing = false
            );
        Task<IList<T>> GetAllByPagingAsync(Expression<Func<T, bool>>? pradicate = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            bool enableTracing = false,
            int currentPage = 1, int pageSize = 3
            );

        Task<T> GetAsync(Expression<Func<T, bool>> pradicate,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
            bool enableTracing = true
            );

        IQueryable<T> Find(Expression<Func<T, bool>> pradicate, bool ebanleTracking = false);

        Task<int> CountAsync(Expression<Func<T, bool>>? pradicate = null);


        //Write
        Task AddAsync(T entity);
        Task AddRangeAsync(IList<T> entities);
        Task<T> UpdateAsync(T entity);
        Task HardDeleteAsync(T entity);
        Task HardDeleteByIdAsync(int id);


        //Save
        Task<int> Save();
    }
}
