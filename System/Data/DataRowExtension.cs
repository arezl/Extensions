﻿namespace System.Data
{
    /// <summary>
    ///   Extension methods for ADO.NET DataRows (DataTable / DataSet)
    /// </summary>
    public static class DataRowExtension
    {

        /// <summary>
        ///   Gets the record value casted to the specified data type or the specified default value.
        /// </summary>
        /// <typeparam name="T"> The return data type </typeparam>
        /// <param name="row"> The data row. </param>
        /// <param name="field"> The name of the record field. </param>
        /// <param name="defaultValue"> The default value. </param>
        /// <returns> The record value </returns>
        public static T Get<T>(this DataRow row, String field, T defaultValue)
        {
            var value = row[field];
            return (value == DBNull.Value) ? defaultValue : value.ConvertTo(defaultValue);
        }

        /// <summary>
        ///   Gets the record value casted to the specified data type or the data types default value.
        /// </summary>
        /// <typeparam name="T"> The return data type </typeparam>
        /// <param name="row"> The data row. </param>
        /// <param name="field"> The name of the record field. </param>
        /// <returns> The record value </returns>
        public static T Get<T>(this DataRow row, String field)
        {
            return Get(row, field, default(T));
        }

        /// <summary>
        ///   Gets the record value casted as byte array.
        /// </summary>
        /// <param name="row"> The data row. </param>
        /// <param name="field"> The name of the record field. </param>
        /// <returns> The record value </returns>
        public static byte[] GetBytes(this DataRow row, String field)
        {
            return (null != row) ? row[field] as byte[] : new byte[] { };
        }

        #region GetString

        /// <summary>
        ///   Gets the record value casted as String or the specified default value.
        /// </summary>
        /// <param name="row"> The data row. </param>
        /// <param name="field"> The name of the record field. </param>
        /// <param name="defaultValue"> The default value. </param>
        /// <returns> The record value </returns>
        public static String GetString(this DataRow row, String field, String defaultValue)
        {
            var value = row[field];
            return (value is String) ? (String) value : defaultValue;
        }

        /// <summary>
        ///   Gets the record value casted as String or null.
        /// </summary>
        /// <param name="row"> The data row. </param>
        /// <param name="field"> The name of the record field. </param>
        /// <returns> The record value </returns>
        public static String GetString(this DataRow row, String field)
        {
            return GetString(row, field, null);
        }
        #endregion

        #region GetGuid
        /// <summary>
        ///   Gets the record value casted as Guid or Guid.Empty.
        /// </summary>
        /// <param name="row"> The data row. </param>
        /// <param name="field"> The name of the record field. </param>
        /// <returns> The record value </returns>
        public static Guid GetGuid(this DataRow row, String field)
        {
            var value = row[field];
            return (value is Guid) ? (Guid) value : Guid.Empty;
        }
        #endregion

        #region GetDateTime

        /// <summary>
        ///   Gets the record value casted as DateTime or the specified default value.
        /// </summary>
        /// <param name="row"> The data row. </param>
        /// <param name="field"> The name of the record field. </param>
        /// <param name="defaultValue"> The default value. </param>
        /// <returns> The record value </returns>
        public static DateTime GetDateTime(this DataRow row, String field, DateTime defaultValue)
        {
            var value = row[field];
            return (value is DateTime) ? (DateTime) value : defaultValue;
        }

        /// <summary>
        ///   Gets the record value casted as DateTime or DateTime.MinValue.
        /// </summary>
        /// <param name="row"> The data row. </param>
        /// <param name="field"> The name of the record field. </param>
        /// <returns> The record value </returns>
        public static DateTime GetDateTime(this DataRow row, String field)
        {
            return GetDateTime(row, field, DateTime.MinValue);
        }
        #endregion

        #region GetDateTimeOffset

        /// <summary>
        ///   Gets the record value casted as DateTimeOffset (UTC) or the specified default value.
        /// </summary>
        /// <param name="row"> The data row. </param>
        /// <param name="field"> The name of the record field. </param>
        /// <param name="defaultValue"> The default value. </param>
        /// <returns> The record value </returns>
        public static DateTimeOffset GetDateTimeOffset(this DataRow row, String field, DateTimeOffset defaultValue)
        {
            var datetime = row.GetDateTime(field);
            return (datetime != DateTime.MinValue) ? new DateTimeOffset(datetime, TimeSpan.Zero) : defaultValue;
        }

        /// <summary>
        ///   Gets the record value casted as DateTimeOffset (UTC) or DateTime.MinValue.
        /// </summary>
        /// <param name="row"> The data row. </param>
        /// <param name="field"> The name of the record field. </param>
        /// <returns> The record value </returns>
        public static DateTimeOffset GetDateTimeOffset(this DataRow row, String field)
        {
            return new DateTimeOffset(GetDateTime(row, field), TimeSpan.Zero);
        }
        #endregion

        /// <summary>
        ///   Gets the record value casted as int or the specified default value.
        /// </summary>
        /// <param name="row"> The data row. </param>
        /// <param name="field"> The name of the record field. </param>
        /// <param name="defaultValue"> The default value. </param>
        /// <returns> The record value </returns>
        public static int GetInt32(this DataRow row, String field, int defaultValue)
        {
            var value = row[field];
            return (value is int) ? (int) value : defaultValue;
        }

        /// <summary>
        ///   Gets the record value casted as int or 0.
        /// </summary>
        /// <param name="row"> The data row. </param>
        /// <param name="field"> The name of the record field. </param>
        /// <returns> The record value </returns>
        public static int GetInt32(this DataRow row, String field)
        {
            return GetInt32(row, field, 0);
        }

        /// <summary>
        ///   Gets the record value casted as long or the specified default value.
        /// </summary>
        /// <param name="row"> The data row. </param>
        /// <param name="field"> The name of the record field. </param>
        /// <param name="defaultValue"> The default value. </param>
        /// <returns> The record value </returns>
        public static long GetInt64(this DataRow row, String field, int defaultValue)
        {
            var value = row[field];
            return (value is long) ? (long) value : defaultValue;
        }

        /// <summary>
        ///   Gets the record value casted as long or 0.
        /// </summary>
        /// <param name="row"> The data row. </param>
        /// <param name="field"> The name of the record field. </param>
        /// <returns> The record value </returns>
        public static long GetInt64(this DataRow row, String field)
        {
            return GetInt64(row, field, 0);
        }

        /// <summary>
        ///   Gets the record value casted as decimal or the specified default value.
        /// </summary>
        /// <param name="row"> The data row. </param>
        /// <param name="field"> The name of the record field. </param>
        /// <param name="defaultValue"> The default value. </param>
        /// <returns> The record value </returns>
        public static decimal GetDecimal(this DataRow row, String field, long defaultValue)
        {
            var value = row[field];
            return (value is decimal) ? (decimal) value : defaultValue;
        }

        /// <summary>
        ///   Gets the record value casted as decimal or 0.
        /// </summary>
        /// <param name="row"> The data row. </param>
        /// <param name="field"> The name of the record field. </param>
        /// <returns> The record value </returns>
        public static decimal GetDecimal(this DataRow row, String field)
        {
            return GetDecimal(row, field, 0);
        }

        /// <summary>
        ///   Gets the record value casted as bool or the specified default value.
        /// </summary>
        /// <param name="row"> The data row. </param>
        /// <param name="field"> The name of the record field. </param>
        /// <param name="defaultValue"> The default value. </param>
        /// <returns> The record value </returns>
        public static bool GetBoolean(this DataRow row, String field, bool defaultValue)
        {
            var value = row[field];
            return (value is bool) ? (bool) value : defaultValue;
        }

        /// <summary>
        ///   Gets the record value casted as bool or false.
        /// </summary>
        /// <param name="row"> The data row. </param>
        /// <param name="field"> The name of the record field. </param>
        /// <returns> The record value </returns>
        public static bool GetBoolean(this DataRow row, String field)
        {
            return GetBoolean(row, field, false);
        }

        /// <summary>
        ///   Gets the record value as Type class instance or the specified default value.
        /// </summary>
        /// <param name="row"> The data row. </param>
        /// <param name="field"> The name of the record field. </param>
        /// <param name="defaultValue"> The default value. </param>
        /// <returns> The record value </returns>
        public static Type GetType(this DataRow row, String field, Type defaultValue)
        {
            var classType = row.GetString(field);
            if (classType.IsNotNullOrEmpty())
            {
                var type = Type.GetType(classType);
                if (null != type) return type;
            }
            return defaultValue;
        }

        /// <summary>
        ///   Gets the record value as Type class instance or null.
        /// </summary>
        /// <param name="row"> The data row. </param>
        /// <param name="field"> The name of the record field. </param>
        /// <returns> The record value </returns>
        public static Type GetType(this DataRow row, String field)
        {
            return GetType(row, field, null);
        }

        /// <summary>
        ///   Gets the record value as class instance from a type name or the specified default type.
        /// </summary>
        /// <param name="row"> The data row. </param>
        /// <param name="field"> The name of the record field. </param>
        /// <param name="defaultValue"> The default value. </param>
        /// <returns> The record value </returns>
        public static object GetTypeInstance(this DataRow row, String field, Type defaultValue)
        {
            var type = row.GetType(field, defaultValue);
            return (null != type) ? Activator.CreateInstance(type) : null;
        }

        /// <summary>
        ///   Gets the record value as class instance from a type name or null.
        /// </summary>
        /// <param name="row"> The data row. </param>
        /// <param name="field"> The name of the record field. </param>
        /// <returns> The record value </returns>
        public static object GetTypeInstance(this DataRow row, String field)
        {
            return GetTypeInstance(row, field, null);
        }

        /// <summary>
        ///   Gets the record value as class instance from a type name or the specified default type.
        /// </summary>
        /// <typeparam name="T"> The type to be casted to </typeparam>
        /// <param name="row"> The data row. </param>
        /// <param name="field"> The name of the record field. </param>
        /// <param name="type"> The type. </param>
        /// <returns> The record value </returns>
        public static T GetTypeInstanceSafe<T>(this DataRow row, String field, Type type) where T : class
        {
            var instance = row.GetTypeInstance(field, null) as T;
            return instance ?? Activator.CreateInstance(type) as T;
        }

        /// <summary>
        ///   Gets the record value as class instance from a type name or null.
        /// </summary>
        /// <typeparam name="T"> The type to be casted to </typeparam>
        /// <param name="row"> The data row. </param>
        /// <param name="field"> The name of the record field. </param>
        /// <returns> The record value </returns>
        public static T GetTypeInstance<T>(this DataRow row, String field) where T : class
        {
            return GetTypeInstance(row, field, null) as T;
        }

        /// <summary>
        ///   Gets the record value as class instance from a type name or an instance from the specified type.
        /// </summary>
        /// <typeparam name="T"> The type to be casted to </typeparam>
        /// <param name="row"> The data row. </param>
        /// <param name="field"> The name of the record field. </param>
        /// <returns> The record value </returns>
        public static T GetTypeInstanceSafe<T>(this DataRow row, String field) where T : class, new()
        {
            var instance = row.GetTypeInstance(field, null) as T;
            return instance ?? new T();
        }

        /// <summary>
        ///   Determines whether the record value is DBNull.Value
        /// </summary>
        /// <param name="row"> The data row. </param>
        /// <param name="field"> The name of the record field. </param>
        /// <returns> <c>true</c> if the value is DBNull.Value; otherwise, <c>false</c> . </returns>
        public static bool IsDBNull(this DataRow row, String field)
        {
            var value = row[field];
            return (value == DBNull.Value);
        }

    }
}