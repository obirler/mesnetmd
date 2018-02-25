using System.Windows.Controls;

namespace MesnetMD.Xaml.User_Controls
{
    /// <summary>
    /// Interaction logic for BeamSupportItem.xaml
    /// </summary>
    public partial class BeamSupportItem : UserControl
    {
        public BeamSupportItem(string header, object support)
        {
            InitializeComponent();
            this.Header.Text = header;
            Support = support;
        }

        public BeamSupportItem()
        {
            InitializeComponent();
        }

        public object Support;
    }
}
