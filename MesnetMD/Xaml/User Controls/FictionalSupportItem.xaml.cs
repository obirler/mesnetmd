using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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

        private FictionalSupport Support;
    }
}
