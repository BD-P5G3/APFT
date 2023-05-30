using APFT.ViewModels;

using Microsoft.UI.Xaml.Controls;
using APFT.Entities;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml;
using Microsoft.UI;
using System.Diagnostics;
using Windows.Storage;
using Microsoft.Windows.ApplicationModel.Resources;
using WinUICommunity;

namespace APFT.Views;

public sealed partial class ObrasPage : Page
{
    public ObrasViewModel ViewModel
    {
        get;
    }

    public ObrasPage()
    {
        ViewModel = App.GetService<ObrasViewModel>();
        InitializeComponent();
    }

    private readonly ResourceLoader _resourceLoader = new("resources.pri", "Constructions");

    protected async override void OnNavigatedTo(NavigationEventArgs e)
    {
        try
        {
            ConstructionsListView.ItemsSource = await Construction.GetConstructionsAsync();
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
        localSettings.Values["ConstructionId"] = ((Construction)e.ClickedItem).Id.ToString();

        Frame.Navigate(typeof(ConstructionDetailsPage)); // TODO: Fix this
    }

    // TODO: Fix this
    private async void AddButton_OnClick(object sender, RoutedEventArgs e)
    {
        var localSettings = ApplicationData.Current.LocalSettings;

        await Construction.AddConstruction(
            int.Parse(IdTextBox.Text),
            LocationTextBox.Text,
            StartDatePicker.Date.Value.Year + "-" + StartDatePicker.Date.Value.Month + "-" + StartDatePicker.Date.Value.Day, 
            string.Empty, 
            int.Parse(CustomerNifTextBox.Text));

        localSettings.Values["ConstructionId"] = IdTextBox.Text;
        Frame.Navigate(typeof(ConstructionDetailsPage));
    }
}
