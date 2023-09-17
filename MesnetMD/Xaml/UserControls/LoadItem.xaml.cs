using System.Windows.Controls;

namespace MesnetMD.Xaml.User_Controls
{
    /// <summary>
    /// Interaction logic for LoadItem.xaml
    /// </summary>
    public partial class LoadItem : UserControl
    {
        public LoadItem(string loadname)
        {
            InitializeComponent();
            load.Text = loadname;
        }
    }
}
