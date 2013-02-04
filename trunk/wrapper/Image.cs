using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace SeleniumWrapper
{

    [Guid("72B52B1B-3D23-4272-B972-54BA7C175990")]
    [ComVisible(true)]
    public interface IImage
    {
        [Description("Save as a PNG image file")]
        void SaveAs(string filePath);

        [Description("Raw image")]
        byte[] Raw{get;set;}

        [Description("Copy the image to the clipboard")]
        void Copy();

        [Description("Returns the image MD5 hash signature")]
        string Signature{get;}
    }


    [Description("Screenshot of a web page")]
    [Guid("3968EF67-AB74-4ADD-82E7-2853F52DAD14")]
    [ComVisible(true), ComDefaultInterface(typeof(IImage)), ClassInterface(ClassInterfaceType.None)]
    public class Image : IImage
    {
        internal byte[] imageBytes;
        private string signature = null;

        public Image(){}

        internal Image(byte[] imageBytes)
        {
            if (imageBytes == null || imageBytes.Length == 0) throw new ApplicationException("Method <Copy> failed !\nScreenshoot is empty");
            this.imageBytes = imageBytes;
        }

        /// <summary>Set an image</summary>
        public byte[] Raw
        {
            get{ return this.imageBytes;}
            set{ this.imageBytes = value;}
        }

        /// <summary>Save the screenshot to the provided path as a PNG image.</summary>
        /// <param name="filePath">PNG file path. Ex : C:\capture01.png</param>
        public void SaveAs(string filePath)
        {
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream(this.imageBytes)){
                System.Drawing.Image.FromStream(ms).Save(filePath, System.Drawing.Imaging.ImageFormat.Png);
            }
        }

        /// <summary>Copy the screenshot to the Clipboard.</summary>
        public void Copy()
        {
            try{
                using (System.IO.MemoryStream ms = new System.IO.MemoryStream(this.imageBytes)){
                    System.Drawing.Image image = System.Drawing.Image.FromStream(ms);
                    System.Windows.Forms.Clipboard.Clear();
                    System.Windows.Forms.Clipboard.SetImage(image);
                }
            }catch(Exception ex){
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>Returns the image MD5 hash signature</summary>
        /// <returns></returns>
        public string Signature
        {
            get{
                if(this.signature != null ) return this.signature;
                using(System.Security.Cryptography.MD5 md5Hasher = System.Security.Cryptography.MD5.Create()){
                    md5Hasher.ComputeHash(this.imageBytes);
                    this.signature = Convert.ToBase64String(md5Hasher.Hash);
                    return this.signature;
                }
            }
        }

    }
}
