using Raylib_cs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core2.Services.Static
{
    public static class TextureCache
    {
        private static Dictionary<string, Texture2D> _cache = [];

        public static Texture2D Get(string key) 
        {
            if (!_cache.ContainsKey(key))
                Add(key);

            return _cache[key];
        }

        public static ICollection<string> Keys => _cache.Keys;

        public static ICollection<Texture2D> Values => _cache.Values;

        public static int Count => _cache.Count;

        public static void Add(string path)
        {
            var texture = Raylib.LoadTexture(path);

            //TODO: Move sprites to GalagaFighter.Core2
            //Use sprites from GalagaFighter.Core project for now
            if(texture.Id == 0)
            {
                var tempPath = "../../../../GalagaFighter.Core/" + path;
                texture = Raylib.LoadTexture(tempPath);
            }

            Set(path, texture);
        }

        public static void Add(string key, string path)
        {
            var texture = Raylib.LoadTexture(path);
            Set(key, texture);
        }

        public static void Add(string key, Texture2D value)
        {
            _cache[key] = value;
        }

        public static void Set(string key, Texture2D value)
        {
            _cache[key] = value;
        }

        public static void Add(KeyValuePair<string, Texture2D> item)
        {
            _cache[item.Key] = item.Value;
        }

        public static void Clear()
        {
            _cache.Clear();
        }

        public static bool Contains(KeyValuePair<string, Texture2D> item)
        {
            return _cache.Contains(item);
        }

        public static bool ContainsKey(string key)
        {
            return _cache.ContainsKey(key);
        }

        public static IEnumerator<KeyValuePair<string, Texture2D>> GetEnumerator()
        {
            return _cache.GetEnumerator();
        }

        public static bool Remove(string key)
        {
            return _cache.Remove(key);
        }

        public static bool Remove(KeyValuePair<string, Texture2D> item)
        {
            return _cache.Remove(item.Key);
        }

        public static bool TryGetValue(string key, [MaybeNullWhen(false)] out Texture2D value)
        {
            return _cache.TryGetValue(key, out value);
        }
    }
}
