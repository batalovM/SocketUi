using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using SocketUi.ViewModels;

namespace SocketUi.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel();

    }
}