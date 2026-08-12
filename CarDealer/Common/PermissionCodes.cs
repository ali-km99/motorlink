namespace CarDealer.API.Common;

public static class PermissionCodes
{
    public const string CarsView = "Cars.View";
    public const string CarsCreate = "Cars.Create";
    public const string CarsUpdate = "Cars.Update";
    public const string CarsDelete = "Cars.Delete";
    public const string CarsUploadImages = "Cars.UploadImages";

    public const string MaintenanceView = "Maintenance.View";
    public const string MaintenanceCreate = "Maintenance.Create";
    public const string MaintenanceUpdate = "Maintenance.Update";
    public const string MaintenanceDelete = "Maintenance.Delete";

    public const string CustomersView = "Customers.View";
    public const string CustomersCreate = "Customers.Create";
    public const string CustomersUpdate = "Customers.Update";
    public const string CustomersDelete = "Customers.Delete";
    public const string SalesView = "Sales.View";
    public const string SalesCreate = "Sales.Create";

    public const string DashboardView = "Dashboard.View";
    public const string TransactionsView = "Transactions.View";
    public const string CarsShare = "Cars.Share";
    public const string UsersManage = "Users.Manage"; // إدارة المستخدمين والصلاحيات نفسها

    // ───  صلاحيات المصروفات ─────────────────────────────────────────
    public const string ExpensesView = "Expenses.View";
    public const string ExpensesCreate = "Expenses.Create";
    public const string ExpensesUpdate = "Expenses.Update";
    public const string ExpensesDelete = "Expenses.Delete";
}