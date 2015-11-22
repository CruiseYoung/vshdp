namespace VisualStudioHelpDownloaderPlus
{
    using System;
    using System.Globalization;
    using System.Collections.Generic;
    
    /// <summary>
    /// The catalog-locale.
    /// </summary>
    public sealed class CatalogLocale : IEquatable<CatalogLocale>, IComparable<CatalogLocale>
    {
        /// <summary>
        /// Gets or sets the locale.
        /// </summary>
        public string Locale
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the catalog-name.
        /// </summary>
        public string CatalogName
        {
            get;
            set;
        }

        /// <summary>
        ///   Gets or sets the description.
        /// </summary>
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        ///   Gets or sets the locale-link.
        /// </summary>
        public string LocaleLink
        {
            get;
            set;
        }

        /// <summary>
        /// Returns a string representing the object
        /// </summary>
        /// <returns>
        /// The Name of the locale
        /// </returns>
        public override string ToString()
        {
            return Locale;
        }

        /// <summary>
        /// Gets or sets the last-modified
        /// </summary>
        public DateTime LastModified
        {
            get;
            set;
        }

        /// <summary>
        /// Create a file name
        /// </summary>
        /// <returns>
        /// A string containing the file name
        /// </returns>
        public string CreateFileName()
        {
            return string.Format(CultureInfo.InvariantCulture, "({0})HelpContentSetup.msha", Locale);
        }

        /// <summary>
        /// Create a file name
        /// </summary>
        /// <returns>
        /// A string containing the file name
        /// </returns>
        public string CreatePackageListFileName()
        {
            return string.Format(CultureInfo.InvariantCulture, "({0})PackageList.txt", Locale);
        }


        public bool Equals(CatalogLocale other)
        {
            if (other == null)
                return false;

            return Locale.ToLowerInvariant().Equals(other.Locale.ToLowerInvariant());
        }

        public int CompareTo(CatalogLocale other)
        {
            if (null == other)
            {
                return 1;
            }

            return string.Compare(Locale, other.Locale, true);
            //return Locale.CompareTo(other.Locale); ;
        }
    }
}
