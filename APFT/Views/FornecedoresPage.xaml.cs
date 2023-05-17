using APFT.ViewModels;

using Microsoft.UI.Xaml.Controls;

namespace APFT.Views;

public sealed partial class FornecedoresPage : Page
{
    public FornecedoresViewModel ViewModel
    {
        get;
    }

    public FornecedoresPage()
    {
        ViewModel = App.GetService<FornecedoresViewModel>();
        InitializeComponent();
    }
}
