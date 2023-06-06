﻿using System.ComponentModel;
using Windows.Storage;
using APFT.Entities;
using APFT.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.Windows.ApplicationModel.Resources;

namespace APFT.Views;

public sealed partial class SupplierDetailsPage : INotifyPropertyChanged
{
    public SupplierDetailsViewModel ViewModel
    {
        get;
    }

    public SupplierDetailsPage()
    {
        ViewModel = App.GetService<SupplierDetailsViewModel>();
        FetchData();
        InitializeComponent();
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

    private string _supplierName;
    public string SupplierName
    {
        get => _supplierName;
        set
        {
            _supplierName = value;
            OnPropertyChanged(nameof(SupplierName));
        }
    }

    private string? _address;
    public string? Address
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

    private async void FetchData()
    {
        var localSettings = ApplicationData.Current.LocalSettings;
        var supplier = await Supplier.GetSupplierByNifAsync(int.Parse(localSettings.Values["SupplierNif"].ToString() ?? "0"));

        Nif = supplier.Nif;
        SupplierName = supplier.Name;
        Email = supplier.Email;
        Phone = supplier.Phone;
        Address = supplier.Address;
        
        OrdersGridView.ItemsSource = await Order.GetOrdersBySupplierNifAsync(Nif);
        if (OrdersGridView.Items.Count > 0)
        {
            OrdersSubtitle.Visibility = Visibility.Visible;
        }
    }

    private readonly ResourceLoader _resourceLoader = new("resources.pri", "SupplierDetails");

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
        await Supplier.EditSupplier(Nif, SupplierName, Phone, Email, Address);

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
            Title = _resourceLoader.GetString("DeleteCD_Title"),
            PrimaryButtonText = _resourceLoader.GetString("DeleteCD_PrimaryButton"),
            DefaultButton = ContentDialogButton.Primary,
            CloseButtonText = _resourceLoader.GetString("DeleteCD_CancelButton"),
            Content = _resourceLoader.GetString("DeleteCD_Content")
        };

        var choice = await dialog.ShowAsync();

        if (choice != ContentDialogResult.Primary) { return; }

        await Supplier.DeleteSupplier(Nif);

        Frame.GoBack();
    }

    private void OdersGridView_OnItemClick(object sender, ItemClickEventArgs e)
    {
        var localSettings = ApplicationData.Current.LocalSettings;
        localSettings.Values["OrderId"] = ((Order)e.ClickedItem).Id.ToString();

        Frame.Navigate(typeof(OrderDetailsPage));
    }
}
