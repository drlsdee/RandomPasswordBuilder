using DrLSDee.Text.RandomPasswordBuilder.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace DrLSDee.Text.RandomPasswordBuilder.Cmdlets
{
    /// <summary>
    /// Produces randomly generated strings.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "RandomPassword")]
    [OutputType(typeof(string))]
    public class GetRandomPasswordCmdlet : PSCmdlet
    {
        /// <summary>
        /// Specifies the minimum password length; the default value is 8.
        /// </summary>
        [Parameter(Position = 0)]
        [ValidateRange(4, int.MaxValue)]
        public int MinLength { get; set; } = 8;

        /// <summary>
        /// Specifies the maximum password length; the default value is 16.
        /// </summary>
        [Parameter(Position = 1)]
        [ValidateRange(4, int.MaxValue)]
        public int MaxLength { get; set; } = 16;

        /// <summary>
        /// Specifies the number of random strings to generate; the default is 1.
        /// </summary>
        [Parameter(Position = 2)]
        [ValidateRange(1, int.MaxValue)]
        public int Count { get; set; } = 1;

        /// <summary>
        /// Specifies the character categories to use. Possible values are <see cref="CharacterCategory.Digits"/>,
        /// <see cref="CharacterCategory.UpperCase"/>, <see cref="CharacterCategory.LowerCase"/>,
        /// <see cref="CharacterCategory.Special"/> and any combinations thereof.
        /// </summary>
        [Parameter(Position = 3)]
        public CharacterCategory CharacterCategory { get; set; } 
            = CharacterCategory.Digits | CharacterCategory.UpperCase 
            | CharacterCategory.LowerCase | CharacterCategory.Special;

        /// <summary>
        /// Indicates whether we must not use XML-unsafe characters: &quot;, &apos;, &lt;, &gt; and &amp;
        /// </summary>
        [Parameter()]
        public SwitchParameter XmlSafe { get; set; }

        /// <summary>
        /// Specifies the characters to not use in generated strings, in addition to the <see cref="XmlSafe"/> parameter or not.
        /// </summary>
        [Parameter()]
        public char[] Exclude { get; set; } = new char[0];

        /// <summary>
        /// Specifies the additional characters to use in generated strings.
        /// </summary>
        [Parameter()]
        public char[] Include { get; set; } = new char[0];

        /// <summary>
        ///  A <see cref="PasswordBuilder"/> instance
        /// </summary>
        private PasswordBuilder passwordBuilder { get; set; }

        /// <summary>
        /// Overrides the <see cref="Cmdlet.BeginProcessing"/> and initializes the <see cref="passwordBuilder"/>
        /// </summary>
        protected override void BeginProcessing()
        {
            passwordBuilder = new PasswordBuilder(CharacterCategory, XmlSafe, MinLength, MaxLength, Exclude, Include);
            base.BeginProcessing();
        }

        /// <summary>
        /// Overrides the <see cref="Cmdlet.EndProcessing"/> and disposes the <see cref="passwordBuilder"/>
        /// </summary>
        protected override void EndProcessing()
        {
            passwordBuilder.Dispose();
            base.EndProcessing();
        }

        /// <summary>
        /// Overrides the <see cref="Cmdlet.StopProcessing"/> and disposes the <see cref="passwordBuilder"/>
        /// </summary>
        protected override void StopProcessing()
        {
            passwordBuilder?.Dispose();
            base.StopProcessing();
        }

        /// <summary>
        /// Overrides the <see cref="Cmdlet.ProcessRecord"/> and produces <see cref="Count"/> random strings
        /// using the <see cref="passwordBuilder"/> instance.
        /// </summary>
        protected override void ProcessRecord()
        {
            for (int i = 0; i < Count; i++)
            {
                WriteObject(passwordBuilder.Next());
            }
            base.ProcessRecord();
        }
    }
}
