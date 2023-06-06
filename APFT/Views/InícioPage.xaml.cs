using APFT.Entities;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using APFT.ViewModels;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.VisualElements;
using Microsoft.UI.Xaml.Controls;
using SkiaSharp;

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
