namespace VisualStudioHelpDownloaderPlus
{
	using System.Globalization;

	/// <summary>
	///     Base class for container items.
	/// </summary>
	internal abstract class ItemBase
	{
		/// <summary>
		///   Gets or sets the Item code (for Uri combination).
		/// </summary>
		public string Code
		{
			get;
			set;
		}

		/// <summary>
		///     Gets or sets the item name.
		/// </summary>
		public string Name
		{
			get;
			set;
		}

		/// <summary>
        ///     Gets or sets the item description.
		/// </summary>
        public string Description
		{
			get;
			set;
		}

        /// <summary>
        ///     Gets or sets the item vendor.
        /// </summary>
        public string Vendor
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
        //public override string ToString()
        //{
        //    return string.Format( CultureInfo.InvariantCulture, "{0} [{1}]", Name ?? "NULL", Locale_c.Name ?? "NULL" );
        //}
	}
}
