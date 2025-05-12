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
