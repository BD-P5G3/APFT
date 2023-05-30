using Microsoft.Data.SqlClient;
using System.Collections.ObjectModel;
using Windows.Storage;

namespace APFT.Entities;

public class Order
{
    public int Id
    {
        get; set;
    }

    public DateTime Date
    {
        get; set;
    }

    public int SupplierNif
    {
        get; set;
    }

    public int ConstructionId
    {
        get; set;
    }

    public string DateString => Date.ToString().Split(" ")[0];
    
    public Order(int id, DateTime date, int supplierNif, int constructionId)
    {
        Id = id;
        Date = date;
        SupplierNif = supplierNif;
        ConstructionId = constructionId;
    }

    public static async Task<ObservableCollection<Order>> GetOrdersAsync()
    {
        var orders = new ObservableCollection<Order>();
        var localSettings = ApplicationData.Current.LocalSettings;

        await using var cn = new SqlConnection(localSettings.Values["SQLConnectionString"].ToString());
        
        await cn.OpenAsync();
        var cmd = new SqlCommand("SELECT * FROM EMPRESA_CONSTRUCAO.ENCOMENDA", cn);

        var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            orders.Add(new Order(
                reader.GetInt32(0),
                reader.GetDateTime(1),
                reader.GetInt32(2),
                reader.GetInt32(3))
            );
        }

        return orders;
    }

    public static async Task<Order> GetOrderByIdAsync(int id)
    {
        var localSettings = ApplicationData.Current.LocalSettings;

        await using var cn = new SqlConnection(localSettings.Values["SQLConnectionString"].ToString());
        
        await cn.OpenAsync();
        var cmd = new SqlCommand("SELECT * FROM EMPRESA_CONSTRUCAO.ENCOMENDA WHERE id=" + id, cn);

        var reader = await cmd.ExecuteReaderAsync();
        await reader.ReadAsync();
        
        return new Order(
            reader.GetInt32(0),
            reader.GetDateTime(1),
            reader.GetInt32(2),
            reader.GetInt32(3));
    }

    public static async Task<int> AddOrder(int id, string dateString, int supplierNif, int constructionId)
    {
        var localSettings = ApplicationData.Current.LocalSettings;
        await using var cn = new SqlConnection(localSettings.Values["SQLConnectionString"].ToString());

        await cn.OpenAsync();
        var cmd = new SqlCommand("EXEC add_encomenda " + id + ", '" + dateString + "', " + supplierNif + ", " + constructionId, cn);

        return await cmd.ExecuteNonQueryAsync();
    }

    public static async Task<int> DeleteOrder(int id)
    {
        var localSettings = ApplicationData.Current.LocalSettings;
        await using var cn = new SqlConnection(localSettings.Values["SQLConnectionString"].ToString());

        await cn.OpenAsync();
        var cmd = new SqlCommand("EXEC delete_order " + id, cn);

        return await cmd.ExecuteNonQueryAsync();
    }

    // Complete with remaining UDFs
}