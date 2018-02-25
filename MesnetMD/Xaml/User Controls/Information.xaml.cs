using System.Windows.Controls;

namespace MesnetMD.Xaml.User_Controls
{
    /// <summary>
    /// Interaction logic for Information.xaml
    /// </summary>
    public partial class Information : UserControl
    {
        public Information(string text)
        {
            InitializeComponent();
            info.Text = text;
        }
    }
}
