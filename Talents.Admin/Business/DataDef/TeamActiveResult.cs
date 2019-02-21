using System.ComponentModel.DataAnnotations;

namespace Business
{

	public partial class TeamActiveResult
	{

		[Display(Name = "附件名称")]
		public string AttachmentName { get; set; }

		[Display(Name = "附件路径")]
		public string AttachmentUrl { get; set; }

	}

}