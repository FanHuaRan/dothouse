using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHouse.Repository
{
    /// <summary>
    /// Dao基本接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IEntityBaseDao<T>
      where T : class
    {
        void Delete(T obj);
        void DeleteById(long id);
        void Dispose();
        System.Collections.Generic.IEnumerable<T> FindAll();
        T FindById(long id);
        IEnumerable<T> FindBySQL(string sql, object objectParam);
        dynamic Save(T obj);
        dynamic Update(T obj);
    }
}
