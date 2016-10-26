grammar MeelanLanguage;

/*
 * Parser Rules
 */

statements	: (statement (SEMICOLON)*)*;

statement	: 'print' expr										# Print
			| 'var' ID ('=' expr)?								# Declaration
			| ID '=' expr										# Assignment
			| 'while' expr 'do' statement						# While
			| 'for' ID 'in' term '...' term statement			# ForIn
			| 'if' expr 'then' statement ('else' statement)?	# IfOptionalElse
			| 'funcdef' ID '(' idlist ')' statement				# FuncDef
			//| '{' statements '}'								# BlockX //--> term already contains this rule (expr --> cmp --> ... --> term)
			| expr												# Expression;

idlist		: (ID (',' ID)*)*;

arglist		: (sum (',' sum)*)*;

expr		: cmp;

cmp			: sum (operator=('<' | '=<' | '==' | '><' | '>=' | '>') sum)?;

sum			: product (operator=('+'|'-') product)*;

product		: unary (operator=('*'|'/'|'%') unary)*;

unary		: '-' unary											# UnaryTerm
			| term												# AtomicTerm;

term		: '(' expr ')'										# TermInBraces
			| ID '(' arglist ')'								# FuncCall
			| ID												# VariableOnly
			| DOUBLE											# NumberOnly
			| '{' statements '}'								# Block
			| 'if' expr 'then' expr 'else' expr					# IfRequiredElse;

/*
 * Lexer Rules
 */

ID			: [a-zA-Z][a-zA-Z0-9_]*;
DOUBLE		: ([0-9]*[.])?[0-9]+;
WS			: [ \t\r\n] -> channel(HIDDEN);
COMMENT		: '/*' .*? '*/' -> skip;
LINE_COMMENT: '//' ~[\r\n]* -> skip;
SEMICOLON	: ';' -> skip;