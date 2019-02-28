using Aspose.Slides;
using Aspose.Slides.Export;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Util.ThirdParty.Aspose
{

   /// <summary>
   /// word converter 
   /// </summary>
   public static class PPTConverter
   {

    


      public static Stream ConvertoHtml(Stream source)
      {

         if (source == null) throw new ArgumentNullException("source stream can not be null");
         //byte[] contentBytes = new byte[source.Length];
         //source.Read(contentBytes, 0, contentBytes.Length);
         var stream = ConvertRightNowAspose(source, SaveFormat.Html);

         return stream;
      }

      public static Stream ConvertoPdf(Stream source)
      {

         if (source == null) throw new ArgumentNullException("source stream can not be null");
         //byte[] contentBytes = new byte[source.Length];
         //source.Read(contentBytes, 0, contentBytes.Length);
         var stream = ConvertRightNowAspose(source, SaveFormat.Pdf);

         return stream;
      }


      internal static Stream ConvertRightNowAspose(Stream stream, SaveFormat saveFormat)
      {
         stream.Seek(0, SeekOrigin.Begin);
         Presentation ppt = new Presentation(stream);
         var firstStream = new MemoryStream();
         ppt.Save(firstStream, SaveFormat.Pdf);
         //if (saveFormat == SaveFormat.Html)
         //{
         //   //HtmlSaveOptions options = new HtmlSaveOptions(saveFormat);
         //   //options.ImageSavingCallback = new HandleImageSaving();
         //   //doc.Save(firstStream, options);
         //}
         //else
         //{
         //   book.Save(firstStream, saveFormat);
         //}

         return firstStream;
      }

   }

}


