using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace filter.framework.utility
{
    /// <summary>
    /// 枚举类型转换
    /// </summary>
    public static class EnumConverter
    {
        /// <summary>
        /// 获取注释
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="myEnum"></param>
        /// <returns></returns>
        public static string ToDescription<T>(T myEnum) where T : struct
        {
            try
            {
                Type type = typeof(T);
                FieldInfo info = type.GetField(myEnum.ToString());
                DescriptionAttribute descriptionAttribute = info.GetCustomAttributes(typeof(DescriptionAttribute), true)[0] as DescriptionAttribute;
                if (descriptionAttribute != null)
                    return descriptionAttribute.Description;
                else
                    return type.ToString();
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// 转换为枚举
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T ToEnum<T>(int value) where T : struct
        {
            return (T)Enum.ToObject(typeof(T), value);
        }

        /// <summary>
        /// 获取枚举说明
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToEnumDescription<T>(int value) where T : struct
        {
            return ToDescription(ToEnum<T>(value));
        }

        /// <summary>
        /// 获取枚举列表
        /// </summary>
        /// <param name="enumType"></param>
        /// <returns></returns>
        public static List<EnumKeyValueName> ToKeyValueList(Type enumType)
        {
            var list = new List<EnumKeyValueName>();

            var values = Enum.GetValues(enumType);
            var names = Enum.GetNames(enumType);

            for (var i = 0; i < names.Length; i++)
            {
                var name = names[i];

                FieldInfo info = enumType.GetField(name);
                DescriptionAttribute descriptionAttribute = info.GetCustomAttributes(typeof(DescriptionAttribute), true)[0] as DescriptionAttribute;

                list.Add(new EnumKeyValueName()
                {
                    Key = Convert.ToInt32(values.GetValue(i)),
                    Value = name,
                    Name = descriptionAttribute != null ? descriptionAttribute.Description : string.Empty
                });
            }

            return list;
        }

        /// <summary>
        /// 获取枚举字典
        /// </summary>
        /// <param name="enumType"></param>
        /// <returns></returns>
        public static Dictionary<int, string> ToDictionary(Type enumType)
        {
            return ToKeyValueList(enumType).ToDictionary(e => e.Key, e => e.Name);
        }

        /// <summary>
        /// 获取枚举Name
        /// </summary>
        /// <param name="enumType"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToEnumName<T>(object value)
        {
            return Enum.GetName(typeof(T), value);
        }
    }

    public class EnumKeyValueName
    {
        public int Key { get; set; }

        public string Value { get; set; }

        public string Name { get; set; }
    }
}
