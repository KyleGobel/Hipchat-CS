namespace HipchatApiV2.Enums
{
	/// <summary>
	/// Type of the card.
	/// </summary>
	public enum CardStyle
	{
		/// <summary>
		/// Use file cards to send files.
		/// </summary>
		File,

		/// <summary>
		/// Use image cards to send images.
		/// </summary>
		Image,

		/// <summary>
		/// Use application cards to send information about an application object.
		/// </summary>
		Application,

		/// <summary>
		/// Use link cards to send information about a web page with content.
		/// </summary>
		Link,

		/// <summary>
		/// Use media card to send content which should be open in HipChat's media viewer.
		/// </summary>
		Media
	}
}
