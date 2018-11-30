using System;
using System.ComponentModel;

namespace filter.framework.utility
{
    public enum DtType
    {
        /// <summary>
        /// yyyy-MM-dd HH:mm:ss
        /// </summary>
        Type1 = 0,
        /// <summary>
        /// yyyy-MM-dd
        /// </summary>
        Type2 = 1,

        /// <summary>
        /// yyyy-MM-dd HH:mm:ss.fff
        /// </summary>
        Type3 = 2,
        /// <summary>
        /// yyyy年MM月dd日
        /// </summary>
        Type4 = 3,
        /// <summary>
        /// yyyy年MM月dd日HH时mm分ss秒
        /// </summary>
        Type5 = 4
    }
    public static class ConvertHelper
    {
        #region TryPrase数据类型转换
        public static int TryPraseInt(string inValue, int defaultValue = default(int))
        {

            int ret = defaultValue;
            int.TryParse(inValue, out ret);
            return ret;
        }
        public static decimal TryPraseDecimal(string inValue, decimal defaultValue = default(decimal))
        {
            decimal ret = defaultValue;
            decimal.TryParse(inValue, out ret);
            return ret;
        }
        public static DateTime TryPraseDateTime(string inValue, DateTime defaultValue = default(DateTime))
        {
            DateTime ret = defaultValue;
            DateTime.TryParse(inValue, out ret);
            return ret;
        }

        public static string ConvertDtToString(DateTime dateTime, DtType dtType = DtType.Type1)
        {
            switch (dtType)
            {
                case DtType.Type1:
                    return dateTime.ToString("yyyy-MM-dd HH:mm:ss");
                case DtType.Type2:
                    return dateTime.ToString("yyyy-MM-dd");
                case DtType.Type3:
                    return dateTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
                case DtType.Type4:
                    return dateTime.ToString("yyyy年MM月dd日");
                case DtType.Type5:
                    return dateTime.ToString("yyyy年MM月dd日HH时mm分ss秒");
            }
            return dateTime.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public static double ConvertDtToTimeSpan(DateTime datetime)
        {
            if (datetime != null)
            {
                TimeSpan ts = datetime - DateTime.Now;
                return Math.Ceiling(ts.TotalSeconds);
            }
            return default(Double);
        }

        public static long ConvertDtToUnixTimeSpan(DateTime time)
        {
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1, 0, 0, 0, 0));
            return (time.Ticks - startTime.Ticks) / 10000000;//秒级时间差  10000
        }

        /// <summary>
        /// 13位timespan
        /// </summary>
        /// <param name="timespan"></param>
        /// <returns></returns>
        public static DateTime ConvertTimeSpan13ToDt(long timespan)
        {
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1, 0, 0, 0, 0));
            DateTime dt = startTime.AddMilliseconds(timespan);
            return dt;
        }
        /// <summary>
        /// 10位timespan
        /// </summary>
        /// <param name="timespan"></param>
        /// <returns></returns>
        public static DateTime ConvertTimeSpan10ToDt(long timespan)
        {
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1, 0, 0, 0, 0));
            DateTime dt = startTime.AddSeconds(timespan);
            return dt;
        }

        #endregion

        /// <summary>
        /// 返回默认值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="param"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static T ConvertTo<T>(object param, T defaultValue = default(T))
        {
            try
            {
                return (T)Convert.ChangeType(param, typeof(T));
            }
            catch
            {
                return defaultValue;
            }
        }


        /// <summary>
        /// 	Converts an object to the specified target type or returns the default value if
        ///     those 2 types are not convertible.
        ///     <para>
        ///     If the <paramref name="value"/> can't be convert even if the types are 
        ///     convertible with each other, an exception is thrown.</para>
        /// </summary>
        /// <typeparam name = "T"></typeparam>
        /// <param name = "value">The value.</param>
        /// <returns>The target type</returns>
        public static T ConvertTo_old<T>(object value)
        {
            return ConvertTo(value, default(T));
        }

        /// <summary>
        /// 	Converts an object to the specified target type or returns the default value.
        ///     <para>Any exceptions are ignored. </para>
        /// </summary>
        /// <typeparam name = "T"></typeparam>
        /// <param name = "value">The value.</param>
        /// <returns>The target type</returns>
        public static T ConvertToAndIgnoreException<T>(object value)
        {
            return ConvertToAndIgnoreException(value, default(T));
        }

        /// <summary>
        /// 	Converts an object to the specified target type or returns the default value.
        ///     <para>Any exceptions are ignored. </para>
        /// </summary>
        /// <typeparam name = "T"></typeparam>
        /// <param name = "value">The value.</param>
        /// <param name = "defaultValue">The default value.</param>
        /// <returns>The target type</returns>
        public static T ConvertToAndIgnoreException<T>(object value, T defaultValue)
        {
            return ConvertTo(value, defaultValue, true);
        }

        /// <summary>
        /// 	Converts an object to the specified target type or returns the default value if
        ///     those 2 types are not convertible.
        ///     <para>
        ///     If the <paramref name="value"/> can't be convert even if the types are 
        ///     convertible with each other, an exception is thrown.</para>
        /// </summary>
        /// <typeparam name = "T"></typeparam>
        /// <param name = "value">The value.</param>
        /// <param name = "defaultValue">The default value.</param>
        /// <returns>The target type</returns>
        public static T ConvertTo_old<T>(object value, T defaultValue)
        {
            try
            {
                if (value != null)
                {
                    var targetType = typeof(T);

                    if (value.GetType() == targetType) return (T)value;

                    var converter = TypeDescriptor.GetConverter(value);
                    if (converter != null)
                    {
                        if (converter.CanConvertTo(targetType))
                            return (T)converter.ConvertTo(value, targetType);
                    }
                    converter = TypeDescriptor.GetConverter(targetType);
                    if (converter != null)
                    {
                        try
                        {
                            if (converter.CanConvertFrom(value.GetType()))
                                return (T)converter.ConvertFrom(value);
                        }
                        catch { return defaultValue; }
                    }
                }
            }
            catch (Exception fe)
            {
                throw new Exception(string.Format("ConvertTo：{0}【{1}】转换成【{2}】失败", fe.Message, value, typeof(T).Name));
            }
            return defaultValue;
        }

        /// <summary>
        /// 	Converts an object to the specified target type or returns the default value if
        ///     those 2 types are not convertible.
        ///     <para>Any exceptions are optionally ignored (<paramref name="ignoreException"/>).</para>
        ///     <para>
        ///     If the exceptions are not ignored and the <paramref name="value"/> can't be convert even if 
        ///     the types are convertible with each other, an exception is thrown.</para>
        /// </summary>
        /// <typeparam name = "T"></typeparam>
        /// <param name = "value">The value.</param>
        /// <param name = "defaultValue">The default value.</param>
        /// <param name = "ignoreException">if set to <c>true</c> ignore any exception.</param>
        /// <returns>The target type</returns>
        public static T ConvertTo<T>(object value, T defaultValue, bool ignoreException)
        {
            if (ignoreException)
            {
                try
                {
                    return ConvertTo<T>(value);
                }
                catch
                {
                    return defaultValue;
                }
            }
            return ConvertTo<T>(value);
        }

        /// <summary>
        /// 	Determines whether the value can (in theory) be converted to the specified target type.
        /// </summary>
        /// <typeparam name = "T"></typeparam>
        /// <param name = "value">The value.</param>
        /// <returns>
        /// 	<c>true</c> if this instance can be convert to the specified target type; otherwise, <c>false</c>.
        /// </returns>
        public static bool CanConvertTo<T>(object value)
        {
            if (value != null)
            {
                var targetType = typeof(T);

                var converter = TypeDescriptor.GetConverter(value);
                if (converter != null)
                {
                    if (converter.CanConvertTo(targetType))
                        return true;
                }

                converter = TypeDescriptor.GetConverter(targetType);
                if (converter != null)
                {
                    if (converter.CanConvertFrom(value.GetType()))
                        return true;
                }
            }
            return false;
        }
    }
}
