using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;

namespace SeleniumWrapper {
    [Guid("D743541A-CDB7-447E-B8EF-F2A56C3A68D4")]
    [ComVisible(true), InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface IPdfFile {
        [Description("Set page size in millimeter")]
        void setPageSize(int width_mm, int heigth_mm);

        [Description("Set page margins in millimeter")]
        void setMargins(int left_mm, int rigth_mm, int top_mm, int bottom_mm);

        [Description("Add an image with borders to the Pdf")]
        void addImage([MarshalAs(UnmanagedType.IDispatch)]Image image, string bookmark = null, bool newpage = false);

        [Description("Add vertical space in millimeter")]
        void addVerticalSpace(int space_mm);

        [Description("Add text to the Pdf")]
        void addText(string text,
            int size = 10,
            string color = "Black",
            bool bold = false,
            bool italic = false,
            bool underline = false,
            bool center = false,
            string font = "Helvetica");

        [Description("Add a page")]
        void addPage();

        [Description("Add a bookmark")]
        void addBookmark(string bookmark, bool newpage = false);

        [Description("Save the PDF file")]
        void saveAs(string pdfpath);
    }

    /// <summary>
    /// Create a new empty PDF file
    /// </summary>
    [Description("Create a new empty PDF file")]
    [Guid("980551C8-0DEB-4774-8A07-CDCD9EB97FD6")]
    [ComVisible(true), ComDefaultInterface(typeof(IPdfFile)), ClassInterface(ClassInterfaceType.None)]
    public class PdfFile : IPdfFile {
        private PdfSharp.Pdf.PdfDocument _doc;
        private PdfSharp.Pdf.PdfPage _page;
        private PdfSharp.Drawing.XGraphics _graphics;
        private PdfSharp.Drawing.XSize _size;
        private PdfSharp.Drawing.XRect _area;
        private double _verticalPosition;
        private double _horizontalePosition;
        private bool _isPageEmpty;
        private int _pageCount;
        private int _bookmarkCount;
        private PdfSharp.Drawing.XFont _font_pageNumber;
        private PdfSharp.Drawing.Layout.XTextFormatter _textformater;

        public PdfFile() {
            try {
                _doc = new PdfSharp.Pdf.PdfDocument();
                _font_pageNumber = new PdfSharp.Drawing.XFont("Arial", 8);
                addPage();
            } catch (Exception ex) {
                throw new ApplicationException(ex.Message);
            }
        }

        ~PdfFile() {
            Dispose();
        }

        public void Dispose() {
            _graphics.Dispose();
            _doc.Dispose();
        }

        /// <summary>Set page size in millimeter</summary>
        /// <param name="width_mm"></param>
        /// <param name="heigth_mm"></param>
        public void setPageSize(int width_mm, int heigth_mm) {
            PdfSharp.Drawing.XUnit width = PdfSharp.Drawing.XUnit.FromMillimeter(width_mm);
            PdfSharp.Drawing.XUnit heigth = PdfSharp.Drawing.XUnit.FromMillimeter(heigth_mm);
            _page.Width = width;
            _page.Height = heigth;
            _size.Width = width;
            _size.Height = heigth;
            _area.Width = _area.Width + (width - _page.Width);
            _area.Height = _area.Height + (heigth - _page.Height);
            _verticalPosition = _verticalPosition + heigth - _page.Height;
        }

        /// <summary>Set page margins in millimeter</summary>
        /// <param name="left_mm">Left margin in millimetter</param>
        /// <param name="rigth_mm">Rigth margin in millimetter</param>
        /// <param name="top_mm">Top margin in millimetter</param>
        /// <param name="bottom_mm">Bottom margin in millimetter</param>
        public void setMargins(int left_mm, int rigth_mm, int top_mm, int bottom_mm) {
            PdfSharp.Drawing.XUnit left = PdfSharp.Drawing.XUnit.FromMillimeter(left_mm);
            PdfSharp.Drawing.XUnit top = PdfSharp.Drawing.XUnit.FromMillimeter(top_mm);
            _horizontalePosition = _horizontalePosition + left - _area.X;
            _verticalPosition = _verticalPosition + top - _area.Y;
            _area = new PdfSharp.Drawing.XRect {
                X = left,
                Y = top,
                Width = _page.Width - PdfSharp.Drawing.XUnit.FromMillimeter(left_mm + rigth_mm),
                Height = _page.Height - PdfSharp.Drawing.XUnit.FromMillimeter(top_mm + bottom_mm)
            };
        }

        /// <summary>Add a new page if the current page is not empty</summary>
        public void addPage() {
            if (!_isPageEmpty) {
                if (_pageCount != 0)
                    addPagenumber();
                _page = new PdfSharp.Pdf.PdfPage();
                if (_pageCount == 0) {
                    _size.Width = _page.Width;
                    _size.Height = _page.Height;
                    _area = new PdfSharp.Drawing.XRect {
                        X = PdfSharp.Drawing.XUnit.FromMillimeter(10),
                        Y = PdfSharp.Drawing.XUnit.FromMillimeter(10),
                        Width = _page.Width - PdfSharp.Drawing.XUnit.FromMillimeter(20),
                        Height = _page.Height - PdfSharp.Drawing.XUnit.FromMillimeter(45)
                    };
                } else {
                    _page = new PdfSharp.Pdf.PdfPage();
                    _page.Width = _size.Width;
                    _page.Height = _size.Height;
                }
                _horizontalePosition = _area.X;
                _verticalPosition = _area.Y;
                _doc.AddPage(_page);
                if (_graphics != null)
                    _graphics.Dispose();
                _graphics = PdfSharp.Drawing.XGraphics.FromPdfPage(_page);
                _textformater = new PdfSharp.Drawing.Layout.XTextFormatter(_graphics);
                _pageCount++;
                _isPageEmpty = true;
            }
        }

        /// <summary>Add vertical space in millimeter</summary>
        /// <param name="space_mm">Vertical space in millimeter</param>
        public void addVerticalSpace(int space_mm) {
            _verticalPosition += PdfSharp.Drawing.XUnit.FromMillimeter(space_mm);
        }

        private void addPagenumber() {
            _graphics.DrawString(_pageCount.ToString(), _font_pageNumber, PdfSharp.Drawing.XBrushes.Black, _area.Right - 15, _area.Bottom + 10);
        }

        private float AvailableHeigth {
            get { return (float)(_area.Bottom - _verticalPosition); }

        }

        /// <summary>Save the PDF file</summary>
        /// <param name="pdfpath">PDF fIle path</param>
        public void saveAs(string pdfpath) {
            addPagenumber();
            using (FileStream fs = File.Create(pdfpath)) {
                _doc.Save(fs);
            }
            _doc.Close();
            //ms.Dispose();
        }

        public void addHtml(string htmlText) {
            throw new Exception("addHtml methode is no longer available!");
        }

        public Image loadImage(string image_path) {
            System.Drawing.Image bitmap = System.Drawing.Bitmap.FromFile(image_path);
            byte[] imageBytes;
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream()) {
                bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                imageBytes = ms.ToArray();
            }
            return new Image(imageBytes);
        }

        /// <summary>Add a bookmark</summary>
        /// <param name="bookmark">Bookmark name</param>
        /// <param name="newpage">Create the page in a new page. Default is true</param>
        public void addBookmark(string bookmark, [Optional][DefaultParameterValue(true)]bool newpage) {
            if (newpage)
                this.addPage();
            bookmark = ++_bookmarkCount + " " + bookmark;
            PdfSharp.Drawing.XFont font = new PdfSharp.Drawing.XFont("Verdana", 12);
            _verticalPosition += font.Height;
            _graphics.DrawString(bookmark, font, PdfSharp.Drawing.XBrushes.Black, _horizontalePosition + 10, _verticalPosition);
            _verticalPosition += font.Height / 2;
            _doc.Outlines.Add(bookmark, _page, true);
            _isPageEmpty = false;
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
            int size = 10,
            string color = "Black",
            bool bold = false,
            bool italic = false,
            bool underline = false,
            bool center = false,
            string font = "Helvetica") {
            PdfSharp.Drawing.XFontStyle xfontStyle =
                (bold ? PdfSharp.Drawing.XFontStyle.Bold : 0)
                | (underline ? PdfSharp.Drawing.XFontStyle.Underline : 0)
                | (italic ? PdfSharp.Drawing.XFontStyle.Italic : 0);
            PdfSharp.Drawing.XFont xfont;
            if (bold || underline || italic)
                xfont = new PdfSharp.Drawing.XFont(font, size, xfontStyle);
            else
                xfont = new PdfSharp.Drawing.XFont(font, size);
            PdfSharp.Drawing.XBrush xBrush;
            try {
                xBrush = (PdfSharp.Drawing.XSolidBrush)typeof(PdfSharp.Drawing.XBrushes).GetProperty(color).GetValue(null, null);
            } catch (Exception) {
                throw new ArgumentException("Color <" + color + "> is not available!");
            }

            if (center) {
                PdfSharp.Drawing.XSize strSize = _graphics.MeasureString(text, xfont);
                double minWidth = Math.Min(strSize.Width, _area.Width);
                PdfSharp.Drawing.XRect rect = new PdfSharp.Drawing.XRect {
                    X = (_area.Width / 2) - (minWidth / 2),
                    Y = +2,
                    Width = minWidth,
                    Height = strSize.Height
                };
                this.AddTextToPdf(text, xfont, xBrush, rect);
            } else {
                string leftText = text;
                while (true) {
                    int lenToAdd;
                    string textToAdd;
                    PdfSharp.Drawing.XSize textSize = _graphics.MeasureString(leftText, xfont, PdfSharp.Drawing.XStringFormats.Default);
                    double textHeight;
                    if (textSize.Width < _area.Width)
                        textHeight = textSize.Height;
                    else
                        textHeight = textSize.Height * 1.2 * (textSize.Width / _area.Width);

                    if (textHeight < this.AvailableHeigth) {
                        PdfSharp.Drawing.XRect rect = new PdfSharp.Drawing.XRect {
                            X = 0,
                            Y = 2,
                            Width = _area.Width,
                            Height = textHeight
                        };
                        this.AddTextToPdf(leftText, xfont, xBrush, rect);
                        break;
                    } else {
                        if (textSize.Width < _area.Width) {
                            lenToAdd = leftText.Length;
                            textToAdd = leftText.Substring(0, lenToAdd);
                        } else {
                            lenToAdd = (int)(((float)leftText.Length) * this.AvailableHeigth / textHeight * 1.1);
                            while (true) {
                                lenToAdd = leftText.LastIndexOf(' ', lenToAdd - 1);
                                string str = leftText.Substring(0, lenToAdd);
                                textSize = _graphics.MeasureString(str, xfont);
                                textHeight = textSize.Height * 1.2 * textSize.Width / _area.Width;
                                if (textHeight < this.AvailableHeigth) break;
                            }
                            textToAdd = leftText.Substring(0, lenToAdd);
                        }
                        if (lenToAdd == 0) break;

                        PdfSharp.Drawing.XRect rect = new PdfSharp.Drawing.XRect {
                            X = 0,
                            Y = 2,
                            Width = _area.Width,
                            Height = textHeight
                        };
                        this.AddTextToPdf(textToAdd, xfont, xBrush, rect);
                        this.addPage();
                        if (leftText.Length == 0) break;
                        leftText = leftText.Substring(lenToAdd);
                    }
                }
            }
        }

        private void AddTextToPdf(string text, PdfSharp.Drawing.XFont xfont, PdfSharp.Drawing.XBrush xBrush, PdfSharp.Drawing.XRect rect) {
            rect.X = _horizontalePosition + rect.X;
            rect.Y = _verticalPosition + rect.Y;
            _textformater.DrawString(text, xfont, xBrush, rect, PdfSharp.Drawing.XStringFormats.TopLeft);
            _verticalPosition = rect.Y + rect.Height;
            _isPageEmpty = false;
        }

        private PdfSharp.Drawing.XSize GetImageSize(System.Drawing.Bitmap bitmap) {
            float resolution = 72;
            PdfSharp.Drawing.XUnit imgWidthPt = PdfSharp.Drawing.XUnit.FromPoint(((float)bitmap.Width) * resolution / bitmap.HorizontalResolution);
            PdfSharp.Drawing.XUnit imgHeightPt = PdfSharp.Drawing.XUnit.FromPoint(((float)bitmap.Height) * resolution / bitmap.VerticalResolution);
            return new PdfSharp.Drawing.XSize(imgWidthPt, imgHeightPt);
        }

        /// <summary>Add an image</summary>
        /// <param name="image">Image object</param>
        /// <param name="bookmark">Optional - Bookmark</param>
        /// <param name="newpage">Optional - Add the image in a new page</param>
        public void addImage(Image image, string bookmark = null, bool newpage = false) {
            var img = (Image)image;
            try {
                using (var bitmap = img.Bitmap) {
                    PdfSharp.Drawing.XSize imgSize = GetImageSize(bitmap);
                    double scale = 1;

                    if (imgSize.Width > _area.Width) {
                        scale = _area.Width / imgSize.Width;
                        imgSize.Width *= scale;
                        imgSize.Height *= scale;
                    }

                    if (newpage)
                        this.addPage();
                    if (!String.IsNullOrEmpty(bookmark))
                        this.addBookmark(bookmark, false);
                    if (this.AvailableHeigth < 20)
                        this.addPage();
                    if (imgSize.Height < this.AvailableHeigth)
                        this.AddImageToPdf(bitmap, imgSize.Width, imgSize.Height);
                    else {
                        double scaledPointsToPixelsY = bitmap.VerticalResolution / scale / 72; // Points scaled -> Points original -> Pixels
                        double yPosPt = 0;
                        double leftHeightPt = imgSize.Height;
                        while (true) {
                            if (leftHeightPt < 1) break;
                            if (this.AvailableHeigth < 20) this.addPage();
                            double insertHeightPt;
                            if (leftHeightPt > this.AvailableHeigth)
                                insertHeightPt = this.AvailableHeigth;
                            else
                                insertHeightPt = leftHeightPt;

                            System.Drawing.Rectangle cropArea = new System.Drawing.Rectangle {
                                X = 0,
                                Y = (int)(yPosPt * scaledPointsToPixelsY),
                                Height = (int)(insertHeightPt * scaledPointsToPixelsY),
                                Width = bitmap.Width
                            };
                            using (System.Drawing.Bitmap bmpCrop = bitmap.Clone(cropArea, bitmap.PixelFormat))
                                this.AddImageToPdf(bmpCrop, imgSize.Width, insertHeightPt);
                            leftHeightPt = imgSize.Height - yPosPt - insertHeightPt;
                            if (leftHeightPt < 1) break;
                            yPosPt += insertHeightPt;
                            if (leftHeightPt > 10) {
                                leftHeightPt += 8;
                                yPosPt -= 8;
                            }
                        }
                    }
                }
            } catch (Exception ex) {
                throw new ApplicationException("addImage methode failed! " + ex.Message);
            }
        }

        private void AddImageToPdf(System.Drawing.Image image, double width_pt, double heigth_pt) {
            var rect = new System.Drawing.RectangleF {
                X = (float)_horizontalePosition,
                Y = (float)_verticalPosition,
                Width = (float)width_pt,
                Height = (float)heigth_pt
            };
            PdfSharp.Drawing.XImage xImage = PdfSharp.Drawing.XImage.FromGdiPlusImage(image);
            PdfSharp.Drawing.XPen pen = new PdfSharp.Drawing.XPen(PdfSharp.Drawing.XColors.Black, 1f);
            _graphics.DrawImage(xImage, rect);
            _graphics.DrawRectangle(pen, rect);
            _verticalPosition += heigth_pt;
            _isPageEmpty = false;
        }


    }

}
