using Business;
using Business.Config;
using Business.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace TheSite.Controllers
{

   public class AttachmentController : BaseController
   {

      static Dictionary<string, PDFHandler> PDFHandlers = new Dictionary<string, PDFHandler>
      {
         {".doc", new WordToPDFHandler()  },
         {".docx",new WordToPDFHandler()  },
         {".xls", new ExcelToPDFHandler()  },
         {".xlsx",new ExcelToPDFHandler()  },
         {".csv", new ExcelToPDFHandler()  },
         {".pptx",new PPTToPDFHandler() },
         {".ppt", new PPTToPDFHandler() },
         {".pdf", new PDFHandler()  },
      };

      [HttpPost]
      public ActionResult UploadFile(HttpPostedFileBase file)
      {
         ThrowNotAjax();

         try
         {
            string filename = DateTime.Now.ToString("yyyyMMddHHmmssffff") + file.FileName.Substring(file.FileName.IndexOf('.'));
            string savepath = GetDirForSaveing();

            string mappedDir = Server.MapPath("~" + savepath);
            if (!Directory.Exists(mappedDir))
               Directory.CreateDirectory(mappedDir);


            file.SaveAs(Path.Combine(mappedDir, filename));

            string url = savepath + "/" + filename;

            // 返回结果
            return Json(new
            {
               result = AjaxResults.Success,
               url = url,
               filename = file.FileName,
               msg = "文件已保存成功"
            });
         }
         catch (Exception ex)
         {
            // 返回结果
            return Json(new
            {
               result = AjaxResults.Error,
               msg = ex.Message
            });
         }

      }


      [HttpPost]
      public ActionResult UploadImage(HttpPostedFileBase file)
      {
         ThrowNotAjax();

         try
         {
            string filename = DateTime.Now.ToString("yyyyMMddHHmmssffff") + ".jpg";
            string savepath = GetDirForSaveing();

            string mappedDir = Server.MapPath("~" + savepath);
            if (!Directory.Exists(mappedDir))
               Directory.CreateDirectory(mappedDir);

            var img = GetThumbnail(file);
            img.Save(Path.Combine(mappedDir, filename), ImageFormat.Jpeg);

            string url = savepath + "/" + filename;

            // 返回结果
            return Json(new
            {
               result = "success",
               url = url,
               msg = "图片已保存成功"
            });
         }
         catch (Exception ex)
         {
            // 返回结果
            return Json(new
            {
               result = "error",
               msg = ex.Message
            });
         }

      }


      public static Bitmap GetThumbnail(HttpPostedFileBase hpf, int destHeight = ThisApp.ImageHeight, int destWidth = ThisApp.ImageWidth)
      {

         System.Drawing.Image imgSource = Image.FromStream(hpf.InputStream);
         System.Drawing.Imaging.ImageFormat thisFormat = imgSource.RawFormat;
         int sW = 0, sH = 0;
         // 按比例缩放           
         int sWidth = imgSource.Width;
         int sHeight = imgSource.Height;
         if (sHeight > destHeight || sWidth > destWidth)
         {
            if ((sWidth * destHeight) > (sHeight * destWidth))
            {
               sW = destWidth;
               sH = (destWidth * sHeight) / sWidth;
            }
            else
            {
               sH = destHeight;
               sW = (sWidth * destHeight) / sHeight;
            }
         }
         else
         {
            sW = sWidth;
            sH = sHeight;
         }
         Bitmap outBmp = new Bitmap(destWidth, destHeight);
         Graphics g = Graphics.FromImage(outBmp);
         g.Clear(Color.Transparent);
         // 设置画布的描绘质量         
         g.CompositingQuality = CompositingQuality.HighQuality;
         g.SmoothingMode = SmoothingMode.HighQuality;
         g.InterpolationMode = InterpolationMode.HighQualityBicubic;
         g.DrawImage(imgSource, new Rectangle((destWidth - sW) / 2, (destHeight - sH) / 2, sW, sH), 0, 0, imgSource.Width, imgSource.Height, GraphicsUnit.Pixel);
         g.Dispose();
         // 以下代码为保存图片时，设置压缩质量     
         EncoderParameters encoderParams = new EncoderParameters();
         long[] quality = new long[1];
         quality[0] = 100;
         EncoderParameter encoderParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);
         encoderParams.Param[0] = encoderParam;
         imgSource.Dispose();
         return outBmp;

      }


      public ActionResult Preview(long id)
      {
         var attachment = db.AttachmentsDal.PrimaryGet(id);
         var extName = Path.GetExtension(attachment.AttachmentName);
         if (PDFHandlers.ContainsKey(extName))
            PDFHandlers[extName].Handle(attachment, new PDFContext { Server = Server });

         return View(attachment);
      }


      private string GetDirForSaveing()
      {
         return ThisApp.UploadFilePath + DateTime.Now.ToString("yyyyMMdd");
      }


   }

   class PDFHandler
   {
      public virtual void Handle(Attachments orginal, PDFContext context) { }
   }

   class WordToPDFHandler : PDFHandler
   {
      public override void Handle(Attachments orginal, PDFContext context)
      {
         var attachment = orginal;
         var server = context.Server;
         var filePath = server.MapPath("~" + attachment.AttachmentUrl);
         var extName = Path.GetExtension(orginal.AttachmentName);
         var pdfVirtualPath = attachment.AttachmentUrl.Replace(extName, ".pdf");
         var pdfPath = server.MapPath("~" + pdfVirtualPath);
         if (!System.IO.File.Exists(pdfPath))
         {
            PDFHelper.WordToPDF(filePath, pdfPath);
         }

         attachment.AttachmentUrl = pdfVirtualPath;

      }
   }

   class ExcelToPDFHandler : PDFHandler
   {
      public override void Handle(Attachments orginal, PDFContext context)
      {
         var attachment = orginal;
         var server = context.Server;
         var filePath = server.MapPath("~" + attachment.AttachmentUrl);
         var extName = Path.GetExtension(orginal.AttachmentName);
         var pdfVirtualPath = attachment.AttachmentUrl.Replace(extName, ".pdf");
         var pdfPath = server.MapPath("~" + pdfVirtualPath);
         if (!System.IO.File.Exists(pdfPath))
         {
            PDFHelper.ExcelToPdf(filePath, pdfPath);
         }

         attachment.AttachmentUrl = pdfVirtualPath;
      }
   }

   class PPTToPDFHandler : PDFHandler
   {
      public override void Handle(Attachments orginal, PDFContext context)
      {
         var attachment = orginal;
         var server = context.Server;
         var filePath = server.MapPath("~" + attachment.AttachmentUrl);
         var extName = Path.GetExtension(orginal.AttachmentName);
         var pdfVirtualPath = attachment.AttachmentUrl.Replace(extName, ".pdf");
         var pdfPath = server.MapPath("~" + pdfVirtualPath);
         if (!System.IO.File.Exists(pdfPath))
         {
            PDFHelper.PPTToPDF(filePath, pdfPath);
         }

         attachment.AttachmentUrl = pdfVirtualPath;
      }
   }

   class PDFContext
   {
      public HttpServerUtilityBase Server { get; set; }
   }

}