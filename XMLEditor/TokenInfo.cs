using System;

namespace XMLEditor
{
    public class TokenInfo
    {
        #region Properties
        public int StartPosition { get; private set; }
        public int Length { get; private set; }
        public string Token { get; private set; }
        #endregion

        #region Constructors

        public TokenInfo(int startPosition, int length, string token)
        {
            StartPosition = startPosition;
            Length = length;
            Token = token;
        }
        #endregion
    }
}
