using APFT.ViewModels;

using Microsoft.UI.Xaml.Controls;

namespace APFT.Views;

public sealed partial class ClientesPage : Page
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
}
