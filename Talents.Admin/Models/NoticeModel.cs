using Business;
using System.ComponentModel.DataAnnotations;

namespace TheSite.Models
{

	public class NoticeModel : Notice
	{

		[Display(Name = "附件名称")]
		public string AttachmentName { get; set; }

		[Display(Name = "附件路径")]
		public string AttachmentUrl { get; set; }

	}

}