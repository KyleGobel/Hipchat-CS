namespace HipchatApiV2.Models
{
	/// <summary>
	/// The activity will generate a collapsable card of one line showing the html and the ability to maximize to see al the content.
	/// </summary>
	public class CardActivity
	{
		/// <summary>
		/// Html for the activity to show in one line a summary of the action that happened
		/// </summary>
		public string Html { get; set; }

		/// <summary>
		/// An icon to be shown in the summary of the activity.
		/// </summary>
		public Icon Icon { get; set; }
	}
}
