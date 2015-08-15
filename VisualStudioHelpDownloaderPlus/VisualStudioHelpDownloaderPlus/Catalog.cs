namespace VisualStudioHelpDownloaderPlus
{
    /// <summary>
    /// The locale.
    /// </summary>
    public sealed class Catalog
    {
        /// <summary>
        ///  Gets the normalized catalog name display.
        /// </summary>
        public string DisplayName
        {
            get
            {
                return NormalizeCatalog(Name);
            }
        }

        /// <summary>
        ///   Gets or sets the catalog name.
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        ///   Gets or sets the catalog description.
        /// </summary>
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the relative location of the catalog page associated with the catalog-link
        /// </summary>
        public string Catalog_Link
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
            return Name;
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
            return value;

            if (null == value)
            {
                return string.Empty;
            }

            string displayValue;
            if (value == "VisualStudio11")
                displayValue = "Visual Studio 2012";
            else if (value == "VisualStudio12")
                displayValue = "Visual Studio 2013";
			else if (value == "VisualStudio14" || value == "dev14")
                displayValue = "Visual Studio 2015";
            else
                displayValue = string.Empty;

            return displayValue;
        }
    }
}