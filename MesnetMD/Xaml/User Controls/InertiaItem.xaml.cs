using System.Windows.Controls;

namespace MesnetMD.Xaml.User_Controls
{
    /// <summary>
    /// Interaction logic for InertiaItem.xaml
    /// </summary>
    public partial class InertiaItem : UserControl
    {
        public InertiaItem(string name)
        {
            InitializeComponent();
            inertiatext.Text = name;
        }
    }
}
