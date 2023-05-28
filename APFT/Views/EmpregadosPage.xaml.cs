using System.Diagnostics;
using APFT.ViewModels;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Windows.Storage;
using Microsoft.Data.SqlClient;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.Windows.ApplicationModel.Resources;
using WinUICommunity;
using System.Collections.ObjectModel;

namespace APFT.Views;

public sealed partial class EmpregadosPage : Page
{
    public EmpregadosViewModel ViewModel
    {
        get;
    }

    public EmpregadosPage()
    {
        ViewModel = App.GetService<EmpregadosViewModel>();
        InitializeComponent();
    }

    private readonly ResourceLoader _resourceLoader = new("resources.pri", "Employees");

    protected async override void OnNavigatedTo(NavigationEventArgs e)
    {
        try
        {
            EmployeesCVS.Source = await Employee.GetEmployeesGroupedAsync();
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
                Title = _resourceLoader.GetString("ContentDialog_Title"),
                PrimaryButtonText = _resourceLoader.GetString("ContentDialog_PrimaryButtonText"),
                DefaultButton = ContentDialogButton.Primary,
                Content = string.Format(_resourceLoader.GetString("ContentDialog_Content"), ex.Message)
            };
            _ = await dialog.ShowAsyncQueue();
        }
    }

    private void ListViewBase_OnItemClick(object sender, ItemClickEventArgs e)
    {
        var localSettings = ApplicationData.Current.LocalSettings;
        localSettings.Values["EmployeeNif"] = ((Employee)e.ClickedItem).Nif.ToString();

        Frame.Navigate(typeof(CustomerDetailsPage));
    }
    

    private async void AddButton_OnClick(object sender, RoutedEventArgs e)
    {
        var localSettings = ApplicationData.Current.LocalSettings;

        await using var cn = new SqlConnection(localSettings.Values["SQLConnectionString"].ToString());

        await cn.OpenAsync();
        var cmd = new SqlCommand("EXEC add_employee " + NifTextBox.Text + ", " +
                                 "'" + FNameTextBox.Text + "', " +
                                 "'" + LNameTextBox.Text + "', " +
                                 "'" + EmailTextBox.Text + "', " +
                                 PhoneTextBox.Text + ", " +
                                 "'" + AddressTextBox.Text + "'", cn);

        _ = await cmd.ExecuteNonQueryAsync();

        localSettings.Values["EmployeeNif"] = NifTextBox.Text;
        Frame.Navigate(typeof(CustomerDetailsPage));
    }

    private async void AutoSuggestBox_OnTextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {
        // Since selecting an item will also change the text,
        // only listen to changes caused by user entering text.
        if(args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
        {
            if (sender.Text.Length >= 3)
            {
                var suitableItems = new List<string>();
                var localSettings = ApplicationData.Current.LocalSettings;

                await using var cn = new SqlConnection(localSettings.Values["SQLConnectionString"].ToString());
        
                await cn.OpenAsync();
                var cmd = new SqlCommand("SELECT * FROM getEmpregadoByName('" + sender.Text.ToLower() + "')", cn);

                var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    suitableItems.Add(reader.GetString(1) + " " + reader.GetString(2) + " (" + reader.GetInt32(0) + ")");
                }

                if(suitableItems.Count == 0)
                {
                    suitableItems.Add(_resourceLoader.GetString("AutoSuggestBox_NotFound"));
                }

                sender.ItemsSource = suitableItems;
            }
            else
            {
                sender.ItemsSource = null;
            }
        }
    }

    private void AutoSuggestBox_OnSuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
    {
        var person = args.SelectedItem.ToString() ?? "(0)";
        if (person == _resourceLoader.GetString("AutoSuggestBox_NotFound"))
        {
            SearchBox.Text = "";
            return;
        }

        var nif = person.Split('(', ')')[1].Trim();

        var localSettings = ApplicationData.Current.LocalSettings;
        localSettings.Values["EmployeeNif"] = nif;

        Frame.Navigate(typeof(CustomerDetailsPage));
    }
}


public class Employee
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

    public string? Gender
    {
        get; private set;
    }

    public DateTime? BirthDate
    {
        get; private set;
    }

    public decimal Salary
    {
        get; private set;
    }

    public int DepartmentId
    {
        get; private set;
    }

    public string Name => FirstName + " " + LastName;

    public Employee(int nif, string firstName, string lastName, string email, int phone, string address, string gender, DateTime birthDate, decimal salary, int departmentId)
    {
        Nif = nif;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Phone = phone;
        Address = address;
        Gender = gender;
        BirthDate = birthDate;
        Salary = salary;
        DepartmentId = departmentId;
    }

    public static async Task<ObservableCollection<GroupInfoList>> GetEmployeesGroupedAsync()
    {
        // Grab Contact objects from pre-existing list (list is returned from function GetContactsAsync())
        var query = from item in await GetEmployeesAsync()

                    // Group the items returned from the query, sort and select the ones you want to keep
                    group item by item.FirstName[..1].ToUpper() into g
                    orderby g.Key

                    // GroupInfoList is a simple custom class that has an IEnumerable type attribute, and
                    // a key attribute. The IGrouping-typed variable g now holds the Contact objects,
                    // and these objects will be used to create a new GroupInfoList object.
                    select new GroupInfoList(g) { Key = g.Key };

        return new ObservableCollection<GroupInfoList>(query);
    }

    public static async Task<ObservableCollection<Employee>> GetEmployeesAsync()
    {
        var employees = new ObservableCollection<Employee>();
        var localSettings = ApplicationData.Current.LocalSettings;

        await using var cn = new SqlConnection(localSettings.Values["SQLConnectionString"].ToString());
        
        await cn.OpenAsync();
        var cmd = new SqlCommand("SELECT * FROM EMPRESA_CONSTRUCAO.EMPREGADO", cn);

        var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            employees.Add(new Employee(
                reader.GetInt32(0),
                reader.GetString(1),
                reader.GetString(2),
                reader.GetString(3),
                reader.GetInt32(4),
                await reader.IsDBNullAsync(5) ? "" : reader.GetString(5),
                await reader.IsDBNullAsync(6) ? "" : reader.GetString(6),
                await reader.IsDBNullAsync(7) ? new DateTime(1970, 1, 1) : reader.GetDateTime(7),
                reader.GetDecimal(8),
                reader.GetInt32(9))
            );
        }

        return employees;
    }
}
