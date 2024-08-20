using DrLSDee.Text.RandomPasswordBuilder.Constants;
using DrLSDee.Text.RandomPasswordBuilder.Enums;
using DrLSDee.Text.RandomPasswordBuilder.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrLSDee.Text.RandomPasswordBuilder
{
    /// <summary>
    /// Helper class to generate character dictionaries.
    /// </summary>
    public class DictionaryBuilder
    {
        /// <summary>
        /// Stores <see cref="UnicodeCategory"/> values allowed to use.
        /// </summary>
        private static readonly UnicodeCategory[] _categoriesSpecial =
        {
            UnicodeCategory.ClosePunctuation,
            UnicodeCategory.ConnectorPunctuation,
            UnicodeCategory.CurrencySymbol,
            UnicodeCategory.DashPunctuation,
            UnicodeCategory.MathSymbol,
            UnicodeCategory.ModifierSymbol,
            UnicodeCategory.OpenPunctuation,
            UnicodeCategory.OtherPunctuation,
            UnicodeCategory.SpaceSeparator,
        };

        /// <summary>
        /// Tries to get the <see cref="CharacterCategory"/> for the character <paramref name="c"/>
        /// and stores the result in the <paramref name="category"/> parameter.
        /// </summary>
        /// <param name="c">An input <see cref="char"/></param>
        /// <param name="category">Resulting <see cref="CharacterCategory"/> result</param>
        /// <returns>Returns <see langword="true"/>, if the <paramref name="category"/> resolved,
        /// and <see langword="false"/>, if not.</returns>
        private static bool _tryGetCategory(char c, 
            out CharacterCategory category)
        {
            if (DefaultCharacters.Digits.Contains(c))
            {
                category = CharacterCategory.Digits; return true;
            }
            if (DefaultCharacters.UpperCase.Contains(c) || char.IsUpper(c))
            {
                category = CharacterCategory.UpperCase; return true;
            }
            if (DefaultCharacters.LowerCase.Contains(c) || char.IsLower(c))
            {
                category = CharacterCategory.LowerCase; return true;
            }
            if (DefaultCharacters.Special.Contains(c) 
                || _categoriesSpecial.Contains(char.GetUnicodeCategory(c)))
            {
                category = CharacterCategory.Special; return true;
            }
            category = default; return false;
        }

        /// <summary>
        /// Selects input of specified <see cref="CharacterCategory"/> <paramref name="key"/> from the 
        /// <paramref name="source"/> list and stores selected in the <see cref="IEnumerable{T}"/> <paramref name="result"/>.
        /// </summary>
        /// <param name="source">Source list of <see cref="char"/>s.</param>
        /// <param name="key"><see cref="CharacterCategory"/> to select.</param>
        /// <param name="result">A <see cref="IEnumerable{T}"/> containing selected input. May be empty.</param>
        /// <returns>Returns <see langword="true"/>, if the <paramref name="result"/> contains input,
        /// and <see langword="false"/>, if not.</returns>
        private static bool _trySelectByCategory(IEnumerable<char> source, CharacterCategory key, out IEnumerable<char> result)
        {
            result = source.Where(character => _tryGetCategory(character, out CharacterCategory category) && key == category);
            return result.Any();
        }

        /// <summary>
        /// Removes character categories specified in the <paramref name="key"/> from <see cref="Items"/> as well as from <see cref="CharacterCategories"/>.
        /// The result indicates whether the dictionary <see cref="Items"/> still not empty.
        /// </summary>
        /// <param name="key"><see cref="CharacterCategory"/> to remove from <see cref="Items"/></param>
        /// <returns>If the dictionary contains any non-empty keys, returns <see langword="true"/>, otherwise <see langword="false"/></returns>
        public bool RemoveCategory(CharacterCategory key)
        {
            foreach (CharacterCategory c in key.Split())
            {
                if (Items.TryGetValue(c, out string v))
                {
                    if (Include.Any() && !string.IsNullOrEmpty(v)) Include.ExceptWith(v);
                    Items.Remove(c);
                    CharacterCategories &= c;
                }
            }
            return Items.Any();
        }

        /// <summary>
        /// Resets character categories specified in the <paramref name="key"/> in <see cref="Items"/> to default values.
        /// If some character categories were not specified previously, they will be added with default values, XML-safe or unsafe.
        /// Because this method resets character set, added characters will be excluded from the <see cref="Exclude"/> property.
        /// </summary>
        /// <param name="key"><see cref="CharacterCategory"/> to reset in the<see cref="Items"/>.</param>
        /// <param name="xmlSafe">If set to <see langword="true"/>, excludes <see cref="DefaultCharacters.XmlUnsafe"/> characters.
        public void ResetCategory(CharacterCategory key, bool xmlSafe = false)
        {
            IsXmlSafe = xmlSafe;
            foreach (CharacterCategory c in key.Split())
            {
                string v = xmlSafe ? DefaultDictionaries.DefaultXmlSafe[c] : DefaultDictionaries.Default[c];
                if (Exclude.Any()) Exclude.ExceptWith(v);
                if (Items.ContainsKey(c))
                {
                    Items[c] = v;
                }
                else
                {
                    Items.Add(c, v);
                    CharacterCategories |= c;
                }
            }
        }

        /// <summary>
        /// Tries to except chars specified in the <paramref name="excludeTo"/> from the charset <paramref name="excludeFrom"/>.
        /// </summary>
        /// <param name="excludeFrom">Character set to clear from disallowed chars.</param>
        /// <param name="excludeTo">Character set to exclude from <paramref name="excludeFrom"/>.</param>
        /// <param name="result">Resulting character set, may be empty.</param>
        /// <returns>Returns <see langword="true"/>, if the <paramref name="result"/> contains characters,
        /// and <see langword="false"/>, if not.</returns>
        private static bool TryExcept(IEnumerable<char> excludeFrom, IEnumerable<char> excludeTo, 
            out IEnumerable<char> result)
        {
            result = excludeFrom.Any() ? excludeFrom.Except(excludeTo) : Enumerable.Empty<char>();
            return result.Any();
        }

        /// <summary>
        /// Overloads the static <see cref="TryExcept(IEnumerable{char}, IEnumerable{char}, out IEnumerable{char})"/>
        /// method, using the <see cref="Exclude"/> property as second argument.
        /// </summary>
        /// <param name="excludeFrom">Character set to clear from disallowed chars.</param>
        /// <param name="result">Resulting character set, may be empty.</param>
        /// <returns>Returns <see langword="true"/>, if the <paramref name="result"/> contains characters,
        /// and <see langword="false"/>, if not.</returns>
        private bool TryExcept(IEnumerable<char> excludeFrom, out IEnumerable<char> result) 
            => TryExcept(excludeFrom, Exclude, out result);

        /// <summary>
        /// Intersects the <paramref name="intersectTo"/> list with an ethalon <paramref name="intersectWith"/>
        /// character set and stores the <paramref name="result"/>.
        /// </summary>
        /// <param name="intersectTo">Source character set.</param>
        /// <param name="intersectWith">Reference character set.</param>
        /// <param name="result">Resulting character set.</param>
        /// <returns>Returns <see langword="true"/>, if the <paramref name="result"/> contains characters,
        /// and <see langword="false"/>, if not.</returns>
        private static bool TryIntersect(IEnumerable<char> intersectTo, IEnumerable<char> intersectWith, 
            out IEnumerable<char> result)
        {
            result = intersectTo.Any() 
                ? intersectTo.Intersect(intersectWith)
                : Enumerable.Empty<char>();
            return result.Any();
        }

        /// <summary>
        /// Unused method. Combines two character sets.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private static bool TryUnion(IEnumerable<char> x, IEnumerable<char> y, out IEnumerable<char> result)
        {
            result = x.Any() ? x.Union(y) : Enumerable.Empty<char>();
            return result.Any();
        }

        /// <summary>
        /// Adds character set from <paramref name="chars"/> to the certain <paramref name="key"/>
        /// of <see cref="Items"/> dictionary. Optionally resets the value.
        /// </summary>
        /// <param name="key">An <see cref="Items"/> key.</param>
        /// <param name="chars">Character set to add.</param>
        /// <param name="clear">If set to <see langword="true"/>, previous value will be deleted.</param>
        private void AddValue(CharacterCategory key, IEnumerable<char> chars, bool clear = false)
        {
            // If after the exclusion of disallowed chars the collection is still not empty
            if (TryExcept(chars, out IEnumerable<char> selected))
            {
                // If the dictionary already contains key
                if (Items.TryGetValue(key, out string oldValue))
                {
                    Items[key] = clear 
                        ? new string(selected.ToArray()) 
                        : new string(selected.Union(oldValue).ToArray());
                }
                else
                {
                    Items.Add(key, new string(selected.ToArray()));
                    CharacterCategories |= key;
                }
            }
        }

        /// <summary>
        /// Removes characters from the <see cref="Items"/> <paramref name="key"/>.
        /// </summary>
        /// <param name="key">An <see cref="Items"/> key.</param>
        /// <param name="chars">Character set to exclude.</param>
        private void RemoveValue(CharacterCategory key, IEnumerable<char> chars)
        {
            // Dictionary contains key
            if (Items.TryGetValue(key, out string oldValue))
            {
                // The resulting value is still not empty, so replace the old one
                if (TryExcept(oldValue, out IEnumerable<char> selected))
                {
                    Items[key] = new string(selected.ToArray());
                }
                else
                {
                    // The resulting value IS empty; remove the key and unset enum bit.
                    Items.Remove(key);
                    CharacterCategories &= key;
                }
            }
        }

        /// <summary>
        /// Adds <paramref name="chars"/> to the <see cref="Items"/>, sorting by categories.
        /// </summary>
        /// <param name="chars">Character set to add.</param>
        /// <param name="clear">If set to <see langword="true"/>, previous value will be deleted.</param>
        /// <returns>Returns <see langword="true"/>, if the <see cref="Items"/> not empty.</returns>
        public bool Add(IEnumerable<char> chars, bool clear = false)
        {
            // Exclude disallowed chars
            if (TryExcept(chars, out IEnumerable<char> selected))
            {
                // Select digits
                if (TryIntersect(selected, DefaultCharacters.Digits, out IEnumerable<char> digits))
                {
                    AddValue(CharacterCategory.Digits, digits, clear);
                }
                // Select uppercase
                if (TryIntersect(selected, DefaultCharacters.UpperCase, out IEnumerable<char> upperCase))
                {
                    AddValue(CharacterCategory.UpperCase, upperCase, clear);
                }
                // Select lowercase
                if (TryIntersect(selected, DefaultCharacters.LowerCase, out IEnumerable<char> lowerCase))
                {
                    AddValue(CharacterCategory.LowerCase, lowerCase, clear);
                }
                // Select special symbols
                if (TryIntersect(selected, DefaultCharacters.Special, out IEnumerable<char> special))
                {
                    AddValue(CharacterCategory.Special, special, clear);
                }
            }
            // Returns true if the dictionary still not empty.
            return Items.Any();
        }

        /// <summary>
        /// Removes <paramref name="chars"/> from the <see cref="Items"/> dictionary.
        /// </summary>
        /// <param name="chars">Character set to remove.</param>
        /// <returns>Returns <see langword="true"/>, if the <see cref="Items"/> not empty.</returns>
        public bool Remove(IEnumerable<char> chars)
        {
            // Add values to the "Exclude" set and remove from the "Include"
            Exclude.UnionWith(chars);
            Include.ExceptWith(chars);
            // Process the dictionary keys
            foreach (CharacterCategory key in Items.Keys)
            {
                RemoveValue(key, chars);
            }
            // Returns true if the dictionary still not empty.
            return Items.Any();
        }

        /// <summary>
        /// Returns base values from the <see cref="DefaultDictionaries.DefaultXmlSafe"/> or <see cref="DefaultDictionaries.Default"/> dictionaries.
        /// </summary>
        /// <param name="key">Dictionary key, <see cref="CharacterCategory"/></param>
        /// <param name="xmlSafe">If set to <see langword="true"/>, uses XML-safe character set.</param>
        /// <returns>Character set as a <see cref="string"/></returns>
        private static string GetBaseCharacterSet(CharacterCategory key, bool xmlSafe = false)
        {
            return xmlSafe ? DefaultDictionaries.DefaultXmlSafe[key] : DefaultDictionaries.Default[key];
        }

        /// <summary>
        /// Overloads the <see cref="GetBaseCharacterSet(CharacterCategory, bool)"/> using the <see cref="IsXmlSafe"/> as a second argument.
        /// </summary>
        /// <param name="key">Dictionary key, <see cref="CharacterCategory"/></param>
        /// <returns>Character set as a <see cref="string"/></returns>
        private string GetBaseCharacterSet(CharacterCategory key) => GetBaseCharacterSet(key, IsXmlSafe);

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
            IEnumerable<char> exclude, IEnumerable<char> include)
        {
            // Clear and populate the include and exclude character sets;
            // Exclude set:
            Exclude.Clear();
            Exclude.UnionWith(exclude);
            if (xmlSafe) Exclude.UnionWith(DefaultCharacters.XmlUnsafe);
            IsXmlSafe = TryIntersect(Exclude, DefaultCharacters.XmlUnsafe, out var _);
            // Include set:
            Include.Clear();
            if (TryExcept(include, out IEnumerable<char> include2)) Include.UnionWith(include2);
            // Clear the dictionary and categories
            CharacterCategories = default;
            Items.Clear();
            // First, process the additional characters
            if (Include.Any()) _ = Add(Include);
            // Process the explicitly declared categories
            foreach (CharacterCategory key in characterCategory.Split())
            {
                // Get the source value
                if (TryExcept(GetBaseCharacterSet(key), out var result)) AddValue(key, result);
            }
            return Items.Any();
        }

        /// <summary>
        /// Overloads the <see cref="TrySetDictionary(CharacterCategory, bool, IEnumerable{char}, IEnumerable{char})"/>,
        /// but without additional character sets to include or exclude.
        /// Previous values of the <see cref="Exclude"/> and <see cref="Include"/> properties will be deleted.
        /// </summary>
        /// <param name="characterCategory"><see cref="CharacterCategory"/> to include.</param>
        /// <param name="xmlSafe">Indicates whether the dictionary must contain only XML-safe characters.</param>
        /// <returns>Returns <see langword="true"/>, if the <see cref="Items"/> dictionary is not empty.</returns>
        public bool TrySetDictionary(CharacterCategory characterCategory, bool xmlSafe = false) 
            => TrySetDictionary(characterCategory, xmlSafe, Enumerable.Empty<char>(), Enumerable.Empty<char>());

        /// <summary>
        /// The default paramless constructor.
        /// </summary>
        public DictionaryBuilder() { }

        /// <summary>
        /// Creates an instance with explicitly specified <see cref="CharacterCategories"/> to use, as well as the
        /// <see cref="Exclude"/> and <see cref="Include"/> characters, <see cref="IsXmlSafe">XML-safe</see> or not.
        /// </summary>
        /// <param name="characterCategory"><see cref="CharacterCategory"/> to include.</param>
        /// <param name="xmlSafe">Indicates whether the dictionary must contain only XML-safe characters.</param>
        /// <param name="exclude">Disallowed character set.</param>
        /// <param name="include">Additional characters to include.</param>
        public DictionaryBuilder(CharacterCategory characterCategory, bool xmlSafe, 
            IEnumerable<char> exclude, IEnumerable<char> include)
        {
            TrySetDictionary(characterCategory, xmlSafe, exclude, include);
        }

        /// <summary>
        /// Creates an instance with explicitly specified <see cref="CharacterCategories"/> to use, <see cref="IsXmlSafe">XML-safe</see> or not.
        /// No additional character sets to <see cref="Exclude"/> or <see cref="Include"/>.
        /// </summary>
        /// <param name="characterCategory"><see cref="CharacterCategory"/> to include.</param>
        /// <param name="xmlSafe">Indicates whether the dictionary must contain only XML-safe characters.</param>
        public DictionaryBuilder(CharacterCategory characterCategory, bool xmlSafe = false)
            => TrySetDictionary(characterCategory, xmlSafe, Enumerable.Empty<char>(), Enumerable.Empty<char>());

        /// <summary>
        /// Base character dictionary, created from the <see cref="DefaultDictionaries.Default"/>.
        /// </summary>
        public Dictionary<CharacterCategory, string> Items { get; private set; }
            = new Dictionary<CharacterCategory, string>(DefaultDictionaries.Default);

        /// <summary>
        /// Stores character categories to use; just for information. The value may be overridden 
        /// using the<see cref="TrySetDictionary(CharacterCategory, bool, IEnumerable{char}, IEnumerable{char})"/>,
        /// <see cref="Add(IEnumerable{char}, bool)"/>, <see cref="Remove(IEnumerable{char})"/> and other methods.
        /// </summary>
        public CharacterCategory CharacterCategories { get; private set; } 
            = CharacterCategory.Digits 
            | CharacterCategory.UpperCase 
            | CharacterCategory.LowerCase 
            | CharacterCategory.Special;

        /// <summary>
        /// Indicates whether passwords must not contain 
        /// <see cref="DefaultCharacters.XmlUnsafe"/> characters.
        /// </summary>
        public bool IsXmlSafe { get; private set; } = false;

        /// <summary>
        /// Stores additional input to exclude from password generation.
        /// </summary>
        public HashSet<char> Exclude { get; private set; } = new HashSet<char>();

        /// <summary>
        /// Stores additional input to include to password generation.
        /// </summary>
        public HashSet<char> Include { get; private set; } = new HashSet<char>();
    }
}
