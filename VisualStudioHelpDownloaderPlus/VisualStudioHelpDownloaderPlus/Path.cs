using System;

namespace VisualStudioHelpDownloaderPlus
{
    /// <summary>
    ///     Represents an MSDN path
    /// </summary>
    internal sealed class MsdnPath : IComparable<MsdnPath>
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

        public int CompareTo(MsdnPath other)
        {
            if (null == other)
            {
            return 1;
            }

            return String.Compare(SkuName, other.SkuName, StringComparison.OrdinalIgnoreCase);
            //return SkuName.CompareTo(other.SkuName);
        }

    }
}
