namespace MesnetMD.Classes.Ui.Base
{
    public class GraphicItem : UiItem
    {
        public GraphicItem()
        {
            ItemType = Global.ItemType.GraphicItem;
        }

        public Global.GraphicType GraphicType;
    }
}
