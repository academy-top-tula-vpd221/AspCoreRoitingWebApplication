
List<Employee> employees = new()
{
    new(){ Name = "Bobby", Age = 34, Id = 3 },
    new(){ Name = "Philipp", Age = 21, Id = 10 },
    new(){ Name = "Jonny", Age = 42, Id = 25 },
    new(){ Name = "Bobby", Age = 27, Id = 18 },
};

var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<RouteOptions>(
    options => options.ConstraintMap.Add("mypass", typeof(PasswordConstraint)));



var app = builder.Build();



//app.Map("{controller:length(7):required=home}/{action=index}/{id:int:range(100, 1000)?}",
//    (string controller, string action, string? id) =>
//    $"Controller: {controller}\nAction: {action}\nId: {id}"
//    );


app.MapGet("/about/{pass:mypass(12345)}/", () => "About");
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

class PasswordConstraint : IRouteConstraint
{
    string password;
    public PasswordConstraint(string password)
    {
        this.password = password;
    }
    public bool Match(HttpContext? httpContext, IRouter? route, string routeKey, RouteValueDictionary values, RouteDirection routeDirection)
    {
        return values[routeKey]?.ToString() == password;
    }
}
