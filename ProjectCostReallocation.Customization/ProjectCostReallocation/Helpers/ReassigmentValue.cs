namespace ProjectCostReallocation
{
    public class ReassigmentValue
    {
        public ReassigmentValue(bool isValid, decimal amount, decimal sourceAmount, decimal destAmount, decimal percentage, string selection)
        {
            IsValid = isValid;
            Amount = amount;
            Percentage = percentage;
            Selection = selection;
            SourceAmount = sourceAmount;
            DestAmount = destAmount;
        }

        public decimal Amount { get; }       
        public decimal SourceAmount { get; }
        public decimal DestAmount { get; }
        public string Selection { get; }
        public decimal Percentage { get; }
        public bool IsValid { get; }
    }
}