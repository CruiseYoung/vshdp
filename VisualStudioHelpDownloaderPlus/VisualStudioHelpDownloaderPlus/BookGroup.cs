using System;
using System.Collections.Generic;
using System.Globalization;

namespace VisualStudioHelpDownloaderPlus
{
    /// <summary>
    ///     Represents an MSDN book-group
    /// </summary>
    internal sealed class BookGroup : IEquatable<BookGroup>, IComparable<BookGroup>
    {
        /// <summary>
        ///   Gets or sets the id.
        /// </summary>
        public string Id
        {
            get;
            set;
        }

        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        ///     Gets or sets the vendor.
        /// </summary>
        public string Vendor
        {
            get;
            set;
        }

        /// <summary>
        ///     Gets or sets the books associated with the book group
        /// </summary>
        public ICollection<Book> Books
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
            return Name/*?? "NULL"*/;
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
        /// Create a file name for the book group index file
        /// </summary>
        /// <returns>
        /// A string containing the file name
        /// </returns>
        public string CreateFileName()
        {
            string retval = null;
            foreach (var book in Books)
            {
                if (book.Locale.ToLowerInvariant() != "en-us")
                {
                    retval = string.Format(CultureInfo.InvariantCulture, "product-{0}({1}).html", Id, book.Locale.ToLowerInvariant());
                    break;
                }
            }

            return retval ?? (string.Format(CultureInfo.InvariantCulture, "product-{0}.html", Id));
        }

        public bool Equals(BookGroup other)
        {
            if (other == null)
                return false;

            return Id.ToLowerInvariant().Equals(other.Id.ToLowerInvariant());
        }

        public int CompareTo(BookGroup other)
        {
            if (null == other)
            {
                return 1;
            }

            return String.Compare(Name, other.Name, StringComparison.OrdinalIgnoreCase);
            //return Name.CompareTo(other.Name); ;
        }
    }
}
