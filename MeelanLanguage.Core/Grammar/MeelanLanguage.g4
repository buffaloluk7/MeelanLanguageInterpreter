grammar MeelanLanguage;

@header{
	using System;
	using System.Collections;
}

options {
    language=CSharp3;
}

/*
 * Parser Rules
 */

statements	: (statement (';')?)*;

statement	: 'print' expr										# Print
			| 'var' ID ('=' expr)?								# Declaration
			| ID '=' expr										# Assignment
			| 'while' expr 'do' statement						# While
			| 'if' expr 'then' statement ('else' statement)?	# IfElse
			| 'funcdef' ID '(' idlist ')' statement				# FuncDef
			//| '{' statements '}'								# Block --> term contains this rule (expr --> cmp --> ... --> term)
			| expr												# Expression;

idlist		: (ID (',' ID)*)*;

arglist		: (term (',' term)*)*;

expr		: cmp;

cmp			: sum (operator=('<' | '=<' | '==' | '><' | '>=' | '>') sum)?;

sum			: product (operator=('+'|'-') product)*;

product		: unary (operator=('*'|'/'|'%') unary)*;

unary		: '-' unary											# UnaryTerm
			| term												# AtomicTerm;

term		: '(' expr ')'										# TermInBraces
			| ID												# VariableOnly
			| ID '(' arglist ')'								# FuncCall
			| INT												# NumberOnly
			| '{' statements '}'								# Block;

/*
 * Lexer Rules
 */

ID			: [a-zA-Z][a-zA-Z0-9_]*;
INT			: [0-9]+;
WS			: [ \t\r\n] -> channel(HIDDEN);