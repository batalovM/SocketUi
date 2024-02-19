

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using ReactiveUI;
using SocketUi.Models;

namespace SocketUi.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private readonly HttpClient _client = new();
    private readonly Server _server = new();
    private List<string>? _disks;
    private List<string>? _directories;
    private string _clientSide;
    private string _serverSide;
    private string? _selectedDisk;
    private string _ipAddress;
    private bool _istransferToServer;
    private bool _isConnecting;
    public List<string>? Disks
    {
        get => _disks;
        set => this.RaiseAndSetIfChanged(ref _disks, value);
    }
    public List<string>? Directories
    {
        get => _directories;
        set => this.RaiseAndSetIfChanged(ref _directories, value);
    }
    public string? SelectedDisk
    {
        get => _selectedDisk;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedDisk, value);
            FilterDirectories();
        }
    }
    public string ClientSide
    {
        get => _clientSide;
        set => this.RaiseAndSetIfChanged(ref _clientSide, value);
    }
    public string ServerSide
    {
        get => _serverSide;
        set => this.RaiseAndSetIfChanged(ref _serverSide, value);
    }
    public string IpAddress
    {
        get => _ipAddress;   
        set => this.RaiseAndSetIfChanged(ref _ipAddress, value);
    }
    public ReactiveCommand<Unit, Unit> ShuttingDownServerButton { get; }
    public ReactiveCommand<Unit, Unit> CloseButton { get; }
    public ReactiveCommand<Unit, Unit> ConnectButton { get; }
    public ReactiveCommand<Unit, Unit> DisconnectButton { get; }
    public ReactiveCommand<Unit, Unit> TransferToServerButton { get; }
    public ReactiveCommand<Unit, Unit> TransferToCustomerButton { get; }
    public MainWindowViewModel()
    {
        _disks = GetDisks();
        CloseButton = ReactiveCommand.CreateFromTask(Close);
        ShuttingDownServerButton = ReactiveCommand.CreateFromTask(ShuttingDown);
        ConnectButton = ReactiveCommand.CreateFromTask(Connect);
        DisconnectButton = ReactiveCommand.CreateFromTask(Disconnect);
        TransferToServerButton = ReactiveCommand.CreateFromTask(TransferToServer);
        TransferToCustomerButton = ReactiveCommand.CreateFromTask(TransferToCustomer);
    }
    private Task TransferToCustomer()
    {
        var mess = $"\nClient takes\n{DateTime.Now}";
        if (Directories != null) mess = Directories.Aggregate(mess, (current, item) => current + $"\n{item}");
        ClientSide += mess;
        return Task.CompletedTask;
    }
    private async Task TransferToServer()//доделан
    {
        if (!_istransferToServer && _isConnecting)
        {
            ServerSide += $"\nServer takes\n{DateTime.Now}\n{SelectedDisk}";
            if (SelectedDisk != null) await _server.SendRequest(IpAddress, SelectedDisk);
            _istransferToServer = true;
        }
    }
    private async Task Disconnect()//доделан
    {
        _isConnecting = false;
        var mess = $"\nClient disconnect\n{DateTime.Now.ToShortTimeString()}";
        ServerSide += mess;
        await _server.SendRequest(IpAddress,mess);
    }
    private async Task Connect()//доделан
    {
        if (!_isConnecting)
        {
            var time = $"\nClient connected\n{DateTime.Now}";
            await _server.SendRequest(IpAddress, time);
            var strTask = await _server.GetDataFromServer(IpAddress);
            ServerSide += strTask;
            ServerSide += $"{time}\nfrom {IpAddress}";
            var mess = $"Client takes\n{DateTime.Now}";
            if (Disks != null) mess = Disks.Aggregate(mess, (current, item) => current + $"\n{item}");
            ClientSide += mess;
            _isConnecting = true;
        }
        
    }

    private async Task ShuttingDown()//доделан
    {
        await _server.Close(IpAddress);
        ServerSide += $"\nServer shutting down\n{DateTime.Now.ToShortTimeString()}";
    }

    private async Task Close()//доделан
    {
        var window = (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
        window?.Close();
        await _server.Close(IpAddress);
    }

    private List<string> GetDisks()//доделан
    {
        var allDrives = DriveInfo.GetDrives();
        var directories = (from drive in allDrives where drive.DriveType == DriveType.Fixed select drive.Name).ToList();
        return directories;
    }

    private List<string> GetDirectories(string name)//доделан
    {
        var directories = new List<string>();
        var allDrives = DriveInfo.GetDrives();
        foreach (var drive in allDrives)
        {
            if (drive.DriveType != DriveType.Fixed) continue;
            try
            {
                if (name == drive.Name)
                {
                    var dirs = Directory.GetDirectories(drive.Name);
                    directories.AddRange(
                        from dir in dirs where dir.Contains(drive.Name) select dir[drive.Name.Length..]);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Ошибка получения каталога" + e.Message);
            }
        }
        return directories;
    }
    private void FilterDirectories()//доделан
    {
        if (!string.IsNullOrEmpty(SelectedDisk)) Directories = GetDirectories(SelectedDisk);
    }
}