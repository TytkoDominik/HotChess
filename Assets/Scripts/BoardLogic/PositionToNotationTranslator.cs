using UnityEngine;

namespace BoardLogic
{
    public class PositionToNotationTranslator
    {
        private const int AsciiACode = 65;
        
        public string GetNotationFromPosition(Vector2 position)
        {
            return (char) (position.x + AsciiACode) + (position.y + 1).ToString();
        }
    }
}