using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Rkl.Settings
{
    /// <summary>
    /// Базовый класс для настроек
    /// </summary>
    public abstract class ConfigSection
    {
        protected IConfiguration Configuration { get; }
        private readonly string _keyTemplate;

        private object _root = new object();

        private readonly Lazy<Dictionary<string, ConfigSection>> _sectionCache;


        /// <summary>
        /// Создаст ConfigSection.
        /// Потокобезопасный. Можно вызывать из разных потоков для одного экземпляра ConfigSection.
        /// </summary>
        /// <typeparam name="T">Тип создаваемого ConfigSection</typeparam>
        /// <param name="keyPrefix"></param>
        /// <param name="ctor"></param>
        /// <returns></returns>
        protected T GetSection<T>(string keyPrefix, Func<T> ctor) where T : ConfigSection
        {
            ConfigSection result;
            if (_sectionCache.Value.TryGetValue(keyPrefix, out result))
            {
                return (T)result;
            }

            lock (_root)
            {
                if (_sectionCache.Value.TryGetValue(keyPrefix, out result))
                {
                    return (T)result;
                }
                var section = ctor();
                _sectionCache.Value.Add(keyPrefix, section);
                return section;
            }
        }

        /// <summary>
        /// Создаст ConfigSection.
        /// Потокобезопасный. Можно вызывать из разных потоков для одного экземпляра ConfigSection.
        /// </summary>
        /// <typeparam name="T">Тип создаваемого ConfigSection</typeparam>
        /// <param name="keyPrefix"></param>
        /// <returns></returns>
        protected T GetSection<T>(string keyPrefix) where T : ConfigSection
        {
            Func<T> ctor = () => (T)Activator.CreateInstance(typeof(T), Configuration, keyPrefix);
            return GetSection(keyPrefix, ctor);
        }

        /// <summary>
        /// Базовый класс для настроек
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="keyPrefix"></param>
        protected ConfigSection(IConfiguration configuration, string keyPrefix)
        {
            Configuration = configuration;
            if (string.IsNullOrEmpty(keyPrefix))
            {
                _keyTemplate = "{0}";
            }
            else
            {
                _keyTemplate = $"{keyPrefix}:{{0}}";
            }

            _sectionCache = new Lazy<Dictionary<string, ConfigSection>>(() => new Dictionary<string, ConfigSection>());
        }

        /// <summary>
        /// Базовый класс для настроек
        /// </summary>
        /// <param name="configuration"></param>
        protected ConfigSection(IConfiguration configuration) : this(configuration, null)
        {
        }

        /// <summary>
        /// получить массив из настроект
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="parser"></param>
        /// <returns></returns>
        protected T[] GetArray<T>(string key, Func<string, T> parser)
        {
            var childs = Configuration.GetSection(string.Format(_keyTemplate, key)).GetChildren();
            var result = childs.Select(c => parser(c.Value)).ToArray();
            return result;
        }

        /// <summary>
        /// получить массив строк из настроект
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected string[] GetArray(string key)
        {
            return GetArray(key, item => item);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected string GetString(string key)
        {
            return Configuration[string.Format(_keyTemplate, key)];
        }

        /// <summary>
        /// получить значение типа double
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected double GetDouble(string key)
        {
            return double.Parse(GetString(key), CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// получить значение типа int
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected int GetInt(string key)
        {
            return int.Parse(GetString(key));
        }

        /// <summary>
        /// получить значение типа TimeSpan
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected TimeSpan GetTimeSpan(string key)
        {
            return TimeSpan.FromMinutes(GetInt(key));
        }

        /// <summary>
        /// получить значение типа bool
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected bool GetBool(string key)
        {
            return bool.Parse(GetString(key));
        }

        /// <summary>
        /// получить значение для enum
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        protected TEnum GetEnum<TEnum>(string key, bool ignoreCase) where TEnum : struct
        {
            return Enum.Parse<TEnum>(GetString(key), ignoreCase);
        }


    }
}
