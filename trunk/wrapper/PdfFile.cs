using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.IO;

namespace SeleniumWrapper
{
    [Guid("D743541A-CDB7-447E-B8EF-F2A56C3A68D4")]
    [ComVisible(true)]
    public interface IPdfFile
    {
        [Description("Add an image with border to the Pdf")]
        void addImage(ref object image, [Optional][DefaultParameterValue(String.Empty)]string title);

        [Description("Add text to the Pdf")]
        void addText(string text);

        [Description("Add Html to the Pdf")]
        void addHtml(string htmlText);

        [Description("Save the PDF file")]
        void saveAs(string pdfpath);
    }

    [Description("Create a new empty PDF file and open it")]
    [Guid("980551C8-0DEB-4774-8A07-CDCD9EB97FD6")]
    [ComVisible(true), ComDefaultInterface(typeof(IPdfFile)), ClassInterface(ClassInterfaceType.None)]
    public class PdfFile : IPdfFile
    {
        iTextSharp.text.Document doc;
        MemoryStream ms;

        public PdfFile(){
            this.doc = new iTextSharp.text.Document();
            try{
                //iTextSharp.text.pdf.PdfWriter.GetInstance(doc, new System.IO.FileStream(pdfpath, System.IO.FileMode.Create));
                //fs = new System.IO.FileStream(Path.GetTempFileName(), FileMode.Create, FileAccess.ReadWrite, FileShare.Read, 4096, FileOptions.DeleteOnClose);
                ms = new MemoryStream();
                iTextSharp.text.pdf.PdfWriter.GetInstance(doc, ms).CloseStream = false;
                this.doc.Open();
                this.doc.SetMargins(20, 20, 20, 30);
            }catch (Exception ex){
                throw new ApplicationException(ex.Message);
            }
        }

        ~PdfFile(){
            Dispose();
        }

        public void Dispose(){
            this.doc.Close();
            ms.Dispose();
        }

        public void saveAs(string pdfpath){
            this.doc.Close();
            using (FileStream fs = File.Create(pdfpath)){
                ms.WriteTo(fs);
            }
            ms.Dispose();
        }

        public void addHtml(string htmlText){
            List<iTextSharp.text.IElement> htmlarraylist = iTextSharp.text.html.simpleparser.HTMLWorker.ParseToList(new StringReader(htmlText), null);
            foreach(iTextSharp.text.IElement element in htmlarraylist)
                this.doc.Add(element);
        }

        public void addImage(ref object image, [Optional][DefaultParameterValue(String.Empty)]string title){
            if (!(image is byte[]))
                throw new Exception("Image format is incorrect. It must to be a byte array !");
            try{
                using(System.IO.MemoryStream ms = new System.IO.MemoryStream((byte[])image)){
                     using(System.Drawing.Image img = System.Drawing.Image.FromStream(ms)){
                        using(System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(img) ){
                            int imgHeight = bitmap.Height;
                            int lBlockHeight = 2100;
                            if( imgHeight < lBlockHeight ){
                                addImageToPdf(img, title);
                            }else{
                                int yPos = 0;
                                while(true){
                                    int lLeftHeight = imgHeight - yPos;
                                    if(lLeftHeight<1) break;
                                    lBlockHeight = (lLeftHeight>lBlockHeight) ? lBlockHeight : lLeftHeight;
                                    System.Drawing.Rectangle cropArea = new System.Drawing.Rectangle{
                                        X = 0, Y = yPos, Height = lBlockHeight, Width = img.Width
                                    };
                                    using(System.Drawing.Bitmap bmpCrop = bitmap.Clone(cropArea, img.PixelFormat)){
                                        using(System.IO.MemoryStream msOut = new System.IO.MemoryStream()){
                                            bmpCrop.Save(msOut, img.RawFormat);
                                            using(System.Drawing.Image imgOut = System.Drawing.Image.FromStream(msOut)){
                                                addImageToPdf(imgOut, yPos==0 ? title : null);
                                            }
                                        }
                                    }
                                    yPos += lBlockHeight;
                                }
                            }
                        }
                     }
                }
            }catch (Exception ex){
                throw new ApplicationException(ex.Message);
            }
        }
        
        private void addImageToPdf(System.Drawing.Image image, string title){
            iTextSharp.text.Image itImg = iTextSharp.text.Image.GetInstance(image, image.RawFormat);
            itImg.Border = iTextSharp.text.Rectangle.BOX;
            itImg.BorderColor = iTextSharp.text.BaseColor.BLACK;
            itImg.BorderWidth = 1f;
            if (itImg.Width > doc.PageSize.Width) itImg.ScalePercent(90f);
            this.doc.SetPageSize(new iTextSharp.text.Rectangle(itImg.Width * 0.9f + 40f, itImg.Height * 0.9f + (title!=null ? 81f : 60f) ));
            this.doc.NewPage();
            this.doc.PageCount = this.doc.PageNumber + 1;
            if(title!=null) this.doc.Add(new iTextSharp.text.Chapter(new iTextSharp.text.Paragraph(title), this.doc.PageNumber));
            this.doc.Add(itImg);
        }

        public void addText(string text){
            if(this.doc==null || this.doc.IsOpen()==false)
                throw new Exception("Pdf document is null! Use method newPdf(string pdfpath) to create a document.");
            this.doc.Add(new iTextSharp.text.Paragraph(text));
        }


    }
}
