using DotNetCoreWebSample.Web.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DotNetCoreWebSample.Web.Repositories
{
    public interface IRepository<TEntity>
    {
        void Add(TEntity entity);
        void Update(TEntity entity);
        TEntity Find(params object[] keyValues);
        Task<TEntity> FindAsync(params object[] keyValues);
        IList<TEntity> Get(Expression<Func<TEntity, bool>> predicate);
        Task<IList<TEntity>> GetAsync(Expression<Func<TEntity, bool>> predicate);
        IList<TEntity> Get();
        Task<IList<TEntity>> GetAsync();
        int GetCount();
        Task<int> GetCountAsync();
        void Remove(TEntity entity);
        void Save();
        Task<int> SaveAsync();
    }

    abstract public class Repository<TEntity> : IRepository<TEntity>
        where TEntity : class
    {
        protected readonly DotnetCoreWebSampleContext _db;
        protected readonly DbSet<TEntity> _set;

        protected Repository(DotnetCoreWebSampleContext db)
        {
            _db = db;
            _set = _db.Set<TEntity>();
        }

        /// <summary>
        /// レコードを追加します (Insert)
        /// </summary>
        /// <param name="entity"></param>
        public void Add(TEntity entity)
        {
            _set.Add(entity);
        }

        /// <summary>
        /// レコードを更新します (Merge)
        /// </summary>
        /// <param name="entity"></param>
        public void Update(TEntity entity)
        {
            _set.Update(entity);
        }

        /// <summary>
        /// キーを条件としてレコードを取得します
        /// </summary>
        /// <param name="keyValues"></param>
        public TEntity Find(params object[] keyValues)
        {
            return _set.Find(keyValues);
        }

        /// <summary>
        /// キーを条件としてレコードを取得します
        /// </summary>
        /// <param name="keyValues"></param>
        public async Task<TEntity> FindAsync(params object[] keyValues)
        {
            return await _set.FindAsync(keyValues);
        }

        /// <summary>
        /// Where句 (ラムダ式) を条件としてレコードを取得します
        /// </summary>
        /// <param name="predicate"></param>
        public IList<TEntity> Get(Expression<Func<TEntity, bool>> predicate)
        {
            return _set.Where(predicate).ToList();
        }

        /// <summary>
        /// Where句 (ラムダ式) を条件としてレコードを取得します
        /// </summary>
        /// <param name="predicate"></param>
        public async Task<IList<TEntity>> GetAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _set.Where(predicate).ToListAsync();
        }

        /// <summary>
        /// レコードを全件取得します
        /// </summary>
        public IList<TEntity> Get()
        {
            return _set.ToList();
        }

        /// <summary>
        /// レコードを全件取得します
        /// </summary>
        public async Task<IList<TEntity>> GetAsync()
        {
            return await _set.ToListAsync();
        }

        /// <summary>
        /// レコード件数を取得します
        /// </summary>
        public int GetCount()
        {
            return _set.Count();
        }

        /// <summary>
        /// レコード件数を取得します
        /// </summary>
        public async Task<int> GetCountAsync()
        {
            return await _set.CountAsync();
        }

        /// <summary>
        /// レコードを削除します
        /// </summary>
        /// <param name="entity"></param>
        public void Remove(TEntity entity)
        {
            _set.Remove(entity);
        }

        /// <summary>
        /// レコードの変更を保存します
        /// </summary>
        public void Save()
        {
            _db.SaveChanges();
        }

        /// <summary>
        /// レコードの変更を保存します
        /// </summary>
        public async Task<int> SaveAsync()
        {
            return await _db.SaveChangesAsync();
        }
    }
}
