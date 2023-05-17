using APFT.ViewModels;

using Microsoft.UI.Xaml.Controls;

namespace APFT.Views;

public sealed partial class EncomendasPage : Page
{
    public EncomendasViewModel ViewModel
    {
        get;
    }

    public EncomendasPage()
    {
        ViewModel = App.GetService<EncomendasViewModel>();
        InitializeComponent();
    }
}
