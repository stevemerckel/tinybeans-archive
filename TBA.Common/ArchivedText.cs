using System;

namespace TBA.Common
{
    /// <summary>
    /// Representation of the text archive
    /// </summary>
    public sealed class ArchivedText : ArchivedContent
    {
        /// <summary>
        /// Default ctor
        /// </summary>
        /// <param name="id">The unique archive id</param>
        /// <param name="textValue">The text value stored</param>
        /// <param name="displayedOn">The date that this was displayed on</param>
        /// <param name="orderDisplayed">The order-position this was shown on <seealso cref="DisplayedOn"/> date</param>
        public ArchivedText(string id, string textValue, DateTime displayedOn, int orderDisplayed) : base(id, displayedOn, orderDisplayed, ArchiveType.Text)
        {
            TextValue = textValue;
        }

        /// <summary>
        /// The text archive entry
        /// </summary>
        public string TextValue { get; private set; }
    }
}