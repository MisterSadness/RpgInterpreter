namespace RpgInterpreter.TypeChecker;

public interface ITypeCheckResult<out T> where T : Type
{
    T Type { get; }
    TypeMap TypeMap { get; }

    public ITypeCheckResult<TNew> WithType<TNew>(TNew type) where TNew : Type =>
        new TypeCheckResult<TNew>(type, TypeMap);

    public ITypeCheckResult<UnitType> WithUnitType() => new TypeCheckResult<UnitType>(new UnitType(), TypeMap);
}