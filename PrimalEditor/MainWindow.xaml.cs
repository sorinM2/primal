﻿using PrimalEditor.GameProject;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.ComponentModel;
using System.IO;

namespace PrimalEditor;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public static string PrimalPath { get; private set; } = @"c:\dev\Primal";
    public MainWindow()
    {
        InitializeComponent();
        Loaded += OnMainWindowLoaded;
        Closing += OnMainWindowClosing;
    }

    private void OnMainWindowLoaded(object sender, RoutedEventArgs e)
    {
        Hide();
        Loaded -= OnMainWindowLoaded;
        GetEnginePath();
        OpenProjectBrowserDialog();
    }

    private void GetEnginePath()
    {
        var enginePath = Environment.GetEnvironmentVariable("PRIMAL_ENGINE", EnvironmentVariableTarget.User);
        if ( enginePath == null|| !Directory.Exists(Path.Combine(enginePath, @"Engine\EngineAPI")))
        {
            var dlg = new EnginePathDialog();
            if ( dlg.ShowDialog() == true)
            {
                PrimalPath = dlg.PrimalPath;
                Environment.SetEnvironmentVariable("PRIMAL_ENGINE", PrimalPath.ToUpper(), EnvironmentVariableTarget.User);
            }
            else
            {
                Application.Current.Shutdown();
            }
        }
        else
        {
            PrimalPath = enginePath;
        }
    }

    private void OnMainWindowClosing(object sender, CancelEventArgs e)
    {
        Closing -= OnMainWindowClosing;
        Project.Current?.Unload();
    }

    private void OpenProjectBrowserDialog()
    {
        var projectBrowser = new ProjectBrowserDialog();
        if ( projectBrowser.ShowDialog() == false || projectBrowser.DataContext == null )
        {
            Application.Current.Shutdown();
        }
        else
        {
            Show();
            Project.Current?.Unload();
            DataContext = projectBrowser.DataContext;
        }
    }
}