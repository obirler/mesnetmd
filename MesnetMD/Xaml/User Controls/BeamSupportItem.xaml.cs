using System.Windows.Controls;
using MesnetMD.Classes.Ui.Base;

namespace MesnetMD.Xaml.User_Controls
{
    /// <summary>
    /// Interaction logic for BeamSupportItem.xaml
    /// </summary>
    public partial class BeamSupportItem : UserControl
    {
        public BeamSupportItem(string header, SupportItem support)
        {
            InitializeComponent();
            this.Header.Text = header;
            Support = support;
        }

        public BeamSupportItem()
        {
            InitializeComponent();
        }

        public SupportItem Support;
    }
}
