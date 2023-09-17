using MesnetMD.Classes;
using MesnetMD.Classes.Ui.Som;
using System.Windows.Controls;

namespace MesnetMD.Xaml.User_Controls
{
    public class SupportItemBase : UserControl
    {
        public Support Support { get; }

        public SupportItemBase(Support support)
        {
            InitializeComponent();
            Support = support;
            supportheader.Text = Global.GetString("support") + " " + Support.SupportId;
        }
    }
}
