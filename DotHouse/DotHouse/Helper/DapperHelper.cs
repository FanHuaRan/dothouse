using Dapper;
using DapperExtensions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHouse.Helper
{
    /// <summary>
    /// Dapper工具类
    /// </summary>
    public class DapperHelper
    {
        public static readonly String CONNECTTION_STRING = ConfigurationManager.ConnectionStrings["housedb"].ConnectionString;

        #region 获取连接
        /// <summary>
        /// 获取已开启的数据库链接
        /// </summary>
        /// <returns></returns>
        public static IDbConnection OpenConnection()
        {
            var connection = new SQLiteConnection(CONNECTTION_STRING);
            connection.Open();
            return connection;
        }
        #endregion
        #region 原生查询
        /// <summary>
        /// 泛型查询多个对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="objects"></param>
        /// <returns></returns>
        public static IEnumerable<T> Query<T>(string sql, object objectParam = null)
        {
            using (var connection = OpenConnection())
            {
                return connection.Query<T>(sql, objectParam);
            }
        }
        /// <summary>
        /// 通过匿名类型查询多个对象
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="objectParam"></param>
        /// <returns></returns>
        public static IEnumerable<dynamic> Query(string sql, object objectParam = null)
        {
            using (var connection = OpenConnection())
            {
                return connection.Query(sql, objectParam);
            }
        }
        /// <summary>
        /// 查询单个对象 泛型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="objectParam"></param>
        /// <returns></returns>
        public static T QuerySingle<T>(string sql, object objectParam = null)
        {
            using (var connection = OpenConnection())
            {
                return connection.QueryFirstOrDefault<T>(sql, objectParam);
            }
        }
        /// <summary>
        /// 查询单个对象 匿名类型
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="objectParam"></param>
        /// <returns></returns>
        public static dynamic QuerySingle(string sql, object objectParam = null)
        {
            using (var connection = OpenConnection())
            {
                return connection.QueryFirstOrDefault(sql, objectParam);
            }
        }
        /// <summary>
        /// 组合查询，多返回结果
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sqls"></param>
        /// <param name="objects"></param>
        /// <returns></returns>
        public static IEnumerable<IEnumerable<dynamic>> MultiQuery(string[] sqls, object objectParam = null)
        {
            var sql = BuildSQL(sqls);
            using (var connection = OpenConnection())
            using (var multipleReader = connection.QueryMultiple(sql, objectParam))
            {
                var results = new List<IEnumerable<dynamic>>();
                for (int i = 0; i < sqls.Length; i++)
                {
                    results.Add(multipleReader.Read());
                }
                return results;
            }
        }
        #endregion
        #region 通用
        /// <summary>
        /// 执行sql语句
        /// 返回受影响的行数
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="paramsObject"></param>
        /// <returns></returns>
        public static int ExecuteSQL(string sql, object paramsObject = null)
        {
            using (var connection = OpenConnection())
            {
                return connection.Execute(sql, paramsObject);
            }
        }
        /// <summary>
        /// 多段sql执行事务
        /// </summary>
        /// <param name="sqls"></param>
        /// <param name="paramsObject"></param>
        /// <returns></returns>
        public static bool ExecuteTransaction(List<string> sqls, object paramsObject = null)
        {
            using (var connection = OpenConnection())
            using (var tran = connection.BeginTransaction())
            {
                var flag = true;
                try
                {
                    foreach (var sql in sqls)
                    {
                        connection.Execute(sql, paramsObject);
                    }
                }
                catch (Exception e)
                {
                    flag = false;
                    tran.Rollback();
                }
                return flag;
            }
        }
        #endregion
        #region 原生Dapper的插入或者修改
        /// <summary>
        /// 执行一个对象的插入或者修改操作
        /// 返回主键
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="enity"></param>
        public static void ExecuteSingleUpdateOrInsert<T>(string sql, T enity)
        {
            using (var connection = OpenConnection())
            {
                connection.ExecuteScalar(sql, enity);
            }
        }
        /// <summary>
        /// 执行一个对象的插入或者修改操作
        /// 返回主键
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="enity"></param>
        public static void ExecuteSingleUpdateOrInsert(string sql, object enity)
        {
            using (var connection = OpenConnection())
            {
                connection.ExecuteScalar(sql, enity);
            }
        }
        /// <summary>
        /// 执行多个对象的插入或者修改 使用了事务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="enity"></param>
        public static bool ExecuteUpdateOrInsert<T>(string sql, IEnumerable<T> enitys)
        {
            using (var connection = OpenConnection())
            using (var tran = connection.BeginTransaction())
            {
                bool flag = true;
                try
                {
                    connection.Execute(sql, enitys);
                }
                catch (Exception e)
                {
                    flag = false;
                    tran.Rollback();
                }
                return flag;
            }
        }
        /// <summary>
        /// 执行多个对象的插入或者修改 使用了事务
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="enitys"></param>
        /// <returns></returns>
        public static bool ExecuteUpdateOrInsert(string sql, IEnumerable<object> enitys)
        {
            using (var connection = OpenConnection())
            using (var tran = connection.BeginTransaction())
            {
                bool flag = true;
                try
                {
                    connection.Execute(sql, enitys);
                }
                catch (Exception e)
                {
                    flag = false;
                    tran.Rollback();
                }
                return flag;
            }
        }
        #endregion
        #region 扩展查询
        /// <summary>
        /// 查询所有
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<T> QueryAllByExtension<T>() where T : class
        {
            using (var connection = OpenConnection())
            {
                return connection.GetList<T>();
            }
        }
        /// <summary>
        /// 通过谓词查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static IEnumerable<T> QueryByExtension<T>(object predicate) where T : class
        {
            using (var connection = OpenConnection())
            {
                return connection.GetList<T>();
            }
        }
        /// <summary>
        /// 通过ID查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public static T QueryByIdExtension<T>(long id) where T : class
        {
            using (var connection = OpenConnection())
            {
                return connection.Get<T>(id);
            }
        }
        #endregion
        #region 扩展插入
        /// <summary>
        /// 插入单个对象 返回主键
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static dynamic SaveByExtension<T>(T entity) where T : class
        {
            using (var connection = OpenConnection())
            {
                return connection.Insert(entity);
            }
        }
        /// <summary>
        /// 批量查入对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entitys"></param>
        /// <returns></returns>
        public static bool SaveByExtension<T>(IEnumerable<T> entitys) where T : class
        {
            using (var connection = OpenConnection())
            using (var tran = connection.BeginTransaction())
            {
                try
                {
                    connection.Insert(entitys, tran);
                    return true;
                }
                catch (Exception e)
                {
                    return false;
                }
            }
        }
        #endregion
        #region 扩展修改
        /// <summary>
        /// Update单个对象 返回主键
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static dynamic UpdateByExtension<T>(T entity) where T : class
        {
            using (var connection = OpenConnection())
            {
                return connection.Update(entity);
            }
        }
        /// <summary>
        /// 批量Update对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entitys"></param>
        /// <returns></returns>
        public static bool UpdateByExtension<T>(IEnumerable<T> entitys) where T : class
        {
            using (var connection = OpenConnection())
            using (var tran = connection.BeginTransaction())
            {
                try
                {
                    connection.Update(entitys, tran);
                    return true;
                }
                catch (Exception e)
                {
                    return false;
                }
            }
        }
        #endregion
        #region 扩展删除
        /// <summary>
        /// 根据对象来删除对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        public static void DeleteEntity<T>(T entity) where T : class
        {
            using (var connection = OpenConnection())
            {
                connection.Delete<T>(entity);
            }
        }

        /// <summary>
        /// 根据对象ID删除对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        public static void DeleteEntity<T>(long id) where T : class
        {
            using (var connection = OpenConnection())
            {
                connection.Delete<T>(id);
            }
        }
        /// <summary>
        /// 根据对象集合批量删除对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        public static void DeleteEntity<T>(IEnumerable<T> entitys) where T : class
        {
            using (var connection = OpenConnection())
            {
                foreach (var entity in entitys)
                {
                    connection.Delete<T>(entity);
                }
            }
        }

        /// <summary>
        /// 根据对象ID集合批量删除对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        public static void DeleteEntity<T>(IEnumerable<int> ids) where T : class
        {
            using (var connection = OpenConnection())
            {
                foreach (var id in ids)
                {
                    connection.Delete<T>(id);
                }
            }
        }
        #endregion
        #region 私有辅助方法
        /// <summary>
        /// 多段sql语句拼接为一句sql
        /// </summary>
        /// <param name="sqls"></param>
        /// <returns></returns>
        private static string BuildSQL(string[] sqls)
        {
            if (sqls == null || sqls.Length == 0)
            {
                throw new ArgumentException();
            }
            var sql = "";
            for (var i = 0; i < sqls.Length; i++)
            {
                sql += sqls[i];
                if (i != sqls.Length - 1)
                {
                    sql += ";";
                }
            }
            return sql;
        }
        #endregion
    }
}

}
