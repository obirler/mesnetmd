using System;
using System.Windows.Controls;
using MesnetMD.Classes;

namespace MesnetMD.Xaml.User_Controls
{
    /// <summary>
    /// Interaction logic for SupportBeamItem.xaml
    /// </summary>
    public partial class SupportBeamItem : UserControl
    {
        public SupportBeamItem(int beamid, Global.Direction direction, double moment)
        {
            InitializeComponent();
            switch (direction)
            {
                case Global.Direction.Right:
                    header.Text = Global.GetString("beam") + " " + beamid + " , " + Global.GetString("rightside") + ",  " +
                                  Math.Round(moment, 4) + " kNm";
                    break;

                case Global.Direction.Left:
                    header.Text = Global.GetString("beam") + " " + beamid + " , " + Global.GetString("leftside") + ",  " +
                                  Math.Round(moment, 4) + " kNm";
                    break;
            }

        }
    }
}
