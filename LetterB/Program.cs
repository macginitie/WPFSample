using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace LetterB
{
    class Program
    {
        const double FontSize = 400.0;
        const double StrokeWidth = 24.0;
        static Geometry _textGeometry;

        static void Main(string[] args)
        {
            FormattedText text = new FormattedText("B",
                new CultureInfo("en-us"),
                FlowDirection.LeftToRight,
                new Typeface(new FontFamily("Arial"), FontStyles.Normal, FontWeights.Normal, new FontStretch()),
                FontSize,
                System.Windows.Media.Brushes.Black // This brush does not matter since we use the geometry of the text.
                );

            // Build the geometry object that represents the text.
            _textGeometry = text.BuildGeometry(new System.Windows.Point(0, 0));

            DrawingVisual drawingVisual = new DrawingVisual();
            DrawingContext drawingContext = drawingVisual.RenderOpen();

            RenderGeometry(drawingContext);
            drawingContext.Close();

            RenderTargetBitmap bmp = new RenderTargetBitmap(300, 400, 96, 96, PixelFormats.Pbgra32);
            bmp.Render(drawingVisual);

            FileStream pngFile = new FileStream("LetterB.png", FileMode.Create);
            SaveAsPng(bmp, pngFile);
        }

        static void RenderGeometry(DrawingContext drawingContext)
        {
            // the black outline
            Brush Stroke = new SolidColorBrush(Color.FromRgb(0x00, 0x00, 0x00));
            Brush Fill = new SolidColorBrush(Color.FromRgb(0xff, 0xff, 0xff));
            drawingContext.DrawGeometry(Fill, new System.Windows.Media.Pen(Stroke, StrokeWidth), _textGeometry);
            // the red outline
            Stroke = new SolidColorBrush(Color.FromRgb(0xff, 0x00, 0x00));
            drawingContext.DrawGeometry(Fill, new System.Windows.Media.Pen(Stroke, StrokeWidth / 3.0), _textGeometry);
        }

        //public static RenderTargetBitmap GetImage(Control view)
        //{
        //    Size size = new Size(view.ActualWidth, view.ActualHeight);
        //    if (size.IsEmpty)
        //        return null;

        //    RenderTargetBitmap result = new RenderTargetBitmap((int)size.Width, (int)size.Height, 96, 96, PixelFormats.Pbgra32);

        //    DrawingVisual drawingvisual = new DrawingVisual();
        //    using (DrawingContext context = drawingvisual.RenderOpen())
        //    {
        //        context.DrawRectangle(new VisualBrush(view), null, new Rect(new Point(), size));
        //        context.Close();
        //    }

        //    result.Render(drawingvisual);
        //    return result;
        //}

        static void SaveAsPng(BitmapSource src, Stream outputStream)
        {
            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(src));
            encoder.Save(outputStream);
        }
    }
}
