List<Employee> employees = new()
{
    new(){ Name = "Bobby", Age = 34, Id = 3 },
    new(){ Name = "Philipp", Age = 21, Id = 10 },
    new(){ Name = "Jonny", Age = 42, Id = 25 },
    new(){ Name = "Bobby", Age = 27, Id = 18 },
};

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.Map("/", () => "Hello World!");
app.MapGet("/about", () => "About");
app.MapGet("/empl/{name?}/{age?}", (string name = "", int age = 0) =>
{
    if (name == "")
    {
        //foreach(var e in employees)
        return employees.ToArray();
    }
    else
    {
        if(age == 0)
            return employees.FindAll(e => e.Name.ToLower() == name.ToLower()).ToArray();
        else
        {
            var el = new List<Employee>();
            el.Add(employees.FirstOrDefault(e => e.Name.ToLower() == name.ToLower()
                                        && e.Age == age));
            return el.ToArray();
        }
            
    }
});
app.Map("/log", async (context) =>
{
    Console.WriteLine($"Log {DateTime.Now.ToShortTimeString()}");
    await context.Response.WriteAsync($"Log {DateTime.Now.ToShortTimeString()}");
});

app.Map("/routes", async (IEnumerable<EndpointDataSource> src) =>
{
    
    var cs = src.SelectMany(e => e.Endpoints);
    string s = String.Join("\n", cs);


    return s;
});


app.Run();


class Employee
{
    public int Id { set; get; }
    public string Name { get; set; }
    public int Age { get; set; }
}
