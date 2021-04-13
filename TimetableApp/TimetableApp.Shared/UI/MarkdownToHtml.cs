using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace TimetableApp.UI
{
    public static class MarkdownToHtml
    {
        #region MARKDOWN STYLES
        private const string ORIGINAL_PATTERN_BEGIN = "<code>";
        private const string ORIGINAL_PATTERN_END = "</code>";
        private const string PARSED_PATTERN_BEGIN = "<font color=\"#888888\" face=\"monospace\"><tt>";
        private const string PARSED_PATTERN_END = "</tt></font>";

        #endregion

        public static string ToHtml(this string markdownText)
        {
            var markdownOptions = new MarkdownOptions
            {
                AutoHyperlink = true,
                AutoNewlines = false,
                EncodeProblemUrlCharacters = false,
                LinkEmails = true,
                StrictBoldItalic = true,
                EmptyElementSuffix = "/>"
            };
            var markdown = new Markdown(markdownOptions);
            var htmlContent = markdown.Transform(markdownText);
            //var regex = new Regex("\n");
            //htmlContent = regex.Replace(htmlContent, "<br/>");

            var html = htmlContent.HtmlWrapped();
            //var regex2 = new Regex("\r");
            //html = regex.Replace(html, string.Empty);
            //html = regex2.Replace(html, string.Empty);
            return html;
        }
 
        /// <summary>
        /// Wrap html with a full html tag
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static string HtmlWrapped(this string html)
        {
            if (!html.StartsWith("<html>") || !html.EndsWith("</html>"))
            {
                html = $"<html><body>{html}</body></html>";
            }
            return html;
        }

        ///<summary>
        /// Parses html with code or pre tags and gives them proper
        /// styled spans so that Android can parse it properly
        /// </summary>
        /// <param name="htmlText">The html string</param>
        /// <returns>The html string with parsed code tags</returns>
        public static string ParseCodeTags(this string htmlText)
        {
            if (htmlText.IndexOf(ORIGINAL_PATTERN_BEGIN) < 0) return htmlText;
            var regex = new Regex(ORIGINAL_PATTERN_BEGIN);
            var regex2 = new Regex(ORIGINAL_PATTERN_END);

            htmlText = regex.Replace(htmlText, PARSED_PATTERN_BEGIN);
            htmlText = regex2.Replace(htmlText, PARSED_PATTERN_END);
            htmlText = htmlText.TrimLines();
            return htmlText;
        }

        public static bool EqualsIgnoreCase(this string text, string text2)
        {
            return text.Equals(text2, StringComparison.CurrentCultureIgnoreCase);
        }

        public static string ReplaceBreaks(this string html)
        {
            var regex = new Regex("<br/>");
        
            html = regex.Replace(html, "\n");
            return html;
        }

        public static string ReplaceBreaksWithSpace(this string html)
        {
            var regex = new Regex("<br/>");
        
            html = regex.Replace(html, " ");
            return html;
        }

        public static string TrimLines(this string originalString)
        {
            originalString = originalString.Trim('\n');
            return originalString;
        }
    }
}
