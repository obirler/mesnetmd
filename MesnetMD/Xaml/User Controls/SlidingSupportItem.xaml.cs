using System.Windows.Controls;
using MesnetMD.Classes;
using MesnetMD.Classes.Ui.Som;

namespace MesnetMD.Xaml.User_Controls
{
    /// <summary>
    /// Interaction logic for SlidingSupportItem.xaml
    /// </summary>
    public partial class SlidingSupportItem : UserControl
    {
        public SlidingSupportItem(SlidingSupport support)
        {
            InitializeComponent();
            Support = support;
            supportheader.Text = Global.GetString("slidingsupport") + " " + Support.SupportId;
        }

        public SlidingSupport Support;
    }
}
