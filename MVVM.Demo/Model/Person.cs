namespace MVVM.Demo.Model;

public class Person
{
    public string Name { get; }
    public string Surname { get; set; }
    public string? Email { get; set; }
    public DateOnly BirthDate { get; }
    public Company? Job { get; private set; }
    
    public Location Location { get; set; }

    public Person(string name, string surname, DateOnly birthDate, Location location)
    {
        Name = name;
        Surname = surname;
        BirthDate = birthDate;
        Location = location;
    }

    public void Hire(Company company)
    {
        if (Job is not null)
            throw new InvalidOperationException("Job already hired. You should be fired from your job before hire new one");
        
        Job = company;
    }

    public void Fire(Company company)
    {
        if (Job is null)
            throw new InvalidOperationException("Person has no job to be fired from");
        
        if (company != Job)
            throw new InvalidOperationException("Person isn't work in this company");

        Job = null;
    }
}