
using System;
using System.Collections;
using System.Globalization;

namespace VisualStudioHelpDownloaderPlus
{
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
        /// Gets or sets the package name
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
        /// Gets or sets the last modified time.
        /// </summary>
        public DateTime LastModified
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the etag associated with the package
        /// </summary>
        public string PackageEtag
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the package relative URL for downloading.
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
        ///  Gets or sets the package-size in bytes
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
        /// Create a file name for the package file
        /// </summary>
        /// <returns>
        /// A string containing the file name
        /// </returns>
        public string CreateFileName()
        {
            return PackageEtag != null ? string.Format(CultureInfo.InvariantCulture, "{0}({1}).cab", Name, PackageEtag) : string.Format(CultureInfo.InvariantCulture, "{0}.cab", Name);
        }

        /// <summary>
        /// Create a file name
        /// </summary>
        /// <returns>
        /// A string containing the file name
        /// </returns>
        public string CreateFileNameUri()
        {
            return PackageEtag != null ? string.Format(CultureInfo.InvariantCulture, "{0}({1}).cab", System.Uri.EscapeDataString(Name), PackageEtag) : string.Format(CultureInfo.InvariantCulture, "{0}.cab", System.Uri.EscapeDataString(Name));
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
            int val;
            if (null == other)
            {
                val = 1;
                return val;
            }

            int idxThis = Name.LastIndexOf('_');
            int idxOther = other.Name.LastIndexOf('_');

            if (idxThis == -1 || idxOther == -1)
                val = String.Compare(Name, other.Name, StringComparison.OrdinalIgnoreCase);
                //val = Name.CompareTo(other.Name);
            else
            {
                int pkgNoThis;
                int pkgNoOther;
                bool resultThis = Int32.TryParse(Name.Substring(idxThis + 1), out pkgNoThis);
                bool resultOther = Int32.TryParse(other.Name.Substring(idxOther + 1), out pkgNoOther);

                if (!resultThis || !resultOther)
                    val = String.Compare(Name, other.Name, StringComparison.OrdinalIgnoreCase);
                else if ((val = String.Compare(Name.Substring(0, idxThis), other.Name.Substring(0, idxOther), StringComparison.OrdinalIgnoreCase)) == 0
                    && (val = Comparer.Default.Compare(pkgNoThis, pkgNoOther)) == 0)
                { }
            }

            return val;
        }
    }
}
