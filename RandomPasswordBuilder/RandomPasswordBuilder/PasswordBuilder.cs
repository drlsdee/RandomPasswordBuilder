using DrLSDee.Text.RandomPasswordBuilder.Constants;
using DrLSDee.Text.RandomPasswordBuilder.Enums;
using DrLSDee.Text.RandomPasswordBuilder.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DrLSDee.Text.RandomPasswordBuilder
{
    /// <summary>
    /// Builds random password strings.
    /// </summary>
    public class PasswordBuilder : IDisposable
    {
        #region PublicProperties
        /// <summary>
        /// Stores the dictionary with chars used to generate passwords.
        /// </summary>
        public Dictionary<CharacterCategory, string> Items { get => dictionaryBuilder.Items; }

        /// <summary>
        /// Enumerates char categories stored in the <see cref="Items"/> dictionary.
        /// </summary>
        public CharacterCategory CharacterCategories { get => dictionaryBuilder.CharacterCategories; }

        /// <summary>
        /// Indicates whether only XML-safe characters are allowed.
        /// </summary>
        public bool IsXmlSafe { get => dictionaryBuilder.IsXmlSafe; }

        /// <summary>
        /// Stores additional input to exclude from password generation.
        /// </summary>
        public HashSet<char> Exclude { get => dictionaryBuilder.Exclude; }

        /// <summary>
        /// Stores additional input to include to password generation.
        /// </summary>
        public HashSet<char> Include { get => dictionaryBuilder.Include; }

        /// <summary>
        /// Stores the minimum allowed password length.
        /// </summary>
        public int MinLength { get; private set; }

        /// <summary>
        /// Stores the maximum allowed password length.
        /// </summary>
        public int MaxLength { get; private set; }
        #endregion PublicProperties

        #region PrivateProperties
        /// <summary>
        /// Indicates that the minimum length is equal to the maximum, i.e. fixed.
        /// </summary>
        internal bool IsFixedLength => MinLength == MaxLength;

        /// <summary>
        /// An <see cref="Items"/> keys as an array to access by index.
        /// </summary>
        private CharacterCategory[] Keys { get => Items.Keys.ToArray(); }

        /// <summary>
        /// A <see cref="DictionaryBuilder"/> instance.
        /// </summary>
        private DictionaryBuilder dictionaryBuilder { get; set; }

        /// <summary>
        /// An instance of <see cref="RNGCryptoServiceProvider"/> used to compute spread
        /// and pick random chars from each category.
        /// </summary>
        private RNGCryptoServiceProvider rNG { get; set; }

        /// <summary>
        /// Exception message: password length is out of range
        /// </summary>
        private const string _msgLenOutOfRange 
            = "The password length cannot be less than the number of character categories used: {0} categories used: {1}";

        /// <summary>
        /// Pre-formatted exception message.
        /// </summary>
        private string msgLenOutOfRange { get => string.Format(_msgLenOutOfRange, Items.Count, CharacterCategories); }

        /// <summary>
        /// Indicates whether the instance is disposed.
        /// </summary>
        private bool disposedValue;
        #endregion PrivateProperties

        #region ProxyMethods
        /// <summary>
        /// Adds <paramref name="chars"/> to the <see cref="Items"/>, sorting by categories.
        /// </summary>
        /// <param name="chars">Character set to add.</param>
        /// <param name="clear">If set to <see langword="true"/>, previous value will be deleted.</param>
        /// <returns>Returns <see langword="true"/>, if the <see cref="Items"/> not empty.</returns>
        public bool Add(IEnumerable<char> chars, bool clear = false) => dictionaryBuilder.Add(chars, clear);

        /// <summary>
        /// Removes <paramref name="chars"/> from the <see cref="Items"/> dictionary.
        /// </summary>
        /// <param name="chars">Character set to remove.</param>
        /// <returns>Returns <see langword="true"/>, if the <see cref="Items"/> not empty.</returns>
        public bool Remove(IEnumerable<char> chars) => dictionaryBuilder.Remove(chars);

        /// <summary>
        /// Rebuilds an <see cref="Items"/> dictionary using the specified <see cref="CharacterCategory"/> <paramref name="characterCategory"/>,
        /// <paramref name="exclude"/> and <paramref name="include"/> character sets.
        /// </summary>
        /// <param name="characterCategory"><see cref="CharacterCategory"/> to include.</param>
        /// <param name="xmlSafe">Indicates whether the dictionary must contain only XML-safe characters.</param>
        /// <param name="exclude">Disallowed character set.</param>
        /// <param name="include">Additional characters to include.</param>
        /// <returns>Returns <see langword="true"/>, if the <see cref="Items"/> dictionary is not empty.</returns>
        public bool TrySetDictionary(CharacterCategory characterCategory, bool xmlSafe,
            IEnumerable<char> exclude, IEnumerable<char> include) => dictionaryBuilder
            .TrySetDictionary(characterCategory, xmlSafe, exclude, include);

        /// <summary>
        /// Overloads the <see cref="TrySetDictionary(CharacterCategory, bool, IEnumerable{char}, IEnumerable{char})"/>,
        /// but without additional character sets to include or exclude.
        /// Previous values of the <see cref="Exclude"/> and <see cref="Include"/> properties will be deleted.
        /// </summary>
        /// <param name="characterCategory"><see cref="CharacterCategory"/> to include.</param>
        /// <param name="xmlSafe">Indicates whether the dictionary must contain only XML-safe characters.</param>
        /// <returns>Returns <see langword="true"/>, if the <see cref="Items"/> dictionary is not empty.</returns>
        public bool TrySetDictionary(CharacterCategory characterCategory, bool xmlSafe = false) 
            => dictionaryBuilder.TrySetDictionary(characterCategory, xmlSafe);

        /// <summary>
        /// Removes character categories specified in the <paramref name="key"/> from <see cref="Items"/> as well as from <see cref="CharacterCategories"/>.
        /// The result indicates whether the dictionary <see cref="Items"/> still not empty.
        /// </summary>
        /// <param name="key"><see cref="CharacterCategory"/> to remove from <see cref="Items"/></param>
        /// <returns>If the dictionary contains any non-empty keys, returns <see langword="true"/>, otherwise <see langword="false"/></returns>
        public bool RemoveCategory(CharacterCategory key) => dictionaryBuilder.RemoveCategory(key);

        /// <summary>
        /// Resets character categories specified in the <paramref name="key"/> in <see cref="Items"/> to default values.
        /// If some character categories were not specified previously, they will be added with default values, XML-safe or unsafe.
        /// Because this method resets character set, added characters will be excluded from the <see cref="Exclude"/> property.
        /// </summary>
        /// <param name="key"><see cref="CharacterCategory"/> to reset in the<see cref="Items"/>.</param>
        /// <param name="xmlSafe">If set to <see langword="true"/>, excludes <see cref="DefaultCharacters.XmlUnsafe"/> characters.</param>
        public void ResetCategory(CharacterCategory key, bool xmlSafe = false) => dictionaryBuilder.ResetCategory(key, xmlSafe);
        #endregion ProxyMethods

        #region Constructors
        /// <summary>
        /// The default paramless constructor, XML-unsafe, generating strings of 8-16 chars length.
        /// </summary>
        public PasswordBuilder()
        {
            rNG = new RNGCryptoServiceProvider();
            dictionaryBuilder = DictionaryBuilder.Default;
            MinLength = 8; MaxLength = 16;
        }

        /// <summary>
        /// Full constructor with all explicitly specified values.
        /// </summary>
        /// <param name="characterCategory"><see cref="CharacterCategory"/> to use</param>
        /// <param name="xmlSafe">Use only XML-safe characters</param>
        /// <param name="minLength">The minimum password length.</param>
        /// <param name="maxLength">The maximum password length.</param>
        /// <param name="exclude">Character set to exclude.</param>
        /// <param name="include">Additional characters to include.</param>
        /// <exception cref="ArgumentOutOfRangeException">The specified length is less than the number of char categories.</exception>
        public PasswordBuilder(CharacterCategory characterCategory, bool xmlSafe, int minLength, int maxLength, IEnumerable<char> exclude, IEnumerable<char> include)
        {
            dictionaryBuilder = new DictionaryBuilder(characterCategory, xmlSafe, exclude, include);
            if (minLength < Items.Count) throw new ArgumentOutOfRangeException(nameof(minLength), 
                minLength, msgLenOutOfRange);
            MinLength = minLength;
            if (maxLength < Items.Count) throw new ArgumentOutOfRangeException(nameof(maxLength), 
                maxLength, msgLenOutOfRange);
            MaxLength = maxLength;
            rNG = new RNGCryptoServiceProvider();
        }

        /// <summary>
        /// Constructor with the specified char categories, fixed length and optionally <see cref="IsXmlSafe"/> flag.
        /// </summary>
        /// <param name="characterCategory"><see cref="CharacterCategory"/> to use</param>
        /// <param name="length">The desired password length</param>
        /// <param name="xmlSafe">Use only XML-safe characters</param>
        /// <exception cref="ArgumentOutOfRangeException">The specified length is less than the number of char categories.</exception>
        public PasswordBuilder(CharacterCategory characterCategory, int length, bool xmlSafe = false)
        {
            dictionaryBuilder = new DictionaryBuilder(characterCategory, xmlSafe);
            if (length < Items.Count) throw new ArgumentOutOfRangeException(nameof(length),
                length, msgLenOutOfRange);
            MinLength = length;
            MaxLength = length;
        }

        private PasswordBuilder(DictionaryBuilder dictionary)
        {
            dictionaryBuilder = dictionary;
            rNG = new RNGCryptoServiceProvider();
            MinLength = 8; MaxLength = 16;
        }
        #endregion Constructors

        /// <summary>
        /// The default XML-unsafe instance with all char categories, without character sets to exclude or include 
        /// and with password length between 8 and 16 chars.
        /// </summary>
        public static PasswordBuilder Default { get { return new PasswordBuilder(); } }

        /// <summary>
        /// The default XML-safe instance with all char categories, without extra character sets to exclude or include 
        /// and with password length between 8 and 16 chars.
        /// </summary>
        public static PasswordBuilder DefaultXmlSafe { get { return new PasswordBuilder(DictionaryBuilder.DefaultXmlSafe); } }

        #region HelperMethods
        /// <summary>
        /// Returns random value between the <see cref="MinLength"/> and <see cref="MaxLength"/> as current password length.
        /// </summary>
        /// <returns>Random <see cref="int"/> value between the <see cref="MinLength"/> and <see cref="MaxLength"/></returns>
        private int GetCurrentLength() => IsFixedLength ? MinLength : rNG.Next(MinLength, MaxLength);

        /// <summary>
        /// Returns a dictionary where key count is <see cref="CharacterCategories"/> count, and
        /// values are number of characters in each category.
        /// </summary>
        /// <param name="passwordLength">Current password length.</param>
        /// <returns>a dictionary where key count is <see cref="CharacterCategories"/> count, and
        /// values are number of characters in each category.</returns>
        private Dictionary<int, int> GetSpread(int passwordLength)
        {
            // Assume that the password length is always greater than or equal to the key count.
            // So just add 1 for each character category.
            Dictionary<int, int> result = new Dictionary<int, int>();
            for (int i = 0; i < Items.Count; i++) result.Add(i, 1);
            int diffLen = passwordLength - result.Count;
            if (diffLen == 0) return result;
            // If the length is greater by 1
            if (diffLen == 1)
            {
                // Add 1 to the random key and return.
                result.Add(rNG.Next(result.Count - 1), 1);
                return result;
            }
            // Add random values to the resulting spread
            while (diffLen > 0)
            {
                for (int key = 0; key < result.Count; key++)
                {
                    // Yes, both "while" and "break"; I know, it's a trash.
                    if (diffLen == 0) break;
                    // Double call for more enthropy.
                    int value = rNG.Next(1, rNG.Next(1, diffLen));
                    result[key] += value;
                    diffLen -= value;
                }
            }
            return result;
        }

        /// <summary>
        /// Overloads the <see cref="GetSpread(int)"/> with random length between the <see cref="MinLength"/> and the <see cref="MaxLength"/>.
        /// </summary>
        /// <returns></returns>
        private Dictionary<int, int> GetSpread() => GetSpread(GetCurrentLength());
        #endregion HelperMethods

        /// <summary>
        /// Generates a string with random chars from ALL categories specified in the <see cref="Items"/> and <see cref="CharacterCategories"/>.
        /// The length of the string is between the <see cref="MinLength"/> and <see cref="MaxLength"/>.
        /// </summary>
        /// <returns>Random string with all specified categories.</returns>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        /// <exception cref="InvalidOperationException">The character dictionary is empty.</exception>
        public string Next()
        {
            // If disposed
            if (disposedValue) throw new ObjectDisposedException(GetType().FullName);
            // Check if the dictionary is occasionally empty
            if (!Items.Any()) throw new InvalidOperationException("The character dictionary is empty");
            // Init spread
            Dictionary<int, int> spread = GetSpread();
            // Declare an empty char list
            List<char> chars = new List<char>();
            foreach (int key in spread.Keys)
            {
                // Get the char category by key index
                string source = Items[Keys[key]];
                // Char number in the current category
                int length = spread[key];
                // Add random source char to the list
                for (int i = 0; i < length; i++) chars.Add(source[rNG.Next(source.Length - 1)]);
            }
            // Shuffle and build a string; ordering by GUID from here:
            // https://github.com/prjseal/PasswordGenerator/blob/master/PasswordGenerator/Password.cs#L247
            return new string(chars.OrderBy(x => Guid.NewGuid()).ToArray());
        }

        /// <summary>
        /// Implements the <see cref="IDisposable.Dispose()"/>
        /// </summary>
        /// <param name="disposing">Indicates that managed objects must be disposed.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    rNG.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                dictionaryBuilder.Clear();
                MinLength = 0;
                MaxLength = 0;
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        /// <summary>
        /// Finalizer
        /// </summary>
        ~PasswordBuilder()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: false);
        }

        /// <summary>
        /// Clears the <see cref="Items"/>, disposes an instance of <see cref="RNGCryptoServiceProvider"/> and vice versa.
        /// Public wrapper for the protected <see cref="Dispose(bool)"/> method.
        /// </summary>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
