using Dapper;
using Dapper.Contrib.Extensions;
using filter.data.model;
using filter.framework.core.Db;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace filter.data.manager
{
    public class DapperDataAccess
    {
        public static IDbConnection CreateDbBase()
        {
            var connectionSetting = ConfigurationManager.ConnectionStrings["mo.filter.com"];
            //var provider = connectionSetting.ProviderName;
            var connectionString = connectionSetting.ConnectionString;
            //return Activator.CreateInstance(
            //    string.IsNullOrWhiteSpace(provider) 
            //        ? typeof(SqlConnection) 
            //        : Type.GetType(provider), new object[] { connectionString }
            //    ) as IDbConnection;
            return new MySqlConnection(connectionString);
        }

        /// <summary>
        /// 开始事务
        /// </summary>
        /// <param name="dbName"></param>
        /// <returns></returns>
        public static IDbTransaction BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.RepeatableRead)
        {
            var connection = CreateDbBase();
            Open(connection);
            return connection.BeginTransaction(isolationLevel);
        }

        /// <summary>
        /// 打开数据库连接
        /// </summary>
        /// <param name="connection"></param>
        public static void Open(IDbConnection connection)
        {
            if (connection != null)
            {
                if (connection.State == ConnectionState.Broken)
                {
                    connection.Close();
                }
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }
            }
        }

        /// <summary>
        /// 关闭数据库连接
        /// </summary>
        /// <param name="connection"></param>
        public static void Close(IDbConnection connection)
        {
            if (connection != null)
            {
                if (connection.State == ConnectionState.Broken || connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }

        /// <summary>
        /// 提交事务
        /// </summary>
        /// <param name="trans"></param>
        public static void Commit(IDbTransaction trans)
        {
            if (trans != null)
            {
                trans.Commit();
                Close(trans.Connection);
            }
        }

        /// <summary>
        /// 回滚事务
        /// </summary>
        /// <param name="trans"></param>
        public static void Rollback(IDbTransaction trans)
        {
            if (trans != null)
            {
                trans.Rollback();
                Close(trans.Connection);
            }
        }

        /// <summary>
        /// 根据Id查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
        public T FindById<T>(object id, IDbTransaction trans = null) where T : class
        {
            IDbConnection connection = (trans == null ? CreateDbBase() : trans.Connection);
            try
            {
                return connection.Get<T>(id, trans);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (trans == null)
                    Close(connection);
            }
        }

        /// <summary>
        /// 根据Id异步查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        public async Task<T> FindByIdAsync<T>(int id, IDbTransaction trans = null, int? commandTimeout = null) where T : class, new()
        {
            IDbConnection connection = (trans == null ? CreateDbBase() : trans.Connection);
            try
            {
                return await connection.GetAsync<T>(id, trans, commandTimeout);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (trans == null)
                    Close(connection);
            }
        }

        /// <summary>
        /// 通过sql语句异步查询单条记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
        public async Task<T> FindBySqlAsync<T>(string sql, object param = null, IDbTransaction trans = null)
        {
            IDbConnection connection = (trans == null ? CreateDbBase() : trans.Connection);
            try
            {
                var data= await connection.QuerySingleOrDefaultAsync<T>(sql, param, trans);
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (trans == null)
                    Close(connection);
            }
        }

        /// <summary>
        /// 通过sql语句异步查询所有
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
        public async Task<IEnumerable<T>> FindAllAsync<T>(string sql, object param = null, IDbTransaction trans = null) where T : class
        {
            IDbConnection connection = (trans == null ? CreateDbBase() : trans.Connection);
            try
            {
                return await connection.QueryAsync<T>(sql, param, trans);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (trans == null)
                    Close(connection);
            }
        }

        /// <summary>
        /// 异步查询所有
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public async Task<IEnumerable<T>> FindAllAsync<T>(IDbTransaction trans = null, int? commandTimeout = null) where T : class, new()
        {
            IDbConnection connection = (trans == null ? CreateDbBase() : trans.Connection);
            try
            {
                return await connection.GetAllAsync<T>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (trans == null)
                    Close(connection);
            }
        }

        /// <summary>
        /// 分页查询 自己拼sql
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">查询语句</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<PagedData<T>> PagedFindAllAsync<T>(string queryStr, string columStr, int pageIndex, int pageSize, object param = null, string orderbyStr = "")
        {
            using (var db = CreateDbBase())
            {
                pageIndex = (pageIndex <= 0 ? 0 : pageIndex - 1) * pageSize;
                StringBuilder querySql = new StringBuilder();
                querySql.Append($"SELECT {columStr} FROM {queryStr} ");
                if (!string.IsNullOrEmpty(orderbyStr))
                    querySql.Append($" ORDER BY {orderbyStr} ");
                querySql.Append($" LIMIT {pageIndex},{pageSize};");

                var countSql = $"SELECT COUNT(1) as count FROM {queryStr}";

                var totalCount = await db.QueryFirstAsync<long>(countSql, param);
                var list = await db.QueryAsync<T>(querySql.ToString(), param);
                PagedData<T> result = new PagedData<T>(totalCount, pageSize, list);
                return result;
            }
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="sqlAdapter"></param>
        /// <returns></returns>
        public async Task<int> InsertAsync<T>(T model, IDbTransaction trans = null, int? commandTimeout = null) where T : class, new()
        {
            IDbConnection connection = (trans == null ? CreateDbBase() : trans.Connection);
            try
            {
                return await connection.InsertAsync<T>(model, trans, commandTimeout);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (trans == null)
                    Close(connection);
            }
        }

        /// <summary>
        /// 批量插入
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="trans"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        public async Task<bool> InsertBatchAsync<T>(List<T> data, IDbTransaction trans = null, int? commandTimeout = null)
        {
            var sql = BuildInsertScript<T>();
            IDbConnection connection = (trans == null ? CreateDbBase() : trans.Connection);
            try
            {
                var result = await connection.ExecuteAsync(sql, data, trans);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (trans == null)
                    Close(connection);
            }
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="trans"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        public bool Update<T>(T entity, IDbTransaction trans = null, int? commandTimeout = null) where T : class
        {
            IDbConnection connection = (trans == null ? CreateDbBase() : trans.Connection);
            try
            {
                var result = connection.Update<T>(entity, trans, commandTimeout);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (trans == null)
                    Close(connection);
            }
        }

        /// <summary>
        /// 异步更新
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="entityToUpdate"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        public async Task<bool> UpdateAsync<T>(T model, IDbTransaction trans = null, int? commandTimeout = null) where T : class, new()
        {
            IDbConnection connection = (trans == null ? CreateDbBase() : trans.Connection);
            try
            {
                return await connection.UpdateAsync<T>(model, trans, commandTimeout);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (trans == null)
                    Close(connection);
            }
        }

        /// <summary>
        /// 异步删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
        public async Task<bool> DeleteAsync<T>(T t, IDbTransaction trans = null) where T : class
        {
            IDbConnection connection = (trans == null ? CreateDbBase() : trans.Connection);
            try
            {
                return await connection.DeleteAsync(t, trans);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (trans == null)
                    Close(connection);
            }
        }

        /// <summary>
        /// 逻辑异步删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ids"></param>
        /// <returns></returns>
        public async Task<bool> LogicDeleteAsync<T>(List<int> ids, IDbTransaction trans = null)
        {
            IDbConnection connection = (trans == null ? CreateDbBase() : trans.Connection);
            try
            {
                var t = typeof(T);
                var attrs = t.GetCustomAttributes(typeof(TableAttribute), false);
                if (attrs != null && attrs.Count() > 0)
                {
                    var tableName = (attrs[0] as TableAttribute).Name;
                    string sql = $" UPDATE `{tableName}` SET IsDelete=1 WHERE Id IN ({string.Join(",", ids)}); ";
                    await connection.ExecuteAsync(sql);
                    return true;
                }
                else return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (trans == null)
                    Close(connection);
            }
        }

        /// <summary>
        /// 异步执行sql
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <returns></returns>
        public async Task<int> ExecAsync<T>(string sql, IDbTransaction trans = null)
        {
            IDbConnection connection = (trans == null ? CreateDbBase() : trans.Connection);
            try
            {
                return await connection.ExecuteAsync(sql, trans);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (trans == null)
                    Close(connection);
            }
        }

        /// <summary>
        /// 生成插入语句
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private string BuildInsertScript<T>()
        {
            var mType = typeof(T);
            var table = mType.GetCustomAttributes(typeof(TableAttribute), false);
            var mTableName = "";
            if (table != null && table.Length > 0)
            {
                mTableName = (table[0] as TableAttribute).Name;
            }
            else
            {
                throw new MissingMemberException("could not find TableNameAttribute");
            }
            var InsertFields = new List<string>();
            var properties = mType.GetProperties();
            foreach (var property in properties)
            {
                var attributes = property.GetCustomAttributes(true);
                if (attributes.All(a => !a.GetType().Equals(typeof(InsertIgnoreAttribute))))
                {
                    InsertFields.Add(property.Name);
                }
            }
            var fileds = InsertFields.Select(f => "`" + f + "`").ToList();
            var values = InsertFields.Select(f => "@" + f).ToList();
            var script = $"INSERT INTO {mTableName} ({ string.Join(" , ", fileds)}) VALUES ({ string.Join(" , ", values)}); SELECT LAST_INSERT_ID();";
            return script;
        }
    }
}
