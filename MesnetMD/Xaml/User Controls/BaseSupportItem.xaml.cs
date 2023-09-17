using MesnetMD.Classes;
using MesnetMD.Classes.Ui.Som;

namespace MesnetMD.Xaml.User_Controls
{
    /// <summary>
    /// Base class for UserControls
    /// </summary>
    public partial class BaseSupportItem : UserControl
    {
        public BasicSupport Support;

        public BaseSupportItem(BasicSupport support)
        {
            InitializeComponent();
            Support = support;
            supportheader.Text = Global.GetString("support") + " " + Support.SupportId;
        }
    }
}
