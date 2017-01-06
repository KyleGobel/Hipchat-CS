namespace HipchatApiV2.Models
{
	/// <summary>
	/// The thumbnail to be shown in the card.
	/// </summary>
	public class CardThumbnail
	{
		/// <summary>
		/// The thumbnail url.
		/// </summary>
		public string Url { get; set; }

		/// <summary>
		/// The thumbnail url in retina.
		/// </summary>
		public string Url2 { get; set; }

		/// <summary>
		/// The original width of the image.
		/// </summary>
		public int Width { get; set; }

		/// <summary>
		/// The original height of the image.
		/// </summary>
		public int Height { get; set; }
	}
}
