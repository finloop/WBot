using System.Security.Cryptography;
using System;
using SaidOut.Security;

namespace Bot.Extensions.Random
{
    public class RandomN
    {
        ///<summary>Retruns a random int within range.Including max number.</summary>
        public static int getRandomUnsignedIntFromRange(int min, int max) => SecureRandom.GenerateRandomValue(min, max);
    }
}