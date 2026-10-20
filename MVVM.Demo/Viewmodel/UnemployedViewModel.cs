using System.Collections.ObjectModel;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using MVVM.Demo.Model;

namespace MVVM.Demo.Viewmodel;

public class UnemployedViewModel
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

    public ICommand HireCommand { get; }
    public ICommand SaveCommand { get; }

    public UnemployedViewModel(MainViewModel main)
    {
        _main = main;

        HireCommand = new RelayCommand(Hire, () => SelectedPerson != null && _main.SelectedCompany != null);
        SaveCommand = new RelayCommand(Save, () => People.Count > 0);
    }

    public void Refresh()
    {
        People.Clear();
        foreach (var person in _main.People.Where(p => p.Job == null))
            People.Add(person);
    }

    private void Hire()
    {
        var person = SelectedPerson!;
        var company = _main.SelectedCompany!;

        company.Hire(person);
        _main.RefreshAll();

        MessageBox.Show($"{person.Name} {person.Surname} нанят(а) в «{company.Name}»", "Информация",
            MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void Save()
    {
        var dialog = new SaveFileDialog();
        dialog.Filter = "JSON файлы (*.json)|*.json";
        dialog.FileName = "Без работы.json";
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
