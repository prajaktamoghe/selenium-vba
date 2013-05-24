using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;

namespace SeleniumWrapper
{
    [Guid("D743541A-CDB7-447E-B8EF-F2A56C3A68D4")]
    [ComVisible(true)]
    public interface IPdfFile
    {
        [Description("Set page size in millimeter")]
        void setPageSize(int width_mm, int heigth_mm);

        [Description("Set page margins in millimeter")]
        void setMargins(int left_mm, int rigth_mm, int top_mm, int bottom_mm);

        [Description("Add an image with borders to the Pdf")]
        void addImage(ref object myimage, [Optional][DefaultParameterValue(null)]string bookmark, [Optional][DefaultParameterValue(false)]bool newpage);

        [Description("Add vertical space in millimeter")]
        void addVerticalSpace(int space_mm);

        [Description("Add text to the Pdf")]
        void addText(string text,
            [Optional][DefaultParameterValue(10)]int size,
            [Optional][DefaultParameterValue("Black")]string color,
            [Optional][DefaultParameterValue(false)]bool bold,
            [Optional][DefaultParameterValue(false)]bool italic,
            [Optional][DefaultParameterValue(false)]bool underline,
            [Optional][DefaultParameterValue(false)]bool center,
            [Optional][DefaultParameterValue("Helvetica")]string font);

        [Description("Add a page")]
        void AddPage();

        [Description("Add a bookmark")]
        void addBookmark(string bookmark, [Optional][DefaultParameterValue(false)]bool newpage);

        [Description("Save the PDF file")]
        void saveAs(string pdfpath);
    }

    /// <summary>
    /// Create a new empty PDF file
    /// </summary>

    [Description("Create a new empty PDF file")]
    [Guid("980551C8-0DEB-4774-8A07-CDCD9EB97FD6")]
    [ComVisible(true), ComDefaultInterface(typeof(IPdfFile)), ClassInterface(ClassInterfaceType.None)]
    public class PdfFile : IPdfFile
    {
        private PdfSharp.Pdf.PdfDocument doc;
        private PdfSharp.Pdf.PdfPage page;
        private PdfSharp.Drawing.XGraphics graphics;
        private PdfSharp.Drawing.XSize size;
        private PdfSharp.Drawing.XRect area;
        private double verticalPosition;
        private double horizontalePosition;
        private bool IsPageEmpty;
        private int pageCount;
        private int bookmarkCount;
        private PdfSharp.Drawing.XFont font_pageNumber;
        private PdfSharp.Drawing.Layout.XTextFormatter textformater;

        public PdfFile(){
            try{
                this.doc = new PdfSharp.Pdf.PdfDocument();
                this.font_pageNumber = new PdfSharp.Drawing.XFont("Arial", 8);
                AddPage();
            }catch (Exception ex){
                throw new ApplicationException(ex.Message);
            }
        }

        ~PdfFile(){
            Dispose();
        }

        public void Dispose(){
            this.graphics.Dispose();
            this.doc.Dispose();
            //if(this.doc.IsOpen())this.doc.Close();
            //ms.Dispose();
        }

        /// <summary>Set page size in millimeter</summary>
        /// <param name="width_mm"></param>
        /// <param name="heigth_mm"></param>
        public void setPageSize(int width_mm, int heigth_mm)
        {
            PdfSharp.Drawing.XUnit width = PdfSharp.Drawing.XUnit.FromMillimeter(width_mm);
            PdfSharp.Drawing.XUnit heigth = PdfSharp.Drawing.XUnit.FromMillimeter(heigth_mm);
            this.page.Width = width;
            this.page.Height = heigth;
            this.size.Width = width;
            this.size.Height = heigth;
            this.area.Width = this.area.Width + (width - this.page.Width);
            this.area.Height = this.area.Height + (heigth - this.page.Height);
            this.verticalPosition = this.verticalPosition + heigth - this.page.Height;
        }

        /// <summary>Set page margins in millimeter</summary>
        /// <param name="left_mm">Left margin in millimetter</param>
        /// <param name="rigth_mm">Rigth margin in millimetter</param>
        /// <param name="top_mm">Top margin in millimetter</param>
        /// <param name="bottom_mm">Bottom margin in millimetter</param>
        public void setMargins(int left_mm, int rigth_mm, int top_mm, int bottom_mm)
        {
            PdfSharp.Drawing.XUnit left = PdfSharp.Drawing.XUnit.FromMillimeter(left_mm);
            PdfSharp.Drawing.XUnit top = PdfSharp.Drawing.XUnit.FromMillimeter(top_mm);
            this.horizontalePosition = this.horizontalePosition + left - this.area.X;
            this.verticalPosition = this.verticalPosition + top - this.area.Y;
            this.area = new PdfSharp.Drawing.XRect{
                X = left,
                Y = top,
                Width = this.page.Width - PdfSharp.Drawing.XUnit.FromMillimeter(left_mm + rigth_mm),
                Height = this.page.Height - PdfSharp.Drawing.XUnit.FromMillimeter(top_mm + bottom_mm)
            };
        }

        /// <summary>Add a new page if the current page is not empty</summary>
        public void AddPage()
        {
            if (!this.IsPageEmpty){
                if(this.pageCount!=0)
                    addPagenumber();
                this.page = new PdfSharp.Pdf.PdfPage();
                if(this.pageCount == 0){
                    this.size.Width = this.page.Width;
                    this.size.Height = this.page.Height;
                    this.area = new PdfSharp.Drawing.XRect{
                        X = PdfSharp.Drawing.XUnit.FromMillimeter(10),
                        Y = PdfSharp.Drawing.XUnit.FromMillimeter(10),
                        Width = this.page.Width - PdfSharp.Drawing.XUnit.FromMillimeter(20),
                        Height = this.page.Height - PdfSharp.Drawing.XUnit.FromMillimeter(45)
                    };
                }else{
                    this.page = new PdfSharp.Pdf.PdfPage();
                    this.page.Width = this.size.Width;
                    this.page.Height = this.size.Height;
                }
                this.horizontalePosition = this.area.X;
                this.verticalPosition = this.area.Y;
                this.doc.AddPage(this.page);
                if (this.graphics!=null)
                    this.graphics.Dispose();
                this.graphics = PdfSharp.Drawing.XGraphics.FromPdfPage(this.page);
                this.textformater = new PdfSharp.Drawing.Layout.XTextFormatter(this.graphics);
                this.pageCount++;
                this.IsPageEmpty = true;
            }
        }

        /// <summary>Add vertical space in millimeter</summary>
        /// <param name="space_mm">Vertical space in millimeter</param>
        public void addVerticalSpace(int space_mm)
        {
            this.verticalPosition += PdfSharp.Drawing.XUnit.FromMillimeter(space_mm);
        }

        private void addPagenumber(){
            this.graphics.DrawString(this.pageCount.ToString(), font_pageNumber, PdfSharp.Drawing.XBrushes.Black, this.area.Right - 15, this.area.Bottom + 10);
        }

        private float AvailableHeigth
        {
            get { return (float)(this.area.Bottom - this.verticalPosition); }

        }

        /// <summary>Save the PDF file</summary>
        /// <param name="pdfpath">PDF fIle path</param>
        public void saveAs(string pdfpath){
            addPagenumber();
            using (FileStream fs = File.Create(pdfpath)){
                this.doc.Save(fs);
            }
            this.doc.Close();
            //ms.Dispose();
        }

        public void addHtml(string htmlText){
            throw new Exception("addHtml methode is no longer available!");
        }

        public Image loadImage(string image_path)
        {
            System.Drawing.Image bitmap = System.Drawing.Bitmap.FromFile(image_path);
            byte[] imageBytes;
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream()){
                bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                imageBytes = ms.ToArray();
            }
            return new Image(imageBytes);
        }

        /// <summary>Add a bookmark</summary>
        /// <param name="bookmark">Bookmark name</param>
        /// <param name="newpage">Create the page in a new page. Default is true</param>
        public void addBookmark(string bookmark, [Optional][DefaultParameterValue(true)]bool newpage){
            if(newpage)
                this.AddPage();
            bookmark = ++this.bookmarkCount + " " + bookmark;
            PdfSharp.Drawing.XFont font = new PdfSharp.Drawing.XFont("Verdana", 12);
            this.verticalPosition += font.Height;
            this.graphics.DrawString(bookmark, font, PdfSharp.Drawing.XBrushes.Black,this.horizontalePosition + 10, this.verticalPosition);
            this.verticalPosition += font.Height / 2;
            this.doc.Outlines.Add(bookmark, this.page, true);
            this.IsPageEmpty = false;
        }

        /// <summary>Add text</summary>
        /// <param name="text">Text to add</param>
        /// <param name="size">Optional - font size</param>
        /// <param name="color">Optional - font color</param>
        /// <param name="bold">Optional - bold</param>
        /// <param name="italic">Optional - italic</param>
        /// <param name="underline">Optional - underline</param>
        /// <param name="center">Optional - center the texte</param>
        /// <param name="font">Optional - Font name. Default is Helvetica</param>
        public void addText(string text, 
            [Optional][DefaultParameterValue(10)]int size,
            [Optional][DefaultParameterValue("Black")]string color,
            [Optional][DefaultParameterValue(false)]bool bold,
            [Optional][DefaultParameterValue(false)]bool italic,
            [Optional][DefaultParameterValue(false)]bool underline,
            [Optional][DefaultParameterValue(false)]bool center,
            [Optional][DefaultParameterValue("Helvetica")]string font)
        {
            PdfSharp.Drawing.XFontStyle xfontStyle = 
                (bold ? PdfSharp.Drawing.XFontStyle.Bold : 0)
                | (underline ? PdfSharp.Drawing.XFontStyle.Underline : 0)
                | (italic ? PdfSharp.Drawing.XFontStyle.Italic : 0);
            PdfSharp.Drawing.XFont xfont;
            if( bold || underline || italic )
                xfont =  new PdfSharp.Drawing.XFont(font, size, xfontStyle);
            else
                xfont =  new PdfSharp.Drawing.XFont(font, size);
            PdfSharp.Drawing.XBrush xBrush;
            try{
                xBrush = (PdfSharp.Drawing.XSolidBrush)typeof(PdfSharp.Drawing.XBrushes).GetProperty(color).GetValue(null, null);
            }catch(Exception){
                throw new ArgumentException("Color <" + color + "> is not available!");
            }

            if (center){
                PdfSharp.Drawing.XSize strSize = this.graphics.MeasureString(text, xfont);
                double minWidth = Math.Min(strSize.Width, this.area.Width);
                PdfSharp.Drawing.XRect rect = new PdfSharp.Drawing.XRect{
                    X = (this.area.Width / 2) - (minWidth / 2),
                    Y =  + 2,
                    Width = minWidth,
                    Height = strSize.Height
                };
                this.AddTextToPdf(text, xfont, xBrush, rect);
            }else{
                string leftText = text;
                while(true){
                    int lenToAdd;
                    string textToAdd;
                    PdfSharp.Drawing.XSize textSize = this.graphics.MeasureString(leftText, xfont, PdfSharp.Drawing.XStringFormats.Default);
                    double textHeight;
                    if (textSize.Width < this.area.Width)
                        textHeight = textSize.Height;
                    else
                        textHeight = textSize.Height * 1.2 * (textSize.Width / this.area.Width);

                    if( textHeight < this.AvailableHeigth){
                        PdfSharp.Drawing.XRect rect = new PdfSharp.Drawing.XRect{
                            X = 0,
                            Y = 2,
                            Width = this.area.Width,
                            Height = textHeight
                        };
                        this.AddTextToPdf(leftText, xfont, xBrush, rect);
                        break;
                    }else{
                        if (textSize.Width < this.area.Width){
                            lenToAdd = leftText.Length;
                            textToAdd = leftText.Substring(0, lenToAdd);
                        }else{
                            lenToAdd = (int)(((float)leftText.Length) * this.AvailableHeigth / textHeight * 1.1);
                            while(true){
                                lenToAdd = leftText.LastIndexOf(' ', lenToAdd -1);
                                string str = leftText.Substring(0, lenToAdd);
                                textSize = this.graphics.MeasureString(str, xfont);
                                textHeight = textSize.Height * 1.2 * textSize.Width / this.area.Width;
                                if ( textHeight < this.AvailableHeigth ) break;
                            }
                            textToAdd = leftText.Substring(0, lenToAdd);
                        }
                        if (lenToAdd == 0) break;

                        PdfSharp.Drawing.XRect rect = new PdfSharp.Drawing.XRect{
                            X = 0,
                            Y = 2,
                            Width = this.area.Width,
                            Height = textHeight
                        };
                        this.AddTextToPdf(textToAdd, xfont, xBrush, rect);
                        this.AddPage();
                        if (leftText.Length == 0) break;
                        leftText = leftText.Substring(lenToAdd);
                    }
                }
            }
        }

        private void AddTextToPdf(string text, PdfSharp.Drawing.XFont xfont, PdfSharp.Drawing.XBrush xBrush, PdfSharp.Drawing.XRect rect )
        {
            rect.X = this.horizontalePosition + rect.X;
            rect.Y = this.verticalPosition + rect.Y;
            this.textformater.DrawString(text, xfont, xBrush, rect, PdfSharp.Drawing.XStringFormats.TopLeft);
            this.verticalPosition = rect.Y + rect.Height;
            this.IsPageEmpty = false;
        }

        private PdfSharp.Drawing.XSize GetImageSize(System.Drawing.Bitmap bitmap)
        {
            float resolution = 72;
            PdfSharp.Drawing.XUnit imgWidthPt = PdfSharp.Drawing.XUnit.FromPoint ( ((float)bitmap.Width) * resolution / bitmap.HorizontalResolution );
            PdfSharp.Drawing.XUnit imgHeightPt = PdfSharp.Drawing.XUnit.FromPoint ( ((float)bitmap.Height) * 72 / bitmap.VerticalResolution );
            return new PdfSharp.Drawing.XSize(imgWidthPt, imgHeightPt);
        }

        /// <summary>Add an image</summary>
        /// <param name="myimage">Image object</param>
        /// <param name="bookmark">Optional - Bookmark</param>
        /// <param name="newpage">Optional - Add the image in a new page</param>
        public void addImage(ref object myimage, [Optional][DefaultParameterValue(null)]string bookmark, [Optional][DefaultParameterValue(false)]bool newpage){

            if (!(myimage is Image)) throw new ArgumentException( "Argument <myimage> is not an Image object" );
            Image image = (Image)myimage;

            try{
                using(System.IO.MemoryStream ms = new System.IO.MemoryStream(image.imageBytes)){
                    using(System.Drawing.Bitmap bitmap = (System.Drawing.Bitmap)System.Drawing.Bitmap.FromStream(ms)){
                        PdfSharp.Drawing.XSize imgSize = GetImageSize(bitmap);
                        double scale = 1;

                        if(imgSize.Width>this.area.Width){
                            scale = this.area.Width / imgSize.Width;
                            imgSize.Width *= scale;
                            imgSize.Height *= scale;
                        }

                        if (newpage)
                            this.AddPage();
                        if( !String.IsNullOrEmpty(bookmark))
                            this.addBookmark(bookmark, false);
                        if (this.AvailableHeigth < 20)
                            this.AddPage();
                        if( imgSize.Height < this.AvailableHeigth )
                            this.AddImageToPdf(bitmap, imgSize.Width, imgSize.Height);
                        else{
                            double scaledPointsToPixelsY = bitmap.VerticalResolution / scale / 72; // Points scaled -> Points original -> Pixels
                            double yPosPt = 0;
                            double leftHeightPt = imgSize.Height;
                            while(true){
                                if (leftHeightPt < 1) break;
                                if (this.AvailableHeigth < 20) this.AddPage();
                                double insertHeightPt;
                                if(leftHeightPt>this.AvailableHeigth)
                                    insertHeightPt = this.AvailableHeigth;
                                else
                                    insertHeightPt = leftHeightPt;

                                System.Drawing.Rectangle cropArea = new System.Drawing.Rectangle{
                                    X = 0,
                                    Y = (int)(yPosPt * scaledPointsToPixelsY), 
                                    Height = (int)(insertHeightPt * scaledPointsToPixelsY),
                                    Width = bitmap.Width
                                };
                                using (System.Drawing.Bitmap bmpCrop = bitmap.Clone(cropArea, bitmap.PixelFormat)){
                                    this.AddImageToPdf(bmpCrop, imgSize.Width, insertHeightPt);
                                }
                                leftHeightPt = imgSize.Height - yPosPt - insertHeightPt;
                                if (leftHeightPt < 1) break;
                                yPosPt += insertHeightPt;
                                if(leftHeightPt>10){
                                    leftHeightPt +=8;
                                    yPosPt -= 8;
                                }
                            }
                        }
                    }
                }
            }catch (Exception ex){
                throw new ApplicationException("addImage methode failed! " + ex.Message);
            }
        }

        private void AddImageToPdf(System.Drawing.Image image, double width_pt, double heigth_pt )
        {
            System.Drawing.RectangleF rect = new System.Drawing.RectangleF(){
                X = (float)this.horizontalePosition,
                Y = (float)this.verticalPosition,
                Width = (float)width_pt,
                Height = (float)heigth_pt
            };
            PdfSharp.Drawing.XImage xImage = PdfSharp.Drawing.XImage.FromGdiPlusImage(image);
            PdfSharp.Drawing.XPen pen = new PdfSharp.Drawing.XPen(PdfSharp.Drawing.XColors.Black, 1f);
            this.graphics.DrawImage(xImage, rect);
            this.graphics.DrawRectangle(pen, rect);
            this.verticalPosition += heigth_pt;
            this.IsPageEmpty = false;
        }


    }

}
