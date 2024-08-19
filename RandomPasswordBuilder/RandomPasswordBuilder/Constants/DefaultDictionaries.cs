using DrLSDee.Text.RandomPasswordBuilder.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrLSDee.Text.RandomPasswordBuilder.Constants
{
    /// <summary>
    /// Stores some default character dictionaries as fields.
    /// </summary>
    public static class DefaultDictionaries
    {
        /// <summary>
        /// The default XML-unsafe dictionary containing 
        /// <see cref="DefaultCharacters.Digits"/>, 
        /// <see cref="DefaultCharacters.UpperCase"/>, 
        /// <see cref="DefaultCharacters.LowerCase"/> and the 
        /// <see cref="DefaultCharacters.Special"/> character sets including 
        /// the <see cref="DefaultCharacters.XmlUnsafe"/> characters.
        /// </summary>
        public static readonly Dictionary<CharacterCategory, string> Default
            = new Dictionary<CharacterCategory, string>()
        {
            { CharacterCategory.Digits, DefaultCharacters.Digits },
            { CharacterCategory.UpperCase, DefaultCharacters.UpperCase },
            { CharacterCategory.LowerCase, DefaultCharacters.LowerCase },
            { CharacterCategory.Special, DefaultCharacters.Special },
        };

        /// <summary>
        /// The default XML-unsafe dictionary containing 
        /// <see cref="DefaultCharacters.Digits"/>, 
        /// <see cref="DefaultCharacters.UpperCase"/>, 
        /// <see cref="DefaultCharacters.LowerCase"/> and the 
        /// <see cref="DefaultCharacters.XmlSafe"/> character sets excluding 
        /// the <see cref="DefaultCharacters.XmlUnsafe"/> characters.
        /// </summary>
        public static readonly Dictionary<CharacterCategory, string> DefaultXmlSafe
            = new Dictionary<CharacterCategory, string>()
        {
            { CharacterCategory.Digits, DefaultCharacters.Digits },
            { CharacterCategory.UpperCase, DefaultCharacters.UpperCase },
            { CharacterCategory.LowerCase, DefaultCharacters.LowerCase },
            { CharacterCategory.Special, DefaultCharacters.XmlSafe },
        };
    }
}
