using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime.Misc;

namespace Scrip.Compiler.AST
{
    public class Section
    {
    }

    public class Paragraph
    {

    }

    public abstract class Block
    {

    }

    public class Text : Block
    {

    }

    public class CodeBlock : Block
    {
        public CodeBlock(string code)
        {
            Code = code;
        }

        public string Code { get; }
    }

    public class Italics : Block
    {

    }

    public class Bold : Block
    {

    }

    public class Underline : Block
    {

    }

    public class Strikeout : Block
    {

    }

    public class Literal : Block
    {

    }

    public class Heading : Block
    {

    }

    public class Textbox : Block
    {

    }

    public class Checkbox : Block
    {
        public Checkbox(bool isChecked)
        {
            IsChecked = isChecked;
        }

        public bool IsChecked { get; }
    }

    public class Quotation : Block
    {
        public Quotation(string qutotation, string author)
        {
            Qutotation = qutotation;
            Author = author;
        }

        public string Qutotation { get; }
        public string Author { get; }
    }

    public class Hashtag : Block
    {
        public string Value { get; }
    }

    public class Mention : Block
    {
        public Mention(string person)
        {
            Person = person;
        }

        public string Person { get; }
    }

    public class Table : Block
    {
        public Table(TableCell[][] cells)
        {
            Cells = cells;
        }

        public TableCell[][] Cells { get; }
    }

    public class TableCell
    {
        public TableCell(Block[] blocks)
        {
            Blocks = blocks;
        }

        public Block[] Blocks { get; }
    }

    public class OrderedItem : Block
    {
        public OrderedItem(Block[] blocks)
        {
            Blocks = blocks;
        }

        public Block[] Blocks { get; }
    }

    public class UnorderedItem : Block
    {
        public UnorderedItem(Block[] blocks)
        {
            Blocks = blocks;
        }

        public Block[] Blocks { get; }
    }

    public class BlockVisitor : ScripBaseVisitor<Block>
    {
        public override Block VisitBold([NotNull] ScripParser.BoldContext context)
        {
            return new Bold();
        }

        public override Block VisitCheckedCheckbox([NotNull] ScripParser.CheckedCheckboxContext context)
        {
            return new Checkbox(true);
        }

        public override Block VisitUncheckedCheckbox([NotNull] ScripParser.UncheckedCheckboxContext context)
        {
            return new Checkbox(false);
        }

        public override Block VisitCodeBlock([NotNull] ScripParser.CodeBlockContext context)
        {
            return new CodeBlock(context.GetText());
        }

        public override Block VisitHashtag([NotNull] ScripParser.HashtagContext context)
        {
            return new Hashtag();
        }

        public override Block VisitHeading([NotNull] ScripParser.HeadingContext context)
        {
            return base.VisitHeading(context);
        }

        public override Block VisitItalics([NotNull] ScripParser.ItalicsContext context)
        {
            return base.VisitItalics(context);
        }

        public override Block VisitLiteral([NotNull] ScripParser.LiteralContext context)
        {
            return base.VisitLiteral(context);
        }

        public override Block VisitMention([NotNull] ScripParser.MentionContext context)
        {
            return base.VisitMention(context);
        }

        public override Block VisitOrderedItem([NotNull] ScripParser.OrderedItemContext context)
        {
            return base.VisitOrderedItem(context);
        }

        public override Block VisitParagraph([NotNull] ScripParser.ParagraphContext context)
        {
            return base.VisitParagraph(context);
        }

        public override Block VisitQuotation([NotNull] ScripParser.QuotationContext context)
        {
            return base.VisitQuotation(context);
        }

        public override Block VisitStrikeout([NotNull] ScripParser.StrikeoutContext context)
        {
            return base.VisitStrikeout(context);
        }

        public override Block VisitTable([NotNull] ScripParser.TableContext context)
        {
            return base.VisitTable(context);
        }

        public override Block VisitTableCell([NotNull] ScripParser.TableCellContext context)
        {
            return base.VisitTableCell(context);
        }

        public override Block VisitTableRow([NotNull] ScripParser.TableRowContext context)
        {
            return base.VisitTableRow(context);
        }

        public override Block VisitText([NotNull] ScripParser.TextContext context)
        {
            return base.VisitText(context);
        }

        public override Block VisitTextbox([NotNull] ScripParser.TextboxContext context)
        {
            return base.VisitTextbox(context);
        }

        public override Block VisitUnderline([NotNull] ScripParser.UnderlineContext context)
        {
            return base.VisitUnderline(context);
        }

        public override Block VisitUnorderedItem([NotNull] ScripParser.UnorderedItemContext context)
        {
            return base.VisitUnorderedItem(context);
        }
    }
}
