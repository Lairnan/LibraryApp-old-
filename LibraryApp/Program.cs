using System;
using DbConnect;

namespace LibraryApp;

public static class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        var app = new App();
        app.InitializeComponent();
        app.Run();
        if (DbConnection.IsConnected)
        {
            DbConnection.Stop();
        }
    }
}