grammar UnitsGrammar;
prog: (statments NEW_LINE_SYMBOLS?)+;
statments: newUnit_Binary
    | newUnit_Unary
    | command
    | expr 
    | operator
    | comment ;
comment:
    '#' ~('#') '#';
newUnit_Binary: WORD DEF WORD VALID_OPERATORS WORD ;
newUnit_Unary: WORD DEF (VALID_OPERATORS | WORD) WORD ;
command: '!' unitName argslist;

operator: unitName '(' (VALID_OPERATORS | LETTER | WORD) ')' DEF operatorDefs;
operatorDefs: operatorDef '|' operatorDefs
    | operatorDef ;
operatorDef: operatorDef_Binary 
    | operatorDef_Unary;
operatorDef_Binary: LETTER '=' LETTER VALID_OPERATORS LETTER;
operatorDef_Unary: LETTER '=' (LETTER | WORD) LETTER ;

expr: left DEF argslist ;
left: unitName '(' unitName ')' ;
argslist: '(' args ')' ARGSSEP argslist
    | '(' args ')'
    | args_esc ARGSSEP argslist
    | args_esc
    | '{' args_esc '}' ARGSSEP argslist
    | '{' args_esc '}' ;
args: arg COMA args
    | arg ;
args_esc: arg_esc COMA args_esc
    | arg_esc ;
arg: (arg_common)*;
arg_esc: (arg_common | '(' | ')')*;
arg_common: SYM_FLOAT | NUM_FLOAT | unitName | VALID_OPERATORS | '˙'| '.';
unitName: WORD | LETTER;
SYM_FLOAT: NUM_FLOAT LETTER;
NUM_FLOAT: NUM_INT '.'?
    | NUM_INT? '.' NUM_INT
    | NUM_INT '.'? ('e' | 'E') ('-' | '+') NUM_INT
    | NUM_INT? '.' NUM_INT ('e' | 'E') ('-' | '+') NUM_INT ;

NUM_INT: NUMBER_SYMBOLS+;

WS_SYMBOLS: (' ' | '\t') -> skip;
NEW_LINE_SYMBOLS: ('\n' | '\r')+;
VALID_OPERATORS: '*' | '+' | '-' | '/' | '<=' | '>=' | '<' | '>' | '==' | '!=' ;
NUMBER_SYMBOLS: '0'..'9' ;
LETTER: 'a'..'z' | 'A'..'Z' ;
WORD: (LETTER | NUMBER_SYMBOLS)+;
DEF: ':=' ;
COMA: ',' ;
ARGSSEP: '|';