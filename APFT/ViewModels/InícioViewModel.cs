using System.Collections.ObjectModel;
using System.Diagnostics;
using APFT.Entities;
using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.VisualElements;
using SkiaSharp;
using Microsoft.UI.Xaml.Controls;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ABI.Microsoft.UI.Xaml.Data;

namespace APFT.ViewModels;

public partial class InícioViewModel : ObservableObject, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public InícioViewModel()
    {
        FetchData();
    }

    private async void FetchData()
    {
        var employeeData = await Employee.GetHoursOfAllEmployees();

        var hoursData = employeeData.Select(employee => employee.Hours.HasValue ? (double)employee.Hours.Value : 0).ToArray();
        var i = 0;

        Series = hoursData.AsLiveChartsPieSeries((value, series) =>
        {
            series.Name = employeeData[i].Name;
            i++;
            series.InnerRadius = 80;
        });
    }

    private IEnumerable<ISeries> _series;
    public IEnumerable<ISeries> Series
    {
        get => _series;
        set
        {
            if (_series != value)
            {
                _series = value;
                OnPropertyChanged(nameof(Series));
            }
        }
    }
}
