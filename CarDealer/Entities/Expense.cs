namespace CarDealer.API.Entities
{
    public class Expense
    {
        public int Id { get; set; }

        public int CategoryId { get; set; }
        public ExpenseCategory Category { get; set; } = null!;

        public decimal Amount { get; set; }
        public string? Description { get; set; }

        public DateTime Date { get; set; } = DateTime.UtcNow;       // تاريخ المصروف الفعلي
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;  // تاريخ الإدخال بالنظام

        public bool IsDeleted { get; set; } = false;
    }
}