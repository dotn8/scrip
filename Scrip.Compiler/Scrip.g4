grammar Scrip;

paragraphs                    : paragraph (PARAGRAPH_SEP+ paragraph)*;
PARAGRAPH_SEP                 : '\r\n';

paragraph                     : block*?;

block                         : text
							  | codeBlock
		                      | italics | bold | underline | strikeout | literal
							  | heading
							  | textbox | checkedCheckbox | uncheckedCheckbox | quotation
				              | hashtag | mention
							  | table
							  | (orderedItem block*?) | (unorderedItem block*?)
						      ;

orderedItem                   : ORDERED_ITEM_DELIMITER block+;
ORDERED_ITEM_DELIMITER        : [0-9]+ '. ';

unorderedItem                 : '- ' block+;

textbox                       : TEXTBOX;
TEXTBOX                       : '_' '_'+;

uncheckedCheckbox             : UNCHECKED_CHECKBOX;
UNCHECKED_CHECKBOX            : '[ ]';
checkedCheckbox               : CHECKED_CHECKBOX;
CHECKED_CHECKBOX              : '[x]';

quotation                     : QUOTATION;
QUOTATION                     : '"""' .*? QUOTATION_AUTHOR? '"""';
QUOTATION_AUTHOR              : (' ' | '\n' | '\t')+ '--' (~(' ' | '\n' | '\t')+ | MENTION);

//parens                        : PARENS_OPEN block* PARENS_CLOSE;
//PARENS_OPEN                   : '(';
//PARENS_CLOSE                  : ')';

text : TEXT;
TEXT : (.
	   | ' / ' | ~' ' '/' ~' '
       | ' * ' | ~' ' '*' ~' '
       | ' ' '-'+ ' ' | ~' ' '-'+ ~' '
       )+?;

italics                       : '/' block* '/';

bold                          : '*' block* '*';

underline                     : '_' block* '_';

strikeout                     : '-' block* '-';

mention                       : MENTION;
MENTION                       : '@' [a-zA-Z]+;

literal                       : LITERAL;
LITERAL                       : '`' .*? '`';

heading                       : '#'+ block*? ('\r\n' | '\n');

codeBlock                     : CODE_BLOCK_DELIMITER_START .+? CODE_BLOCK_DELIMITER_STOP;
CODE_BLOCK_DELIMITER_START    :  '#!';
CODE_BLOCK_DELIMITER_STOP     :  '!#';

table                         : tableRow+;
tableRow                      : '|' tableCell ('|' tableCell)* '\r\n';
tableCell                     : block*?;

hashtag                       : HASHTAG_WITH_PARAMETERS | HASHTAG_WITHOUT_PARAMETERS;
HASHTAG_WITH_PARAMETERS       : '#' [a-zA-Z]+ '{' .*? ('|' .*?)*? '}';
HASHTAG_WITHOUT_PARAMETERS    : '#' [a-zA-Z]+;
