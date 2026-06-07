namespace InventoryAPI.Exceptions
{
    public class DomainException : Exception
    {
        public string Code {get; }

        public DomainException(string code, string message) : base(message)
        {
            Code = code;
        }

        public class ProductNotFoundException : DomainException
        {
            public const string ERROR_CODE = "PRODUCT_NOT_FOUND";
            public const string ERROR_MESSAGE = "Product not found";

            public ProductNotFoundException()
                : base(ERROR_CODE, ERROR_MESSAGE)
            {
            }
        }

        public class WarehouseNotFoundException : DomainException
        {
            public const string ERROR_CODE = "WAREHOUSE_NOT_FOUND";
            public const string ERROR_MESSAGE = "Warehouse not found";

            public WarehouseNotFoundException()
                : base(ERROR_CODE, ERROR_MESSAGE)
            {
            }
        }

        public class UserNotFoundException : DomainException
        {
            public const string ERROR_CODE = "USER_NOT_FOUND";
            public const string ERROR_MESSAGE = "User not found";

            public UserNotFoundException()
                : base(ERROR_CODE, ERROR_MESSAGE)
            {
            }
        }

        public class ProductInactiveException : DomainException
        {
            public const string ERROR_CODE = "PRODUCT_INACTIVE";
            public const string ERROR_MESSAGE = "Product already inactive";

            public ProductInactiveException()
                : base(ERROR_CODE, ERROR_MESSAGE)
            {
            }
        }

        public class WarehouseInactiveException : DomainException
        {
            public const string ERROR_CODE = "WAREHOUSE_INACTIVE";
            public const string ERROR_MESSAGE = "Warehouse already inactive";

            public WarehouseInactiveException()
                : base(ERROR_CODE, ERROR_MESSAGE)
            {
            }
        }

        public class ProductAlreadyExistsException : DomainException
        {
            public ProductAlreadyExistsException(string sku)
                : base("PRODUCT_DUPLICATE_SKU", $"Product with SKU '{sku}' already exists")
            {
            }
        }

        public class WarehouseAlreadyExistsException : DomainException
        {
            public WarehouseAlreadyExistsException(string name)
                : base("WAREHOUSE_DUPLICATE_NAME", $"Warehouse with Name '{name}' already exists")
            {
            }
        }

        public class InsufficientStockException : DomainException
        {
            public InsufficientStockException(int productId, int warehouseId)
                : base("INSUFFICIENT_STOCK", $"Insufficient stock for product {productId} in warehouse {warehouseId}")
            {
            }
        }
    }
}