using System.Windows.Controls;
using MesnetMD.Classes.Ui.Base;

namespace MesnetMD.Classes.Ui
{
    public class TreeViewSupportItem:TreeViewItem
    {
        public TreeViewSupportItem(SupportItem support)
        {
            _support = support;
        }

        private SupportItem _support;

        public SupportItem Support
        {
            get { return _support; }
        }
    }
}
