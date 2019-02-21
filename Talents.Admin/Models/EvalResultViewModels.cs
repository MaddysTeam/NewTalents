using System.ComponentModel.DataAnnotations;

namespace TheSite.Models
{

   public class EvalCommentViewModel
   {

      public long EvalResultId { get; set; }
      public string ExpertName { get; set; }
      public string EvalComment { get; set; }

   }

}