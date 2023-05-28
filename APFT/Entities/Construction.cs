using Microsoft.Data.SqlClient;
using System.Collections.ObjectModel;
using Windows.Storage;

namespace APFT.Entities;

public class Construction
{
    public int Id
    {
        get;
    }
    public string Location
    {
        get;
    }
    public DateTime StartDate
    {
        get;
    }
    public DateTime EndDate
    {
        get;
    }
    public int CustomerNif
    {
        get;
    }
    public string StartDateString => StartDate.ToString().Split(" ")[0];
    public string EndDateString => EndDate.ToString().Split(" ")[0];

    public Construction(int id, string location, DateTime startDate, DateTime endDate, int customerNif)
    {
        Id = id;
        Location = location;
        StartDate = startDate;
        EndDate = endDate;
        CustomerNif = customerNif;
    }

    public static async Task<ObservableCollection<Construction>> GetConstructionsByCustomerNifAsync(int customerNif)
    {
        var constructions = new ObservableCollection<Construction>();
        var localSettings = ApplicationData.Current.LocalSettings;

        await using var cn = new SqlConnection(localSettings.Values["SQLConnectionString"].ToString());

        await cn.OpenAsync();
        var cmd = new SqlCommand("SELECT * FROM getObraByClientNif(" + customerNif + ")", cn);

        var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            constructions.Add(new Construction(
                reader.GetInt32(0),
                reader.GetString(1),
                reader.GetDateTime(2),
                reader.GetDateTime(3),
                reader.GetInt32(4)
            ));
        }

        return constructions;
    }
}