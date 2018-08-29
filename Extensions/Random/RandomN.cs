using System.Security.Cryptography;
using System;

namespace Bot.Extensions.Random
{
    public class RandomN
    {
        public static int getRandomUnsignedInt()
        {
            RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
            var byteArray = new byte[4];
            provider.GetBytes(byteArray);   
            var randomInteger = BitConverter.ToInt32(byteArray, 0);
            if(randomInteger < 0)
                randomInteger *= -1;
            return randomInteger;
        }

        ///<summary>Retruns a random int within range.Including max number.</summary>
        public static int getRandomUnsignedIntFromRange(int min, int max)
        {
            return min + getRandomUnsignedInt() % ((max +1 - min));
        }
    }
}