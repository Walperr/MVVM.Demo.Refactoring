using System.Windows;
using MVVM.Demo.Model;

namespace MVVM.Demo.Viewmodel;

public class MainViewModel : ViewModelBase
{
    private Company? _selectedCompany;

    public List<Company> Companies { get; } = [];
    public List<Person> People { get; } = [];

    public Company? SelectedCompany
    {
        get => _selectedCompany;
        set
        {
            _selectedCompany = value;
            OnPropertyChanged();
            RefreshAll();
        }
    }

    public MenuViewModel Menu { get; }
    public EmployeesViewModel Employees { get; }
    public UnemployedViewModel Unemployed { get; }

    public MainViewModel(Window window)
    {
        var horns = new Company("Рога и копыта", CreateLocation("Москва"));
        var vector = new Company("Вектор", CreateLocation("Казань"));
        Companies.Add(horns);
        Companies.Add(vector);

        var ivan = new Person("Иван", "Петров", new DateOnly(1990, 3, 14), CreateLocation("Москва")) { Email = "ivan@mail.ru" };
        var maria = new Person("Мария", "Иванова", new DateOnly(1995, 7, 2), CreateLocation("Москва")) { Email = "maria@mail.ru" };
        var oleg = new Person("Олег", "Сидоров", new DateOnly(1985, 11, 23), CreateLocation("Казань"));
        var anna = new Person("Анна", "Кузнецова", new DateOnly(2000, 1, 9), CreateLocation("Казань")) { Email = "anna@gmail.com" };
        People.Add(ivan);
        People.Add(maria);
        People.Add(oleg);
        People.Add(anna);

        horns.Hire(ivan);
        vector.Hire(oleg);

        Menu = new MenuViewModel(this, window);
        Employees = new EmployeesViewModel(this);
        Unemployed = new UnemployedViewModel(this);

        SelectedCompany = Companies.First();
    }

    public void RefreshAll()
    {
        Employees.Refresh();
        Unemployed.Refresh();
    }

    private static Location CreateLocation(string city) => new()
    {
        Country = "Россия",
        State = city,
        City = city,
        PostalCode = "000000",
        Street = "Ленина",
        BuildingNumber = 1,
        OfficeNumber = 1
    };
}
