using System.Windows.Controls;
using MesnetMD.Classes.Ui.Som;

namespace MesnetMD.Xaml.User_Controls
{
    /// <summary>
    /// Interaction logic for FictionalSupportItem.xaml
    /// </summary>
    public partial class FictionalSupportItem : UserControl
    {
        public FictionalSupportItem(FictionalSupport support)
        {
            InitializeComponent();
            Support = support;
            supportheader.Text = "Fictional Support " + Support.FID;
        }

        public FictionalSupport Support;
    }
}
