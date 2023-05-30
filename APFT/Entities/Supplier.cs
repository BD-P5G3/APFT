using Microsoft.Data.SqlClient;
using System.Collections.ObjectModel;
using Windows.Storage;

namespace APFT.Entities;

public class Supplier
{
    public int Id
    {
        get; set;
    }

    public string Name
    {
        get; set;
    }

    public int Phone
    {
        get; set;
    }

    public string Email
    {
        get; set;
    }

    public string? Address
    {
        get; set;
    }
    
    public Supplier(int id, string name, int phone, string email, string? address)
    {
        Id = id;
        Name = name;
        Phone = phone;
        Email = email;
        Address = address;
    }

    public static async Task<ObservableCollection<Supplier>> GetSuppliersAsync()
    {
        var suppliers = new ObservableCollection<Supplier>();
        var localSettings = ApplicationData.Current.LocalSettings;

        await using var cn = new SqlConnection(localSettings.Values["SQLConnectionString"].ToString());
        
        await cn.OpenAsync();
        var cmd = new SqlCommand("SELECT * FROM EMPRESA_CONSTRUCAO.FORNECEDOR", cn);

        var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            suppliers.Add(new Supplier(
                reader.GetInt32(0),
                reader.GetString(1),
                reader.GetInt32(2),
                reader.GetString(3),
                reader.GetString(4))
            );
        }

        return suppliers;
    }

    public static async Task<ObservableCollection<Supplier>> GetSuppliersByNameAsync(string name)
    {
        var suppliers = new ObservableCollection<Supplier>();
        var localSettings = ApplicationData.Current.LocalSettings;

        await using var cn = new SqlConnection(localSettings.Values["SQLConnectionString"].ToString());
        
        await cn.OpenAsync();
        var cmd = new SqlCommand("SELECT * FROM getFornecedorByName('" + name + "')", cn);

        var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            suppliers.Add(new Supplier(
                reader.GetInt32(0),
                reader.GetString(1),
                reader.GetInt32(2),
                reader.GetString(3),
                reader.GetString(4))
            );
        }

        return suppliers;
    }

    public static async Task<Supplier> GetSupplierByNifAsync(int nif)
    {
        var localSettings = ApplicationData.Current.LocalSettings;

        await using var cn = new SqlConnection(localSettings.Values["SQLConnectionString"].ToString());
        
        await cn.OpenAsync();
        var cmd = new SqlCommand("SELECT * FROM EMPRESA_CONSTRUCAO.FORNECEDOR WHERE nif=" + nif, cn);

        var reader = await cmd.ExecuteReaderAsync();
        await reader.ReadAsync();
        
        return new Supplier(
            reader.GetInt32(0),
            reader.GetString(1),
            reader.GetInt32(2),
            reader.GetString(3),
            reader.GetString(4));
    }

    public static async Task<int> AddSupplier(int nif, string name, int phone, string email, string? address)
    {
        var localSettings = ApplicationData.Current.LocalSettings;
        await using var cn = new SqlConnection(localSettings.Values["SQLConnectionString"].ToString());

        await cn.OpenAsync();
        var cmd = new SqlCommand("EXEC create_fornecedor " + nif + ", '" + name + "', " + phone + ", '" + email + "', " + 
                                 (string.IsNullOrEmpty(address) ? "null" : "'" + address + "'"), cn);

        return await cmd.ExecuteNonQueryAsync();
    }

    public static async Task<int> EditSupplier(int nif, string name, int phone, string email, string? address)
    {
        var localSettings = ApplicationData.Current.LocalSettings;
        await using var cn = new SqlConnection(localSettings.Values["SQLConnectionString"].ToString());

        await cn.OpenAsync();
        var cmd = new SqlCommand("EXEC update_fornecedor " + nif + ", '" + name + "', " + phone + ", '" + email + "', " + 
                                 (string.IsNullOrEmpty(address) ? "null" : "'" + address + "'"), cn);

        return await cmd.ExecuteNonQueryAsync();
    }

    public static async Task<int> DeleteSupplier(int nif)
    {
        var localSettings = ApplicationData.Current.LocalSettings;
        await using var cn = new SqlConnection(localSettings.Values["SQLConnectionString"].ToString());

        await cn.OpenAsync();
        var cmd = new SqlCommand("EXEC delete_fornecedor " + nif, cn);

        return await cmd.ExecuteNonQueryAsync();
    }
}