using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core2.Services.Static
{
    public static class DebugWriter
    {
        public static void Write<T>(T text)
        {
            Raylib.DrawText(text.ToString(), 500, 200, 18, Color.White);
        }
    }
}
