using DotHouse.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHouse.Repository
{
    /// <summary>
    /// Dao基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EntityBaseDao<T> : IEntityBaseDao<T>, IDisposable where T : class
    {
        public void Delete(T obj)
        {
            DapperHelper.DeleteEntity<T>(obj);
        }

        public dynamic Save(T obj)
        {
            try
            {
                return DapperHelper.SaveByExtension(obj);
            }
            catch
            {
                return null;
            }
        }

        public dynamic Update(T obj)
        {
            try
            {
                return DapperHelper.UpdateByExtension<T>(obj);
            }
            catch
            {
                return null;
            }
        }

        public T FindById(long id)
        {
            return DapperHelper.QueryByIdExtension<T>(id);
        }

        public IEnumerable<T> FindAll()
        {
            return DapperHelper.QueryAllByExtension<T>();
        }

        public void DeleteById(long id)
        {
            DapperHelper.DeleteEntity<T>(id);
        }

        public IEnumerable<T> FindBySQL(string sql, object objectParam)
        {
            return DapperHelper.Query<T>(sql, objectParam);
        }
        public void Dispose()
        {
            //noting
        }
    }
}
