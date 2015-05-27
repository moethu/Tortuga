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

    public class Calculate : GH_Component
    {
        public Calculate() : base("Tortuga Calculator", "Calculator", "Calculate LCA Values", "Tortuga", "Tortuga") { }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Material", "M", "Tortuga Material", GH_ParamAccess.item);

            List<int> optinalParameters = new List<int>();

            optinalParameters.Add(pManager.AddNumberParameter("Area", "A", "Optional: Area to calculate", GH_ParamAccess.item));
            optinalParameters.Add(pManager.AddSurfaceParameter("Surface", "S", "Optional: Surface to calculate", GH_ParamAccess.item));

            foreach (int optional in optinalParameters) pManager[optional].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("LCA Result", "L", "Calculated LCA Result", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Types.Assembly assembly = new Types.Assembly();
            DA.GetData<Types.Assembly>("Material", ref assembly);



            GH_Number area = new GH_Number(0);
            GH_Surface surface = new GH_Surface();

            if (!DA.GetData<GH_Number>("Area", ref area)) area = new GH_Number(0);
            if (!DA.GetData<GH_Surface>("Surface", ref surface)) surface = new GH_Surface();

            double calculationArea = area.Value;
            if (surface.Value != null) calculationArea += surface.Value.GetArea();


            Types.Result result = new Types.Result()
            {
                GlobalWarmingPotential = new Types.UnitDouble<Types.LCA.CO2e>(assembly.GlobalWarmingPotential.Value * calculationArea),
                Acidification = new Types.UnitDouble<Types.LCA.kgSO2>(assembly.Acidification.Value * calculationArea),
                DepletionOfNonrenewbles = new Types.UnitDouble<Types.LCA.MJ>(assembly.DepletionOfNonrenewbles.Value * calculationArea),
                DepletionOfOzoneLayer = new Types.UnitDouble<Types.LCA.kgCFC11>(assembly.DepletionOfOzoneLayer.Value * calculationArea),
                Eutrophication = new Types.UnitDouble<Types.LCA.kgPhostphate>(assembly.Eutrophication.Value * calculationArea),
                FormationTroposphericOzone = new Types.UnitDouble<Types.LCA.kgNOx>(assembly.FormationTroposphericOzone.Value * calculationArea)
            };



            DA.SetData("LCA Result", result);
        }


        // Properties
        public override Guid ComponentGuid
        {
            get
            {
                return new Guid("{5ea4aa3d-d171-3a9f-a712-4323bfeb3b1a}");
            }
        }
        protected override System.Drawing.Bitmap Internal_Icon_24x24
        {
            get
            {
                return Properties.Resources.tortuga_calc;
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