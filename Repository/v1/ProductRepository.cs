public partial class ProductRepository
{
    //Hier allgemeiner Code für das Repository
    private readonly string connectionString;

    public ProductRepository(IConfiguration configuration)
    {
        connectionString = configuration.GetConnectionString("DefaultConnection");
    }
}