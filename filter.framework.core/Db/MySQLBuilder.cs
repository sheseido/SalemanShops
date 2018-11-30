using System;
using System.Collections.Generic;
using System.Linq;

namespace filter.framework.core.Db
{
    /// <summary>
    /// MySQL语句生成
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MySQLBuilder<T> : SQLBuidler<T>
            where T : class
    {
        public MySQLBuilder()
        { }

        #region Common

        /// <summary>
        /// 生成删除语句
        /// </summary>
        /// <returns></returns>
        public override string BuildDelete()
        {
            return $@"DELETE FROM {TableName} WHERE `{mPrimaryKeyName}` = @Id";
        }

        /// <summary>
        /// 生成批量删除语句
        /// </summary>
        /// <returns></returns>
        public override string BuildDeleteByList()
        {
            return $@"DELETE FROM {TableName} WHERE {DeleteBatch}";
        }

        /// <summary>
        /// 生成查询所有数据语句
        /// </summary>
        /// <returns></returns>
        public override string BuildFindAll()
        {
            return $@"SELECT * FROM {TableName}";
        }

        /// <summary>
        /// 生成根据Code查询数据语句
        /// </summary>
        /// <returns></returns>
        public override string BuildFindByCode()
        {
            return $@"SELECT * FROM {TableName} WHERE `{mCodeName}` = @Code";
        }

        /// <summary>
        /// 生成根据主键查询语句
        /// </summary>
        /// <returns></returns>
        public override string BuildFindById()
        {
            return $@"SELECT * FROM {TableName} WHERE `{mPrimaryKeyName}` = @Id";
        }

        /// <summary>
        /// 生成新增数据语句
        /// </summary>
        /// <returns></returns>
        public override string BuildInsert()
        {
            var fileds = InsertFields.Select(f => "`" + f + "`").ToList();
            var values = InsertFields.Select(f => "@" + f).ToList();

            return $"INSERT INTO {TableName} ({ string.Join(" , ", fileds)}) VALUES ({ string.Join(" , ", values)}); SELECT LAST_INSERT_ID();";
        }

        /// <summary>
        /// 生成更新语句
        /// </summary>
        /// <returns></returns>
        public override string BuildUpdate()
        {
            var fields = UpdateFields.Select(f => $"`{f}` = @{f}").ToList();
            return $@"UPDATE {TableName} SET { string.Join(" , ", fields) } WHERE `{mPrimaryKeyName}` = @{mPrimaryKeyName}";
        }

        /// <summary>
        /// 生成更新语句
        /// </summary>
        /// <param name="exceptColumn">不需要更新的列</param>
        /// <returns></returns>
        public override string BuildUpdate(List<string> exceptColumn)
        {
            if (exceptColumn?.Count == 0)
            {
                throw new MissingFieldException("except column should not be empty");
            }

            var columns = UpdateFields.Where(f => !exceptColumn.Contains(f)).Select(f => $"`{f}` = @{f}").ToList();
            return $@"UPDATE {TableName} SET { string.Join(" , ", columns) } WHERE `{mPrimaryKeyName}` = @{mPrimaryKeyName}";
        }

        /// <summary>
        /// 生成更新语句
        /// </summary>
        /// <param name="includeColumn">需要更新的列</param>
        /// <returns></returns>
        public override string BuildUpdateOnly(List<string> includeColumn)
        {
            if (includeColumn?.Count == 0)
            {
                throw new MissingFieldException("include column should not be empty");
            }

            var columns = UpdateFields.Where(f => includeColumn.Contains(f)).Select(f => $"`{f}` = @{f}").ToList();
            return $@"UPDATE {TableName} SET { string.Join(" , ", columns) } WHERE `{mPrimaryKeyName}` = @{mPrimaryKeyName}";
        }

        /// <summary>
        /// 生成根据条件查询数据语句
        /// </summary>
        /// <param name="conditions"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        public override string FindAll(Dictionary<string, string> conditions = null, Dictionary<string, int> sort = null)
        {
            string sortString;
            if (sort != null && sort.Count > 0)
            {
                sortString = " ORDER BY " + string.Join(",", sort.Select(c =>
                {
                    if (c.Value > 0)
                        return c.Key;
                    return c.Key + " DESC ";
                }));
            }
            else
            {
                if (AllFields.Contains("CreatedAt"))
                {
                    sortString = " ORDER BY CreatedAt DESC";
                }
                else
                {
                    sortString = $" ORDER BY {PrimaryKeyName} DESC";
                }
            }
            string sql = $@"SELECT * FROM {TableName}";
            var list = conditions.BuildConditionList(typeof(T));

            if (list.Count > 0)
            {
                sql += " WHERE " + string.Join(" AND ", list);
            }

            sql += sortString;

            sql += " LIMIT @from, @length;";
            return sql;
        }

        /// <summary>
        /// 生成根据条件查询数量语句
        /// </summary>
        /// <param name="conditions"></param>
        /// <returns></returns>
        public override string FindAllCount(Dictionary<string, string> conditions = null)
        {
            string sql = $@"SELECT count(*) FROM {TableName}";
            var list = conditions.BuildConditionList(typeof(T));

            if (list.Count > 0)
            {
                sql += " WHERE " + string.Join(" AND ", list);
            }
            sql += ";";

            return sql;
        }

        #endregion

        #region Logical Delete

        /// <summary>
        /// 生成逻辑删除语句
        /// </summary>
        /// <returns></returns>
        public override string LogicalBuildDelete()
        {
            var fields = LogicalDeleteColumns.Select(f => $"`{f}` = @{f}").ToList();
            return $@"UPDATE {TableName} SET { string.Join(" , ", fields) } WHERE `{mPrimaryKeyName}` = @Id";
        }

        /// <summary>
        /// 生成批量逻辑删除语句
        /// </summary>
        /// <returns></returns>
        public override string LogicalBuildDeleteByList()
        {
            var fields = LogicalDeleteColumns.Select(f => $"`{f}` = @{f}").ToList();
            return $@"UPDATE {TableName} SET { string.Join(" , ", fields) } WHERE {DeleteBatch}";
        }

        /// <summary>
        /// 生成根据逻辑删除标识查询全部数据语句
        /// </summary>
        /// <returns></returns>
        public override string LogicalBuildFindAll()
        {
            return $@"SELECT * FROM {TableName} WHERE {LogicalDeleteName} = 0";
        }

        /// <summary>
        /// 生成根据逻辑删除标识及Code查询数据语句
        /// </summary>
        /// <returns></returns>
        public override string LogicalBuildFindByCode()
        {
            return $@"SELECT * FROM {TableName} WHERE `{mCodeName}` = @Code AND {LogicalDeleteName} = 0";
        }

        /// <summary>
        /// 生成根据逻辑删除标识及Id查询数据语句
        /// </summary>
        /// <returns></returns>
        public override string LogicalBuildFindById()
        {
            return $@"SELECT * FROM {TableName} WHERE `{mPrimaryKeyName}` = @Id AND {LogicalDeleteName} = 0";
        }

        /// <summary>
        /// 生成根据逻辑删除标识及条件查询数据语句
        /// </summary>
        /// <param name="conditions"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        public override string LogicalFindAll(Dictionary<string, string> conditions = null, Dictionary<string, int> sort = null, bool paged = true)
        {
            string sortString;
            if (sort != null && sort.Count > 0)
            {
                sortString = " ORDER BY " + string.Join(",", sort.Select(c =>
                {
                    if (c.Value > 0)
                        return c.Key;
                    return c.Key + " DESC ";
                }));
            }
            else
            {
                if (AllFields.Contains("CreatedAt"))
                {
                    sortString = " ORDER BY CreatedAt DESC";
                }
                else
                {
                    sortString = $" ORDER BY {PrimaryKeyName} DESC";
                }
            }
            string sql = $@"SELECT * FROM {TableName}";
            if(conditions == null)
            {
                conditions = new Dictionary<string, string>();
            }
            if (!conditions.ContainsKey(LogicalDeleteName))
            {
                conditions.Add(LogicalDeleteName, "0");
            }
            var list = conditions.BuildConditionList(typeof(T));

            if (list.Count > 0)
            {
                sql += " WHERE " + string.Join(" AND ", list);
            }

            sql += sortString;

            if (paged)
                sql += " LIMIT @from, @length;";
            return sql;
        }

        /// <summary>
        /// 生成根据逻辑删除标识及条件查询数量语句
        /// </summary>
        /// <param name="conditions"></param>
        /// <returns></returns>
        public override string LogicalFindAllCount(Dictionary<string, string> conditions = null)
        {
            string sql = $@"SELECT count(*) FROM {TableName}";
            if (conditions == null)
            {
                conditions = new Dictionary<string, string>();
            }
            if (!conditions.ContainsKey(LogicalDeleteName))
            {
                conditions.Add(LogicalDeleteName, "0");
            }
            var list = conditions.BuildConditionList(typeof(T));

            if (list.Count > 0)
            {
                sql += " WHERE " + string.Join(" AND ", list);
            }
            sql += ";";

            return sql;
        }

        #endregion

    }
}
