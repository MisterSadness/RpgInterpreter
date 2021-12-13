using RpgInterpreter.Parser;

namespace RpgInterpreter.NonTerminals;

public record NonTerminal : Symbol;

public record RpgProgram : NonTerminal;

public record Paren : NonTerminal;

public record Block : NonTerminal;

public record BlockInner : NonTerminal;

public record Value : NonTerminal;

public record Name : NonTerminal;

public record List : NonTerminal;

public record ListInner : NonTerminal;

public record ListInnerCont : NonTerminal;

public record FieldReference : NonTerminal;

public record If : NonTerminal;

public record Expression : NonTerminal;

public record Start1 : NonTerminal;

public record Next1 : NonTerminal;

public record Start2 : NonTerminal;

public record Next2 : NonTerminal;

public record Start3 : NonTerminal;

public record Next3 : NonTerminal;

public record Invoke : NonTerminal;

public record InvokeInner : NonTerminal;

public record InvokeList : NonTerminal;

public record Start4 : NonTerminal;

public record Next4 : NonTerminal;

public record Start5 : NonTerminal;

public record Next5 : NonTerminal;

public record Start6 : NonTerminal;

public record Next6 : NonTerminal;

public record Statement : NonTerminal;

public record ObjectDeclaration : NonTerminal;

public record BaseList : NonTerminal;

public record TraitListStart : NonTerminal;

public record TraitListNext : NonTerminal;

public record Fields : NonTerminal;

public record Field : NonTerminal;

public record FieldList : NonTerminal;

public record ObjInner : NonTerminal;

public record TraitDeclaration : NonTerminal;

public record TraitRequirements : NonTerminal;

public record Assignment : NonTerminal;

public record AssignmentOption : NonTerminal;

public record ObjectCreation : NonTerminal;

public record Function : NonTerminal;

public record FunctionParameters : NonTerminal;
