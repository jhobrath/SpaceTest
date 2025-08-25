using System;
using System.Numerics;
using Raylib_cs;
using GalagaFighter.CharacterScreen.Models;

namespace GalagaFighter.CharacterScreen.UI
{
    public class SelectionLayoutHelper
    {
        private readonly int _screenWidth;
        private readonly int _screenHeight;
        private readonly float _uniformScale;

        public SelectionLayoutHelper(int screenWidth, int screenHeight, float uniformScale)
        {
            _screenWidth = screenWidth;
            _screenHeight = screenHeight;
            _uniformScale = uniformScale;
        }

        public (int startX, int centerY, int itemSpacing) GetShipPanelLayout(bool isPlayer1)
        {
            int baseScreenWidth = 1920;
            int baseScreenHeight = 1080;
            int startX = isPlayer1
                ? (int)((baseScreenWidth / 2 - 670) * _uniformScale)
                : (int)((baseScreenWidth / 2 + 175) * _uniformScale);
            int centerY = (int)((baseScreenHeight / 2.25) * _uniformScale);
            int itemSpacing = (int)((baseScreenHeight * 0.17f) * _uniformScale);
            return (startX, centerY, itemSpacing);
        }

        public (int indicatorX, int indicatorStartY, int indicatorSize, int indicatorSpacing) GetIndicatorLayout(bool isPlayer1, int shipWidth, int itemSpacing, int centerY)
        {
            int indicatorCount = 10;
            int indicatorSize = (int)(22 * _uniformScale);
            int indicatorAreaHeight = itemSpacing * 2 + shipWidth;
            int indicatorSpacing = indicatorCount > 1 ? (indicatorAreaHeight - indicatorSize) / (indicatorCount - 1) : 0;
            int indicatorStartY = centerY - itemSpacing;
            int indicatorX = isPlayer1
                ? (int)(GetShipPanelLayout(isPlayer1).startX - (40 * _uniformScale))
                : (int)(GetShipPanelLayout(isPlayer1).startX + shipWidth + (300 * _uniformScale));
            return (indicatorX, indicatorStartY, indicatorSize, indicatorSpacing);
        }

        public (int startX, int startY) GetEffectPanelLayout(bool isPlayer1)
        {
            int startX = isPlayer1 ? 200 : _screenWidth - 600;
            int startY = 300;
            return (startX, startY);
        }
    }
}
