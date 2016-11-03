using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HipchatApiV2.Enums;
namespace HipchatApiV2.Models
{
	/// <summary>
	/// An attribute value shown in the card.
	/// </summary>
	public class CardAttributeValue
	{
		/// <summary>
		/// Url to be opened when a user clicks on the label.
		/// </summary>
		public string Url { get; set; }

		/// <summary>
		/// The text representation of the value.
		/// </summary>
		public string Label { get; set; }

		/// <summary>
		/// AUI Integrations.
		/// </summary>
		public CardAttributeValueStyle Style { get; set; }

		/// <summary>
		/// The icon to be shown in the attribute.
		/// </summary>
		public Icon Icon { get; set; }
	}
}
