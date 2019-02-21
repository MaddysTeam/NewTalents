using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheSite.Models
{

   public class LogModel
   {

      public long UserID { get; set; }

      public DateTime OperationDate { get; set; }

      public string Where { get; set; }

      public string DoSomthing { get; set; }

      
      public virtual string ToLogString()
      {
         return string.Empty;
      }

   }

}
   