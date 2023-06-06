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
using String = System.String;

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
        var i = 0;

        var employeeData = await Employee.GetHoursOfAllEmployees();
        var hoursData = employeeData.Select(employee => employee.Hours.HasValue ? (double)employee.Hours.Value : 0).ToArray();
        EmployeeSeries = hoursData.AsLiveChartsPieSeries((value, series) =>
        {
            series.Name = employeeData[i].Name;
            i++;
            series.InnerRadius = 80;
        });
        
        var tableData = await Info.GetTableInfo();
        var tableDataRowNumbers = tableData.Select(table => table.Rows ?? 0).ToArray();
        RowsSeries = new ISeries[]
        {
            new ColumnSeries<long> { Values = tableDataRowNumbers}
        };
        RowsXAxes = new Axis[] 
        {
            new() 
            {
                // Use the labels property for named or static labels 
                Labels = tableData.Select(table => table.TableName ?? string.Empty).ToArray(), 
                LabelsRotation = 15
            }
        };
    }

    private IEnumerable<ISeries> _employeeSeries;
    public IEnumerable<ISeries> EmployeeSeries
    {
        get => _employeeSeries;
        set
        {
            if (_employeeSeries != value)
            {
                _employeeSeries = value;
                OnPropertyChanged(nameof(EmployeeSeries));
            }
        }
    }

    private ISeries[] _rowsSeries;
    public ISeries[] RowsSeries
    {
        get => _rowsSeries;
        set
        {
            if (_rowsSeries != value)
            {
                _rowsSeries = value;
                OnPropertyChanged(nameof(RowsSeries));
            }
        }
    }

    private Axis[] _rowsXAxes;
    public Axis[] RowsXAxes
    {
        get => _rowsXAxes;
        set
        {
            if (_rowsXAxes != value)
            {
                _rowsXAxes = value;
                OnPropertyChanged(nameof(RowsXAxes));
            }
        }
    }
}
