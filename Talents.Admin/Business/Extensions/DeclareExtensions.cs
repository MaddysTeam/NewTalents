using Business.Helper;
using Symber.Web.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using TheSite.Models;

namespace Business
{
	public static class DeclareExtensions
	{

		public static string GetDeclareContent(List<DeclareContent> list, string key)
			=> list.Find(m => m.ContentKey == key) == null ? "" : list.Find(m => m.ContentKey == key).ContentValue;

	}
}
