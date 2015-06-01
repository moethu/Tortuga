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

    public class CalculateSurface : TortugaCalculateComponent
    {
        public CalculateSurface() : base("Tortuga Surface Calculator", "Calculator", "Calculate LCA Values", "Tortuga", "Calculate") { }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Material", "Material", "Tortuga Material", GH_ParamAccess.item);

            List<int> optinalParameters = new List<int>();

            optinalParameters.Add(pManager.AddNumberParameter("Area", "Area", "Optional: Area to calculate", GH_ParamAccess.item));
            optinalParameters.Add(pManager.AddSurfaceParameter("Surface", "Surface", "Optional: Surface to calculate", GH_ParamAccess.item));

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

    public class CalculateVolume : TortugaCalculateComponent
    {
        public CalculateVolume() : base("Tortuga Volume Calculator", "Calculator", "Calculate LCA Values", "Tortuga", "Calculate") { }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Material", "Material", "Tortuga Material", GH_ParamAccess.item);

            List<int> optinalParameters = new List<int>();

            optinalParameters.Add(pManager.AddNumberParameter("Volume", "Volume", "Optional: Volume to calculate", GH_ParamAccess.item));
            optinalParameters.Add(pManager.AddBrepParameter("Brep", "Brep", "Optional: Brep to calculate", GH_ParamAccess.item));

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

            GH_Brep brep = new GH_Brep();
            GH_Number volume = new GH_Number(0);

            if (!DA.GetData<GH_Number>("Volume", ref volume)) volume = new GH_Number(0);
            if (!DA.GetData<GH_Brep>("Brep", ref brep)) brep = new GH_Brep();

            double calculationVolume = volume.Value;
            if (brep.Value != null) calculationVolume += brep.Value.GetVolume();

            

            Types.Result result = new Types.Result()
            {
                GlobalWarmingPotential = new Types.UnitDouble<Types.LCA.CO2e>(assembly.GlobalWarmingPotential.Value * calculationVolume),
                Acidification = new Types.UnitDouble<Types.LCA.kgSO2>(assembly.Acidification.Value * calculationVolume),
                DepletionOfNonrenewbles = new Types.UnitDouble<Types.LCA.MJ>(assembly.DepletionOfNonrenewbles.Value * calculationVolume),
                DepletionOfOzoneLayer = new Types.UnitDouble<Types.LCA.kgCFC11>(assembly.DepletionOfOzoneLayer.Value * calculationVolume),
                Eutrophication = new Types.UnitDouble<Types.LCA.kgPhostphate>(assembly.Eutrophication.Value * calculationVolume),
                FormationTroposphericOzone = new Types.UnitDouble<Types.LCA.kgNOx>(assembly.FormationTroposphericOzone.Value * calculationVolume)
            };



            DA.SetData("LCA Result", result);
        }


        // Properties
        public override Guid ComponentGuid
        {
            get
            {
                return new Guid("{5ea4aa3d-d171-3a9f-a712-4323bfeb3b2b}");
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
    
     public class CalculateLength : TortugaCalculateComponent
    {
        public CalculateLength() : base("Tortuga Length Calculator", "Calculator", "Calculate LCA Values", "Tortuga", "Calculate") { }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Material", "Material", "Tortuga Material", GH_ParamAccess.item);
            List<int> optinalParameters = new List<int>();
            
            pManager.AddCurveParameter("Curve", "C", "Curve to calculate", GH_ParamAccess.item);
            
            optinalParameters.Add(pManager.AddNumberParameter("Radius", "Radius", "Optional: Radius to calculate", GH_ParamAccess.item));

            optinalParameters.Add(pManager.AddNumberParameter("Height", "Height", "Optional: Height to calculate", GH_ParamAccess.item));
            optinalParameters.Add(pManager.AddNumberParameter("Width", "Width", "Optional: Width to calculate", GH_ParamAccess.item));

            optinalParameters.Add(pManager.AddSurfaceParameter("Profile", "Profile", "Optional: Profile to calculate", GH_ParamAccess.item));
            
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

            GH_Curve curve = new GH_Curve();
            GH_Number radius = new GH_Number(0);
            GH_Number height = new GH_Number(0);
            GH_Number width = new GH_Number(0);
            GH_Surface profile = null;

            if (!DA.GetData<GH_Number>("Radius", ref radius)) radius.Value = 0;
            if (!DA.GetData<GH_Number>("Height", ref height)) height.Value = 0;
            if (!DA.GetData<GH_Number>("Width", ref width)) width.Value = 0;
            if (!DA.GetData<GH_Surface>("Profile", ref profile)) profile = null;
            DA.GetData<GH_Curve>("Curve", ref curve);

            double calculationVolume = 0;


            if (profile != null)
            {
                calculationVolume = curve.Value.GetLength() * profile.Value.GetArea();
                drawColumn(curve.Value.PointAtStart, curve.Value.PointAtEnd,profile.Value);
            }
            else
            {
                if (radius.Value > 0)
                {
                    drawColumn(curve.Value.PointAtStart, curve.Value.PointAtEnd, radius.Value);
                    calculationVolume = curve.Value.GetLength() * radius.Value * radius.Value * Math.PI;
                }
                else
                {
                    drawColumn(curve.Value.PointAtStart, curve.Value.PointAtEnd, height.Value, width.Value);
                    calculationVolume = curve.Value.GetLength() * height.Value * width.Value;
                }
            }

 
                

            Types.Result result = new Types.Result()
            {
                GlobalWarmingPotential = new Types.UnitDouble<Types.LCA.CO2e>(assembly.GlobalWarmingPotential.Value * calculationVolume),
                Acidification = new Types.UnitDouble<Types.LCA.kgSO2>(assembly.Acidification.Value * calculationVolume),
                DepletionOfNonrenewbles = new Types.UnitDouble<Types.LCA.MJ>(assembly.DepletionOfNonrenewbles.Value * calculationVolume),
                DepletionOfOzoneLayer = new Types.UnitDouble<Types.LCA.kgCFC11>(assembly.DepletionOfOzoneLayer.Value * calculationVolume),
                Eutrophication = new Types.UnitDouble<Types.LCA.kgPhostphate>(assembly.Eutrophication.Value * calculationVolume),
                FormationTroposphericOzone = new Types.UnitDouble<Types.LCA.kgNOx>(assembly.FormationTroposphericOzone.Value * calculationVolume)
            };



            DA.SetData("LCA Result", result);
        }


        // Properties
        public override Guid ComponentGuid
        {
            get
            {
                return new Guid("{5ea4aa3d-d171-3a9f-a721-4323bfeb3b2b}");
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
  
    public class CalculateFoundation : TortugaCalculateComponent
    {
        public CalculateFoundation() : base("Tortuga Foundation Calculator", "Calculator", "Calculate LCA Values", "Tortuga", "Calculate") { }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Material", "Material", "Tortuga Material", GH_ParamAccess.item);
            
            pManager.AddPointParameter("Point", "Point", "Location", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Piles", "Piles", "Number of Piles", GH_ParamAccess.item);
            pManager.AddNumberParameter("Pile Radius", "Radius", "Radius for Piles", GH_ParamAccess.item);
            pManager.AddNumberParameter("Pile Length", "Length", "Plate Length", GH_ParamAccess.item);
            pManager.AddNumberParameter("Plate Thickness", "PT", "Plate Thickness", GH_ParamAccess.item);
            pManager.AddNumberParameter("Plate Length", "PL", "Plate Length", GH_ParamAccess.item);
            pManager.AddNumberParameter("Plate Width", "PW", "Plate Width", GH_ParamAccess.item);

        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("LCA Result", "L", "Calculated LCA Result", GH_ParamAccess.item);
        }


        
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Types.Assembly assembly = new Types.Assembly();
            DA.GetData<Types.Assembly>("Material", ref assembly);
            
            
            GH_Point point = new GH_Point();
            GH_Integer piles = new GH_Integer(1);
            GH_Number radius = new GH_Number(0);
            GH_Number plateThickness = new GH_Number(0);
            GH_Number plateLength = new GH_Number(0);
            GH_Number plateWidth = new GH_Number(0);
            GH_Number pileLength = new GH_Number(0);

            DA.GetData<GH_Point>("Point", ref point);
            DA.GetData<GH_Number>("Pile Radius", ref radius);
            DA.GetData<GH_Integer>("Piles", ref piles);
            DA.GetData<GH_Number>("Plate Thickness", ref plateThickness);
            DA.GetData<GH_Number>("Plate Length", ref plateLength);
            DA.GetData<GH_Number>("Plate Width", ref plateWidth);
            DA.GetData<GH_Number>("Pile Length", ref pileLength);


            drawExtrusion(point.Value, plateLength.Value, plateWidth.Value, plateThickness.Value);
            Rhino.Geometry.Circle circle = new Rhino.Geometry.Circle(point.Value, (plateWidth.Value / 2) * 0.8);

            double numberOfPiles = piles.Value;

            for (int i = 1; i <= piles.Value; i++)
            {
                double iteration = i;
                double factor = 2.0 * Math.PI * (iteration / numberOfPiles);
                Rhino.Geometry.Point3d center = (piles.Value == 1)? point.Value : circle.ToNurbsCurve().PointAt(factor);
                drawColumn(center, pileLength.Value, radius.Value);
            }


            double calculationVolume = (piles.Value * pileLength.Value * radius.Value) + (plateThickness.Value * plateLength.Value * plateWidth.Value);


            Types.Result result = new Types.Result()
            {
                GlobalWarmingPotential = new Types.UnitDouble<Types.LCA.CO2e>(assembly.GlobalWarmingPotential.Value * calculationVolume),
                Acidification = new Types.UnitDouble<Types.LCA.kgSO2>(assembly.Acidification.Value * calculationVolume),
                DepletionOfNonrenewbles = new Types.UnitDouble<Types.LCA.MJ>(assembly.DepletionOfNonrenewbles.Value * calculationVolume),
                DepletionOfOzoneLayer = new Types.UnitDouble<Types.LCA.kgCFC11>(assembly.DepletionOfOzoneLayer.Value * calculationVolume),
                Eutrophication = new Types.UnitDouble<Types.LCA.kgPhostphate>(assembly.Eutrophication.Value * calculationVolume),
                FormationTroposphericOzone = new Types.UnitDouble<Types.LCA.kgNOx>(assembly.FormationTroposphericOzone.Value * calculationVolume)
            };



            DA.SetData("LCA Result", result);
        }


        // Properties
        public override Guid ComponentGuid
        {
            get
            {
                return new Guid("{5ea4ab1d-d171-3a9f-a721-4323bfeb3b2b}");
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
    
    
    public class TortugaCalculateComponent : GH_Component
    {
        public TortugaCalculateComponent(string name, string nickname, string description, string panel, string group) : base(name, nickname, description, panel, group) { }

        public List<Rhino.Geometry.Brep> Preview = new List<Rhino.Geometry.Brep>();



        public void SetPreview(Rhino.Geometry.Brep brep)
        {
            if (!Preview.Contains(brep)) Preview.Add(brep);

        }

        public void drawColumn(Rhino.Geometry.Point3d point1, Rhino.Geometry.Point3d point2, double radius)
        {
            Rhino.Geometry.Plane plane = new Rhino.Geometry.Plane(point1, new Rhino.Geometry.Vector3d((point2 - point1)));
            Rhino.Geometry.Circle c = new Rhino.Geometry.Circle(plane,point1,radius);

            Rhino.Geometry.Surface srf = Rhino.Geometry.Surface.CreateExtrusion(c.ToNurbsCurve(), new Rhino.Geometry.Vector3d((point2 - point1)));
            
            SetPreview(srf.ToBrep());
        }

        public void drawColumn(Rhino.Geometry.Point3d point1, Rhino.Geometry.Point3d point2, Rhino.Geometry.Brep brep)
        {
            foreach (Rhino.Geometry.Curve curve in brep.Curves3D)
            {
                Rhino.Geometry.Surface srf = Rhino.Geometry.Surface.CreateExtrusion(curve, new Rhino.Geometry.Vector3d((point2 - point1)));
                SetPreview(srf.ToBrep());
            }
            
        }

        public void drawColumn(Rhino.Geometry.Point3d point1, Rhino.Geometry.Point3d point2, double height, double width)
        {
            Rhino.Geometry.Plane plane = new Rhino.Geometry.Plane(point1, new Rhino.Geometry.Vector3d((point2 - point1)));
            Rhino.Geometry.Point3d cornerA = new Rhino.Geometry.Point3d(point1.X - width/2, point1.Y - height/2,point1.Z);
            Rhino.Geometry.Point3d cornerB = new Rhino.Geometry.Point3d(point1.X + width/2, point1.Y + height/2,point1.Z);
            Rhino.Geometry.Rectangle3d rect = new Rhino.Geometry.Rectangle3d(plane, cornerA,cornerB);

            Rhino.Geometry.Surface srf = Rhino.Geometry.Surface.CreateExtrusion(rect.ToNurbsCurve(), new Rhino.Geometry.Vector3d((point2 - point1)));

            SetPreview(srf.ToBrep());
        }

        public void drawColumn(Rhino.Geometry.Point3d point1, double length, double radius)
        {
            
            Rhino.Geometry.Circle c = new Rhino.Geometry.Circle(point1,radius);
            
            Rhino.Geometry.Surface srf = Rhino.Geometry.NurbsSurface.CreateExtrusion(c.ToNurbsCurve(),
                new Rhino.Geometry.Vector3d(0,0,(length*-1)));
            
            SetPreview(srf.ToBrep());
        }

        public void drawExtrusion(Rhino.Geometry.Point3d point1, double height, double width, double thickness)
        {
            Rhino.Geometry.Plane plane = new Rhino.Geometry.Plane(point1,Rhino.Geometry.Vector3d.ZAxis);
            Rhino.Geometry.Point3d cornerA = new Rhino.Geometry.Point3d(point1.X - width / 2, point1.Y - height / 2, point1.Z);
            Rhino.Geometry.Point3d cornerB = new Rhino.Geometry.Point3d(point1.X + width / 2, point1.Y + height / 2, point1.Z);
            Rhino.Geometry.Rectangle3d rect = new Rhino.Geometry.Rectangle3d(plane, cornerA, cornerB);
            Rhino.Geometry.Surface srf = Rhino.Geometry.Surface.CreateExtrusion(rect.ToNurbsCurve(), new Rhino.Geometry.Vector3d(0, 0, thickness));
            SetPreview(srf.ToBrep());
        }



        public override void DrawViewportMeshes(IGH_PreviewArgs args)
        {
            foreach (Rhino.Geometry.Brep brep in Preview) args.Display.DrawBrepShaded(brep, args.ShadeMaterial);
        }

        protected override void BeforeSolveInstance()
        {
            this.Preview = new List<Rhino.Geometry.Brep>();
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {

        }

        protected override void SolveInstance(IGH_DataAccess DA) { }

        public override Guid ComponentGuid
        {
            get
            {
                return new Guid("");
            }
        }




    }


}
