using Raylib_cs;
using System.Collections.Generic;

namespace GalagaFighter.Core.Services
{
    public static class TextureService
    {
        public static Dictionary<string, Texture2D> _library = [];

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

        public static SpriteWrapper GetFrame(string texturePath, int frameCount, int frameIndex)
        {
            var texture = Get(texturePath);
            return new SpriteWrapper(texture, frameIndex, frameCount);
        }

        // Extract a single frame from a spritesheet as a new Texture2D
        public static Texture2D GetFrame(Texture2D texture, int frameCount, int frameIndex)
        {
            int frameWidth = texture.Width / frameCount;
            int frameHeight = texture.Height;
            Image image = Raylib.LoadImageFromTexture(texture);
            Rectangle sourceRec = new(frameWidth * frameIndex, 0, frameWidth, frameHeight);
            Image frameImage = Raylib.ImageFromImage(image, sourceRec);
            Texture2D frameTexture = Raylib.LoadTextureFromImage(frameImage);
            Raylib.UnloadImage(image);
            Raylib.UnloadImage(frameImage);
            return frameTexture;
        }
    }
}
