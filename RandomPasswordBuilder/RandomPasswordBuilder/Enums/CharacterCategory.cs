using DrLSDee.Text.RandomPasswordBuilder.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DrLSDee.Text.RandomPasswordBuilder.Enums
{
    /// <summary>
    /// <para type="description">Enumerates character categories to use in generated strings.</para>
    /// </summary>
    [Flags()]
    public enum CharacterCategory
    {
        /// <summary>
        /// <para type="description">If this flag is set, the generated string must contain digit 
        /// characters: from '0' to '9', defined in the
        /// <see cref="DefaultCharacters.Digits"/>.</para>
        /// </summary>
        Digits = 0b_00001,

        /// <summary>
        /// <para type="description">If this flag is set, the generated string must contain lowercase 
        /// letters: e.g. basic latin from 'a' to 'z' defined in the
        /// <see cref="DefaultCharacters.LowerCase"/>.</para>
        /// </summary>
        UpperCase = 0b_00010,

        /// <summary>
        /// <para type="description">If this flag is set, the generated string must contain UPPERCASE 
        /// letters: e.g. basic latin from 'A' to 'Z'  defined in the
        /// <see cref="DefaultCharacters.UpperCase"/>.</para>
        /// </summary>
        LowerCase = 0b_00100,

        /// <summary>
        /// <para type="description">If this flag is set, the generated string must contain special 
        /// characters from the set defined in the
        /// <see cref="DefaultCharacters.Special"/> constant.</para>
        /// </summary>
        Special = 0b_01000
    }
}
