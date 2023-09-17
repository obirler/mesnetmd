using System.Windows.Controls;

namespace MesnetMD.Xaml.User_Controls
{
    /// <summary>
    /// Interaction logic for ConcentratedLoadItem.xaml
    /// </summary>
    public partial class ConcentratedLoadItem : UserControl
    {
        public ConcentratedLoadItem(string name)
        {
            InitializeComponent();
            concload.Text = name;
        }
    }
}
