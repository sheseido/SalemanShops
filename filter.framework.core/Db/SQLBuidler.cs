using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace filter.framework.core.Db
{
    /// <summary>
    /// SQL语句生成基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class SQLBuidler<T>
            where T : class
    {
        /// <summary>
        /// 批量删除标识
        /// </summary>
        public const string DeleteBatch = "@@PRIMARYKEY@@";

        /// <summary>
        /// 表名
        /// </summary>
        protected string mTableName;
        /// <summary>
        /// 主键列名
        /// </summary>
        protected string mPrimaryKeyName;
        /// <summary>
        /// Code列名
        /// </summary>
        protected string mCodeName;

        /// <summary>
        /// 逻辑删除列名
        /// </summary>
        protected string mLogicalDeleteName;

        /// <summary>
        /// 逻辑删除特性
        /// </summary>
        protected LogicalDeleteAttribute mLogicalDeleteAttribute;

        /// <summary>
        /// 逻辑删除的所有列
        /// </summary>
        private List<string> mLogicalDeleteColumns;

        /// <summary>
        /// 数据类型
        /// </summary>
        protected Type mType;

        /// <summary>
        /// 类型缓存
        /// </summary>
        private static Dictionary<string, Tuple<Type, Type>> Caches = null;

        /// <summary>
        /// 静态构造函数
        /// </summary>
        static SQLBuidler()
        {
            Caches = new Dictionary<string, Tuple<Type, Type>>();
        }

        /// <summary>
        /// 新增列名
        /// </summary>
        protected List<string> InsertFields { get; set; }
        /// <summary>
        /// 更新列名
        /// </summary>
        protected List<string> UpdateFields { get; set; }

        protected List<string> AllFields { get; set; }

        /// <summary>
        /// 表名
        /// </summary>
        public virtual string TableName
        {
            get
            {
                return mTableName;
            }
        }

        /// <summary>
        /// 主键名
        /// </summary>
        public virtual string PrimaryKeyName
        {
            get
            {
                return mPrimaryKeyName;
            }
        }

        /// <summary>
        /// 逻辑删除列名
        /// </summary>
        public virtual string LogicalDeleteName
        {
            get
            {
                return mLogicalDeleteName;
            }
        }

        /// <summary>
        /// 逻辑删除标识
        /// </summary>
        public virtual LogicalDeleteAttribute LogicalDeleteAttribute
        {
            get
            {
                return mLogicalDeleteAttribute;
            }
        }

        /// <summary>
        /// 逻辑删除所有列
        /// </summary>
        public virtual List<string> LogicalDeleteColumns
        {
            get
            {
                if (mLogicalDeleteColumns == null || mLogicalDeleteColumns.Count == 0)
                {
                    mLogicalDeleteColumns = new List<string>() { mLogicalDeleteName }.Concat(mLogicalDeleteAttribute.GetColumns()).ToList();
                }
                return mLogicalDeleteColumns;
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public SQLBuidler()
        {
            mType = typeof(T);
            var table = mType.GetCustomAttributes(typeof(TableNameAttribute), false);
            if (table != null && table.Length > 0)
            {
                mTableName = (table[0] as TableNameAttribute).TableName;
            }
            else
            {
                throw new MissingMemberException("could not find TableNameAttribute");
            }

            var properties = mType.GetProperties();

            InsertFields = new List<string>();
            UpdateFields = new List<string>();
            AllFields = new List<string>();

            foreach (var property in properties)
            {
                AllFields.Add(property.Name);
                var attributes = property.GetCustomAttributes(true);
                var primaryAttr = attributes.FirstOrDefault(a => a.GetType().Equals(typeof(PrimaryKeyAttribute)));
                if (primaryAttr != null)
                {
                    mPrimaryKeyName = property.Name;
                    continue;
                }
                if (attributes.All(a => !a.GetType().Equals(typeof(InsertIgnoreAttribute))))
                {
                    InsertFields.Add(property.Name);
                }
                if (attributes.All(a => !a.GetType().Equals(typeof(UpdateIgnoreAttribute))))
                {
                    UpdateFields.Add(property.Name);
                }
                if (attributes.Any(a => a.GetType().Equals(typeof(CodeAttribute))))
                {
                    mCodeName = property.Name;
                }
                object logicalDeleteAttribute = attributes.FirstOrDefault(a => a.GetType().Equals(typeof(LogicalDeleteAttribute)));
                if (logicalDeleteAttribute != null)
                {
                    mLogicalDeleteAttribute = (LogicalDeleteAttribute)logicalDeleteAttribute;
                    mLogicalDeleteName = property.Name;
                }
            }
            if (string.IsNullOrWhiteSpace(mPrimaryKeyName))
            {
                throw new MissingMemberException("cound not find PrimaryKeyAttribute");
            }
        }

        #region Common

        /// <summary>
        /// 生成新增语句
        /// </summary>
        /// <returns></returns>
        public abstract string BuildInsert();

        /// <summary>
        /// 生成更新语句
        /// </summary>
        /// <returns></returns>
        public abstract string BuildUpdate();

        /// <summary>
        /// 生成更新语句
        /// </summary>
        /// <param name="exceptColumn">不需要更新的列名</param>
        /// <returns></returns>
        public abstract string BuildUpdate(List<string> exceptColumn);

        /// <summary>
        /// 生成更新语句
        /// </summary>
        /// <param name="includeColumn">需要更新的列名</param>
        /// <returns></returns>
        public abstract string BuildUpdateOnly(List<string> includeColumn);

        /// <summary>
        /// 生成删除语句
        /// </summary>
        /// <returns></returns>
        public abstract string BuildDelete();

        /// <summary>
        /// 生成批量删除语句
        /// </summary>
        /// <returns></returns>
        public abstract string BuildDeleteByList();

        /// <summary>
        /// 生成查询所有数据语句
        /// </summary>
        /// <returns></returns>
        public abstract string BuildFindAll();

        /// <summary>
        /// 生成根据Id查询语句
        /// </summary>
        /// <returns></returns>
        public abstract string BuildFindById();

        /// <summary>
        /// 生成根据Code查询语句
        /// </summary>
        /// <returns></returns>
        public abstract string BuildFindByCode();

        /// <summary>
        /// 生成根据条件查询语句
        /// </summary>
        /// <param name="conditions"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        public abstract string FindAll(Dictionary<string, string> conditions = null, Dictionary<string, int> sort = null);

        /// <summary>
        /// 生成根据条件查询数量语句
        /// </summary>
        /// <param name="conditions"></param>
        /// <returns></returns>
        public abstract string FindAllCount(Dictionary<string, string> conditions = null);

        #endregion

        #region Logical Delete

        /// <summary>
        /// 生成逻辑删除语句
        /// </summary>
        /// <returns></returns>
        public abstract string LogicalBuildDelete();

        /// <summary>
        /// 生成批量逻辑删除语句
        /// </summary>
        /// <returns></returns>
        public abstract string LogicalBuildDeleteByList();

        /// <summary>
        /// 生成根据逻辑删除标识查询全部数据语句
        /// </summary>
        /// <returns></returns>
        public abstract string LogicalBuildFindAll();

        /// <summary>
        /// 生成根据逻辑删除标识及Id查询数据语句
        /// </summary>
        /// <returns></returns>
        public abstract string LogicalBuildFindById();

        /// <summary>
        /// 生成根据逻辑删除标识及Code查询数据语句
        /// </summary>
        /// <returns></returns>
        public abstract string LogicalBuildFindByCode();

        /// <summary>
        /// 生成根据逻辑删除标识及条件查询数据语句
        /// </summary>
        /// <param name="conditions"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        public abstract string LogicalFindAll(Dictionary<string, string> conditions = null, Dictionary<string, int> sort = null, bool paged = true);

        /// <summary>
        /// 生成根据逻辑删除标识及条件查询数量语句
        /// </summary>
        /// <param name="conditions"></param>
        /// <returns></returns>
        public abstract string LogicalFindAllCount(Dictionary<string, string> conditions = null);

        #endregion

        /// <summary>
        /// 动态创建类型
        /// </summary>
        /// <returns></returns>
        public virtual Tuple<Type, Type> CreateType()
        {
            Tuple<Type, Type> result = null;

            lock (Caches)
            {
                if (Caches.ContainsKey(mType.Name))
                {
                    result = Caches[mType.Name];
                }
                else
                {
                    var current = Assembly.GetExecutingAssembly();
                    var name = current.ManifestModule.Name.Replace(".dll", "");

                    AssemblyName assemblyName = new AssemblyName(name + ".Dynamic");
                    AssemblyBuilder dynamicAssembly = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
                    ModuleBuilder moduleBuilder = dynamicAssembly.DefineDynamicModule(assemblyName.Name, true);

                    TypeBuilder typeInsertBuilder = moduleBuilder.DefineType(mType.Name + "Insert", TypeAttributes.Public);
                    TypeBuilder typeUpdateBuilder = moduleBuilder.DefineType(mType.Name + "Update", TypeAttributes.Public);

                    var properties = mType.GetProperties();
                    FieldBuilder fieldBuilder = null;
                    PropertyBuilder propertyBuilder = null;
                    MethodBuilder methodBuilder = null;
                    MethodAttributes methodAttribute = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig;
                    ILGenerator generator = null;
                    foreach (var property in properties)
                    {
                        var attributes = property.GetCustomAttributes(true);
                        if (attributes.All(a => !a.GetType().Equals(typeof(InsertIgnoreAttribute))))
                        {
                            fieldBuilder = typeInsertBuilder.DefineField("m" + property.Name, property.PropertyType, FieldAttributes.Private);
                            propertyBuilder = typeInsertBuilder.DefineProperty(property.Name, PropertyAttributes.HasDefault, property.PropertyType, null);

                            methodBuilder = typeInsertBuilder.DefineMethod("get_" + property.Name, methodAttribute, property.PropertyType, Type.EmptyTypes);
                            generator = methodBuilder.GetILGenerator();
                            generator.Emit(OpCodes.Ldarg_0);
                            generator.Emit(OpCodes.Ldfld, fieldBuilder);
                            generator.Emit(OpCodes.Ret);
                            propertyBuilder.SetGetMethod(methodBuilder);

                            methodBuilder = typeInsertBuilder.DefineMethod("set_" + property.Name, methodAttribute, null, new Type[] { property.PropertyType });
                            generator = methodBuilder.GetILGenerator();
                            generator.Emit(OpCodes.Ldarg_0);
                            generator.Emit(OpCodes.Ldarg_1);
                            generator.Emit(OpCodes.Stfld, fieldBuilder);
                            generator.Emit(OpCodes.Ret);
                            propertyBuilder.SetSetMethod(methodBuilder);
                        }
                        if (attributes.All(a => !a.GetType().Equals(typeof(UpdateIgnoreAttribute))))
                        {
                            fieldBuilder = typeUpdateBuilder.DefineField("m" + property.Name, property.PropertyType, FieldAttributes.Private);
                            propertyBuilder = typeUpdateBuilder.DefineProperty(property.Name, PropertyAttributes.HasDefault, property.PropertyType, null);

                            methodBuilder = typeUpdateBuilder.DefineMethod("get_" + property.Name, methodAttribute, property.PropertyType, Type.EmptyTypes);
                            generator = methodBuilder.GetILGenerator();
                            generator.Emit(OpCodes.Ldarg_0);
                            generator.Emit(OpCodes.Ldfld, fieldBuilder);
                            generator.Emit(OpCodes.Ret);
                            propertyBuilder.SetGetMethod(methodBuilder);

                            methodBuilder = typeUpdateBuilder.DefineMethod("set_" + property.Name, methodAttribute, null, new Type[] { property.PropertyType });
                            generator = methodBuilder.GetILGenerator();
                            generator.Emit(OpCodes.Ldarg_0);
                            generator.Emit(OpCodes.Ldarg_1);
                            generator.Emit(OpCodes.Stfld, fieldBuilder);
                            generator.Emit(OpCodes.Ret);
                            propertyBuilder.SetSetMethod(methodBuilder);
                        }
                    }

                    Type insertType = typeInsertBuilder.CreateType();
                    Type updateType = typeUpdateBuilder.CreateType();
                    result = new Tuple<Type, Type>(insertType, updateType);
                    Caches.Add(mType.Name, result);
                }
            }

            return result;
        }
    }
}
