using APFT.ViewModels;

using Microsoft.UI.Xaml.Controls;

namespace APFT.Views;

public sealed partial class DatabaseStatusPage : Page
{
    public DatabaseStatusViewModel ViewModel
    {
        get;
    }

    public DatabaseStatusPage()
    {
        ViewModel = App.GetService<DatabaseStatusViewModel>();
        InitializeComponent();
    }
}
