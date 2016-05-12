using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CP.Common
{
    public sealed class ImageHelper
    {
        private static readonly ImageCodecInfo PhotoCodecInfo = GetEncoderInfo("image/jpeg");
        public static void GetPicSize(String picPath, out Int32 pWidth, out Int32 pHeight)
        {
            Image myOriPic = null;

            try
            {
                myOriPic = Image.FromFile(picPath);
                pWidth = myOriPic.Width;
                pHeight = myOriPic.Height;
                myOriPic.Dispose();
            }
            catch
            {
                pWidth = 0;
                pHeight = 0;
            }
        }
        /// <summary>
        /// 生成缩略图
        /// </summary>
        /// <param name="oSideLen">需求图片尺寸</param>
        /// <param name="oriFilePath">原图路径</param>
        /// <param name="nWidth">输出参数宽</param>
        /// <returns>返回文件存储路径，非URL地址</returns>
        public static void GetThumbnail(Int32 oSideLen, String oriFilePath, String desFileName, string format, out Int32 nWidth, out Int32 nHeight)
        {
            Bitmap myOriPic = null;
            Bitmap myDesPic = null;
            Graphics g = null;
            bool reDraw = true;
            try
            {
                myOriPic = new Bitmap(oriFilePath);
                Single oWidth = (Single)myOriPic.Width;
                Single oHeight = (Single)myOriPic.Height;
                Single oRatio = oWidth / oHeight;

                //需求图片尺寸 > 原图片尺寸  
                if (oSideLen >= oWidth && oSideLen >= oHeight)
                {
                    nWidth = (int)oWidth;
                    nHeight = (int)oHeight;

                    if (format == "gif")
                    {
                        reDraw = false;
                        //复制原图
                        myOriPic.Save(desFileName);
                    }
                }
                else
                {
                    nWidth = (Int32)((oRatio >= 1) ? oSideLen : oSideLen * oRatio);
                    nHeight = (Int32)((oRatio <= 1) ? oSideLen : oSideLen / oRatio);
                }

                //生成与原图同等尺寸图片
                if (reDraw)
                {
                    myDesPic = new Bitmap(nWidth, nHeight);
                    g = Graphics.FromImage(myDesPic);
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    //清空画布并以透明背景色填充
                    //g.Clear(System.Drawing.Color.Transparent);
                    g.DrawImage(myOriPic, new Rectangle(0, 0, nWidth, nHeight), new RectangleF(0, 0, oWidth, oHeight), GraphicsUnit.Pixel);


                    EncoderParameters encoderParams = new EncoderParameters(1);
                    encoderParams.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 90L);
                    myDesPic.Save(desFileName, PhotoCodecInfo, encoderParams);
                }
            }
            catch (Exception e)
            {
                nWidth = 0;
                nHeight = 0;
            }
            finally
            {
                if (g != null) g.Dispose();
                if (myDesPic != null) myDesPic.Dispose();
                if (myOriPic != null) myOriPic.Dispose();
            }
        }
        private static ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            for (Int32 j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                    return encoders[j];
            }
            return null;
        }
    }
}
