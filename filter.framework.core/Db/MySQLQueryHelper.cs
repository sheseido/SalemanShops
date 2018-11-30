using System;
using System.Collections.Generic;
using System.Linq;

namespace filter.framework.core.Db
{
    public static class MySQLQueryHelper
    {
        public static List<string> BuildConditionList(this Dictionary<string, string> conditions, Type type,
            string prefix = "")
        {
            if (conditions == null)
                return new List<string>();
            var list = conditions
                .Where(c => !string.IsNullOrEmpty(c.Value))
                .Select(c => Build(type, prefix, c.Key, c.Value))
                .Where(c => !string.IsNullOrEmpty(c)).ToList();
            return list;
        }

        private static string Build(Type type, string prefix, string key, string value)
        {
            if (key.Contains("_or_"))
            {
                var list =
                    key.Split(new[] { "_or_" }, StringSplitOptions.RemoveEmptyEntries)
                        .ToList()
                        .Select(t => Build(type, prefix, t, value))
                        .Where(t => !string.IsNullOrEmpty(t))
                        .ToList();

                if (list.Count <= 0)
                {
                    return null;
                }
                return " ( " + string.Join(" OR ", list) + " ) ";
            }

            var strings = key.Split(new[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
            var s = strings[0];
            for (int i = 1; i < strings.Length - 1; i++)
            {
                s += "_" + strings[i];
            }
            var propertyInfo = type.GetProperty(s);
            if (propertyInfo == null)
                return null;

            s = "`" + s + "`";
            if (propertyInfo.PropertyType == typeof(int) ||
                propertyInfo.PropertyType == typeof(float) ||
                propertyInfo.PropertyType == typeof(long) ||
                propertyInfo.PropertyType == typeof(double) ||
                propertyInfo.PropertyType == typeof(decimal) ||
                propertyInfo.PropertyType == typeof(DateTime) ||
                propertyInfo.PropertyType == typeof(TimeSpan))
            {
                if (strings.Length <= 1)
                {
                    return s + "=" + value;
                }
                switch (strings[strings.Length - 1]?.ToLower())
                {
                    case "gt":
                        return prefix + s + ">" + value;
                    case "lt":
                        return prefix + s + "<" + value;
                    case "gte":
                        return prefix + s + ">=" + value;
                    case "lte":
                        return prefix + s + "<=" + value;
                    case "eq":
                        return prefix + s + "=" + value;
                    case "in":
                        return prefix + s + " in (" + value + ") ";
                }
            }
            else
            {
                if (strings.Length <= 1)
                {
                    return prefix + s + "='" + value + "'";
                }
                switch (strings[strings.Length - 1]?.ToLower())
                {
                    case "contains":
                        return prefix + s + " like '%" + value + "%'";
                    case "startswith":
                        return prefix + s + " like '" + value + "%'";
                    case "endswith":
                        return prefix + s + " like '%" + value + "'";
                    case "gt":
                        return prefix + s + ">'" + value + "'";
                    case "lt":
                        return prefix + s + "<'" + value + "'";
                    case "gte":
                        return prefix + s + ">='" + value + "'";
                    case "lte":
                        return prefix + s + "<='" + value + "'";
                    case "eq":
                        return prefix + s + "='" + value + "'";
                    case "in":
                        return prefix + s + " in (" + value + ") ";
                }
            }
            return s + "=" + value;
        }
    }
}