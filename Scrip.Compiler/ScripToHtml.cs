using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;

namespace Scrip.Compiler
{
    public class ScripToHtml : IScripListener
    {
        private readonly StreamWriter _streamWriter;
        private readonly string _folder;

        public static void ConvertToHtml(string scripFilePath)
        {
            var lexer = new ScripLexer(new AntlrFileStream(scripFilePath));

            var tokens = new CommonTokenStream(lexer);

            var parser = new ScripParser(tokens);
            var tree = parser.paragraphs();

            var walker = new ParseTreeWalker();
            var htmlFileName = Path.ChangeExtension(scripFilePath, ".html");
            using (var fileStream = File.Open(htmlFileName, FileMode.Create))
            using (var streamWriter = new StreamWriter(fileStream))
            {
                var listener = new ScripToHtml(Path.GetDirectoryName(htmlFileName), streamWriter);
                walker.Walk(listener, tree);
            }
        }

        private void ConvertDynamicScrip(string evaluatedText)
        {
            var lexer = new ScripLexer(new AntlrInputStream(evaluatedText));

            var tokens = new CommonTokenStream(lexer);

            var parser = new ScripParser(tokens);
            var tree = parser.paragraphs();

            var walker = new ParseTreeWalker();
            var listener = new ScripToHtml(_folder, _streamWriter);
            walker.Walk(listener, tree);
        }

        public ScripToHtml(string folder, StreamWriter streamWriter)
        {
            _folder = folder;
            _streamWriter = streamWriter;
        }

        public void EnterBlock([NotNull] ScripParser.BlockContext context)
        {
        }

        public void EnterEveryRule(ParserRuleContext ctx)
        {
        }

        public void EnterQuotation(ScripParser.QuotationContext context)
        {
            var text = context.QUOTATION().GetText().Trim(' ', '\t', '\r', '\n', '"');
            var lastIndex = text.LastIndexOf("--");
            if (lastIndex >= 0)
            {
                var author = text.Substring(lastIndex);
                var textWithoutAuthor = text.Substring(0, lastIndex);
                Write($"<blockquote>{textWithoutAuthor}");
            }
            else
            {
                // No author
                Write($"<blockquote>{text}");
            }
        }

        public void ExitQuotation(ScripParser.QuotationContext context)
        {
            var text = context.QUOTATION().GetText().Trim(' ', '\t', '\r', '\n', '"');
            var lastIndex = text.LastIndexOf("--");
            if (lastIndex >= 0)
            {
                var author = text.Substring(lastIndex);
                var textWithoutAuthor = text.Substring(0, lastIndex);
                Write($"<span class=\"quotation-author\">{author}</span><blockquote>");
            }
            else
            {
                // No author
                Write($"</blockquote>");
            }
        }

        public void EnterItalics([NotNull] ScripParser.ItalicsContext context)
        {
            Write("<i>");
        }

        public void ExitTextbox(ScripParser.TextboxContext context)
        {
            Write("</input>");
        }

        public void EnterCheckedCheckbox(ScripParser.CheckedCheckboxContext context)
        {
            Write("<input type=\"checkbox\" checked disabled>");
        }

        public void ExitCheckedCheckbox(ScripParser.CheckedCheckboxContext context)
        {
            Write("</input>");
        }

        public void EnterUncheckedCheckbox(ScripParser.UncheckedCheckboxContext context)
        {
            Write("<input type=\"checkbox\" disabled>");
        }

        public void ExitUncheckedCheckbox(ScripParser.UncheckedCheckboxContext context)
        {
            Write("</input>");
        }

        public void ExitBold(ScripParser.BoldContext context)
        {
            Write("</b>");
        }

        public void EnterUnderline(ScripParser.UnderlineContext context)
        {
            Write("<u>");
        }

        public void ExitUnderline(ScripParser.UnderlineContext context)
        {
            Write("</u>");
        }

        public void EnterStrikeout(ScripParser.StrikeoutContext context)
        {
            Write("<s>");
        }

        public void ExitStrikeout(ScripParser.StrikeoutContext context)
        {
            Write("</s>");
        }

        public void EnterHashtag(ScripParser.HashtagContext context)
        {
            Write("<span class=\"hashtag\">");
            Write(context.HASHTAG().GetText());
        }

        public void ExitHashtag(ScripParser.HashtagContext context)
        {
            Write("</span>");
        }

        public void EnterMention(ScripParser.MentionContext context)
        {
            Write("<span class=\"mention\">");
            Write(context.MENTION().GetText());
        }

        public void ExitMention(ScripParser.MentionContext context)
        {
            Write("</span>");
        }

        public void EnterLiteral(ScripParser.LiteralContext context)
        {
            var code = context.GetText().Trim('`');

            Write($"<code>{code}</code>");
        }

        public void ExitLiteral(ScripParser.LiteralContext context)
        {
        }

        public void EnterHeading1(ScripParser.Heading1Context context)
        {
            Write("<h1>");
        }

        public void ExitHeading1(ScripParser.Heading1Context context)
        {
            Write("</h1>");
        }

        public void EnterHeading2(ScripParser.Heading2Context context)
        {
            Write("<h2>");
        }

        public void ExitHeading2(ScripParser.Heading2Context context)
        {
            Write("</h2>");
        }

        public void EnterHeading3(ScripParser.Heading3Context context)
        {
            Write("<h3>");
        }

        public void ExitHeading3(ScripParser.Heading3Context context)
        {
            Write("</h3>");
        }

        public void EnterHeading4(ScripParser.Heading4Context context)
        {
            Write("<h4>");
        }

        public void ExitHeading4(ScripParser.Heading4Context context)
        {
            Write("</h4>");
        }

        public void EnterHeading5(ScripParser.Heading5Context context)
        {
            Write("<h5>");
        }

        public void ExitHeading5(ScripParser.Heading5Context context)
        {
            Write("</h5>");
        }

        public void EnterHeading6(ScripParser.Heading6Context context)
        {
            Write("<h6>");
            Write(context.GetText().Trim('#', ' ', '\r', '\n'));
        }

        public void ExitHeading6(ScripParser.Heading6Context context)
        {
            Write("</h6>");
        }

        public void EnterCodeBlock(ScripParser.CodeBlockContext context)
        {
            var code = context.GetText();
            code = code.Substring(2, code.Length - 4);
            var command = code.Substring(0, code.IndexOf('\n')).Trim();
            code = code.Substring(code.IndexOf('\n')).Trim();
            var linesOfCode = code.Split('\n').Select(line => line.Trim()).ToArray();

            var codeFilePath = Path.GetTempFileName();
            File.WriteAllText(codeFilePath, code);

            var process = new Process();

            process.StartInfo.FileName = command;
            process.StartInfo.Arguments = codeFilePath;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.EnableRaisingEvents = true;
            process.Start();

            foreach (var line in linesOfCode) process.StandardInput.WriteLine(line);

            var result = string.Empty;
            while (!process.HasExited) result += process.StandardOutput.ReadLine();

            Write($"<pre><code>{result}</code></pre>");
        }

        public void ExitCodeBlock(ScripParser.CodeBlockContext context)
        {
        }

        public void EnterParagraphs(ScripParser.ParagraphsContext context)
        {
        }

        public void ExitParagraphs(ScripParser.ParagraphsContext context)
        {
        }

        public void EnterParagraph(ScripParser.ParagraphContext context)
        {
            if (context.ChildCount == 0)
                return;
            Write("<p>");
        }

        public void ExitParagraph(ScripParser.ParagraphContext context)
        {
            if (context.ChildCount == 0)
                return;
            Write("</p>\n\n");
        }

        public void EnterOrderedItem(ScripParser.OrderedItemContext context)
        {
            var number = context.ORDERED_ITEM_DELIMITER().GetText().Trim('(', '.', ' ');
            Write($"<ol start=\"{number}\"><li>");
        }

        public void ExitOrderedItem(ScripParser.OrderedItemContext context)
        {
            Write("</li></ol>");
        }

        public void EnterUnorderedItem(ScripParser.UnorderedItemContext context)
        {
            Write("<ul><li>");
        }

        public void ExitUnorderedItem(ScripParser.UnorderedItemContext context)
        {
            Write("</li></ul>");
        }

        public void EnterText([NotNull] ScripParser.TextContext context)
        {
            Write(context.GetText());
        }

        public void ExitBlock([NotNull] ScripParser.BlockContext context)
        {
        }

        public void EnterTable(ScripParser.TableContext context)
        {
            Write("<table>");
        }

        public void ExitTable(ScripParser.TableContext context)
        {
            Write("</table>");
        }

        public void EnterTableRow(ScripParser.TableRowContext context)
        {
            Write("<tr>");
        }

        public void ExitTableRow(ScripParser.TableRowContext context)
        {
            Write("</tr>");
        }

        public void EnterTableCell(ScripParser.TableCellContext context)
        {
            Write("<td>");
        }

        public void ExitTableCell(ScripParser.TableCellContext context)
        {
            Write("</td>");
        }

        public void EnterTextbox(ScripParser.TextboxContext context)
        {
            Write("<input type=\"text\" disabled>");
        }

        public void ExitEveryRule(ParserRuleContext ctx)
        {
        }

        public void ExitItalics([NotNull] ScripParser.ItalicsContext context)
        {
            Write("</i>");
        }

        public void EnterBold(ScripParser.BoldContext context)
        {
            Write("<b>");
        }

        public void ExitText([NotNull] ScripParser.TextContext context)
        {
        }

        public void VisitErrorNode(IErrorNode node)
        {
        }

        public void VisitTerminal(ITerminalNode node)
        {
        }

        private void Write(string str)
        {
            _streamWriter.Write(str);
            Console.Write(str);
        }

        public void EnterLink([NotNull] ScripParser.LinkContext context)
        {
            var regex = new Regex(@"#Link\{(.+)\|(.+)\}");
            var match = regex.Match(context.LINK().GetText());
            var linkTitle = match.Groups[1];
            var linkTarget = match.Groups[2].ToString();

            if (linkTarget.ToString().EndsWith(".scrip"))
            {
                var nestedTarget = GetAbsolutePath(linkTarget);
                ConvertToHtml(nestedTarget);
                linkTarget = Path.ChangeExtension(linkTarget, ".html");
            }

            Write($"<a href=\"{linkTarget}\">{linkTitle}</a>");
        }

        public void ExitLink([NotNull] ScripParser.LinkContext context)
        {

        }

        public void EnterImage([NotNull] ScripParser.ImageContext context)
        {
            var regex = new Regex(@"#Image\{(.+)\}");
            var text = context.IMAGE().GetText();
            var match = regex.Match(text);
            var linkTarget = match.Groups[1];
             Write($"<img src=\"{linkTarget}\" />");
        }

        public void ExitImage([NotNull] ScripParser.ImageContext context)
        {

        }

        private string GetAbsolutePath(string relativePath)
        {
            return Path.Combine(_folder, relativePath);
        }

        public void EnterNested([NotNull] ScripParser.NestedContext context)
        {
            var regex = new Regex(@"#Nested\{(.+)\}");
            var text = context.NESTED().GetText();
            var match = regex.Match(text);
            var nestedTarget = match.Groups[1].ToString();
            nestedTarget = GetAbsolutePath(nestedTarget);
            var nestedTargetFolder = Path.GetDirectoryName(nestedTarget);

            var lexer = new ScripLexer(new AntlrFileStream(nestedTarget));
            var tokens = new CommonTokenStream(lexer);
            var parser = new ScripParser(tokens);
            var tree = parser.paragraphs();

            var walker = new ParseTreeWalker();
            var listener = new ScripToHtml(nestedTargetFolder, _streamWriter);
            walker.Walk(listener, tree);
        }

        public void ExitNested([NotNull] ScripParser.NestedContext context)
        {
            
        }

        public void EnterAutoNested([NotNull] ScripParser.AutoNestedContext context)
        {
            var entries = Directory.EnumerateFileSystemEntries(_folder).OrderBy(fse => fse);

            var dynamicScrip = "";

            foreach (var fse in entries)
            {
                if (Directory.Exists(fse))
                {
                    var absolutePath = Path.Combine(fse, "Notes.scrip");
                    if (!File.Exists(absolutePath))
                    {
                        continue;
                    }

                    var name = Path.GetFileName(fse);

                    dynamicScrip += $"## #Link{{{name}|{name}/Notes.scrip}}\n";
                }
                //else
                //{
                //    if (!path.EndsWith(".scrip"))
                //    {
                //        continue;
                //    }

                //    dynamicScrip += $"## #Link{{{fse}}}\n";
                //}
            }

            ConvertDynamicScrip(dynamicScrip);
        }

        public void ExitAutoNested([NotNull] ScripParser.AutoNestedContext context)
        {
            
        }

        //public void EnterMacro([NotNull] ScripParser.MacroContext context)
        //{

        //}

        //public void ExitMacro([NotNull] ScripParser.MacroContext context)
        //{

        //}
    }
}