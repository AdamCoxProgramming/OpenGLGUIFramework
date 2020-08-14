/*
using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;
using static Layout.LayoutBaseClasses;
using static Layout.LayoutImplementations;
using static Layout.ReactiveLayout;

namespace Layout.Layouts
{
    public class ImageLayout : PerantBoundLayout
    {
        public BitmapImage bitmap;
        
        public void setImgSrc(Bitmap bitmap)
        {
            this.bitmap = BitmapToImageSource(bitmap);
        }

        private BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }

        public override CalculatedLayoutItem getCalculatedItem(Bounds perantBounds)
        {
            CalculatedLayoutItem calculatedItem = base.getCalculatedItem(perantBounds);
            calculatedItem.type = "ImageLayout";
            calculatedItem.metaData = this;
            calculatedItem.reactiveView = this;
            calculatedItem.reactiveView.isContainer = false;
            return calculatedItem;
        }

        public override MotionHandleResult handleMotionEvent(MotionEvent motionEvent)
        {
            return new MotionHandleResult(motionEvent,false);
        }
    }
}
*/