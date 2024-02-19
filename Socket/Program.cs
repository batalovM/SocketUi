
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
var time = $"Server started\n{DateTime.Now}";
app.Run(async(context) =>
{
    await context.Response.WriteAsync(time);
    Console.WriteLine("Время запуска сервера отправлено");
    if (context.Request.Method == "POST")
    {
        using var reader = new StreamReader(context.Request.Body, Encoding.UTF8);
        var requestBody = await reader.ReadToEndAsync();
        if (requestBody == "exit") await app.StopAsync();
        Console.WriteLine(requestBody);
    }
});
app.Run();
