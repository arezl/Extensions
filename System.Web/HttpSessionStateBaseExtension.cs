﻿namespace System.Web
{
    /// <summary>
    ///   Extensions classes for the ASP.NET Session State class
    /// </summary>
    public static class HttpSessionStateBaseExtension
    {

        /// <summary>
        ///   Returns a typed value from the ASP.NET MVC session state or the provided default value
        /// </summary>
        /// <typeparam name="TValue">The generic type to be returned</typeparam>
        /// <param name = "sessionState">The session state.</param>
        /// <param name = "key">The session state key.</param>
        /// <param name = "defaultValue">The default value to be returned.</param>
        /// <returns>The session state value.</returns>
        /// <example>
        ///   <code>
        ///     public List&lt;String&gt; StringValues {
        ///     get { return this.Session.Ensure&lt;List&lt;String&gt;&gt;("StringValues"); }
        ///     set { this.ViewState.Set("StringValues", value); }
        ///     }
        ///   </code>
        /// </example>
        public static TValue GetValue<TValue>(this HttpSessionStateBase sessionState, String key, TValue defaultValue = default(TValue))
        {
            var value = sessionState[key];
            return (TValue) (value ?? defaultValue);
        }

        /// <summary>
        ///   Ensures a specific key to be either already in the ASP.NET MVC session state or to be newly created
        /// </summary>
        /// <typeparam name = "T">The generic type to be returned</typeparam>
        /// <param name = "sessionState">The session state.</param>
        /// <param name = "key">The session state key.</param>
        /// <returns>The session state value.</returns>
        /// <example>
        ///   <code>
        ///     public List&lt;String&gt; StringValues 
        ///     {
        ///     get { return this.Session.Ensure&lt;List&lt;String&gt;&gt;("StringValues"); }
        ///     set { this.ViewState.Set("StringValues", value); }
        ///     }
        ///   </code>
        /// </example>
        public static T Ensure<T>(this HttpSessionStateBase sessionState, String key) where T : class, new()
        {
            var value = sessionState.GetValue<T>(key);
            if (null == value)
            {
                value = new T();
                sessionState[key] = value;
            }
            return value;
        }

    }
}