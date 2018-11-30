using Newtonsoft.Json;

namespace filter.framework.utility
{
    /// <summary>
    /// Json拓展
    /// </summary>
    public static class JsonExt
    {
        /// <summary>
        /// 将对象转换为Json字符串
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns></returns>
        public static string ToJson(this object obj)
        {
            var result = string.Empty;

            if (obj != null)
            {
                result = JsonConvert.SerializeObject(obj);
            }

            return result;
        }

        /// <summary>
        /// 将Json字符串转换为对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonString">Json字符串</param>
        /// <returns></returns>
        public static T ToObject<T>(this string jsonString)
        {
            T result = default(T);

            if (!string.IsNullOrWhiteSpace(jsonString))
            {
                result = JsonConvert.DeserializeObject<T>(jsonString);
            }

            return result;
        }
    }
}
