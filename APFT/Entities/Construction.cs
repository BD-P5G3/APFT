using Microsoft.Data.SqlClient;
using System.Collections.ObjectModel;
using Windows.Storage;

namespace APFT.Entities;

public class Construction
{
    public int Id
    {
        get; set;
    }

    public string Location
    {
        get; set;
    }

    public DateTime StartDate
    {
        get; set;
    }

    public DateTime? EndDate
    {
        get; set;
    }

    public int CustomerNif
    {
        get; set;
    }

    public string StartDateString => StartDate.ToString().Split(" ")[0];
    public string EndDateString => EndDate == null ? EndDate.ToString().Split(" ")[0] : string.Empty;

    public Construction(int id, string location, DateTime startDate, DateTime? endDate, int customerNif)
    {
        Id = id;
        Location = location;
        StartDate = startDate;
        EndDate = endDate;
        CustomerNif = customerNif;
    }

    public static async Task<ObservableCollection<Construction>> GetConstructionsAsync()
    {
        var constructions = new ObservableCollection<Construction>();
        var localSettings = ApplicationData.Current.LocalSettings;

        await using var cn = new SqlConnection(localSettings.Values["SQLConnectionString"].ToString());

        await cn.OpenAsync();
        var cmd = new SqlCommand("SELECT * FROM EMPRESA_CONSTRUCAO.OBRA", cn);

        var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            constructions.Add(new Construction(
                reader.GetInt32(0),
                reader.GetString(1),
                reader.GetDateTime(2),
                await reader.IsDBNullAsync(3) ? null : reader.GetDateTime(3),
                reader.GetInt32(4)
            ));
        }

        return constructions;
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
                await reader.IsDBNullAsync(3) ? null : reader.GetDateTime(3),
                reader.GetInt32(4)
            ));
        }

        return constructions;
    }

    public static async Task<ObservableCollection<Construction>> GetConstructionsByDateAsync(string startDateString, string endDateString)
    {
        var constructions = new ObservableCollection<Construction>();
        var localSettings = ApplicationData.Current.LocalSettings;

        await using var cn = new SqlConnection(localSettings.Values["SQLConnectionString"].ToString());

        await cn.OpenAsync();

        var cmd = new SqlCommand("SELECT * FROM getObraByDate('" + startDateString + "', '" + endDateString + "')", cn);

        var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            constructions.Add(new Construction(
                reader.GetInt32(0),
                reader.GetString(1),
                reader.GetDateTime(2),
                await reader.IsDBNullAsync(3) ? null : reader.GetDateTime(3),
                reader.GetInt32(4)
            ));
        }

        return constructions;
    }
}