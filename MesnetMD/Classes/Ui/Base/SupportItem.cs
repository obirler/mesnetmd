using MesnetMD.Classes.Ui.Base;

namespace MesnetMD.Classes.Ui
{
    public class SupportItem : SomItem
    {
        public SupportItem()
        {
            SupportId = supportcount++;
        }

        public int SupportId=0;

        private static int supportcount = 0;
    }
}
