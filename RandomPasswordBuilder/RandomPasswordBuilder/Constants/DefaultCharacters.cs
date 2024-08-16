using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Text.RandomPasswordBuilder.Constants
{
    public static class DefaultCharacters
    {
        /// <summary>
        /// Digits
        /// </summary>
        public const string Digits = "0123456789";

        /// <summary>
        /// Latin letters - uppercase
        /// </summary>
        public const string UpperCase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        /// <summary>
        /// Latin letters - lowercase
        /// </summary>
        public const string LowerCase = "abcdefghijklmnopqrstuvwxyz";

        /// <summary>
        /// Special characters; this character set is taken from this Microsoft Knowledge Base page: 
        /// <see href="https://learn.microsoft.com/en-us/previous-versions/windows/it-pro/windows-10/security/threat-protection/security-policy-settings/password-must-meet-complexity-requirements"/>
        /// </summary>
        public const string Special = "'-!\"#$%&()*,./:;?@[]^_{|}~+<=>";

        /// <summary>
        /// These characters are unsafe to use in XML; see <see href="https://stackoverflow.com/a/1091953"/>;
        /// they must be either excluded or escaped, or the resulting string must be encoded, e.g. as Base64.
        /// </summary>
        public const string XmlUnsafe = "\"'<>&";

        /// <summary>
        /// Special characters excluding <see cref="XmlUnsafe"/>
        /// </summary>
        public const string XmlSafe = "-!#$%()*,./:;?@[]^_{|}~+=";
    }
}
