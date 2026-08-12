namespace CarDealer.API.Entities
{
    public class ExpenseCategory
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty; // رواتب / إيجار / كهرباء وماء ...

        public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
    }
}