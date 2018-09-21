using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfTextRender
{
    public class OutlineText : Control
    {
        static OutlineText()
        {
            CreateText("B");
            DefaultStyleKeyProperty.OverrideMetadata(typeof(OutlineText), new FrameworkPropertyMetadata(typeof(OutlineText)));
        }

        static Geometry _textGeometry;

        /// <summary>
        /// Create the outline geometry based on the formatted text.
        /// </summary>
        public static void CreateText(string Text, FontFamily Font = null, bool Bold = false, bool Italic = false,
                                      double FontSize = 359.0, bool Highlight = false)
        {
            System.Windows.FontStyle fontStyle = FontStyles.Normal;
            FontWeight fontWeight = FontWeights.Medium;

            if (Bold == true) fontWeight = FontWeights.Bold;
            if (Italic == true) fontStyle = FontStyles.Italic;
            if (null == Font) Font = new FontFamily("Arial");

            // this loop brute forces the size (i.e, the conversion from FontSize to 300px height)
            do
            {
                // Create the formatted text based on the properties set.
                FormattedText formattedText = new FormattedText(
                    Text,
                    CultureInfo.GetCultureInfo("en-us"),
                    FlowDirection.LeftToRight,
                    new Typeface(
                        Font,
                        fontStyle,
                        fontWeight,
                        FontStretches.Normal),
                    FontSize,
                    System.Windows.Media.Brushes.Black // This brush does not matter since we use the geometry of the text. 
                    );

                // Build the geometry object that represents the text.
                _textGeometry = formattedText.BuildGeometry(new System.Windows.Point(0, 0));
                FontSize += 0.1;
            } while (_textGeometry.Bounds.Bottom - _textGeometry.Bounds.Top < 260); // magic # = 300 - 20*2 [stroke width top+bottom]
        }

        /// <summary>
        /// OnRender override draws the geometry of the text and optional highlight.
        /// </summary>
        /// <param name="drawingContext">Drawing context of the OutlineText control.</param>
        protected override void OnRender(DrawingContext drawingContext)
        {
            // the black outline
            Brush Stroke = new SolidColorBrush(Color.FromRgb(0x00, 0x00, 0x00));
            Brush Fill = new SolidColorBrush(Color.FromRgb(0xff, 0xff, 0xff));
            double StrokeThickness = 22.0;
            drawingContext.DrawGeometry(Fill, new System.Windows.Media.Pen(Stroke, StrokeThickness), _textGeometry);
            // the red outline
            StrokeThickness /= 3.0; // half on each side
            Stroke = new SolidColorBrush(Color.FromRgb(0xff, 0x00, 0x00));
            drawingContext.DrawGeometry(Fill, new System.Windows.Media.Pen(Stroke, StrokeThickness), _textGeometry);
        }
    }
}
