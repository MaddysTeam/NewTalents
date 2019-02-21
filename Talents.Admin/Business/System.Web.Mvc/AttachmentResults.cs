namespace System.Web.Mvc
{

	public class AttachmentResults
	{

		public AttachmentResults()
		{
			msg = "文件上传成功";
			result = AjaxResults.Success;
		}


		public string msg { get; set; }

		public string result { get; set; }

		public long id { get; set; }

		public string filename { get; set; }

		public string ext { get; set; }

		public int size { get; set; }

		public string url { get; set; }

		public string showUrl { get; set; }

	}

}