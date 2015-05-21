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
    public class Result : GH_Component
    {
        public Result() : base("Tortuga Calculator Result", "Calculator Result", "Calculator LCA Values", "Tortuga", "Tortuga") { }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("LCA Result", "L", "Calculated LCA Result", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("GlobalWarmingPotential", "CO2e", "Global Warming Potential in CO2e", GH_ParamAccess.item);
            pManager.AddNumberParameter("Acidification", "kgSO2", "Acidification in kgSO2", GH_ParamAccess.item);
            pManager.AddNumberParameter("DepletionOfNonrenewbles", "MJ", "Depletion of nonrenewbles in MJ", GH_ParamAccess.item);
            pManager.AddNumberParameter("DepletionOfOzoneLayer", "kgCFC11", "Depletion of Ozone Layer in kgCFC11", GH_ParamAccess.item);
            pManager.AddNumberParameter("Eutrophication", "kgPhosphate", "Eutrophication in kg Phosphate", GH_ParamAccess.item);
            pManager.AddNumberParameter("FormationTroposphericOzone", "kgNOX", "Formation Tropospheric Ozone in kgNOX", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Types.Result result = new Types.Result();
            DA.GetData<Types.Result>("LCA Result", ref result);

            DA.SetData("GlobalWarmingPotential", result.GlobalWarmingPotential.Value);
            DA.SetData("Acidification", result.Acidification.Value);
            DA.SetData("DepletionOfNonrenewbles", result.DepletionOfNonrenewbles.Value);
            DA.SetData("DepletionOfOzoneLayer", result.DepletionOfOzoneLayer.Value);
            DA.SetData("Eutrophication", result.Eutrophication.Value);
            DA.SetData("FormationTroposphericOzone", result.FormationTroposphericOzone.Value);

        }


        // Properties
        public override Guid ComponentGuid
        {
            get
            {
                return new Guid("{5ea4bb3d-d171-3a9f-a777-4323bfeb3b1a}");
            }
        }
        protected override System.Drawing.Bitmap Internal_Icon_24x24
        {
            get
            {
                return Properties.Resources.report_green;
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