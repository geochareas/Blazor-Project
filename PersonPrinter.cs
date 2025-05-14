using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorApp.Shared.Services;

public static class PersonPrinter
{
    public static void PrintName(object person)
    {
        var nameProp = person.GetType().GetProperty("Name");
        if (nameProp != null)
        {
            Console.WriteLine(nameProp.GetValue(person));
        }
    }
}

/*
public interface IPerson
{
    string Name { get; }
}

public class Employee : IPerson
{
    public string Name { get; set; }
}

public class Manager : IPerson
{
    public string Name { get; set; }
}

public static class PersonPrinter
{
    public static void PrintName(IPerson person)
    {
        Console.WriteLine($"Name: {person.Name}");
    }
}
*/