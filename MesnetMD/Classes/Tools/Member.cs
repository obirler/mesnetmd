/*
========================================================================
    Copyright (C) 2016 Omer Birler.
    
    This file is part of Mesnet.

    Mesnet is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    Mesnet is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with Mesnet.  If not, see <http://www.gnu.org/licenses/>.
========================================================================
*/

using MesnetMD.Classes.Ui.Som;

namespace MesnetMD.Classes.Tools
{
    public class Member
    {
        public Member()
        {
            
        }

        public Member(Beam beam, Global.Direction direction)
        {
            _beam = beam;
            _direction = direction;
        }

        private Beam _beam;

        private Global.Direction _direction;

        public Beam Beam
        {
            get { return _beam; }
            set { _beam = value; }
        }

        public Global.Direction Direction
        {
            get { return _direction; }
            set { _direction = value; }
        }

        public double Moment
        {
            get
            {
                double moment = 0;
                switch (_direction)
                {
                    case Global.Direction.Left:

                        moment = _beam.LeftEndMoment;

                        break;

                    case Global.Direction.Right:

                        moment = _beam.RightEndMoment;

                        break;
                }

                return moment;
            }
        }
    }
}
