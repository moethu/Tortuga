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
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

using System.Net.Sockets;
using System.Net;
using System.Xml;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.IO;
using System.Threading;


namespace Tortuga.GrasshopperComponents
{


    public class Add : GH_Component
    {
        public Add() : base("Tortuga Material Addition", "Addition", "Add Tortuga Materials", "Tortuga", "Operators") { }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Material A", "A", "Tortuga Material", GH_ParamAccess.item);
            pManager.AddGenericParameter("Material B", "B", "Tortuga Material", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Result", "R", "Tortuga Material", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Types.Assembly assemblyA = new Types.Assembly();
            DA.GetData<Types.Assembly>("Material A", ref assemblyA);

            Types.Assembly assemblyB = new Types.Assembly();
            DA.GetData<Types.Assembly>("Material B", ref assemblyB);

            DA.SetData("Result", assemblyA + assemblyB);
        }


        // Properties
        public override Guid ComponentGuid
        {
            get
            {
                return new Guid("{5ea4aa3d-d171-3a9f-a712-4322afeb3b1a}");
            }
        }
        protected override System.Drawing.Bitmap Internal_Icon_24x24
        {
            get
            {
                return Properties.Resources.tortuga_add;
            }
        }
        public override GH_Exposure Exposure
        {
            get
            {
                return GH_Exposure.secondary;
            }
        }


    }

    public class Multiply : GH_Component
    {
        public Multiply() : base("Tortuga Material Multiplication", "Multiplication", "Multiply Tortuga Materials", "Tortuga", "Operators") { }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Material", "M", "Tortuga Material", GH_ParamAccess.item);
            pManager.AddNumberParameter("Factor", "F", "Factor", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Result", "R", "Tortuga Material", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Types.Assembly assembly = new Types.Assembly();
            DA.GetData<Types.Assembly>("Material", ref assembly);

            GH_Number factor = new GH_Number(0);
            DA.GetData<GH_Number>("Factor", ref factor);

            DA.SetData("Result", assembly * factor.Value);
        }


        // Properties
        public override Guid ComponentGuid
        {
            get
            {
                return new Guid("{5ea4aa3d-d171-3a9f-a712-4323bfeb1b5a}");
            }
        }
        protected override System.Drawing.Bitmap Internal_Icon_24x24
        {
            get
            {
                return Properties.Resources.tortuga_multi;
            }
        }
        public override GH_Exposure Exposure
        {
            get
            {
                return GH_Exposure.secondary;
            }
        }


    }

}