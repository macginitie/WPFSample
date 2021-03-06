﻿using System;
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
        // this value can be arbitrarily chosen, given the rest of this code
        const double FontSize = 300.0;
        
        const int BGRectWidth = 300;
        const int BGRectHeight = 400;
        const int TargetHeight = 300;
        const int DPI = 96;

        static Brush _blackOutline;
        static Brush _redOutline;
        static double _strokeWidth;
        static FormattedText _text;
        static Geometry _textGeometry;

        static void Main(string[] args)
        {
            Initialize();
            CreateGeometry();

            DrawingVisual drawingVisual = new DrawingVisual();
            DrawingContext drawingContext = drawingVisual.RenderOpen();

            RenderGeometry(drawingContext, new Rect(0, 0, BGRectWidth, BGRectHeight));
            drawingContext.Close();

            // System.ArgumentException: Bgr24 not supported :-( Rgb24 not supported :-P
            // ... so I brute-forced a solution with the ConvertTo24bpp() method
            //RenderTargetBitmap bmp = new RenderTargetBitmap(BGRectWidth, BGRectHeight, 96, 96, PixelFormats.Bgr24);

            RenderTargetBitmap bmp = new RenderTargetBitmap(BGRectWidth, BGRectHeight, DPI, DPI, PixelFormats.Pbgra32);
            bmp.Render(drawingVisual);

            FileStream pngFile = new FileStream("LetterB.png", FileMode.Create);
            SaveAsPng(ConvertTo24bpp(bmp), pngFile);
        }

        static void Initialize()
        {
            _strokeWidth = TargetHeight * 0.09; // so it will be about 3% after dividing by 3
            // the black outline
            _blackOutline = new SolidColorBrush(Color.FromRgb(0x00, 0x00, 0x00));
            // the red outline
            _redOutline = new SolidColorBrush(Color.FromRgb(0xff, 0x00, 0x00));

            _text = new FormattedText(
                "B",
                new CultureInfo("en-us"),
                FlowDirection.LeftToRight,
                new Typeface(new FontFamily("Arial"), FontStyles.Normal, FontWeights.Normal, new FontStretch()),
                FontSize,
                System.Windows.Media.Brushes.Black // This brush does not matter since we use the geometry of the text.
            );
        }

        static void CreateGeometry()
        {
            // Build the geometry object that represents the text, offset to 10,10
            _textGeometry = _text.BuildGeometry(new System.Windows.Point(0,0));
            // try to fit exactly 300px height requirement as illustrated in the spec...
            double scaleXY = 300.0 / _textGeometry.GetWidenedPathGeometry(new Pen(_blackOutline, _strokeWidth)).Bounds.Height;
            // but the result was 295px, so:
            double ff = 0.025; // fudge factor
            _textGeometry.Transform = new ScaleTransform(scaleXY+ff, scaleXY+ff);
        }

        static void RenderGeometry(DrawingContext drawingContext, Rect bgRect)
        {
            Brush Fill = new SolidColorBrush(Color.FromRgb(0xff, 0xff, 0xff));
            // 1st fill the background
            drawingContext.DrawRectangle(Fill, null, bgRect);
            // the black outline
            drawingContext.DrawGeometry(Fill, new System.Windows.Media.Pen(_blackOutline, _strokeWidth), _textGeometry);
            // the red outline
            drawingContext.DrawGeometry(Fill, new System.Windows.Media.Pen(_redOutline, _strokeWidth / 3.0), _textGeometry);
        }

        // brute force ftw! :-/
        static BitmapSource ConvertTo24bpp(RenderTargetBitmap img)
        {
            byte[] srcPixelData = new byte[img.PixelWidth * img.PixelHeight * 4];
            byte[] destPixelData = new byte[img.PixelWidth * img.PixelHeight * 3];
            img.CopyPixels(srcPixelData, img.PixelWidth * 4, 0);
            int srcIndex = 0;
            int destIndex = 0;
            for (int i = 0; i < img.PixelWidth; ++i)
            {
                for (int j = 0; j < img.PixelHeight; ++j)
                {
                    destPixelData[destIndex + 2] = srcPixelData[srcIndex];
                    destPixelData[destIndex + 1]= srcPixelData[srcIndex + 1];
                    destPixelData[destIndex] = srcPixelData[srcIndex + 2];
                    destIndex += 3;
                    srcIndex += 4; // skip the alpha channel
                }
            }
            var bmp = BitmapSource.Create(
                img.PixelWidth,
                img.PixelHeight,
                DPI, DPI,
                PixelFormats.Rgb24,
                null, 
                destPixelData, 
                img.PixelWidth * 3);

            return bmp;
        }

        static void SaveAsPng(BitmapSource src, Stream outputStream)
        {
            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(src));
            encoder.Save(outputStream);
        }
    }
}
