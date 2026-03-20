using System.Collections.Generic;
using InventoryAPI.Models;
using System.Data.SqlClient;
using Microsoft.Data.SqlClient;

public partial class ProductRepository
{
    public async Task<Results<List<Dictionary<string, object>>>> GetAll()
    {
        using var conn = new SqlConnection(connectionString);
        conn.Open();

        using var cmd = new SqlCommand("SELECT * FROM dbo.products", conn);
        using var reader = cmd.ExecuteReader();
        
        var results = HelpingFunctions.ConvertToResults(reader);

        return results;
    }
}