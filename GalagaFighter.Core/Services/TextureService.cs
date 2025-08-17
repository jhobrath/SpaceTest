using Raylib_cs;
using System.Collections.Generic;

namespace GalagaFighter.Core.Services
{
    public static class TextureService
    {
        public static Dictionary<string, Texture2D> _library = new Dictionary<string, Texture2D>();

        public static Texture2D Get(string path)
        {
            if (!_library.ContainsKey(path))
                _library[path] = Raylib.LoadTexture(path);

            return _library[path];
        }

        public static void Set(string path, Texture2D texture)
        {
            _library[path] = texture;
        }
    }
}
