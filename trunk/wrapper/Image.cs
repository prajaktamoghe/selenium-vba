using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace SeleniumWrapper {

    [Guid("72B52B1B-3D23-4272-B972-54BA7C175990")]
    [ComVisible(true), InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface IImage {
        [Description("Returns true if the image is null, false otherwise")]
        bool IsNull { get; }

        [Description("Save as a PNG image file")]
        void SaveAs(string filePath);

        [Description("Copy the image to the clipboard")]
        void Copy();

        [Description("Returns the image MD5 hash signature")]
        string Signature { get; }

        [Description("Compare this image to another image(path or image) and return the result")]
        Image CompareTo(Object image, bool center = false, int offsetX = 0, int offsetY = 0, double scaleX = 1, double scaleY = 1);

        [Description("Image width")]
        int Width { get; }

        [Description("Image height")]
        int Height { get; }

        [Description("Number of unmatching pixels resulting of a comparison")]
        double DiffCount { get; }

        [Description("Dispose the image resources")]
        void Dispose();
    }

    /// <summary>
    /// Screenshot of a web page or web element.
    /// </summary>

    [Description("Screenshot of a web page or web element.")]
    [Guid("3968EF67-AB74-4ADD-82E7-2853F52DAD14")]
    [ComVisible(true), ComDefaultInterface(typeof(IImage)), ClassInterface(ClassInterfaceType.None)]
    public class Image : IDisposable, IImage {
        private readonly MemoryStream _stream;
        private string signature = null;
        private System.Drawing.Bitmap _bitmap;
        private double _diffCount = 0;

        internal Image() {
            _stream = null;
        }

        public void Dispose() {
            if (!IsNull)
                _stream.Dispose();
            if (_bitmap != null)
                _bitmap.Dispose();
        }

        public int Width {
            get { return Bitmap.Width; }
        }

        public int Height {
            get { return Bitmap.Height; }
        }


        /// <summary>
        /// Number of unmatching pixels resulting of a comparis
        /// </summary>
        public double DiffCount {
            get{return _diffCount;}
        }

        internal Image(byte[] imageBytes) {
            if (imageBytes == null || imageBytes.Length == 0)
                throw new ApplicationException("Method <Copy> failed !\nScreenshot is empty");
            _stream = new MemoryStream(imageBytes);
        }

        public Image(Bitmap image) {
            _stream = new MemoryStream();
            image.Save(_stream, ImageFormat.Png);
        }

        /// <summary>Return true if the image is null, false otherwise</summary>
        public bool IsNull {
            get { return _stream == null || _stream.Length == 0; }
        }

        /// <summary>Get the image bytes</summary>
        public byte[] Bytes {
            get { return IsNull ? null : _stream.ToArray(); }
        }

        /// <summary>Get the image bytes</summary>
        public Stream Stream {
            get {
                if (IsNull)
                    throw new Exception("Image is null");
                return _stream;
            }
        }

        /// <summary>Get the image bytes</summary>
        public Bitmap Bitmap {
            get {
                if (IsNull)
                    throw new Exception("Image is null");
                if (_bitmap == null)
                    _bitmap = (Bitmap)Bitmap.FromStream(_stream);
                return _bitmap;
            }
        }

        /// <summary>Save the image to the provided path as a PNG image.</summary>
        /// <param name="filePath">PNG file path. Ex : C:\capture01.png</param>
        public void SaveAs(string filePath) {
            using (var img = Bitmap.FromStream(_stream))
                img.Save(filePath, ImageFormat.Png);
        }

        /// <summary>Copy the image to the Clipboard.</summary>
        public void Copy() {
            try {
                using (var image = Bitmap) {
                    System.Windows.Forms.Clipboard.Clear();
                    System.Windows.Forms.Clipboard.SetImage(image);
                }
            } catch (Exception ex) {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>Compare to the provided image</summary>
        /// <param name="image">Image path or Image object</param>
        /// <param name="center">Center the image B horizontally on image A</param>
        /// <param name="offsetX">Translates image B horizontally by the specified amount.</param>
        /// <param name="offsetY">Translates image B vertically by the specified amount.</param>
        /// <param name="scaleX">Resize image B horizontally.</param>
        /// <param name="scaleY">Resize image B vertically.</param>
        /// <returns>Output - Image representing all the differences</returns>
        public Image CompareTo(Object image, bool center = false, int offsetX = 0, int offsetY = 0, double scaleX = 1, double scaleY = 1) {
            Bitmap imgB = null;
            if (image is string) {
                var imagePath = image as string;
                if (!File.Exists(imagePath))
                    throw new Exception("File not found. Path:" + imagePath);
                imgB = new Bitmap(imagePath);
            } else if (image is Image) {
                imgB = (image as Image).Bitmap;
            } else {
                throw new Exception("Invalide argument exception. A string or an Image object is required for the image argument.");
            }
            using (imgB)
            using (var imgA = this.Bitmap) {
                Bitmap imgDiff;
                double error = CompareBitmaps(imgA, imgB, out imgDiff, center, offsetX, offsetY, scaleX, scaleY);
                return new SeleniumWrapper.Image(imgDiff) { _diffCount = error };
            }
        }

        /// <summary>Returns the image MD5 hash signature</summary>
        /// <returns></returns>
        public string Signature {
            get {
                if (this.signature != null) return this.signature;
                using (System.Security.Cryptography.MD5 md5Hasher = System.Security.Cryptography.MD5.Create()) {
                    md5Hasher.ComputeHash(this.Bytes);
                    this.signature = Convert.ToBase64String(md5Hasher.Hash);
                    return this.signature;
                }
            }
        }

        /// <summary>Compare two images</summary>
        /// <param name="imageA">First image</param>
        /// <param name="imageB">Second image</param>
        /// <param name="imageDiff">Output - Image representing all the differences</param>
        /// <param name="center">Center the image B horizontally on image A</param>
        /// <param name="offsetX">Translates image B horizontally by the specified amount.</param>
        /// <param name="offsetY">Translates image B vertically by the specified amount.</param>
        /// <param name="scaleX">Resize image B horizontally.</param>
        /// <param name="scaleY">Resize image B vertically.</param>
        /// <returns>Ratio of non matching pixels between 0 and 1</returns>
        public static int CompareBitmaps(Bitmap imageA, Bitmap imageB, out Bitmap imageDiff, bool center = false, int offsetX = 0, int offsetY = 0, double scaleX = 1, double scaleY = 1) {
            if (scaleX != 1 || scaleY != 1)
                imageB = new System.Drawing.Bitmap(imageB, (int)((double)imageB.Width * scaleX), (int)((double)imageB.Height * scaleY));
            if (center)
                offsetX += (imageA.Width - imageB.Width) / 2;
            var rectA = new Rectangle(offsetX < 0 ? -offsetX : 0, offsetY < 0 ? -offsetY : 0, imageA.Width, imageA.Height);
            var rectB = new Rectangle(offsetX > 0 ? offsetX : 0, offsetY > 0 ? offsetY : 0, imageB.Width, imageB.Height);
            var rectI = Rectangle.Intersect(rectA, rectB);
            var rectDiff = Rectangle.Union(rectA, rectB);
            imageDiff = new System.Drawing.Bitmap(rectDiff.Width, rectDiff.Height);
            BitmapData dataA = null, dataB = null, dataR = null, dataDiff = null;
            Rectangle rectR;

            int pixelDiffCount = 0;
            unsafe {
                dataA = imageA.LockBits(new Rectangle(System.Drawing.Point.Empty, rectA.Size), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
                dataB = imageB.LockBits(new Rectangle(System.Drawing.Point.Empty, rectB.Size), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
                dataDiff = imageDiff.LockBits(new Rectangle(System.Drawing.Point.Empty, rectDiff.Size), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                try {
                    byte* ptrA, ptrB, ptrR, ptrDiff;
                    int padA, padB, padR, padDiff;

                    //Writes the difference between image A and B where they intersect
                    int width = rectI.Width;
                    int height = rectI.Height;
                    ptrA = (byte*)dataA.Scan0 + rectB.Top * dataA.Stride + rectB.Left * 3;
                    ptrB = (byte*)dataB.Scan0 + rectA.Top * dataB.Stride + rectA.Left * 3;
                    ptrDiff = (byte*)dataDiff.Scan0 + rectI.Y * dataDiff.Stride + rectI.X * 3;
                    padA = dataA.Stride - rectI.Width * 3;
                    padB = dataB.Stride - rectI.Width * 3;
                    padDiff = dataDiff.Stride - rectI.Width * 3;
                    for (int y = 0; y < height; y++, ptrA += padA, ptrB += padB, ptrDiff += padDiff) {
                        for (int x = 0; x < width; x++, ptrA += 3, ptrB += 3, ptrDiff += 3) {
                            ptrDiff[0] = (byte)(ptrA[0] > ptrB[0] ? ptrA[0] - ptrB[0] : ptrB[0] - ptrA[0]);
                            ptrDiff[1] = (byte)(ptrA[1] > ptrB[1] ? ptrA[1] - ptrB[1] : ptrB[1] - ptrA[1]);
                            ptrDiff[2] = (byte)(ptrA[2] > ptrB[2] ? ptrA[2] - ptrB[2] : ptrB[2] - ptrA[2]);
                            if ((ptrDiff[0] + ptrDiff[1] + ptrDiff[2]) > 0)
                                pixelDiffCount++;
                        }
                    }

                    //Writes what's left on the top
                    if (rectI.Top != 0) {
                        dataR = rectA.Top == 0 ? dataA : dataB;
                        rectR = rectA.Top == 0 ? rectA : rectB;
                        width = rectR.Width;
                        height = rectI.Y;
                        ptrR = (byte*)dataR.Scan0;
                        ptrDiff = (byte*)dataDiff.Scan0 + rectR.Left * 3;
                        padR = dataR.Stride - width * 3;
                        padDiff = dataDiff.Stride - width * 3;
                        pixelDiffCount += ProcessImageData(ptrR, padR, ptrDiff, padDiff, width, height);
                    }

                    //Writes what's left on the left
                    if (rectI.Left != rectDiff.Left) {
                        dataR = rectA.Left == 0 ? dataA : dataB;
                        rectR = rectA.Left == 0 ? rectA : rectB;
                        width = rectI.X;
                        height = rectR.Height;
                        ptrR = (byte*)dataR.Scan0;
                        ptrDiff = (byte*)dataDiff.Scan0 + rectR.Top * dataDiff.Stride;
                        padR = dataR.Stride - width * 3;
                        padDiff = dataDiff.Stride - width * 3;
                        pixelDiffCount += ProcessImageData(ptrR, padR, ptrDiff, padDiff, width, height);
                    }

                    //Writes what's left on the right
                    if (rectI.Right != rectDiff.Right) {
                        dataR = rectA.Right == rectDiff.Right ? dataA : dataB;
                        rectR = rectA.Right == rectDiff.Right ? rectA : rectB;
                        width = (rectR.Right - rectI.Right) * 3;
                        height = rectR.Bottom - rectI.Top;
                        ptrR = (byte*)dataR.Scan0 + (rectI.Top - rectR.Top) * dataR.Stride + (rectI.Right - rectR.Left) * 3;
                        ptrDiff = (byte*)dataDiff.Scan0 + rectI.Top * dataDiff.Stride + rectI.Right * 3;
                        padR = dataR.Stride - width * 3;
                        padDiff = dataDiff.Stride - width * 3;
                        pixelDiffCount += ProcessImageData(ptrR, padR, ptrDiff, padDiff, width, height);
                    }

                    //Writes what's left on the bottom
                    if (rectI.Bottom != rectDiff.Bottom) {
                        dataR = rectA.Bottom == rectDiff.Bottom ? dataA : dataB;
                        rectR = rectA.Bottom == rectDiff.Bottom ? rectA : rectB;
                        width = rectI.Width;
                        height = rectR.Bottom - rectI.Bottom;
                        ptrR = (byte*)dataR.Scan0 + (rectI.Bottom - rectR.Top) * dataR.Stride + (rectI.Left - rectR.Left) * 3;
                        ptrDiff = (byte*)dataDiff.Scan0 + rectI.Bottom * dataDiff.Stride + rectI.Left * 3;
                        padR = dataR.Stride - width * 3;
                        padDiff = dataDiff.Stride - width * 3;
                        pixelDiffCount += ProcessImageData(ptrR, padR, ptrDiff, padDiff, width, height);
                    }
                } finally {
                    if (dataA != null) imageA.UnlockBits(dataA);
                    if (dataB != null) imageB.UnlockBits(dataB);
                    if (dataDiff != null) imageDiff.UnlockBits(dataDiff);
                }
            }
            return pixelDiffCount;
        }

        unsafe static private int ProcessImageData(byte* ptrR, int padR, byte* ptrW, int padW, int width, int height) {
            int unmatchingPixels = 0;
            for (int y = 0; y < height; y++, ptrR += padR, ptrW += padW) {
                for (int x = 0; x < width; x++, ptrR += 3, ptrW += 3) {
                    ptrW[0] = (byte)(byte.MaxValue - ptrR[0]);
                    ptrW[1] = (byte)(byte.MaxValue - ptrR[1]);
                    ptrW[2] = (byte)(byte.MaxValue - ptrR[2]);
                    if ((ptrW[0] + ptrW[1] + ptrW[2]) != 0)
                        unmatchingPixels++;
                }
            }
            return unmatchingPixels;
        }
    }



}
