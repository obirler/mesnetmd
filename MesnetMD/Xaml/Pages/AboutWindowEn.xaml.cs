using System.Windows;
using System.Windows.Navigation;
using MesnetMD.Classes;

namespace MesnetMD.Xaml.Pages
{
    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class AboutWindowEn : Window
    {
        public AboutWindowEn()
        {
            InitializeComponent();

            versiontext.Text = "V " + Config.VersionNumber;
        }

        private void closebtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void developerpage_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.linkedin.com/in/ömer-birler-9582696b");
            e.Handled = true;
        }

        private void instructorpage_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start("http://knot.gidb.itu.edu.tr/gemi/personel/bayraktarkatal.html");
            e.Handled = true;
        }

        private void mailtodeveloper_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start("mailto:omer.birler@gmail.com");
            e.Handled = true;
        }

        private void sourcecodepage_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start("https://bitbucket.org/omerbirler/mesnet");
            e.Handled = true;
        }

        private void gpllicencepage_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.gnu.org/licenses");
            e.Handled = true;
        }

        private void methoddetail_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start("https://en.wikipedia.org/wiki/Direct_stiffness_method");
            e.Handled = true;
        }
    }
}
