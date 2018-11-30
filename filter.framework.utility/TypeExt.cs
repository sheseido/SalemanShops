using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace filter.framework.utility
{
    public static class TypeExt
    {
        /// <summary>
        /// 获取对象的命令空间
        /// </summary>
        /// <param name="obj">对象实例</param>
        /// <param name="prefix">前缀</param>
        /// <param name="suffix">后缀</param>
        /// <returns></returns>
        public static string GetNamespace(this object obj, string prefix = null, string suffix = null)
        {
            StringBuilder sBuilder = new StringBuilder();

            if (prefix != null)
            {
                sBuilder.Append(prefix);
            }

            if (obj != null)
            {
                sBuilder.Append(obj.GetType().Namespace);
            }

            if (suffix != null)
            {
                sBuilder.Append(suffix);
            }

            return sBuilder.ToString();
        }

        /// <summary>
        /// 获取全类名
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetClassFullName(this object obj)
        {
            return obj.GetType().FullName;
        }

        /// <summary>
        /// 获取实例的程序集名称
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetAssemblyName<T>(T obj)
        {
            string result = null;

            if (obj != null)
            {
                result = GetAssemblyName(obj.GetType());
            }

            return result;
        }

        /// <summary>
        /// 获取类型的程序集名称
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetAssemblyName(Type type)
        {
            string result = null;

            if (type != null)
            {
                result = type.Assembly.GetName().Name;
            }

            return result;
        }

        /// <summary>
        /// 获取类型属性列表
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static List<PropertyInfo> GetPropertyList(this object obj)
        {
            if (obj == null)
            {
                return null;
            }
            return obj.GetType().GetProperties().ToList();
        }

        /// <summary>
        /// 根据对象获取键值对
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Dictionary<string, object> GetObjectDictionary(this object obj, string prefix = "")
        {
            if (obj == null)
            {
                return null;
            }

            var result = new Dictionary<string, object>();

            if (obj is Dictionary<string, object>)
            {
                var source = (Dictionary<string, object>)obj;
                foreach (var kvp in source)
                {
                    result.Add(prefix + kvp.Key, kvp.Value);
                }
            }
            else
            {
                var pros = obj.GetPropertyList().Where(p => p.CanRead).ToList();
                pros.ForEach(p =>
                {
                    result.Add(prefix + p.Name, p.GetValue(obj));
                });
            }

            return result;
        }

        /// <summary>
        /// 类型转换，相同属性名称可复制（区分大小写）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T Convert<T>(this object obj)
            where T : new()
        {
            var sources = obj.GetPropertyList();
            T result = new T();
            var target = result.GetPropertyList();

            foreach (var t in target)
            {
                var attrName = "";
                object[] typeChangeAttributs = t.GetCustomAttributes(typeof(TypeChangeAttribute), true);
                if (typeChangeAttributs.Length > 0)
                {
                    TypeChangeAttribute attr = typeChangeAttributs[0] as TypeChangeAttribute;
                    if (attr.Ignore)
                        continue;
                    if (!string.IsNullOrEmpty(attr.ColumnName))
                        attrName = attr.ColumnName;
                }
                var sp = sources.FirstOrDefault(s => (s.Name == t.Name || s.Name == attrName) && s.PropertyType.Equals(t.PropertyType));
                if (sp != null)
                {
                    t.SetValue(result, sp.GetValue(obj));
                }
            }

            return result;
        }
    }
    /// <summary>
    /// 类型转换属性
    /// </summary>
    public class TypeChangeAttribute : Attribute
    {
        public bool Ignore { get; set; }
        public string ColumnName { get; set; }
    }
}
