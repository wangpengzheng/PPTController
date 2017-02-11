using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PPTControllerHost
{
    public class Utils
    {
        public static string getCurComputerName()
        {
            IPHostEntry host;
            host = Dns.GetHostEntry(Dns.GetHostName());

            return host.HostName;
        }

        public static Bitmap CopyPrimaryScreen()
        {
            Screen s = Screen.PrimaryScreen;
            Rectangle r = s.Bounds;
            int w = r.Width;
            int h = r.Height;
            Bitmap bmp = new Bitmap(w, h);
            Graphics g = Graphics.FromImage(bmp);
            g.CopyFromScreen
            (
            new Point(0, 0),
            new Point(0, 0),
            new Size(w, h)
            );
            return bmp;
        }

        public static byte[] GetCopyPrimaryScreenAsBytes()
        {

            Bitmap screen = Utils.CopyPrimaryScreen();
            MemoryStream ms = new MemoryStream();
            //screen.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            EncoderParameter p;
            EncoderParameters ps;
            ps = new EncoderParameters(1);
            p = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 5L);
            ps.Param[0] = p;
            screen.Save(ms, GetCodecInfo("image/jpeg"), ps);

            byte[] len = System.BitConverter.GetBytes(ms.Length);
            byte[] result = new byte[len.Length + ms.Length];
            len.CopyTo(result, 0);
            ms.ToArray().CopyTo(result, len.Length);
            ms.Close();

            return result;



        }
        private static ImageCodecInfo GetCodecInfo(string mimeType)
        {
            ImageCodecInfo[] CodecInfo = ImageCodecInfo.GetImageEncoders();
            foreach (ImageCodecInfo ici in CodecInfo)
            {
                if (ici.MimeType == mimeType) return ici;
            }
            return null;
        }
    }
}
