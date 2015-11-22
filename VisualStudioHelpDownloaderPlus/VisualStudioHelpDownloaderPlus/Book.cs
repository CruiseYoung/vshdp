namespace VisualStudioHelpDownloaderPlus
{
    using System;
    using System.Globalization;
    using System.Collections.Generic;

    /// <summary>
    /// Represents an MSDN book
    /// </summary>
    internal sealed class Book : IEquatable<Book>, IComparable<Book>
    {
    	/// <summary>
		/// Gets or sets the id
		/// </summary>
		public string Id
		{
			get;
			set;
		}

        /// <summary>
        /// Gets or sets the locale
        /// </summary>
        public string Locale
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the name
		/// </summary>
		public string Name
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the description
		/// </summary>
		public string Description
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the BrandingPackageName
		/// </summary>
		public string BrandingPackageName
		{
			get;
			set;
		}
		/// <summary>
		///    Gets or sets the collection of packages associated with the path
		/// </summary>
        public ICollection<MSDNPath> Paths
		{
			get;
			set;
		}

        /// <summary>
        ///    Gets or sets the collection of packages associated with the book
        /// </summary>
        public string PackagesBeforeContext
        {
            get;
            set;
        }

		/// <summary>
		///    Gets or sets the collection of packages associated with the book
		/// </summary>
		public ICollection<Package> Packages
		{
			get;
			set;
		}

        /// <summary>
        /// Returns a string representing the object
        /// </summary>
        /// <returns>
        /// 
        /// </returns>
        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// Gets or sets the display category for the book
        /// </summary>
        public string Category
        {
            get;
            set;
        }

		/// <summary>
		/// Gets or sets a value indicating whether the a download of the book has been requested.
		/// </summary>
        public bool Wanted
        {
            get;
            set;
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
            string retval = string.Empty;
            if (Locale.ToLowerInvariant() == "en-us")
                retval = string.Format(CultureInfo.InvariantCulture, "book-{0}.html", Id);
            else
                retval = string.Format(CultureInfo.InvariantCulture, "book-{0}({1}).html", Id, Locale.ToLowerInvariant());
            return retval;
            //return string.Format(CultureInfo.InvariantCulture, "book-{0}.html", Id);
        }


        public bool Equals(Book other)
        {
            if (other == null)
                return false;

            return Id.ToLowerInvariant().Equals(other.Id.ToLowerInvariant());
        }

        public int CompareTo(Book other)
        {
            if (null == other)
            {
                return 1;
            }

            return string.Compare(Name, other.Name, true);
            //return Name.CompareTo(other.Name);
        }

    }
}
