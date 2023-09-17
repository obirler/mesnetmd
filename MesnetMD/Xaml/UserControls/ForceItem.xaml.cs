using System.Windows.Controls;

namespace MesnetMD.Xaml.User_Controls
{
    /// <summary>
    /// Interaction logic for ForceItem.xaml
    /// </summary>
    public partial class ForceItem : UserControl
    {
        public ForceItem(string name)
        {
            InitializeComponent();
            force.Text = name;
        }
    }
}
