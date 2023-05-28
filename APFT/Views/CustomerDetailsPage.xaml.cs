using APFT.ViewModels;

using Microsoft.UI.Xaml.Controls;
using Windows.Storage;
using Microsoft.Data.SqlClient;
using System.ComponentModel;
using Microsoft.UI.Xaml;
using Microsoft.Windows.ApplicationModel.Resources;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Microsoft.UI.Xaml.Media.Animation;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace APFT.Views;

public sealed partial class CustomerDetailsPage : INotifyPropertyChanged
{
    public CustomerDetailsViewModel ViewModel
    {
        get;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private int _nif;
    public int Nif
    {
        get => _nif;
        set
        {
            _nif = value;
            OnPropertyChanged(nameof(Nif));
        }
    }

    private string _firstName;
    public string FirstName
    {
        get => _firstName;
        set
        {
            _firstName = value;
            OnPropertyChanged(nameof(FirstName));
        }
    }

    private string _lastName;
    public string LastName
    {
        get => _lastName;
        set
        {
            _lastName = value;
            OnPropertyChanged(nameof(LastName));
        }
    }

    private string _address;
    public string Address
    {
        get => _address;
        set
        {
            _address = value;
            OnPropertyChanged(nameof(Address));
        }
    }

    private string _email;
    public string Email
    {
        get => _email;
        set
        {
            _email = value;
            OnPropertyChanged(nameof(Email));
        }
    }

    private int _phone;
    public int Phone
    {
        get => _phone;
        set
        {
            _phone = value;
            OnPropertyChanged(nameof(Phone));
        }
    }

    public CustomerDetailsPage()
    {
        ViewModel = App.GetService<CustomerDetailsViewModel>();
        FetchData();
        InitializeComponent();
    }

    private async void FetchData()
    {
        var localSettings = ApplicationData.Current.LocalSettings;

        await using var cn = new SqlConnection(localSettings.Values["SQLConnectionString"].ToString());
        await cn.OpenAsync();

        var cmd = new SqlCommand("SELECT * FROM EMPRESA_CONSTRUCAO.CLIENTE WHERE nif=" + localSettings.Values["CustomerNif"], cn);
        var reader = await cmd.ExecuteReaderAsync();
        await reader.ReadAsync();

        Nif = reader.GetInt32(0);
        FirstName = reader.GetString(1);
        LastName = reader.GetString(2);
        Email = reader.GetString(3);
        Phone = reader.GetInt32(4);
        Address = reader.GetString(5);

        ConstructionsGridView.ItemsSource = await Construction.GetConstructionsAsync(Nif);
        if (ConstructionsGridView.Items.Count > 0)
        {
            ConstructionsSubtitle.Visibility = Visibility.Visible;
        }
    }

    private readonly ResourceLoader _resourceLoader = new("resources.pri", "CustomerDetails");

    private async void EditInfoButton_OnClick(object sender, RoutedEventArgs e)
    {
        // Create a fade-out animation for the edit icon
        var fadeOutEditAnimation = new DoubleAnimation
        {
            From = 1,
            To = 0,
            Duration = TimeSpan.FromSeconds(0.2),
            EnableDependentAnimation = true
        };
        Storyboard.SetTarget(fadeOutEditAnimation, EditInfoIcon);
        Storyboard.SetTargetProperty(fadeOutEditAnimation, "Opacity");

        // Create a fade-in animation for the progress ring
        var fadeInPrAnimation = new DoubleAnimation
        {
            From = 0,
            To = 1,
            Duration = TimeSpan.FromSeconds(0.2),
            EnableDependentAnimation = true
        };
        Storyboard.SetTarget(fadeInPrAnimation, EditInfoProgressRing);
        Storyboard.SetTargetProperty(fadeInPrAnimation, "Opacity");

        // Run animations
        var sb = new Storyboard();
        sb.Children.Add(fadeOutEditAnimation);
        sb.Children.Add(fadeInPrAnimation);
        sb.Begin();


        // Action
        var localSettings = ApplicationData.Current.LocalSettings;
        await using var cn = new SqlConnection(localSettings.Values["SQLConnectionString"].ToString());

        await cn.OpenAsync();
        var cmd = new SqlCommand("EXEC update_client " + Nif + ", " +
                                 "'" + FirstName + "', " +
                                 "'" + LastName + "', " +
                                 "'" + Email + "', " +
                                 Phone + ", " +
                                 "'" + Address + "'", cn);

        _ = await cmd.ExecuteNonQueryAsync();


        // Create a fade-out animation for the ProgressRing
        var fadeOutPrAnimation = new DoubleAnimation
        {
            From = 1,
            To = 0,
            Duration = TimeSpan.FromSeconds(0.2),
            EnableDependentAnimation = true
        };
        Storyboard.SetTarget(fadeOutPrAnimation, EditInfoProgressRing);
        Storyboard.SetTargetProperty(fadeOutPrAnimation, "Opacity");

        // Create a fade-in animation for the checkmark icon
        var fadeInCheckmarkAnimation = new DoubleAnimation
        {
            From = 0,
            To = 1,
            Duration = TimeSpan.FromSeconds(0.2),
            EnableDependentAnimation = true
        };
        Storyboard.SetTarget(fadeInCheckmarkAnimation, EditInfoIconCheckmark);
        Storyboard.SetTargetProperty(fadeInCheckmarkAnimation, "Opacity");

        // Run animations
        sb = new Storyboard();
        sb.Children.Add(fadeOutPrAnimation);
        sb.Children.Add(fadeInCheckmarkAnimation);
        sb.Begin();


        await Task.Delay(TimeSpan.FromSeconds(2));

        // Create a fade-out animation for the checkmark icon
        var fadeOutCheckmarkAnimation = new DoubleAnimation
        {
            From = 1,
            To = 0,
            Duration = TimeSpan.FromSeconds(0.2),
            EnableDependentAnimation = true
        };
        Storyboard.SetTarget(fadeOutCheckmarkAnimation, EditInfoIconCheckmark);
        Storyboard.SetTargetProperty(fadeOutCheckmarkAnimation, "Opacity");

        // Create a fade-in animation for the edit icon
        var fadeInEditAnimation = new DoubleAnimation
        {
            From = 0,
            To = 1,
            Duration = TimeSpan.FromSeconds(0.2),
            EnableDependentAnimation = true
        };
        Storyboard.SetTarget(fadeInEditAnimation, EditInfoIcon);
        Storyboard.SetTargetProperty(fadeInEditAnimation, "Opacity");

        sb = new Storyboard();
        sb.Children.Add(fadeOutCheckmarkAnimation);
        sb.Children.Add(fadeInEditAnimation);
        sb.Begin();
    }

    private async void DeleteButton_OnClick(object sender, RoutedEventArgs e)
    {
        var dialog = new ContentDialog
        {
            XamlRoot = ContentArea.XamlRoot,
            Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
            Title = _resourceLoader.GetString("DeleteCustomerCD_Title"),
            PrimaryButtonText = _resourceLoader.GetString("DeleteCustomerCD_PrimaryButton"),
            DefaultButton = ContentDialogButton.Primary,
            CloseButtonText = _resourceLoader.GetString("DeleteCustomerCD_CancelButton"),
            Content = _resourceLoader.GetString("DeleteCustomerCD_Content")
        };

        var choice = await dialog.ShowAsync();

        if (choice != ContentDialogResult.Primary) { return; }

        var localSettings = ApplicationData.Current.LocalSettings;

        await using var cn = new SqlConnection(localSettings.Values["SQLConnectionString"].ToString());

        await cn.OpenAsync();
        var cmd = new SqlCommand("EXEC delete_client " + Nif, cn);

        var rowsAffected = await cmd.ExecuteNonQueryAsync();
        Debug.Assert(rowsAffected > 0);
        Debug.WriteLine(rowsAffected);

        Frame.GoBack();
    }

    private async void ConstructionsGridView_OnItemClick(object sender, ItemClickEventArgs e)
    {
        var dialog = new ContentDialog
        {
            XamlRoot = ContentArea.XamlRoot,
            Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
            Title = _resourceLoader.GetString("NotImplemented_Title"),
            PrimaryButtonText = _resourceLoader.GetString("NotImplemented_Close"),
            DefaultButton = ContentDialogButton.Primary,
            Content = _resourceLoader.GetString("NotImplemented_Content")
        };
        _ = await dialog.ShowAsync();
    }
}

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

    public static async Task<ObservableCollection<Construction>> GetConstructionsAsync(int customerNif)
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