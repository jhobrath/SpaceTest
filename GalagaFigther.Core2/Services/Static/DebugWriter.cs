using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFigther.Core2.Services.Static
{
    public static class DebugWriter
    {
        public static void Write(string text)
        {
            Raylib.DrawText(text, 500, 200, 18, Color.White);
        }
    }
}
