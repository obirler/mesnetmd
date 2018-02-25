using System.Windows.Controls;

namespace MesnetMD.Xaml.User_Controls
{
    /// <summary>
    /// Interaction logic for MomentItem.xaml
    /// </summary>
    public partial class MomentItem : UserControl
    {
        public MomentItem(string name)
        {
            InitializeComponent();
            moment.Text = name;
        }
    }
}
