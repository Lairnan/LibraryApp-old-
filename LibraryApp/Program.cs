using System;

namespace LibraryApp;

public static class Program
{
    internal const string SecretHash = "testSecret";

    [STAThread]
    public static void Main(string[] args)
    {
        var app = new App();
        LibraryDb.Context.LibraryDbContext.SetHash(SecretHash);
        app.InitializeComponent();
        app.Run();
        
        GC.Collect();
    }
}