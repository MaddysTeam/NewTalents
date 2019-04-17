using Business.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Business
{

   public partial class DeclareReview
   {

      public string DeclareCompnay { get; set; }

      public string DeclareTargetName { get; set; }

      public string Company { get; set; }

      public string Subject { get; set; }

      public string DeclareSubject { get; set; }

      public string Gender { get; set; }

      public string GoodYear { get; set; }

      public string IsDeclareBroke { get; set; }

      public string IsMaterialBroke { get; set; }

      public string PhoneMobile { get; set; }

      public string Birthday { get; set; }

      public string SkillTitle { get; set; }

      public string IsAllowDowngrade { get; set; }

      public string IsAllowFlowToSchool { get; set; }

      // 仅仅用在校管理员审核时验证
      public bool IsReviewValidate => TeacherId > 0 && DeclareTargetPKID > 0 && CompanyId > 0 && PeriodId > 0 && !string.IsNullOrEmpty(TypeKey);

      public bool IsSubmit => !string.IsNullOrEmpty(StatusKey) && StatusKey != DeclareKeys.ReviewBack;
   }

}