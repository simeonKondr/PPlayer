using System.Drawing;

namespace PPlayer
{
    class Program
    {
        static void Main(string[] args)
        {

        }

        private void SaveScreenShot(string filename, ImageFormat format) // сохраняем скриншот экрана 
        {
            Bitmap screenShot = CaptureScreenShot();
            screenShot.Save(filename, format);
        }

        private Bitmap CaptureScreenShot() // делаем скриншот экрана 
        {
            Rectangle bounds = Screen.GetBounds(Point.Empty);
            Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height);
            using (Graphics gr = Graphics.FromImage(bitmap))
            {
                gr.CopyFromScreen(Point.Empty, Point.Empty, bounds.Size);
            }
            return bitmap;
        }
    }
}
