using System.Net;
using System.Threading;
using System.Windows;

namespace PokeFinder.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e) {
            Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.CreateSpecificCulture("en-US");
            Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.CreateSpecificCulture("en-US");
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            base.OnStartup(e);
        }
    }
}
