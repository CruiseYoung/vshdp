
using System;
using System.Collections;

namespace VisualStudioHelpDownloaderPlus
{
    /// <summary>
    /// The catalog.
    /// </summary>
    public sealed class Catalog : IEquatable<Catalog>, IComparable<Catalog>
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the catalog-link.
        /// </summary>
        public string CatalogLink
        {
            get;
            set;
        }

        /// <summary>
        ///  Gets the normalized catalog name display.
        /// </summary>
        public string DisplayName => NormalizeCatalog(Name);

        /// <summary>
        /// Returns a string representing the object
        /// </summary>
        /// <returns>
        /// The Name of the locale
        /// </returns>
        public override string ToString()
        {
            //return Name;
            return DisplayName;
        }

        /// <summary>
        /// Returns a string representing the object
        /// </summary>
        /// <returns>
        /// The Name of the locale
        /// </returns>
        public override bool Equals(Object obj)
        {
            Catalog personObj = obj as Catalog;
            if (personObj == null)
                return false;
            return Name.Equals(personObj.Name);
        }

        public bool Equals(Catalog other)
        {
            if (other == null)
                return false;

            return Name.ToLowerInvariant().Equals(other.Name.ToLowerInvariant());
        }

        public int CompareTo(Catalog other)
        {
            if (null == other)
            {
                return 1;
            }

            int catalogNoThis = 0;
            int catalogNoOther = 0;
            bool resultThis = false;
            bool resultOther = false;

            if (Name.Length > 2)
                resultThis = Int32.TryParse(Name.Substring(Name.Length - 2), out catalogNoThis);
            if (other.Name.Length > 2)
                resultOther = Int32.TryParse(other.Name.Substring(other.Name.Length - 2), out catalogNoOther);

            int val;
            if (!resultThis || !resultOther)
                val = String.Compare(Name, other.Name, StringComparison.OrdinalIgnoreCase);
            else if ((val = Comparer.Default.Compare(catalogNoThis, catalogNoOther)) == 0
                && (val = String.Compare(Name.Substring(0, Name.Length - 2), other.Name.Substring(0, other.Name.Length - 2), StringComparison.OrdinalIgnoreCase)) == 0)
            { }

            return val;
            //return string.Compare(Name, other.Name, true);
            //return Name.CompareTo(other.Name); ;
        }

        /// <summary>
        /// Returns a string representing the object
        /// </summary>
        /// <returns>
        /// The Name of the locale
        /// </returns>
        public override int GetHashCode()
        {
            return NormalizeCatalog(Name).GetHashCode();
        }

        /// <summary>
        /// Normalizes the locale code 
        /// </summary>
        /// <param name="value">
        /// The locale code to normalize
        /// </param>
        /// <returns>
        /// The normalized locale code
        /// </returns>
        private static string NormalizeCatalog(string value)
        {
            //return value;

            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            string displayValue;
            if (value == "VisualStudio10" || value == "dev10")
                displayValue = "Visual Studio 2010";
            else if (value == "VisualStudio11")
                displayValue = "Visual Studio 2012";
            else if (value == "VisualStudio12")
                displayValue = "Visual Studio 2013";
            else if (value == "VisualStudio14" || value == "dev14")
                displayValue = "Visual Studio 2015";
            else if (value == "VisualStudio15" || value == "dev15")
                displayValue = "Visual Studio 2017";
            else
                displayValue = value;

            return displayValue;
        }
    }
}