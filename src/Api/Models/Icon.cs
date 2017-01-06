using System.Runtime.Serialization;

namespace HipchatApiV2.Models
{
	/// <summary>
	/// An Icon to be used in the Hipchat Api.
	/// </summary>
	[DataContract]
	public class Icon
	{
		/// <summary>
		/// The url where the icon is.
		/// </summary>
		[DataMember]
		public string Url { get; set; }

		/// <summary>
		/// The url for the icon in retina.
		/// </summary>
		[DataMember(Name = "Url@2x")]
		public string Url2 { get; set; }
	}
}
