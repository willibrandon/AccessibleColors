using System.Globalization;

namespace AccessibleColors.UI;

internal static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        ApplicationConfiguration.Initialize();

        //// Set UI culture before creating the form
        //Thread.CurrentThread.CurrentUICulture = new CultureInfo("zh-Hans");

        Application.Run(new MainForm());
    }
}