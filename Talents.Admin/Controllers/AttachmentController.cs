using Business;
using Business.Config;
using Business.Helper;
using Business.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Util.ThirdParty.WangsuCloud;

namespace TheSite.Controllers
{

   public class AttachmentController : BaseController
   {

     // public static string needConvertExt = ".xlsx,.xls,.ppt,.pptx,.doc,.docx,.csv";

      [HttpPost]
      public ActionResult UploadFile(HttpPostedFileBase file)
      {
         ThrowNotAjax();

         Stream fileStream = file.InputStream;
         Stream pdfStream = file.InputStream;
         var acceptfileTypes = AttachmentsKeys.DocumentSuffix + AttachmentsKeys.ImageSuffix+ AttachmentsKeys.ZipSuffix;
         var ext = Path.GetExtension(file.FileName);
         if (acceptfileTypes.IndexOf(ext.ToLower()) < 0)
         {
            return Json(new
            {
               result = AjaxResults.Error,
               msg = "不支持该类型的文件上传"
            });
         }

         try
         {
            string filename = DateTime.Now.ToString("yyyyMMddHHmmssffff") + file.FileName.Substring(file.FileName.IndexOf('.'));
            var filePath = GenerateFilePath(filename);
            var result = Upload(fileStream, filePath, false);

            //上传preview 的pdf
            if (AttachmentsKeys.NeedConvertExt.IndexOf(ext) >= 0)
            {
               pdfStream = fileStream.ConvertToPDF(ext);

               filePath = GenerateFilePath(filename.Replace(ext, string.Empty) + ".pdf");
               Upload(pdfStream, filePath, false);
            }

            // 返回结果
            return Json(new
            {
               result = AjaxResults.Success,
               url = result.FileUrl,
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
         finally
         {
            fileStream.Close();
            fileStream.Dispose();
            pdfStream.Close();
            pdfStream.Dispose();
         }

      }


      [HttpPost]
      public ActionResult UploadImage(HttpPostedFileBase file)
      {
         ThrowNotAjax();

         Stream imgStream = file.InputStream;

         try
         {
            string filename = DateTime.Now.ToString("yyyyMMddHHmmssffff") + file.FileName.Substring(file.FileName.IndexOf('.'));
            var filePath = GenerateFilePath(filename);

            var img = GetThumbnail(file);
            imgStream = BitmapToStream(img, Path.GetExtension(file.FileName));

            var result = Upload(imgStream, filePath, false);

            // 返回结果
            return Json(new
            {
               result = "success",
               url = result.FileUrl,
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
         finally
         {
            imgStream.Close();
            imgStream.Dispose();
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
         var ext = Path.GetExtension(attachment.AttachmentName);
         attachment.AttachmentUrl = attachment.AttachmentUrl.Replace(ext,".pdf");
         return View(attachment);
      }


      private string GetDirForSaveing()
      {
         return ThisApp.UploadFilePath + DateTime.Now.ToString("yyyyMMdd");
      }


      private Stream BitmapToStream(Bitmap bitmap, string ext)
      {
         MemoryStream ms = null;
         try
         {
            ms = new MemoryStream();
            bitmap.Save(ms, ImageFormat.Jpeg);
         }
         catch (ArgumentNullException ex)
         {
            throw ex;
         }

         return ms;
      }


      private UploadResult Upload(Stream stream, string filePath, bool isforceClose)
      {
         var result = FileUploader.SliceUpload(new UploadFile
         {
            Stream = stream,
            FileName = filePath
         });

         if (isforceClose)
         {
            stream.Close();
            stream.Dispose();
         }

         return result;
      }

      private string GenerateFilePath(string fileName)
      {
         return $"hktd2/{DateTime.Today.ToString("yyyyMMdd")}/{fileName}";
      }

   }

}