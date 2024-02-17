

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DynamicData;
using ReactiveUI;

namespace SocketUi.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private List<string>? _disks;
    private List<string>? _directories;
   
    private string? _selectedDisk;

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
    public MainWindowViewModel()
    {
        _disks = GetDisks();
    }
    private List<string> GetDisks()
    {
        var allDrives = DriveInfo.GetDrives();
        var directories = (from drive in allDrives where drive.DriveType == DriveType.Fixed select drive.Name).ToList();
        directories.ForEach(Console.WriteLine);
        return directories;
    }

    private List<string> GetDirectories(string name)
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
    private void FilterDirectories()
    {
        if (!string.IsNullOrEmpty(SelectedDisk)) Directories = GetDirectories(SelectedDisk);
    }
}