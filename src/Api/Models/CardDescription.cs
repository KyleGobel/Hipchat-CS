using HipchatApiV2.Enums;

namespace HipchatApiV2.Models
{
	/// <summary>
	/// The description of the card.
	/// </summary>
	public class CardDescription
	{
		/// <summary>
		/// The format of the description.
		/// </summary>
		public HipchatMessageFormat Format { get; set; }

		/// <summary>
		/// The description in the specific format.
		/// </summary>
		public string Value { get; set; }
	}
}
