namespace MVVM.Demo.Model;

public class Company
{
    private readonly List<Person> _employees = [];

    public IReadOnlyList<Person> Employees => _employees;
    
    public string Name { get; }
    public Location  Location { get; }

    public Company(string name, Location location)
    {
        Name = name;
        Location = location;
    }

    public void Hire(Person employee)
    {
        employee.Hire(this);
        _employees.Add(employee);
    }
    
    public void Fire(Person employee)
    {
        employee.Fire(this);
        _employees.Remove(employee);
    }
}