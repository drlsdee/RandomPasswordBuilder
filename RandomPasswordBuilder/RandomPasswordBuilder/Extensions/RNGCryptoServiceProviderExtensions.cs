using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace DrLSDee.Text.RandomPasswordBuilder.Extensions
{
    /// <summary>
    /// Provides some extension methods for the <see cref="RNGCryptoServiceProvider"/> class; 
    /// these methods are kind of implementation of <see cref="Random.Next(int, int)"/>, 
    /// <see cref="Random.Next(int)"/> and <see cref="Random.Next()"/> methods but slightly different.
    /// </summary>
    internal static class RNGCryptoServiceProviderExtensions
    {
        /// <summary>
        /// Implements a kind of <see cref="Random.Next(int, int)"/> method and 
        /// returns a value in the range between two <see cref="int">integers</see>; 
        /// but unlike it, this method does not require the minimum and maximum 
        /// values ​​to be specified in strict order; either of the arguments 
        /// "<paramref name="x"/>" and "<paramref name="y"/>" can be the 
        /// minimum or maximum value of the range.
        /// </summary>
        /// <param name="rNG">This <see cref="RNGCryptoServiceProvider"/> instance</param>
        /// <param name="x">The first <see cref="int"/> value, minimum or maximum</param>
        /// <param name="y">The second <see cref="int"/> value, minimum or maximum</param>
        /// <returns>An <see cref="int">integer</see> value in the specified range</returns>
        /// <exception cref="ObjectDisposedException">The instance is <see langword="null"/>, 
        /// maybe disposed</exception>
        internal static int Next(this RNGCryptoServiceProvider rNG, int x, int y)
        {
            // Null check
            if (rNG == null) throw new ObjectDisposedException(nameof(rNG));
            // Range check
            if (x == y) return y;
            // Be more tolerant of human errors (e.g. when the x and y
            // parameters are accidentally set in the wrong order)
            int baseValue = Math.Min(x, y);
            // Added 1 to include the extreme value
            int rangeLength = Math.Max(x, y) - baseValue + 1;
            // Byte buffer
            byte[] bytes = new byte[sizeof(int)];
            // Fill the buffer with bytes
            rNG.GetBytes(bytes);
            // Convert bytes to an integer, get the remainder of that integer
            // divided by the range length, convert that to an unsigned value,
            // then add that to the base value and finally return the result:
            // int converted = BitConverter.ToInt32(bytes, 0);
            // int remainder = converted % rangeLength;
            // int absolute = Math.Abs(remainder);
            // int result = baseValue + absolute;
            // return result;
            return baseValue + Math.Abs(BitConverter.ToInt32(bytes, 0) % rangeLength);
        }

        /// <summary>
        /// Implements a kind of <see cref="Random.Next(int)"/> method 
        /// and returns a value between the specified <see cref="int">integer</see>
        /// and zero; but unlike it, this method allows you to specify 
        /// a negative value as an argument.
        /// </summary>
        /// <param name="rNG">This <see cref="RNGCryptoServiceProvider"/> instance</param>
        /// <param name="value">The extreme <see cref="int">integer</see> value in the range</param>
        /// <returns>An <see cref="int">integer</see> value in the 
        /// range between zero and the specified <paramref name="value"/></returns>
        internal static int Next(this RNGCryptoServiceProvider rNG, int value) => rNG.Next(0, value);

        /// <summary>
        /// Implements a kind of <see cref="Random.Next()"/> method 
        /// and returns a value between the <see cref="int.MinValue"/> 
        /// and <see cref="int.MaxValue"/>.
        /// </summary>
        /// <param name="rNG">The <see cref="RNGCryptoServiceProvider"/> 
        /// instance as self</param>
        /// <returns>An <see cref="int">integer</see> value in the range 
        /// between the <see cref="int.MinValue"/> and 
        /// <see cref="int.MaxValue"/></returns>
        internal static int Next(this RNGCryptoServiceProvider rNG) => rNG.Next(int.MinValue, int.MaxValue);
    }
}
