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

    public abstract class Repository<TEntity> : IRepository<TEntity>
        where TEntity : class
    {
        protected readonly DotnetCoreWebSampleContext Db;
        protected readonly DbSet<TEntity> Set;

        protected Repository(DotnetCoreWebSampleContext db)
        {
            Db = db;
            Set = Db.Set<TEntity>();
        }

        /// <summary>
        /// レコードを追加します (Insert)
        /// </summary>
        /// <param name="entity"></param>
        public void Add(TEntity entity)
        {
            Set.Add(entity);
        }

        /// <summary>
        /// レコードを更新します (Merge)
        /// </summary>
        /// <param name="entity"></param>
        public void Update(TEntity entity)
        {
            Set.Update(entity);
        }

        /// <summary>
        /// キーを条件としてレコードを取得します
        /// </summary>
        /// <param name="keyValues"></param>
        public TEntity Find(params object[] keyValues)
        {
            return Set.Find(keyValues);
        }

        /// <summary>
        /// キーを条件としてレコードを取得します
        /// </summary>
        /// <param name="keyValues"></param>
        public async Task<TEntity> FindAsync(params object[] keyValues)
        {
            return await Set.FindAsync(keyValues);
        }

        /// <summary>
        /// Where句 (ラムダ式) を条件としてレコードを取得します
        /// </summary>
        /// <param name="predicate"></param>
        public IList<TEntity> Get(Expression<Func<TEntity, bool>> predicate)
        {
            return Set.Where(predicate).ToList();
        }

        /// <summary>
        /// Where句 (ラムダ式) を条件としてレコードを取得します
        /// </summary>
        /// <param name="predicate"></param>
        public async Task<IList<TEntity>> GetAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await Set.Where(predicate).ToListAsync();
        }

        /// <summary>
        /// レコードを全件取得します
        /// </summary>
        public IList<TEntity> Get()
        {
            return Set.ToList();
        }

        /// <summary>
        /// レコードを全件取得します
        /// </summary>
        public async Task<IList<TEntity>> GetAsync()
        {
            return await Set.ToListAsync();
        }

        /// <summary>
        /// レコード件数を取得します
        /// </summary>
        public int GetCount()
        {
            return Set.Count();
        }

        /// <summary>
        /// レコード件数を取得します
        /// </summary>
        public async Task<int> GetCountAsync()
        {
            return await Set.CountAsync();
        }

        /// <summary>
        /// レコードを削除します
        /// </summary>
        /// <param name="entity"></param>
        public void Remove(TEntity entity)
        {
            Set.Remove(entity);
        }

        /// <summary>
        /// レコードの変更を保存します
        /// </summary>
        public void Save()
        {
            Db.SaveChanges();
        }

        /// <summary>
        /// レコードの変更を保存します
        /// </summary>
        public async Task<int> SaveAsync()
        {
            return await Db.SaveChangesAsync();
        }
    }
}
