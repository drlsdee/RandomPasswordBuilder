using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Text.RandomPasswordBuilder.Enums
{
    /// <summary>
    /// Enumerates character categories to use in generated strings.
    /// </summary>
    [Flags()]
    public enum CharacterCategory
    {
        /// <summary>
        /// If this flag is set, the generated string must contain digit 
        /// characters: from '0' to '9', defined in the
        /// <see cref="Constants.DefaultCharacters.Digits"/>.
        /// </summary>
        Digits = 0b_00001,

        /// <summary>
        /// If this flag is set, the generated string must contain lowercase 
        /// letters: e.g. basic latin from 'a' to 'z' defined in the
        /// <see cref="Constants.DefaultCharacters.LowerCase"/>.
        /// </summary>
        UpperCase = 0b_00010,

        /// <summary>
        /// If this flag is set, the generated string must contain UPPERCASE 
        /// letters: e.g. basic latin from 'A' to 'Z'  defined in the
        /// <see cref="Constants.DefaultCharacters.UpperCase"/>.
        /// </summary>
        LowerCase = 0b_00100,

        /// <summary>
        /// If this flag is set, the generated string must contain special 
        /// characters from the set defined in the
        /// <see cref="Constants.DefaultCharacters.Special"/> constant.
        /// </summary>
        Special = 0b_01000
    }
}
