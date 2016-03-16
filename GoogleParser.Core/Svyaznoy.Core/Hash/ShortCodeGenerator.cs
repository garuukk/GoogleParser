using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Svyaznoy.Core.Hash
{
    public static class ShortCodeGenerator
    {
        // define characters allowed in passcode.  set length so divisible into 256
        private static readonly char[] ValidChars = {'2','3','4','5','6','7','8','9',
                           'A','B','C','D','E','F','G','H',
                           'J','K','L','M','N','P','Q',
                           'R','S','T','U','V','W','X','Y','Z'}; // len=32

        private const string Hashkey = "sharedhashkey"; //key for HMAC function

        private const int CodeLength = 6; // length of passcode

        public static string Generate(string inputString, int codeLenght = CodeLength)
        {
            if (inputString == null) throw new ArgumentNullException("inputString");

            byte[] hash;

            using (var sha1 = new HMACSHA1(Encoding.ASCII.GetBytes(Hashkey)))
                hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(inputString));

            int startpos = hash[hash.Length - 1] % (hash.Length - codeLenght);

            var passbuilder = new StringBuilder();

            for (int i = startpos; i < startpos + codeLenght; i++)
                passbuilder.Append(ValidChars[hash[i] % ValidChars.Length]);

            return passbuilder.ToString();
        }

        public static string Generate(int codeLenght = CodeLength)
        {
            return Generate(Guid.NewGuid().ToString(), codeLenght: codeLenght);
        }
    }
}