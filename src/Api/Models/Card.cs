using System.Collections.Generic;
using HipchatApiV2.Enums;

namespace HipchatApiV2.Models
{
	/// <summary>
	/// An optional card object.
	/// May be null.
	/// </summary>
	public class Card
	{
		/// <summary>
		/// Type of the card.
		/// </summary>
		public CardStyle Style { get; set; }

		/// <summary>
		/// The url the card will open when clicked on.
		/// </summary>
		public string Url { get; set; }

		/// <summary>
		/// The title of the card.
		/// </summary>
		public string Title { get; set; }

		/// <summary>
		/// An id that will help Hipchat recognise the same card when it is sent multiple times.
		/// </summary>
		public string Id { get; set; }

		/// <summary>
		/// The description of the card.
		/// </summary>
		public CardDescription Description { get; set; }

		/// <summary>
		/// The icon to be shown in the card.
		/// </summary>
		public Icon Icon { get; set; }

		/// <summary>
		/// The format of the card.
		/// </summary>
		public CardFormat Format { get; set; }

		/// <summary>
		/// The thumbnail to be shown in the card.
		/// </summary>
		public CardThumbnail Thumbnail { get; set; }

		/// <summary>
		/// The activity will generate a collapsable card of one line showing the html and the ability to maximize to see all the content.
		/// </summary>
		public CardActivity Activity { get; set; }

		/// <summary>
		/// List of attributes to show below the card. Sample {label}:{value.icon} {value.label}.
		/// </summary>
		public List<CardAttribute> Attributes { get; set; }
	}
}
