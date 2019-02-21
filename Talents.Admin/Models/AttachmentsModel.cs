using System.ComponentModel.DataAnnotations;

namespace TheSite.Models
{

	public class AttachmentsViewModel
	{

		public string Name { get; set; }

		public string Url { get; set; }

	}


	public class AttachmentsDataModel
	{

		public long JoinId { get; set; }

		public long UserId { get; set; }

		public string Type { get; set; }

		public string Name { get; set; }

		public string Url { get; set; }

	}

}