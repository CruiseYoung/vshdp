
namespace VisualStudioHelpDownloaderPlus
{
	using System;
	using System.Globalization;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// The possible download states for a package
    /// </summary>
    public enum PackageState
	{
		/// <summary>
		/// The package has not been downloaded yet
		/// </summary>
		NotDownloaded, 

		/// <summary>
		/// The package has been previously downloaded but is now out of date
		/// </summary>
		OutOfDate, 

		/// <summary>
		/// The package has been downloaded and is up to date
		/// </summary>
		Ready
	}

	/// <summary>
	///     Represents an MSDN package
	/// </summary>
	internal sealed class Package : IEquatable<Package>, IComparable<Package>
    {
        /// <summary>
        /// Gets or sets the packageType.
        /// </summary>
        public string PackageType
		{
			get;
			set;
		}

        /// <summary>
        /// Gets or sets the packageFormat
        /// </summary>
        public string PackageFormat
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
        /// Gets or sets the deployed
        /// </summary>
        public string DeployedBeforeContext
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the deployed
        /// </summary>
        public string Deployed
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
        /// Gets or sets the package-etag
        /// </summary>
        public string PackageEtag
		{
			get;
			set;
		}

        /// <summary>
        /// Gets or sets the current-link
        /// </summary>
        public string CurrentLink
		{
			get;
			set;
		}

        /// <summary>
        /// Gets or sets the current-link
        /// </summary>
        public string CurrentLinkContext
        {
            get;
            set;
        }

        /// <summary>
        ///  Gets or sets the package-size-bytes
        /// </summary>
        public long PackageSizeBytes
		{
			get;
			set;
		}

        /// <summary>
        /// Gets or sets the package-size-bytes-uncompressed
        /// </summary>
        public long PackageSizeBytesUncompressed
		{
			get;
			set;
		}

        /// <summary>
        /// Gets or sets the package-constituent-link
        /// </summary>
        public string ConstituentLinkBeforeContext
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the package-constituent-link
        /// </summary>
        public string PackageConstituentLink
        {
			get;
			set;
		}

        /// <summary>
        /// Gets or sets the package-constituent-link
        /// </summary>
        public string PackageConstituentLinkContext
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the package-constituent-link
        /// </summary>
        public string ConstituentLinkAfterContext
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the state.
        /// </summary>
        public PackageState State
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
            string fileName = null;
            if(PackageEtag != null)
                fileName = string.Format(CultureInfo.InvariantCulture, "{0}({1}).cab", Name, PackageEtag);
            else
                fileName = string.Format(CultureInfo.InvariantCulture, "{0}.cab", Name);
            return fileName;
        }

		/// <summary>
		/// Returns a string representing the object
		/// </summary>
		/// <returns>
		/// String representing the object
		/// </returns>
		public override string ToString()
		{
			return CurrentLinkContext /*?? "NULL"*/;
        }

        public bool Equals(Package other)
        {
            if (other == null)
                return false;

            return CurrentLink.ToLowerInvariant().Equals(other.CurrentLink.ToLowerInvariant());
        }

        public int CompareTo(Package other)
        {
            int val = 0;
            if (null == other)
            {
                val = 1;
                return val;
            }

            int idx_this = Name.LastIndexOf('_');
            int idx_other = other.Name.LastIndexOf('_');

            if (idx_this == -1 || idx_other == -1)
                val = string.Compare(Name, other.Name, true);
                //val = Name.CompareTo(other.Name);
            else 
            {
                int pkgNo_this = 0;
                int pkgNo_other = 0;
                bool result_this = Int32.TryParse(Name.Substring(idx_this + 1), out pkgNo_this);
                bool result_other = Int32.TryParse(other.Name.Substring(idx_other + 1), out pkgNo_other);

                if(!result_this || !result_other)
                    val = string.Compare(Name, other.Name, true);
                else if ((val = string.Compare(Name.Substring(0, idx_this), other.Name.Substring(0, idx_other), true)) == 0
                    && (val = Comparer.Default.Compare(pkgNo_this, pkgNo_other)) == 0)
                { }
            }

            return val;
        }
    }
}
