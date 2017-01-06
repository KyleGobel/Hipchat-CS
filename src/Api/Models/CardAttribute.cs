namespace HipchatApiV2.Models
{
	/// <summary>
	/// List of attributes to show below the card.
	/// Sample: {label}:{value.icon} {value.label}
	/// </summary>
	public class CardAttribute
	{
		/// <summary>
		/// The value of the attribute.
		/// </summary>
		public CardAttributeValue Value { get; set; }

		/// <summary>
		/// The label of the attribute.
		/// </summary>
		public string Label { get; set; }
	}
}
