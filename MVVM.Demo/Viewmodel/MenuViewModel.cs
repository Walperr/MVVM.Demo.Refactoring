using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using MVVM.Demo.Model;

namespace MVVM.Demo.Viewmodel;

public class MenuViewModel
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
    private readonly Window _window;

    public ICommand OpenCommand { get; }
    public ICommand SaveCommand { get; }
    public ICommand SaveToFolderCommand { get; }
    public ICommand ExitCommand { get; }

    public MenuViewModel(MainViewModel main, Window window)
    {
        _main = main;
        _window = window;

        OpenCommand = new RelayCommand(Open);
        SaveCommand = new RelayCommand(Save);
        SaveToFolderCommand = new RelayCommand(SaveToFolder);
        ExitCommand = new RelayCommand(Exit);
    }

    private void Open()
    {
        var dialog = new OpenFileDialog();
        dialog.Filter = "JSON файлы (*.json)|*.json";
        if (dialog.ShowDialog() != true)
            return;

        foreach (var company in _main.Companies)
        {
            foreach (var employee in company.Employees.ToList())
                company.Fire(employee);
        }
        _main.People.Clear();

        var json = File.ReadAllText(dialog.FileName);
        var records = JsonSerializer.Deserialize<List<PersonRecord>>(json)!;

        foreach (var record in records)
        {
            var location = new Location
            {
                Country = "Россия",
                State = record.City,
                City = record.City,
                PostalCode = "000000",
                Street = "Ленина",
                BuildingNumber = 1,
                OfficeNumber = 1
            };

            var person = new Person(record.Name, record.Surname, record.BirthDate, location)
            {
                Email = record.Email
            };

            var company = _main.Companies.FirstOrDefault(c => c.Name == record.CompanyName);
            company?.Hire(person);

            _main.People.Add(person);
        }

        _main.RefreshAll();
    }

    private void Save()
    {
        var dialog = new SaveFileDialog();
        dialog.Filter = "JSON файлы (*.json)|*.json";
        dialog.FileName = "people.json";
        if (dialog.ShowDialog() != true)
            return;

        var records = _main.People.Select(p => new PersonRecord
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

    private void SaveToFolder()
    {
        var dialog = new OpenFolderDialog();
        if (dialog.ShowDialog() != true)
            return;

        var path = Path.Combine(dialog.FolderName, "people.json");

        var records = _main.People.Select(p => new PersonRecord
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
        File.WriteAllText(path, json);

        MessageBox.Show($"Сохранено в {path}", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void Exit()
    {
        _window.Close();
    }
}
