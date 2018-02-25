using System.Windows.Controls;
using MesnetMD.Classes;
using MesnetMD.Classes.Ui.Som;

namespace MesnetMD.Xaml.User_Controls
{
    /// <summary>
    /// Interaction logic for LeftFixedSupportItem.xaml
    /// </summary>
    public partial class LeftFixedSupportItem : UserControl
    {
        public LeftFixedSupportItem(LeftFixedSupport support)
        {
            InitializeComponent();
            Support = support;
            supportheader.Text = Global.GetString("leftfixedsupport") + " " + Support.SupportId;
        }

        public LeftFixedSupport Support;
    }
}
