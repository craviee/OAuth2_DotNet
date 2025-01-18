namespace BuildingBlocks.Domain;

public class BusinessRuleValidationException(IBusinessRule brokenRule) : Exception(brokenRule.Message)
{
    public IBusinessRule BrokenRule { get; } = brokenRule;
    public string Details { get; } = brokenRule.Message;
    public override string ToString() => 
        $"{BrokenRule.GetType().FullName}: {BrokenRule.Message}";
}