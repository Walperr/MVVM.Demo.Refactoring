using System.Collections.ObjectModel;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using MVVM.Demo.Model;

namespace MVVM.Demo.Viewmodel;

public class EmployeesViewModel
{
    private class PersonRecord
    {
        public string Name { get; set; } = "";
        public string Surname { get; set; } = "";
        public string? Email { get; set; }
        public DateOnly BirthDate { get; set; }
        public string City { get; set; } = "";
        public string? CompanyName { get; set; }
    }

    private readonly MainViewModel _main;

    public ObservableCollection<Person> People { get; } = [];
    public Person? SelectedPerson { get; set; }

    public ICommand FireCommand { get; }
    public ICommand SaveCommand { get; }

    public EmployeesViewModel(MainViewModel main)
    {
        _main = main;

        FireCommand = new RelayCommand(Fire, () => SelectedPerson != null);
        SaveCommand = new RelayCommand(Save, () => _main.SelectedCompany != null);
    }

    public void Refresh()
    {
        People.Clear();
        foreach (var person in _main.SelectedCompany?.Employees ?? [])
            People.Add(person);
    }

    private void Fire()
    {
        var person = SelectedPerson!;
        var result = MessageBox.Show($"Уволить {person.Name} {person.Surname}?", "Подтверждение",
            MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (result != MessageBoxResult.Yes)
            return;

        _main.SelectedCompany!.Fire(person);
        _main.RefreshAll();
    }

    private void Save()
    {
        var dialog = new SaveFileDialog();
        dialog.Filter = "JSON файлы (*.json)|*.json";
        dialog.FileName = $"{_main.SelectedCompany!.Name}.json";
        if (dialog.ShowDialog() != true)
            return;

        var records = People.Select(p => new PersonRecord
        {
            Name = p.Name,
            Surname = p.Surname,
            Email = p.Email,
            BirthDate = p.BirthDate,
            City = p.Location.City,
            CompanyName = p.Job?.Name
        }).ToList();

        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        var json = JsonSerializer.Serialize(records, options);
        File.WriteAllText(dialog.FileName, json);

        MessageBox.Show($"Сохранено в {dialog.FileName}", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
    }
}
