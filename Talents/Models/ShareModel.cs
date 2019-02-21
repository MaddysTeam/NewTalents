using Business;
using System.Collections.Generic;

namespace TheSite.Models
{

   public class ShareModel : Share
   {

      public string RealName { get; set; }
      public int AttachmentCount { get; set; }

   }

}