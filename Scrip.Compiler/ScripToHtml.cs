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

        public static void ConvertFileToHtml(string scripFilePath)
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
                listener.ConvertToHtml(tree, walker);
            }
        }

        private void ConvertScripToHtml(string evaluatedText)
        {
            var lexer = new ScripLexer(new AntlrInputStream(evaluatedText));

            var tokens = new CommonTokenStream(lexer);

            var parser = new ScripParser(tokens);
            var tree = parser.paragraphs();

            var walker = new ParseTreeWalker();
            var listener = new ScripToHtml(_folder, _streamWriter);
            walker.Walk(listener, tree);
        }

        private void ConvertToHtml(ScripParser.ParagraphsContext tree, ParseTreeWalker walker)
        {
            var cssPath = typeof(ScripToHtml).Assembly.Location;
            cssPath = Path.GetDirectoryName(cssPath);
            cssPath = Path.Combine(cssPath, "ScripToHtml.css");
            var cssPathTarget = Path.Combine(_folder, "ScripToHtml.css");
            if (File.Exists(cssPathTarget))
            {
                File.Delete(cssPathTarget);
            }

            File.Copy(cssPath, cssPathTarget);

            Write("<html>");
            Write("<head>");
            Write($"<link rel=\"stylesheet\" href=\"ScripToHtml.css\">");
            Write("<link href=\"http://netdna.bootstrapcdn.com/bootstrap/3.0.3/css/bootstrap.min.css\" rel=\"stylesheet\">");
            Write("</head>");
            Write("<body>");
            walker.Walk(this, tree);
            Write("</body>");
            Write("</html>");
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
            Write(" <i>" + context.GetChild(0).ToString().Replace("/", ""));
        }

        public void ExitTextbox(ScripParser.TextboxContext context)
        {
            Write("</input>");
        }

        public void EnterCheckedCheckbox(ScripParser.CheckedCheckboxContext context)
        {
            Write("<input class=\"checkbox\" type=\"checkbox\" checked disabled>");
        }

        public void ExitCheckedCheckbox(ScripParser.CheckedCheckboxContext context)
        {
            Write("</input>");
        }

        public void EnterUncheckedCheckbox(ScripParser.UncheckedCheckboxContext context)
        {
            Write("<input class=\"checkbox\" type=\"checkbox\" disabled>");
        }

        public void ExitUncheckedCheckbox(ScripParser.UncheckedCheckboxContext context)
        {
            Write("</input>");
        }

        public void ExitBold(ScripParser.BoldContext context)
        {
            Write(context.children.Last().ToString().Replace("*", "") + "</b> ");
        }

        public void EnterUnderline(ScripParser.UnderlineContext context)
        {
            Write(" <u>" + context.GetChild(0).ToString().Replace("_", ""));
        }

        public void ExitUnderline(ScripParser.UnderlineContext context)
        {
            Write(context.children.Last().ToString().Replace("_", "") + "</u> ");
        }

        public void EnterStrikeout(ScripParser.StrikeoutContext context)
        {
            Write(" <s>" + context.GetChild(0).ToString().Replace("-", ""));
        }

        public void ExitStrikeout(ScripParser.StrikeoutContext context)
        {
            Write(context.children.Last().ToString().Replace("-", "") + "</s> ");
        }

        public void EnterHashtag(ScripParser.HashtagContext context)
        {
            if (context.HASHTAG_WITHOUT_PARAMETERS() != null)
            {
                var text = context.HASHTAG_WITHOUT_PARAMETERS().GetText();
                var regex = new Regex(@"#([a-zA-Z]+)");
                var match = regex.Match(text);
                var hashtagName = match.Groups[1].ToString();

                if (hashtagName == "AutoNested")
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
                    }

                    ConvertScripToHtml(dynamicScrip);
                }
            }

            if (context.HASHTAG_WITH_PARAMETERS() != null)
            {
                var text = context.HASHTAG_WITH_PARAMETERS().GetText();
                var regex = new Regex(@"#([a-zA-Z]+)\{(.+)\}");
                var match = regex.Match(text);
                var hashtagName = match.Groups[1].ToString();
                var parameterText = match.Groups[2].ToString();
                var parameters = parameterText.Split('|');

                if (hashtagName == "Link")
                {
                    var linkTitle = parameters.Length > 1 ? parameters[1] : Path.GetFileNameWithoutExtension(parameters[0]);
                    var linkTarget = parameters.Length > 1 ? parameters[1] : parameters[0];

                    if (linkTarget.EndsWith(".scrip"))
                    {
                        var nestedTarget = GetAbsolutePath(linkTarget);
                        ConvertFileToHtml(nestedTarget);
                        linkTarget = Path.ChangeExtension(linkTarget, ".html");
                    }

                    Write($"<a href=\"{linkTarget}\">{linkTitle}</a>");
                }
                else if (hashtagName == "Image")
                {
                    var linkTarget = parameters[0];
                    Write($"<img src=\"{linkTarget}\" />");
                }
                else if (hashtagName == "Nested")
                {
                    var nestedTarget = parameters[0];
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
            }
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

        public void EnterHeading(ScripParser.HeadingContext context)
        {
            var regex = new Regex("^(#+)");

            var match = regex.Match(context.GetText());
            var level = match.Groups[1].ToString().Length;

            Write($"<h{level}>");
        }

        public void ExitHeading(ScripParser.HeadingContext context)
        {
            var regex = new Regex("^(#+)");

            var match = regex.Match(context.GetText());
            var level = match.Groups[1].ToString().Length;

            Write($"</h{level}>");
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
            Write("<table class=\"table\">");
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
            Write("<input class=\"textbox\" type=\"text\" disabled>");
        }

        public void ExitEveryRule(ParserRuleContext ctx)
        {
        }

        public void ExitItalics([NotNull] ScripParser.ItalicsContext context)
        {
            Write(context.children.Last().ToString().Replace("/", "") + "</i> ");
        }

        public void EnterBold(ScripParser.BoldContext context)
        {
            Write(" <b>" + context.GetChild(0).ToString().Replace("*", ""));
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

        private string GetAbsolutePath(string relativePath)
        {
            return Path.Combine(_folder, relativePath);
        }

        //public void EnterMacro([NotNull] ScripParser.MacroContext context)
        //{

        //}

        //public void ExitMacro([NotNull] ScripParser.MacroContext context)
        //{

        //}
    }
}