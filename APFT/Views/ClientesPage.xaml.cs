using System.Collections.ObjectModel;
using System.Diagnostics;
using APFT.ViewModels;
using Microsoft.Data.SqlClient;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Windows.Storage;

namespace APFT.Views;

public sealed partial class ClientesPage
{
    public ClientesViewModel ViewModel
    {
        get;
    }

    public ClientesPage()
    {
        ViewModel = App.GetService<ClientesViewModel>();
        InitializeComponent();
    }

    protected async override void OnNavigatedTo(NavigationEventArgs e)
    {
        try
        {
            CustomersCVS.Source = await Customer.GetClientsGroupedAsync();
            FetchingDataGrid.Visibility = Visibility.Collapsed;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            ShellPage.Current.SetInfoBadgeColor(Colors.Red);
            FetchingDataGrid.Visibility = Visibility.Collapsed;
            FetchingDataGridError.Visibility = Visibility.Visible;

            var dialog = new ContentDialog
            {
                XamlRoot = ContentArea.XamlRoot,
                Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
                Title = "Table content",
                PrimaryButtonText = "Close",
                DefaultButton = ContentDialogButton.Primary,
                Content = "Unable to fetch data due to the following error: " + ex.Message,
            };
            _ = await dialog.ShowAsync();
        }
    }
}


// Contact class definition:
public class Customer
{
    public int Nif
    {
        get; private set;
    }
    public string FirstName
    {
        get; private set;
    }
    public string LastName
    {
        get; private set;
    }
    public string Email
    {
        get; private set;
    }
    public int Phone
    {
        get; private set;
    }
    public string? Address
    {
        get; private set;
    }
    public string Name => FirstName + " " + LastName;

    public Customer(int nif, string firstName, string lastName, string email, int phone, string address)
    {
        Nif = nif;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Phone = phone;
        Address = address;
    }

    public static async Task<ObservableCollection<GroupInfoList>> GetClientsGroupedAsync()
    {
        // Grab Contact objects from pre-existing list (list is returned from function GetContactsAsync())
        var query = from item in await GetClientsAsync()

                    // Group the items returned from the query, sort and select the ones you want to keep
                    group item by item.LastName.Substring(0, 1).ToUpper() into g
                    orderby g.Key

                    // GroupInfoList is a simple custom class that has an IEnumerable type attribute, and
                    // a key attribute. The IGrouping-typed variable g now holds the Contact objects,
                    // and these objects will be used to create a new GroupInfoList object.
                    select new GroupInfoList(g) { Key = g.Key };

        Debug.WriteLine(query);

        return new ObservableCollection<GroupInfoList>(query);
    }

    public static async Task<ObservableCollection<Customer>> GetClientsAsync()
    {
        var customers = new ObservableCollection<Customer>();
        var localSettings = ApplicationData.Current.LocalSettings;

        await using var cn = new SqlConnection(localSettings.Values["SQLConnectionString"].ToString());
        
        await cn.OpenAsync();
        var cmd = new SqlCommand("SELECT * FROM EMPRESA_CONSTRUCAO.CLIENTE", cn);

        var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            customers.Add(new Customer(
                reader.GetInt32(0),
                reader.GetString(1),
                reader.GetString(2),
                reader.GetString(3),
                reader.GetInt32(4),
                reader.GetString(5))
            );
        }

        return customers;
    }
}

// GroupInfoList class definition:
public class GroupInfoList : List<object>
{
    public GroupInfoList(IEnumerable<object> items) : base(items)
    {
        Key = 0; // Fix
    }
    public object Key
    {
        get; set;
    }

    public override string ToString()
    {
        return "Group " + Key;
    }
}