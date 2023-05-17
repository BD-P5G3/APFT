using APFT.ViewModels;

using Microsoft.UI.Xaml.Controls;

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
}
