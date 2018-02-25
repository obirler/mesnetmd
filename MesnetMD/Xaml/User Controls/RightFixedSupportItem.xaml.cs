using System.Windows.Controls;
using MesnetMD.Classes;
using MesnetMD.Classes.Ui.Som;

namespace MesnetMD.Xaml.User_Controls
{
    /// <summary>
    /// Interaction logic for RightFixedSupportItem.xaml
    /// </summary>
    public partial class RightFixedSupportItem : UserControl
    {
        public RightFixedSupportItem(RightFixedSupport support)
        {
            InitializeComponent();
            Support = support;
            supportheader.Text = Global.GetString("rightfixedsupport") + " " + Support.SupportId;
        }

        public RightFixedSupport Support;
    }
}
