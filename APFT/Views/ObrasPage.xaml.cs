using APFT.ViewModels;

using Microsoft.UI.Xaml.Controls;

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
}
