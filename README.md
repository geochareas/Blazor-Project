
# Description

You are given a solution that contains a Blazor Web App with a Customer model class.

You should fork this project and provide a github link for your solution.

You have to develop 

Required: 
- A grid with all customers with server side paging
- CRUD Operations on “Customer” model with new, edit and delete functionalities
- Expose all CRUD Operations as an API 
- Configure application to use Sql Server
- Manage migrations
- Below are the two classes Employee and Manager. Your task is to create a method in a new class that takes either Manager or an Employee as a parameter and prints its name.

```
public class Employee
{
	public string Name { get; set; }
}

public class Manager
{
	public string Name { get; set; }
}

```

Extra (nice to have) 
- Add authentication with the provided demo of Duende IdentityServer https://demo.duendesoftware.com/
- Protect your API with authentication with the provided demo of Duende IdentityServer done in the previous step
- Unit & Integration Tests

## Requirements 

- C#
- .NET 8+ 
- Blazor Wasm

Optional
- Blazor UI framework


# PrintName Task
To satisfy the task of printing the name of an Employee or Manager, I created two approaches:

### 1. Reflection
```csharp
public static class PersonPrinter
{
    public static void PrintName(object person)
    {
        var type = person.GetType();
        var nameProperty = type.GetProperty("Name");

        if (nameProperty != null && nameProperty.PropertyType == typeof(string))
        {
            var name = nameProperty.GetValue(person) as string;
            Console.WriteLine($"Name: {name}");
        }
        else
        {
            Console.WriteLine("The provided object does not have a valid 'Name' property.");
        }
    }
}


PersonPrinter.PrintName(employee);
PersonPrinter.PrintName(manager);
```

### 2. Inheritance / Interface
```csharp
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

```

> You can find the source in `PersonPrinter.cs`

# Screenshots
![screenshot_1](https://github.com/user-attachments/assets/90f7c8f5-07b3-4990-bcff-f784af82ae76)
![screenshot_2](https://github.com/user-attachments/assets/79e6b0cf-e267-48f6-bd81-8f3e88fbb9ac)
![screenshot_3](https://github.com/user-attachments/assets/e6402f70-0d2c-4277-a4ba-268136f65d9c)
![screenshot_4](https://github.com/user-attachments/assets/41e51ee4-0377-4ebd-a9e3-c3727dfd8183)
![image](https://github.com/user-attachments/assets/0ff8322a-6bfd-4bbc-b206-5c91b24e9b60)





