using System;
using System.Collections.Generic;
using System.Windows.Controls;
using MesnetMD.Classes.Math;
using MesnetMD.Classes.Ui.Base;
using MesnetMD.Classes.Ui.Som;

namespace MesnetMD.Classes.Ui.Graphics
{
    public class ConcentratedLoad : GraphicItem, IGraphic
    {
        public ConcentratedLoad(KeyValueCollection loads, Beam beam, int c = 200)
        {
            GraphicType = Global.GraphicType.ConcentratedLoad;

            _beam = beam;

            _loads = loads;

            _length = _beam.Length;

            _labellist = new List<TextBlock>();

            Draw(c);
        }

        private Beam _beam;

        /// <summary>
        /// The load list. List<KeyValuePair<xpos, loadmagnitude>>
        /// </summary>
        private KeyValueCollection _loads;

        private List<TextBlock> _labellist;

        private double _length;

        private double coeff;

        public double Length
        {
            get { return _length; }
            set
            {
                _length = value;
            }
        }

        public KeyValueCollection Loads
        {
            get { return _loads; }
        }
        public void Draw(int c)
        {
            throw new NotImplementedException();
        }

        public void Show()
        {
            throw new NotImplementedException();
        }

        public void Hide()
        {
            throw new NotImplementedException();
        }

        public void RemoveLabels()
        {
            throw new NotImplementedException();
        }
    }
}
