namespace VisualStudioHelpDownloaderPlus
{
    using System;
    using System.Globalization;

    /// <summary>
    ///     Represents an MSDN Path
    /// </summary>
    internal sealed class MSDNPath
    {
        /// <summary>
        ///     Gets or sets the languages.
        /// </summary>
        public string Languages
        {
            get;
            set;
        }

        /// <summary>
        ///     Gets or sets the membership
        /// </summary>
        public string Membership
        {
            get;
            set;
        }

        /// <summary>
        ///     Gets or sets the package name
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        ///     Gets or sets the priority.
        /// </summary>
        public long Priority
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the size in skuId.
        /// </summary>
        public long SkuId
        {
            get;
            set;
        }

        /// <summary>
        ///     Gets or sets the skuName.
        /// </summary>
        public string SkuName
        {
            get;
            set;
        }

        /// <summary>
        /// Create a file name for the package file
        /// </summary>
        /// <returns>
        /// A string containing the file name
        /// </returns>
        //public string CreateFileName()
        //{
        //    return string.Format(CultureInfo.InvariantCulture, "{0}({1}).cab", Name.ToLowerInvariant(), Tag);
        //}

        /// <summary>
        /// Returns a string representing the object
        /// </summary>
        /// <returns>
        /// String representing the object
        /// </returns>
        public override string ToString()
        {
            return Name ?? "NULL";
        }

    }
}
