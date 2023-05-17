using APFT.ViewModels;

using Microsoft.UI.Xaml.Controls;

namespace APFT.Views;

public sealed partial class InícioPage : Page
{
    public InícioViewModel ViewModel
    {
        get;
    }

    public InícioPage()
    {
        ViewModel = App.GetService<InícioViewModel>();
        InitializeComponent();
    }
}
