namespace VisualStudioHelpDownloaderPlus
{
	using System;
	using System.Globalization;

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
	internal sealed class Package
	{
		/// <summary>
		///     Gets or sets the packageType with the package, "packageType".
		/// </summary>
		public string PackageType
		{
			get;
			set;
		}

		/// <summary>
		///     Gets or sets the packageFormat with the package, "packageFormat"
		/// </summary>
		public string PackageFormat
		{
			get;
			set;
		}

		/// <summary>
		///     Gets or sets the package name, "name".
		/// </summary>
		public string Name
		{
			get;
			set;
		}

		/// <summary>
		///     Gets or sets the deployed with the package, "deployed".
		/// </summary>
		public string Deployed
		{
			get;
			set;
		}

        /// <summary>
		///     Gets or sets the last modified time, "last-modified".
		/// </summary>
		public DateTime LastModified
		{
			get;
			set;
		}

		/// <summary>
		///     Gets or sets the tag associated with the package, "package-etag".
		/// </summary>
		public string Tag
		{
			get;
			set;
		}

		/// <summary>
		///     Gets or sets the package relative URL for downloading, "current-link".
		/// </summary>
		public string Link
		{
			get;
			set;
		}

		/// <summary>
		///     Gets or sets the size in bytes, "package-size-bytes".
		/// </summary>
		public long Size
		{
			get;
			set;
		}

		/// <summary>
		///     Gets or sets the size in bytes, "package-size-bytes-uncompressed".
		/// </summary>
        public long UncompressedSize
		{
			get;
			set;
		}

		/// <summary>
		///     Gets or sets the package constituent URL, "package-constituent-link".
		/// </summary>
		public string ConstituentLink
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
		/// Create a file name for the package file
		/// </summary>
		/// <returns>
		/// A string containing the file name
		/// </returns>
        public string CreateFileName()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}({1}).cab", Name, Tag);
            //return string.Format(CultureInfo.InvariantCulture, "{0}({1}).cab", Name.ToLowerInvariant(), Tag);
        }

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
