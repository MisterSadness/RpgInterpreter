using RpgInterpreter.NonTerminals;
using RpgInterpreter.Parser;
using RpgInterpreter.Tokens;

namespace RpgInterpreter.Productions;

public abstract record Production
{
    public abstract NonTerminal LeftSide { get; }
    public abstract Symbol[] RightSide { get; init; }

    public string Formatted => $"{LeftSide} -> {string.Join(" , ", RightSide.Select(x => x.ToString()))}";
}

public record Production<T>(Symbol[] RightSide) : Production where T : NonTerminal, new()
{
    public override T LeftSide { get; } = new();
}

public record Syntax : NonTerminal;

public record Start() : Production<Syntax>(new Symbol[]
{
    new RpgProgram(), new Terminal<EndOfInput>()
});

public record RpgProgram_Epsilon() : Production<RpgProgram>(new Symbol[] { new Epsilon() });

public record RpgProgram_Statements() : Production<RpgProgram>(new Symbol[]
{
    new Statement(), new Terminal<Semicolon>(), new RpgProgram()
});

public record Statement_Object() : Production<Statement>(new Symbol[] { new NonTerminals.ObjectDeclaration() });

public record Statement_Trait() : Production<Statement>(new Symbol[] { new NonTerminals.TraitDeclaration() });

public record Statement_Function() : Production<Statement>(new Symbol[] { new NonTerminals.Function() });

public record Statement_FunctionCall() : Production<Statement>(new Symbol[]
{
    new Paren(),
    new NonTerminals.Invoke()
});

public record Expression_If() : Production<NonTerminals.Expression>(new Symbol[]
{
    new NonTerminals.If()
});

public record Statement_Assignment() : Production<Statement>(new Symbol[]
{
    new NonTerminals.Assignment()
});

public record Expression() : Production<NonTerminals.Expression>(new Symbol[]
{
    new NonTerminals.Start1(), new Next1()
});

public record Start1() : Production<NonTerminals.Start1>(new Symbol[]
{
    new NonTerminals.Start2(), new Next2()
});

public record Next1_Epsilon() : Production<Next1>(new Symbol[] { new Epsilon() });

public record Next1_And() : Production<Next1>(new Symbol[]
{
    new Terminal<BooleanAnd>(), new NonTerminals.Expression()
});

public record Next1_Or() : Production<Next1>(new Symbol[]
{
    new Terminal<BooleanOr>(), new NonTerminals.Expression()
});

public record Start2() : Production<NonTerminals.Start2>(new Symbol[]
{
    new NonTerminals.Start3(), new Next3()
});

public record Next2_Epsilon() : Production<Next2>(new Symbol[] { new Epsilon() });

public record Next2_Eq() : Production<Next2>(new Symbol[]
{
    new Terminal<Equality>(), new NonTerminals.Start1()
});

public record Next2_Neq() : Production<Next2>(new Symbol[]
{
    new Terminal<Inequality>(), new NonTerminals.Start1()
});

public record Next2_lt() : Production<Next2>(new Symbol[]
{
    new Terminal<Less>(), new NonTerminals.Start1()
});

public record Next2_Gt() : Production<Next2>(new Symbol[]
{
    new Terminal<Greater>(), new NonTerminals.Start1()
});

public record Next2_Leq() : Production<Next2>(new Symbol[]
{
    new Terminal<LessOrEqual>(), new NonTerminals.Start1()
});

public record Next2_Geq() : Production<Next2>(new Symbol[]
{
    new Terminal<GreaterOrEqual>(), new NonTerminals.Start1()
});

public record Start3() : Production<NonTerminals.Start3>(new Symbol[]
{
    new NonTerminals.Start4(), new Next4()
});

public record Start3_Unary() : Production<NonTerminals.Start3>(new Symbol[]
{
    new Terminal<Minus>(), new NonTerminals.Start2()
});

public record Next3_Epsilon() : Production<Next3>(new Symbol[] { new Epsilon() });

public record Start4() : Production<NonTerminals.Start4>(new Symbol[]
{
    new NonTerminals.Start5(), new Next5()
});

public record Next4_Add() : Production<Next4>(new Symbol[]
{
    new Terminal<Addition>(), new NonTerminals.Start3()
});

public record Next4_Minus() : Production<Next4>(new Symbol[]
{
    new Terminal<Minus>(), new NonTerminals.Start3()
});

public record Next4_Eps() : Production<Next4>(new Symbol[] { new Epsilon() });

public record Start5() : Production<NonTerminals.Start5>(new Symbol[]
{
    new NonTerminals.Start6(), new Next6()
});

public record Next5_Mul() : Production<Next5>(new Symbol[]
{
    new Terminal<Multiplication>(), new NonTerminals.Start4()
});

public record Next5_Div() : Production<Next5>(new Symbol[]
{
    new Terminal<Division>(), new NonTerminals.Start4()
});

public record Next5_Eps() : Production<Next5>(new Symbol[] { new Epsilon() });

public record Start6() : Production<NonTerminals.Start6>(new Symbol[]
{
    new Paren()
});

public record Next6_Concat() : Production<Next6>(new Symbol[]
{
    new Terminal<Concatenation>(), new NonTerminals.Start5()
});

public record Next6_Invoke() : Production<Next6>(new Symbol[]
{
    new NonTerminals.Invoke()
});

public record Next6_Epsilon() : Production<Next6>(new Symbol[] { new Epsilon() });

public record Invoke() : Production<NonTerminals.Invoke>(new Symbol[]
{
    new Terminal<OpenParen>(), new NonTerminals.InvokeInner(), new Terminal<CloseParen>()
});

public record InvokeInner() : Production<NonTerminals.InvokeInner>(new Symbol[]
{
    new NonTerminals.Expression(), new InvokeList()
});

public record InvokeInner_Epsilon() : Production<NonTerminals.InvokeInner>(new Symbol[] { new Epsilon() });

public record InvokeList_List() : Production<InvokeList>(new Symbol[]
{
    new Terminal<Comma>(), new NonTerminals.Expression(), new InvokeList()
});

public record InvokeList_Epsilon() : Production<InvokeList>(new Symbol[] { new Epsilon() });

public record Paren_Parenthesis() : Production<Paren>(new Symbol[]
{
    new Terminal<OpenParen>(), new NonTerminals.Expression(), new Terminal<CloseParen>()
});

public record Paren_Value() : Production<Paren>(new Symbol[]
{
    new Value()
});

public record Paren_Block() : Production<Paren>(new Symbol[]
{
    new NonTerminals.Block()
});

public record Paren_Name() : Production<Paren>(new Symbol[]
{
    new Name()
});

public record Block() : Production<NonTerminals.Block>(new Symbol[]
{
    new Terminal<OpenBrace>(), new BlockInner(), new Terminal<CloseBrace>()
});

public record BlockInner_Epsilon() : Production<BlockInner>(new Symbol[] { new Epsilon() });

public record BlockInner_List() : Production<BlockInner>(new Symbol[]
{
    new SingleBlockLine(), new Terminal<Semicolon>(), new BlockInner()
});

public record SingleBlockLine_If() : Production<SingleBlockLine>(new Symbol[]
{
    new NonTerminals.If()
});

public record SingleBlockLine_Assignment() : Production<SingleBlockLine>(new Symbol[]
{
    new NonTerminals.Assignment()
});

public record SingleBlockLine_Invoke() : Production<SingleBlockLine>(new Symbol[]
{
    new Paren(), new NonTerminals.Invoke()
});

public record Value_Dice() : Production<Value>(new Symbol[]
{
    new Terminal<DiceLiteral>()
});

public record Value_Natural() : Production<Value>(new Symbol[]
{
    new Terminal<NaturalLiteral>()
});

public record Value_Bool() : Production<Value>(new Symbol[]
{
    new Terminal<BooleanLiteral>()
});

public record Value_String() : Production<Value>(new Symbol[]
{
    new Terminal<StringLiteral>()
});

public record Value_List() : Production<Value>(new Symbol[]
{
    new List()
});

public record Value_ObjectCreation() : Production<Value>(new Symbol[]
{
    new NonTerminals.ObjectCreation()
});

public record Assignment() : Production<NonTerminals.Assignment>(new Symbol[]
{
    new Terminal<Set>(), new Name(), new Terminal<Tokens.Assignment>(), new NonTerminals.Expression()
});

public record Name_Id() : Production<Name>(new Symbol[]
{
    new Terminal<LowercaseIdentifier>(), new FieldReference()
});

public record Name_This() : Production<Name>(new Symbol[]
{
    new Terminal<This>(), new FieldReference()
});

public record Name_Base() : Production<Name>(new Symbol[]
{
    new Terminal<Base>(), new FieldReference()
});

public record FieldReference_Eps() : Production<FieldReference>(new Symbol[]
    { new Epsilon() });

public record FieldReference_Id() : Production<FieldReference>(new Symbol[]
{
    new Terminal<Access>(), new Terminal<UppercaseIdentifier>()
});

public record ListLiteral() : Production<List>(new Symbol[]
{
    new Terminal<OpenBracket>(), new ListInner(), new Terminal<CloseBracket>()
});

public record ListInner_Eps() : Production<ListInner>(new Symbol[]
    { new Epsilon() });

public record ListInner_List() : Production<ListInner>(new Symbol[]
{
    new NonTerminals.Expression(), new ListInnerCont()
});

public record ListInnerCont_Eps() : Production<ListInnerCont>(new Symbol[] { new Epsilon() });

public record ListInnerCont_List() : Production<ListInnerCont>(new Symbol[]
{
    new Terminal<Comma>(), new NonTerminals.Expression(), new ListInnerCont()
});

public record If() : Production<NonTerminals.If>(new Symbol[]
{
    new Terminal<Tokens.If>(), new NonTerminals.Expression(), new Terminal<Then>(),
    new NonTerminals.Expression(), new Terminal<Else>(), new NonTerminals.Expression()
});

public record ObjectDeclaration() : Production<NonTerminals.ObjectDeclaration>(new Symbol[]
{
    new Terminal<UppercaseIdentifier>(), new NonTerminals.BaseList(), new NonTerminals.ObjInner()
});

public record BaseList() : Production<NonTerminals.BaseList>(new Symbol[]
{
    new Terminal<Extends>(), new Terminal<UppercaseIdentifier>(), new NonTerminals.TraitListStart()
});

public record BaseList_Eps() : Production<NonTerminals.BaseList>(new Symbol[]
    { new Epsilon() });

public record TraitListStart() : Production<NonTerminals.TraitListStart>(new Symbol[]
{
    new Terminal<With>(), new Terminal<UppercaseIdentifier>(), new NonTerminals.TraitListNext()
});

public record TraitListStart_Eps() : Production<NonTerminals.TraitListStart>(new Symbol[]
    { new Epsilon() });

public record TraitListNext() : Production<NonTerminals.TraitListNext>(new Symbol[]
{
    new Terminal<And>(), new Terminal<UppercaseIdentifier>(), new NonTerminals.TraitListNext()
});

public record TraitListNext_Eps() : Production<NonTerminals.TraitListNext>(new Symbol[]
    { new Epsilon() });

public record Fields() : Production<NonTerminals.Fields>(new Symbol[]
{
    new NonTerminals.Field(), new FieldList()
});

public record Fields_Eps() : Production<NonTerminals.Fields>(new Symbol[] { new Epsilon() });

public record FieldsList() : Production<FieldList>(new Symbol[]
{
    new Terminal<Comma>(), new NonTerminals.Field(), new FieldList()
});

public record FieldsList_Eps() : Production<FieldList>(new Symbol[] { new Epsilon() });

public record Field() : Production<NonTerminals.Field>(new Symbol[]
{
    new Terminal<UppercaseIdentifier>(), new Terminal<Colon>(), new NonTerminals.Expression()
});

public record ObjInner() : Production<NonTerminals.ObjInner>(new Symbol[]
{
    new Terminal<OpenBrace>(), new NonTerminals.Fields(), new Terminal<CloseBrace>()
});

public record TraitDeclaration() : Production<NonTerminals.TraitDeclaration>(new Symbol[]
{
    new Terminal<Trait>(), new Terminal<UppercaseIdentifier>(), new TraitRequirements(), new NonTerminals.ObjInner()
});

public record TraitRequirements_Body() : Production<TraitRequirements>(new Symbol[]
{
    new Terminal<For>(), new Terminal<UppercaseIdentifier>()
});

public record TraitRequirements_Eps() : Production<TraitRequirements>(new Symbol[]
    { new Epsilon() });

public record ObjectCreation() : Production<NonTerminals.ObjectCreation>(new Symbol[]
{
    new Terminal<New>(), new Terminal<UppercaseIdentifier>(), new NonTerminals.TraitListStart()
});

public record Function() : Production<NonTerminals.Function>(new Symbol[]
{
    new Terminal<Fun>(), new Terminal<LowercaseIdentifier>(), new Terminal<OpenParen>(),
    new NonTerminals.FunctionParameters(), new Terminal<CloseParen>(), new NonTerminals.Block()
});

public record FunctionParameters() : Production<NonTerminals.FunctionParameters>(new Symbol[]
{
    new Terminal<LowercaseIdentifier>(), new Terminal<Colon>(), new Terminal<UppercaseIdentifier>(),
    new NonTerminals.FunctionParametersNext()
});

public record FunctionParameters_Eps() : Production<NonTerminals.FunctionParameters>(new Symbol[]
    { new Epsilon() });

public record FunctionParametersNext() : Production<NonTerminals.FunctionParametersNext>(new Symbol[]
{
    new Terminal<Comma>(), new Terminal<LowercaseIdentifier>(), new Terminal<Colon>(),
    new Terminal<UppercaseIdentifier>(), new NonTerminals.FunctionParametersNext()
});

public record FunctionParametersNext_Eps() : Production<NonTerminals.FunctionParametersNext>(new Symbol[]
    { new Epsilon() });