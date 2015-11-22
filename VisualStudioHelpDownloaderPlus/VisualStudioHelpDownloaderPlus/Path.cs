namespace VisualStudioHelpDownloaderPlus
{
    using System;
    using System.Globalization;

    /// <summary>
    ///     Represents an MSDN path
    /// </summary>
    internal sealed class MSDNPath : IComparable<MSDNPath>
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
        ///     Gets or sets the name
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
        /// Gets or sets the skuId.
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
        /// Returns a string representing the object
        /// </summary>
        /// <returns>
        /// String representing the object
        /// </returns>
        public override string ToString()
        {
            return Name /*?? "NULL"*/;
        }

        public int CompareTo(MSDNPath other)
        {
            if (null == other)
            {
                return 1;
            }

            return string.Compare(SkuName, other.SkuName, true);
            //return SkuName.CompareTo(other.SkuName);
        }

    }
}
