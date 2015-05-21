//
//  Tortuga - Life Cycle Analysis for McNeel Rhino Grassopper 3D (R)
//  Copyright (C) 2015  Maximilian Thumfart
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
//
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tortuga.Types
{

    public interface IUnit { }

    public static class LCA
    {
        public interface ILCA : IUnit { }

        public class CO2e : ILCA { }
        public class kgCFC11 : ILCA { }
        public class kgSO2 : ILCA { }
        public class kgPhostphate : ILCA { }
        public class kgNOx : ILCA { }
        public class MJ : ILCA { }
    }

    public class UnitDouble<T> where T : IUnit
    {
        public readonly double Value;
        public UnitDouble(double value)
        {
            Value = value;
        }

        public static UnitDouble<T> operator +(UnitDouble<T> first, UnitDouble<T> second)
        {
            return new UnitDouble<T>(first.Value + second.Value);
        }

        public static UnitDouble<T> operator -(UnitDouble<T> first, UnitDouble<T> second)
        {
            return new UnitDouble<T>(first.Value - second.Value);
        }

        public static UnitDouble<T> operator /(UnitDouble<T> first, UnitDouble<T> second)
        {
            return new UnitDouble<T>(first.Value / second.Value);
        }

        public static UnitDouble<T> operator *(UnitDouble<T> first, UnitDouble<T> second)
        {
            return new UnitDouble<T>(first.Value * second.Value);
        }
    }

}


