grammar Scrip;

paragraphs                    : paragraph (PARAGRAPH_SEP+ paragraph)*;
PARAGRAPH_SEP                 : '\r\n';

paragraph                     : block*?;

block                         : text
							  | codeBlock
                              //| parens //| braces | square_brackets | angle_brackets
		                      | italics | bold | underline | strikeout | literal
							  | heading1 | heading2 | heading3 | heading4 | heading5 | heading6
							  | textbox | checkedCheckbox | uncheckedCheckbox | quotation
				              | hashtag | mention
							  | table
							  | (orderedItem block*?) | (unorderedItem block*?)
						      ;

orderedItem                   : ORDERED_ITEM_DELIMITER block+;
ORDERED_ITEM_DELIMITER        : [0-9]+ '. ';

unorderedItem                 : '- ' block+;
//UNORDERED_ITEM_DELIMITER      : '-';

textbox                       : TEXTBOX;
TEXTBOX                       : '_' '_'+;

uncheckedCheckbox             : UNCHECKED_CHECKBOX;
UNCHECKED_CHECKBOX            : '[ ]';
checkedCheckbox               : CHECKED_CHECKBOX;
CHECKED_CHECKBOX              : '[x]';

//quotation                     : QUOTATION;
//QUOTATION                     : '"""' .*? '"""';
//QUOTATION                     : '"""' .*? QUOTATION_AUTHOR? '"""';
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

hashtag                       : HASHTAG;
HASHTAG                       : '#' [a-zA-Z]+;

mention                       : MENTION;
MENTION                       : '@' [a-zA-Z]+;

literal                       : LITERAL;
LITERAL                       : '`' .*? '`';

heading1                      : HEADING1;
HEADING1                      : '# ' ~[\r\n]* ('\r\n' | '\n');

heading2                      : HEADING2;
HEADING2                      : '## ' ~[\r\n]* ('\r\n' | '\n');

heading3                      : HEADING3;
HEADING3                      : '### ' ~[\r\n]* ('\r\n' | '\n');

heading4                      : HEADING4;
HEADING4                      : '#### ' ~[\r\n]* ('\r\n' | '\n');

heading5                      : HEADING5;
HEADING5                      : '##### ' ~[\r\n]* ('\r\n' | '\n');

heading6                      : HEADING6;
HEADING6                      : '###### ' ~[\r\n]* ('\r\n' | '\n');

codeBlock                     : CODE_BLOCK_DELIMITER_START .+? CODE_BLOCK_DELIMITER_STOP;
CODE_BLOCK_DELIMITER_START    :  '#!';
CODE_BLOCK_DELIMITER_STOP     :  '!#';

table                         : '#Table{' tableRow ('\r\n' tableRow)*? '}';
tableRow                      : tableCell (',' tableCell)*;
tableCell                     : block*?;

text : TEXT;
TEXT : .+?;

//WS : [ \t\r\n]+ -> skip ;

/*
blocks : block+;

block : parens
      //| braces
      //| brackets
      //| angle_brackets
      //| code
      //| single_quotes
      | double_quotes
      //| italics
      //| bold
      //| underline
      //| text
	  | TEXT
      //| code_literal
      //| heading1
      //| heading2
      //| heading3
      //| heading4
      //| heading5
      //| heading6
	  //| PARAGRAPH_BREAK
	  ;

// list
parens : '(' block* ')';

// map or set
braces : '{' block* '}';

// multi-dimensional array or matrix
brackets : '[' block* ']';

// ???
angle_brackets : '<' block* '>';

code : '#!' ('\\#!' | '\\\\' | .)*? '#!';

single_quotes: '\'' ('\\\'' | '\\\\' | .)*? '\'';
double_quotes: '"' ('\\"' | '\\\\' | .)*? '"';

italics: '/' ('\\/' | '\\\\' | .)*? '/';
bold: '*' ('\\*' | '\\\\' | .)*? '*';
underline: '_' ('\\_' | '\\\\' | .)*? '_';

//text : ~[()\\[\]{}<>/_*"'`]+;
TEXT : .+?;

PARAGRAPH_BREAK : '\r\n\r\n';

code_literal : '`' ('\\`' | '\\\\' | .)*? '`';

heading1: '#' block*? '#';
heading2: '##' block*? '##';
heading3: '###' block*? '###';
heading4: '####' block*? '####';
heading5: '#####' block*? '#####';
heading6: '######' block*? '######';

//WS : [ \t\r\n]+ -> skip ;
*/