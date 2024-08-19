using DrLSDee.Text.RandomPasswordBuilder.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;

namespace DrLSDee.Text.RandomPasswordBuilder.Extensions
{
    /// <summary>
    /// Extensions for enums in this project
    /// </summary>
    internal static class CharacterCategoryExtensions
    {
        /// <summary>
        /// Simple override for the <see cref="Enum.HasFlag(Enum)"/> method 
        /// without unboxing; see on
        /// <see href="https://medium.com/@arphox/the-boxing-overhead-of-the-enum-hasflag-method-c62a0841c25a">
        /// Medium.com</see>;
        /// see also on
        /// <seealso href="https://code-corner.dev/2023/11/04/Understanding-Flag-Enums-in-C/">Code-Corner.dev</seealso>.
        /// </summary>
        /// <param name="instance">This instance of <see cref="CharacterCategory"/> enum</param>
        /// <param name="flag"><see cref="CharacterCategory"/> flag to 
        /// check</param>
        /// <returns>Similar to <see cref="Enum.HasFlag(Enum)"/>, returns
        /// <see langword="true"/>, if the <paramref name="instance"/> has
        /// <paramref name="flag"/>, and <see langword="false"/>, if not.
        /// </returns>
        internal static bool HasFlagUnsafe(this CharacterCategory instance,
            CharacterCategory flag) => (instance & flag) == flag;

        /// <summary>
        /// Splits this <see cref="CharacterCategory"/> <paramref name="instance"/> into a list of single-bit instances.
        /// </summary>
        /// <param name="instance">This instance of <see cref="CharacterCategory"/> enum</param>
        /// <returns>A <see cref="List{T}"/> of single-bit <see cref="CharacterCategory"/> instances.</returns>
        internal static IEnumerable<CharacterCategory> Split(this CharacterCategory instance)
        {
            List<CharacterCategory> result = new List<CharacterCategory>();
            int maxValue = (int)instance;
            int flag = 1;
            while (flag < maxValue)
            {
                if ((maxValue & flag) == flag) result.Add((CharacterCategory)flag);
                flag <<= 1;
            }
            return result;
        }
    }
}
