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
    public class Profiles : GH_Component
    {
        public Profiles() : base("Tortuga Profiles", "Profiles", "Profiles", "Tortuga", "Material") { }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {

        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Profiles", "Profiles", "Profiles", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Types.Profile.LoadProfiles();

            DA.SetDataList("Profiles", Types.Profile.Profiles);
        }


        // Properties
        public override Guid ComponentGuid
        {
            get
            {
                return new Guid("{5ea4bb3d-d171-3a9f-a771-4121bfeb1b1a}");
            }
        }
        protected override System.Drawing.Bitmap Internal_Icon_24x24
        {
            get
            {
                return Properties.Resources.tortuga_result;
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

    public class GetProfileByName : GH_Component
    {
        public GetProfileByName() : base("Tortuga Get Profile By Name", "Profiles", "Profiles", "Tortuga", "Material") { }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Name", "Name", "Profile Name", GH_ParamAccess.item);
            pManager.AddGenericParameter("Profiles", "Profiles", "Profiles", GH_ParamAccess.list);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Profile", "Profile", "Profile", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GH_String name = new GH_String("");
            DA.GetData<GH_String>("Name", ref name);

            List<Types.Profile> profiles = new List<Types.Profile>();
            DA.GetDataList<Types.Profile>("Profiles", profiles);

            List<Types.Profile> returnProfiles = new List<Types.Profile>();

            foreach (Types.Profile profile in profiles)
            {
                if (profile.Name.ToLower() == name.Value.ToLower()) returnProfiles.Add(profile);
            }

            DA.SetDataList("Profile", returnProfiles);
        }


        // Properties
        public override Guid ComponentGuid
        {
            get
            {
                return new Guid("{5ea4bb3d-d171-1a1f-a171-4121bfeb1b1a}");
            }
        }
        protected override System.Drawing.Bitmap Internal_Icon_24x24
        {
            get
            {
                return Properties.Resources.tortuga_result;
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