using InventoryAPI.Models;
using Microsoft.Data.SqlClient;

public class HelpingFunctions
{
    //Dynamische Ausgabe eines SQL Ergebnisses in das Results Objekt
    public static Results<List<Dictionary<string, object>>> ConvertToResults(SqlDataReader reader)
    {
        var results = new Results<List<Dictionary<string, object>>>
        {
            Data = new List<Dictionary<string, object>>()
        };

        try
        {
            while (reader.Read())
            {
                var row = new Dictionary<string, object>();

                for (int i = 0; i < reader.FieldCount; i++)
                {
                    row[reader.GetName(i)] = reader.GetValue(i);
                }

                results.Data.Add(row);
            }
        }
        catch (Exception ex)
        {
            results.Success = false;
            results.Error = ex.Message;
        }

        results.Success = true;
        results.Error = null;

        return results;
    }
}