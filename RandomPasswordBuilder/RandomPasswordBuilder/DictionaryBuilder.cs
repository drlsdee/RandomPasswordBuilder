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
        /// <param name="category">Resulting <see cref="CharacterCategory"/> value</param>
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
        /// Selects chars of specified <see cref="CharacterCategory"/> <paramref name="key"/> from the 
        /// <paramref name="source"/> list and stores selected in the <see cref="IEnumerable{T}"/> <paramref name="result"/>.
        /// </summary>
        /// <param name="source">Source list of <see cref="char"/>s.</param>
        /// <param name="key"><see cref="CharacterCategory"/> to select.</param>
        /// <param name="result">A <see cref="IEnumerable{T}"/> containing selected chars. May be empty.</param>
        /// <returns>Returns <see langword="true"/>, if the <paramref name="result"/> contains chars,
        /// and <see langword="false"/>, if not.</returns>
        private static bool _trySelectByCategory(IEnumerable<char> source, CharacterCategory key, out IEnumerable<char> result)
        {
            result = source.Where(character => _tryGetCategory(character, out CharacterCategory category) && key == category);
            return result.Any();
        }

        /// <summary>
        /// Takes the source <see cref="string"/> <paramref name="value"/>;
        /// optionally includes characters from the <paramref name="include"/> list,
        /// selected by the <paramref name="key"/> <see cref="CharacterCategory"/>;
        /// optionally excludes characters from the <paramref name="exclude"/> list;
        /// finally creates a <see cref="string"/> result and returns <see langword="true"/> or <see langword="false"/>.
        /// </summary>
        /// <param name="value">The source <see cref="string"/></param>
        /// <param name="key">The <see cref="CharacterCategory"/> to select. It assumes that 
        /// the source <paramref name="value"/> always contains only chars of this category.</param>
        /// <param name="exclude">An optional character set to exclude.</param>
        /// <param name="include">An optional character set to include.</param>
        /// <param name="result">Output <see cref="string"/></param>
        /// <returns>Returns <see langword="true"/>, if the <paramref name="result"/> is not empty;
        /// otherwise returns <see langword="false"/>.</returns>
        private bool _tryGetNewValue(string value, CharacterCategory key, 
            IEnumerable<char> exclude, IEnumerable<char> include, out string result)
        {
            result = _trySelectByCategory(include, key, out IEnumerable<char> select)
                ? new string(value.Union(select).Except(exclude).ToArray())
                : new string(value.Except(exclude).ToArray());
            return !string.IsNullOrEmpty(value);
        }

        /// <summary>
        /// Populates the <see cref="Items"/> dictionary with keys and values, optionally including or excluding additional characters.
        /// If the dictionary contains any non-empty keys, returns <see langword="true"/>, otherwise <see langword="false"/>
        /// </summary>
        /// <param name="characterCategory"><see cref="CharacterCategories"/> to use.
        /// If the <paramref name="exclude"/> and/or <paramref name="include"/> parameters are specified,
        /// the value of this parameter may be ignored: if the <see cref="Exclude"/> list contains 
        /// whole <see cref="CharacterCategory"/>, the property will be overwritten.</param>
        /// <param name="xmlSafe">If set to <see langword="true"/>, excludes <see cref="DefaultCharacters.XmlUnsafe"/> characters.
        /// This value may be overridden by the <paramref name="exclude"/> parameter.</param>
        /// <param name="exclude">An extra set of characters to exclude. May override the <paramref name="xmlSafe"/> parameter.
        /// If the <see cref="Exclude"/> list contains whole <see cref="CharacterCategory"/>, 
        /// the <see cref="CharacterCategories"/> property will be overwritten.</param>
        /// <param name="include">An extra set of characters to include. If this set contains any chars of
        /// <see cref="CharacterCategory"/> not listed in the <paramref name="characterCategory"/> parameter,
        /// this category will be added to <see cref="CharacterCategories"/> property.
        /// But the <paramref name="exclude"/> parameter has higher priority, 
        /// so if the <paramref name="exclude"/> and <paramref name="include"/> lists overlap, 
        /// the items common to both will be removed from the <paramref name="include"/> list.</param>
        /// <returns>If the dictionary contains any non-empty keys, returns <see langword="true"/>, otherwise <see langword="false"/></returns>
        public bool TrySetDictionary(CharacterCategory characterCategory, bool xmlSafe, 
            IEnumerable<char> exclude, IEnumerable<char> include)
        {
            CharacterCategories = characterCategory;
            // Clear sets
            Exclude.Clear();
            Include.Clear();
            // If set is specified, add to the initial value
            if (exclude.Any()) Exclude.UnionWith(exclude);
            // If the flag is set explicitly, add chars to exclude
            if (xmlSafe)
            {
                IsXmlSafe = true;
                Exclude.UnionWith(DefaultCharacters.XmlUnsafe);
            }
            // Or detect if the exclude charset contains ALL XML-unsafe chars
            else IsXmlSafe = Exclude.IsSupersetOf(DefaultCharacters.XmlUnsafe);
            // Additional chars to include
            if (include.Any()) Include.UnionWith(include);
            // Remove explicitly disallowed characters
            if (Exclude.Any()) Include.ExceptWith(Exclude);
            // Set the initial dictionary
            Items = IsXmlSafe ? DefaultDictionaries.DefaultXmlSafe 
                : DefaultDictionaries.Default;
            // Add or remove chars and characterCategory
            foreach (CharacterCategory key in Items.Keys)
            {
                // There are no chars to include or exclude, so look only at the category
                if (!Include.Any() && !Exclude.Any())
                {
                    if (!CharacterCategories.HasFlagUnsafe(key))
                    {
                        // The category is disallowed, remove the key
                        Items.Remove(key);
                    }
                }
                // There ARE additional chars to include or/and exclude
                else
                {
                    // Explicitly specified additional character sets have higher priority than the CharacterCategory flag
                    if (_tryGetNewValue(Items[key], key, Exclude, Include, out string value))
                    {
                        Items[key] = value;
                    }
                    else
                    {
                        Items.Remove(key);
                        CharacterCategories &= key;
                    }
                }
            }
            // Return the result
            return Items.Any();
        }

        /// <summary>
        /// An overload for the <see cref="TrySetDictionary(CharacterCategory, bool, IEnumerable{char}, IEnumerable{char})"/> method,
        /// populating an <see cref="Items"/> dictionary with default values without extra character sets.
        /// The <see cref="Include"/> and <see cref="Exclude"/> properties are cleared.
        /// </summary>
        /// <param name="characterCategory"><see cref="CharacterCategories"/> to use.
        /// <param name="xmlSafe">If set to <see langword="true"/>, excludes <see cref="DefaultCharacters.XmlUnsafe"/> characters.</param>
        /// <returns>If the dictionary contains any non-empty keys, returns <see langword="true"/>, otherwise <see langword="false"/></returns>
        public bool TrySetDictionary(CharacterCategory characterCategory, bool xmlSafe = false) 
            => TrySetDictionary(characterCategory, xmlSafe, new List<char>(), new List<char>());

        /// <summary>
        /// Base character dictionary.
        /// </summary>
        public Dictionary<CharacterCategory, string> Items { get; private set; }
            = DefaultDictionaries.Default;

        /// <summary>
        /// Stores character categories to use; just for information.
        /// </summary>
        public CharacterCategory CharacterCategories { get; private set; }

        /// <summary>
        /// Indicates whether passwords must not contain 
        /// <see cref="DefaultCharacters.XmlUnsafe"/> characters.
        /// </summary>
        public bool IsXmlSafe { get; private set; } = false;

        /// <summary>
        /// Stores additional chars to exclude from password generation.
        /// </summary>
        public HashSet<char> Exclude { get; private set; } = new HashSet<char>();

        /// <summary>
        /// Stores additional chars to include to password generation.
        /// </summary>
        public HashSet<char> Include { get; private set; } = new HashSet<char>();
    }
}
