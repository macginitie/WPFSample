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
    /// <summary>
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:WpfTextRender"
    ///
    ///
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:WpfTextRender;assembly=WpfTextRender"
    ///
    /// You will also need to add a project reference from the project where the XAML file lives
    /// to this project and Rebuild to avoid compilation errors:
    ///
    ///     Right click on the target project in the Solution Explorer and
    ///     "Add Reference"->"Projects"->[Browse to and select this project]
    ///
    ///
    /// Step 2)
    /// Go ahead and use your control in the XAML file.
    ///
    ///     <MyNamespace:OutlineText/>
    ///
    /// </summary>
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
                                      double FontSize = 240.0, bool Highlight = false)
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
                FontSize,
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
            Brush Stroke = new SolidColorBrush(Color.FromRgb(0x00, 0x00, 0x00));
            Brush Fill = new SolidColorBrush(Color.FromRgb(0xff, 0xff, 0xff));
            double StrokeThickness = 20.0;
            // Draw the black outline based on the properties that are set.
            drawingContext.DrawGeometry(Fill, new System.Windows.Media.Pen(Stroke, StrokeThickness), _textGeometry);
            StrokeThickness /= 4.0;
            Stroke = new SolidColorBrush(Color.FromRgb(0xff, 0x00, 0x00));
            // Draw the red outline based on the properties that are set.
            drawingContext.DrawGeometry(Fill, new System.Windows.Media.Pen(Stroke, StrokeThickness), _textGeometry);
        }
    }
}
