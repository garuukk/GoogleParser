using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Svyaznoy.Core
{
    public static class PathExtensions
    {
        /// <summary>
        /// Removes from file name invalid chars and replaces them with 'placeholder'
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="placeholder"></param>
        /// <returns></returns>
        public static string CleanFilePath(this string fileName, char placeholder = '-')
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return fileName;

            fileName = new string(fileName.Select(c =>
            {
                if (Path.GetInvalidFileNameChars().All(ic => ic != c))
                    return c;
                else
                {
                    return placeholder;
                }
            }).ToArray());
            return fileName;
        }
    }
}
