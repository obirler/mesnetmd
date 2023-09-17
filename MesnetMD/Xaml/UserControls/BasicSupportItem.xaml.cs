using System.Windows.Controls;
using MesnetMD.Classes;
using MesnetMD.Classes.Ui.Som;

namespace MesnetMD.Xaml.User_Controls
{
    /// <summary>
    /// Interaction logic for BasicSupportItem.xaml
    /// </summary>
    public partial class BasicSupportItem : UserControl
    {
        public BasicSupportItem(BasicSupport support)
        {
            InitializeComponent();
            Support = support;
            supportheader.Text = Global.GetString("basicsupport") + " " + Support.SupportId;
        }

        public BasicSupport Support;
    }
}
