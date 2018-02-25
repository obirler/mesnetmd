using System.Windows;
using System.Windows.Controls;
using MesnetMD.Classes;
using MesnetMD.Classes.Ui.Som;

namespace MesnetMD.Xaml.User_Controls
{
    /// <summary>
    /// Interaction logic for BeamItem.xaml
    /// </summary>
    public partial class BeamItem : UserControl
    {
        public BeamItem(Beam beam)
        {
            InitializeComponent();
            Beam = beam;
            beamname.Text = Global.GetString("beam") + " " + Beam.BeamId;
        }

        public Beam Beam;

        public void SetCritical(bool critic)
        {
            if (critic)
            {
                warning.Visibility = Visibility.Visible;
            }
            else
            {
                warning.Visibility = Visibility.Collapsed;
            }
        }
    }
}
