
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace SocketUi.Models;

public class Server
{
    public async Task SendRequest(string ipAddress, string message)
    {
        using var client = new HttpClient();
        var content = new StringContent($"{message}");
        await client.PostAsync($"http://{ipAddress}", content);
        Console.WriteLine($"For {ipAddress} send message: {message}");
    }
    public async Task<string> GetDataFromServer(string ipAddress)
    {
        using var client = new HttpClient();
        var response = await client.GetAsync($"http://{ipAddress}"); // Замените URL на адрес вашего сервера и путь, по которому доступны данные
        var responseData = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"Message from server: {responseData}");
        return responseData;
    }

    public async Task Close(string ipAddress)
    {
        await SendRequest(ipAddress, "exit");
    }
}