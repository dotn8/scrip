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
							  | link | image | nested | autoNested
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

italics                       : ITALICS_DELIMITER block* ITALICS_DELIMITER;
ITALICS_DELIMITER             : '/';

bold                          : BOLD_DELIMITER block* BOLD_DELIMITER;
BOLD_DELIMITER                : '*';

underline                     : UNDERLINE_DELIMITER block* UNDERLINE_DELIMITER;
UNDERLINE_DELIMITER           : '_';

strikeout                     : STRIKEOUT_DELIMITER block* STRIKEOUT_DELIMITER;
STRIKEOUT_DELIMITER           : '-';

//italics                       : ITALICS_DELIMITER_START block* ITALICS_DELIMITER_STOP;
//ITALICS_DELIMITER_START       : (' ' | '\n' | '\t') '/';
//ITALICS_DELIMITER_STOP        : '/' (' ' | '\n' | '\t');

//bold                          : BOLD_DELIMITER_START block* BOLD_DELIMITER_STOP;
//BOLD_DELIMITER_START          : (' ' | '\n' | '\t') '*';
//BOLD_DELIMITER_STOP           : '*' (' ' | '\n' | '\t');

//underline                     : UNDERLINE_DELIMITER_START block* UNDERLINE_DELIMITER_STOP;
//UNDERLINE_DELIMITER_START     : (' ' | '\n' | '\t') '_';
//UNDERLINE_DELIMITER_STOP      : '_' (' ' | '\n' | '\t');

//strikeout                     : STRIKEOUT_DELIMITER_START block* STRIKEOUT_DELIMITER_STOP;
//STRIKEOUT_DELIMITER_START     : (' ' | '\n' | '\t') '-';
//STRIKEOUT_DELIMITER_STOP      : '-' (' ' | '\n' | '\t');

mention                       : MENTION;
MENTION                       : '@' [a-zA-Z]+;

literal                       : LITERAL;
LITERAL                       : '`' .*? '`';

heading                       : '#'+ ' ' block*? ('\r\n' | '\n');

codeBlock                     : CODE_BLOCK_DELIMITER_START .+? CODE_BLOCK_DELIMITER_STOP;
CODE_BLOCK_DELIMITER_START    :  '#!';
CODE_BLOCK_DELIMITER_STOP     :  '!#';

table                         : tableRow+;
tableRow                      : '|' tableCell ('|' tableCell)* '\r\n';
tableCell                     : block*?;

link                          : LINK;
LINK                          : '#Link{' .*? '|' .*? '}';

image                         : IMAGE;
IMAGE                         : '#Image{' .*? '}';

nested                        : NESTED;
NESTED                        : '#Nested{' .*? '}';

autoNested                    : AUTO_NESTED;
AUTO_NESTED                   : '#AutoNested{' .*? '}';

hashtag                       : HASHTAG;
HASHTAG                       : '#' [a-zA-Z]+;

text : TEXT;
TEXT : .+?;
