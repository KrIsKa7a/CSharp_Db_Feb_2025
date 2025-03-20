namespace NetPay.Common
{
    public static class EntityValidationConstants
    {
        public static class Household
        {
            public const int ContactPersonMinLength = 5;
            public const int ContactPersonMaxLength = 50;

            public const int EmailMinLength = 6;
            public const int EmailMaxLength = 80;

            public const int PhoneNumberLength = 15;
            public const string PhoneNumberColumnType = @"CHAR(15)";
            public const string PhoneNumberValidationRegex = @"^\+\d{3}/\d{3}-\d{6}$";
        }

        public static class Expense
        {
            public const int ExpenseNameMinLength = 5;
            public const int ExpenseNameMaxLength = 50;

            public const decimal AmountMinValue = 0.01m;
            public const string AmountMinValueStr = "0.01";
            public const decimal AmountMaxValue = 100_000m;
            public const string AmountMaxValueStr = "100000";
            public const string AmountColumnType = "decimal(18,2)";

            public const string DueDateFormat = "yyyy-MM-dd";
        }

        public static class Service
        {
            public const int ServiceNameMinLength = 5;
            public const int ServiceNameMaxLength = 30;
        }

        public static class Supplier
        {
            public const int SupplierNameMinLength = 3;
            public const int SupplierNameMaxLength = 60;
        }
    }
}
