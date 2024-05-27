using System.Drawing;


namespace Common.Helpers
{
    public class ImageConvertes
    {
        public static Image ConvertImage(Image img)
        {
            return (Image)(new Bitmap(img, new Size(20, 20)));
        }
    }
}
