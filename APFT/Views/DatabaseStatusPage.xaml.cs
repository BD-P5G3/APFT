using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Resources;
using System.Xml.Linq;
using APFT.ViewModels;
using Microsoft.Data.SqlClient;
using Microsoft.UI;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Windows.ApplicationModel.Resources;
using Windows.Storage;
using Windows.UI;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace APFT.Views;

public sealed partial class DatabaseStatusPage : Page, INotifyPropertyChanged
{
    public DatabaseStatusViewModel ViewModel
    {
        get;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public static readonly string serverAddress = "tcp:mednat.ieeta.pt\\SQLSERVER,8101";

    private SolidColorBrush iconBadgeColor;
    public SolidColorBrush IconBadgeColor
    {
        get => iconBadgeColor;
        set
        {
            iconBadgeColor = value;
            OnPropertyChanged(nameof(IconBadgeColor));
        }
    }

    private string connectionStatus;
    public string ConnectionStatus
    {
        get => connectionStatus;
        set
        {
            connectionStatus = value;
            OnPropertyChanged(nameof(ConnectionStatus));
        }
    }

    private string database;
    public string Database
    {
        get => database;
        set
        {
            database = value;
            OnPropertyChanged(nameof(Database));
        }
    }

    private string username;
    public string Username
    {
        get => username;
        set
        {
            username = value;
            OnPropertyChanged(nameof(Username));
        }
    }

    private string password;
    public string Password
    {
        get => password;
        set
        {
            password = value;
            OnPropertyChanged(nameof(Password));
        }
    }

    public DatabaseStatusPage()
    {
        LoadStoredValues();
        ViewModel = App.GetService<DatabaseStatusViewModel>();
        InitializeComponent();
    }

    private readonly ResourceLoader resourceLoader = new("resources.pri", "DatabaseStatus");

    private void LoadStoredValues()
    {
        var localSettings = ApplicationData.Current.LocalSettings;

        var colorString = localSettings.Values.ContainsKey("IconBadgeColor") ? localSettings.Values["IconBadgeColor"].ToString() : "#FFFF0000";
        IconBadgeColor = new SolidColorBrush(colorString == "#FF008000" ? Colors.Green : Colors.Red);

        ConnectionStatus = localSettings.Values.ContainsKey("ConnectionStatus") ? localSettings.Values["ConnectionStatus"].ToString() : resourceLoader.GetString("New");
        Database = localSettings.Values.ContainsKey("Database") ? localSettings.Values["Database"].ToString() : "p5g3";
        Username = localSettings.Values.ContainsKey("Username") ? localSettings.Values["Username"].ToString() : "p5g3";
        Password = localSettings.Values.ContainsKey("Password") ? localSettings.Values["Password"].ToString() : "";
    }

    private void SavePropertyValues()
    {
        var localSettings = ApplicationData.Current.LocalSettings;
        localSettings.Values["IconBadgeColor"] = IconBadgeColor.Color.ToString();
        localSettings.Values["ConnectionStatus"] = ConnectionStatus;
        localSettings.Values["Database"] = Database;
        localSettings.Values["Username"] = Username;
        localSettings.Values["Password"] = Password;
        localSettings.Values["SQLConnectionString"] = "Data Source = " + serverAddress + "; " +
                                                      "Initial Catalog = " + Database + "; uid = " + Username + "; " +
                                                      "password = " + Password + "; TrustServerCertificate = True";
    }

    private async void TestDBConnection(string dbServer, string dbName, string userName, string userPass)
    {
        using var cn = new SqlConnection("Data Source = " + dbServer + "; " +
                                         "Initial Catalog = " + dbName + "; uid = " + userName + "; " +
                                         "password = " + userPass + "; TrustServerCertificate = True");
        try
        {
            await cn.OpenAsync();
            ShellPage.Current.SetInfoBadgeColor(Colors.Green);
            IconBadgeColor = new SolidColorBrush(Colors.Green);
            ConnectionStatus = string.Format(resourceLoader.GetString("Success"), cn.Database, cn.DataSource);
        }
        catch (Exception ex)
        {
            ShellPage.Current.SetInfoBadgeColor(Colors.Red);
            IconBadgeColor = new SolidColorBrush(Colors.Red);
            ConnectionStatus = string.Format(resourceLoader.GetString("Error"), cn.Database, ex.Message);
        }

        SavePropertyValues();
    }

    private void testConnectionButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        TestDBConnection(serverAddress, databaseTextBox.Text, userTextBox.Text, passwordBox.Password);
    }
}
