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
            // hard-coded magic letter?
            CreateText("B");
            DefaultStyleKeyProperty.OverrideMetadata(typeof(OutlineText), new FrameworkPropertyMetadata(typeof(OutlineText)));
        }

        static Geometry _textGeometry;

        static double _fontSize = 400.0;
        public static void SetFontSize(double newSize)
        {
            _fontSize = newSize;
        }

        static double _strokeWidth = 24.0;
        public static void SetStrokeWidth(double newWidth)
        {
            _strokeWidth = newWidth;
        }

        /// <summary>
        /// Create the outline geometry based on the formatted text.
        /// </summary>
        public static void CreateText(string Text, FontFamily Font = null, bool Bold = false, bool Italic = false)
        {
            System.Windows.FontStyle fontStyle = FontStyles.Normal;
            FontWeight fontWeight = FontWeights.Medium;

            if (Bold == true) fontWeight = FontWeights.Bold;
            if (Italic == true) fontStyle = FontStyles.Italic;
            if (null == Font) Font = new FontFamily("Arial");

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
                _fontSize,
                System.Windows.Media.Brushes.Black // This brush does not matter since we use the geometry of the text. 
                );

            // Build the geometry object that represents the text.
            _textGeometry = formattedText.BuildGeometry(new System.Windows.Point(0, 0));
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
            drawingContext.DrawGeometry(Fill, new System.Windows.Media.Pen(Stroke, _strokeWidth), _textGeometry);
            // the red outline
            Stroke = new SolidColorBrush(Color.FromRgb(0xff, 0x00, 0x00));
            drawingContext.DrawGeometry(Fill, new System.Windows.Media.Pen(Stroke, _strokeWidth/3.0), _textGeometry);
        }
    }
}
