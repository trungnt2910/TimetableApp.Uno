using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;

namespace TimetableApp.UI
{
    public static class MarkdownTextRenderer
    {
        const string ElementA = "A";
        const string ElementB = "B";
        const string ElementStrong = "STRONG";
        const string ElementI = "I";
        const string ElementEm = "EM";
        const string ElementU = "U";
        const string ElementBr = "BR";
        const string ElementP = "P";
        const string ElementLi = "LI";
        const string ElementUl = "UL";
        const string ElementDiv = "DIV";

        public static readonly DependencyProperty MarkdownTextProperty = DependencyProperty.Register(
          "MarkdownText",
          typeof(string),
          typeof(TextBlock),
          new PropertyMetadata(null, new PropertyChangedCallback(OnMarkdownTextChanged))
        );

        public static string GetMarkdownText(TextBlock obj)
        {
            return (string)obj.GetValue(MarkdownTextProperty);
        }

        public static void SetMarkdownText(TextBlock obj, string value)
        {
            obj.SetValue(MarkdownTextProperty, value);
        }


        private static void OnMarkdownTextChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            (sender as TextBlock).RenderText(args.NewValue as string);
        }

        private static void RenderText(this TextBlock block, string text)
        {
            text = text.ToHtml();

            // Just incase we are not given text with elements.
            string modifiedText = string.Format("<div>{0}</div>", text);

            block.Inlines.Clear();
            try
            {
                var element = XElement.Parse(modifiedText);
                ParseText(element, block.Inlines);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
                // if anything goes wrong just show the html
                block.Text = text;
            }
        }

        /// <summary>
        /// Traverses the XElement and adds text to the InlineCollection.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="inlines"></param>
        private static void ParseText(XElement element, InlineCollection inlines)
        {
            if (element == null) return;

            InlineCollection currentInlines = inlines;
            var elementName = element.Name.ToString().ToUpper();
            switch (elementName)
            {
                case ElementA:
                    var link = new Hyperlink();
                    var href = element.Attribute("href");
                    if (href != null)
                    {
                        try
                        {
                            link.NavigateUri = new Uri(href.Value);
                        }
                        catch (System.FormatException) { /* href is not valid */ }
                    }
                    inlines.Add(link);
                    currentInlines = link.Inlines;
                    break;
                case ElementB:
                case ElementStrong:
                    var bold = new Bold();
                    inlines.Add(bold);
                    currentInlines = bold.Inlines;
                    break;
                case ElementI:
                case ElementEm:
                    var italic = new Italic();
                    inlines.Add(italic);
                    currentInlines = italic.Inlines;
                    break;
                case ElementU:
                    var underline = new Underline();
                    inlines.Add(underline);
                    currentInlines = underline.Inlines;
                    break;
                case ElementBr:
                    inlines.Add(new LineBreak());
                    break;
                case ElementP:
                    // Add two line breaks, one for the current text and the second for the gap.
                    if (AddLineBreakIfNeeded(inlines))
                    {
                        inlines.Add(new LineBreak());
                    }

                    Span paragraphSpan = new Span();
                    inlines.Add(paragraphSpan);
                    currentInlines = paragraphSpan.Inlines;
                    break;
                case ElementLi:
                    inlines.Add(new LineBreak());
                    inlines.Add(new Run { Text = " • " });
                    break;
                case ElementUl:
                case ElementDiv:
                    AddLineBreakIfNeeded(inlines);
                    Span divSpan = new Span();
                    inlines.Add(divSpan);
                    currentInlines = divSpan.Inlines;
                    break;
            }
            foreach (var node in element.Nodes())
            {
                XText textElement = node as XText;
                if (textElement != null)
                {
                    currentInlines.Add(new Run { Text = textElement.Value });
                }
                else
                {
                    ParseText(node as XElement, currentInlines);
                }
            }
            // Add newlines for paragraph tags
            if (elementName == ElementP)
            {
                currentInlines.Add(new LineBreak());
            }
        }
        /// <summary>
        /// Check if the InlineCollection contains a LineBreak as the last item.
        /// </summary>
        /// <param name="inlines"></param>
        /// <returns></returns>
        private static bool AddLineBreakIfNeeded(InlineCollection inlines)
        {
            if (inlines.Count > 0)
            {
                var lastInline = inlines[inlines.Count - 1];
                while ((lastInline is Span))
                {
                    var span = (Span)lastInline;
                    if (span.Inlines.Count > 0)
                    {
                        lastInline = span.Inlines[span.Inlines.Count - 1];
                    }
                }
                if (!(lastInline is LineBreak))
                {
                    inlines.Add(new LineBreak());
                    return true;
                }
            }
            return false;
        }
    }
}
