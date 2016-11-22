using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace ReadingSpeedTester
{
    class TextUtils
    {
        public static TextPointer GetTextPointAt(TextPointer from, int pos)
        {
    
            TextPointer ret = from;
            int i = 0;

            while ((i < pos) && (ret != null))
            {
                if ((ret.GetPointerContext(LogicalDirection.Backward) == TextPointerContext.Text) || (ret.GetPointerContext(LogicalDirection.Backward) == TextPointerContext.None))
                    i++;

                if (ret.GetPositionAtOffset(1, LogicalDirection.Forward) == null)
                    return ret;

                ret = ret.GetPositionAtOffset(1, LogicalDirection.Forward);
            }

            return ret;
        }

        private static TextRange getTextRange(RichTextBox rtb,int offset, int length)
        {
            // Get text selection:
          

            // Get text starting point:
            TextPointer start = rtb.Document.ContentStart;

            // Get begin and end requested:
            TextPointer startPos = GetTextPointAt(start, offset);
            TextPointer endPos = GetTextPointAt(start, offset + length);
            TextRange textRange = new TextRange( startPos, endPos);
            // New selection of text:
            //textRange.Select(startPos, endPos);
            return textRange;
        }
        public static string colorizeTextBackground(RichTextBox rtb, int offset, int length, Color color)
        {
            TextRange textRange = getTextRange(rtb, offset, length);
            // Apply property to the selection:
            textRange.ApplyPropertyValue(TextElement.BackgroundProperty, new SolidColorBrush(color));

            // Return selection text:
            return rtb.Selection.Text;
        }
        public static string colorizeText(RichTextBox rtb, int offset, int length, Color color)
        {

            TextRange textRange = getTextRange(rtb, offset, length);
            // Apply property to the selection:
            textRange.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(color));

            // Return selection text:
            return rtb.Selection.Text;
        }

        public static void colorizeTextRange(RichTextBox rtb,TextRange textRange,Color color)
        {
            textRange.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(color));
        }

        public static int toCarretPosition(RichTextBox rtb)
        {
            var start = rtb.Document.ContentStart;
            var here = rtb.CaretPosition;
            var range = new TextRange(start, here);
            int indexInText = range.Text.Length;
            //Console.WriteLine("text left:"+here.GetTextInRun(LogicalDirection.Backward));
            //Console.WriteLine("text left:" + here.GetTextInRun(LogicalDirection.Forward));
            return indexInText;
            /*rtb.CaptureMouse();          
           TextPointer position = rtb.GetPositionFromPoint(pt, true);
            if (position == null) return null;
            var start = rtb.Document.ContentStart;
            var range = new TextRange(start, position);
            int indexInText = range.Text.Length;
            rtb.ReleaseMouseCapture();
            return indexInText; */
        }

        public static string textFromRichTextBox(RichTextBox rtb)
        {
            string richText = new TextRange(rtb.Document.ContentStart, rtb.Document.ContentEnd).Text;
            return richText;
        }

        public static void setRichTextBoxValue(RichTextBox rtb,string text)
        {
            rtb.Document.Blocks.Clear();
            rtb.Document.Blocks.Add(new Paragraph(new Run(text)));
        }

        public static string setRichTextBoxRtf(RichTextBox rtb, string rtfValue)
        {
            rtb.SelectAll();
            MemoryStream stream = new MemoryStream(ASCIIEncoding.Default.GetBytes(rtfValue));
            rtb.Selection.Load(stream, DataFormats.Rtf);
            TextRange textRange = new TextRange(
    // TextPointer to the start of content in the RichTextBox.
    rtb.Document.ContentStart,
    // TextPointer to the end of content in the RichTextBox.
    rtb.Document.ContentEnd
);
            return textRange.Text;
        }
    }
}
