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
using System.Xml;

namespace Tortuga.GrasshopperComponents
{

    public class MaterialEditor : GH_Component
    {
        public MaterialEditor() : base("Tortuga Material", "Material", "LCA Material", "Tortuga", "Material") { }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            List<int> optionals = new List<int>();

            

            optionals.Add(pManager.AddBooleanParameter("Quartz", "Q", "Optional: Use Quartz Database [true]", GH_ParamAccess.item));            
            optionals.Add(pManager.AddBooleanParameter("OekobauDat", "O", "Optional: Use OekobauDat Database [false]", GH_ParamAccess.item));
            optionals.Add(pManager.AddTextParameter("Path", "P", "Optional: Filepath if you want to load your own CSV data [empty]", GH_ParamAccess.item));

            optionals.Add(pManager.AddBooleanParameter("Production", "A1-A3", "Optional: Production Stage (A1-A3) [true]", GH_ParamAccess.item));
            optionals.Add(pManager.AddBooleanParameter("Waste Processing", "C3", "Optional: Waste processing Stage (C3) [true]", GH_ParamAccess.item));
            optionals.Add(pManager.AddBooleanParameter("Recycling Potential", "D", "Optional: Recycling Potential Stage (D) [true]", GH_ParamAccess.item));
            optionals.Add(pManager.AddTextParameter("URL", "URL", "URL pointing to resource, usually either http://www.quartzproject.org for Quartz or http://www.oekobaudat.de/OEKOBAU.DAT", GH_ParamAccess.item));
            optionals.Add(pManager.AddTextParameter("Lang", "l", "Language for Oekobaudat ('de' or 'en')", GH_ParamAccess.item));
            foreach (int i in optionals) pManager[i].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Material", "M", "Tortuga Material", GH_ParamAccess.item);
        }

        private Types.Material material;
        private string alternativeDataSourcePath;
        private Dictionary<string, Types.Material> LoadedMaterials;
        private string Url;
        private List<Types.LifecycleStage> Stages;

        public override void CreateAttributes()
        {
            m_attributes = new TortugaComponentAttributes(this);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Get any buffered Material
            string serializedData = this.GetValue("material", "");          
            if (serializedData != "")
                this.material = (Types.Material)Serialization.Utilities.Deserialize(serializedData, typeof(Types.Material));
            
            // Declare Variables
            GH_String path = new GH_String("");
            GH_Boolean productionStage = new GH_Boolean(true);
            GH_Boolean wasteProcessingStage = new GH_Boolean(true);
            GH_Boolean recyclingPotentialStage = new GH_Boolean(true);
            GH_Boolean useQuartz = new GH_Boolean(true);
            GH_Boolean useOekobaudat = new GH_Boolean(false);
            Stages = new List<Types.LifecycleStage>();

            GH_String url = new GH_String("");
            if (!DA.GetData<GH_String>("URL", ref url))
                this.Url = "";
            else
                this.Url = url.Value;

            // Get Data from Ports
            DA.GetData<GH_String>("Path", ref path);
            this.alternativeDataSourcePath = path.Value;
            DA.GetData<GH_Boolean>("Production", ref productionStage);
            DA.GetData<GH_Boolean>("Waste Processing", ref wasteProcessingStage);
            DA.GetData<GH_Boolean>("Recycling Potential", ref recyclingPotentialStage);
            DA.GetData<GH_Boolean>("Quartz", ref useQuartz);
            DA.GetData<GH_Boolean>("OekobauDat", ref useOekobaudat);

            GH_String lang = new GH_String("de");
            DA.GetData<GH_String>("Lang", ref lang);

            if (this.Url == "")
            {
                if (useOekobaudat.Value)
                    this.Url = "http://www.oekobaudat.de/OEKOBAU.DAT";
                if (useQuartz.Value)
                    this.Url = "http://www.quartzproject.org";
            }

            // Set selected Stages
            if (productionStage.Value) Stages.Add(new Types.LifecycleStage() { Name = "A1-A3", Column = 0 });
            if (wasteProcessingStage.Value) Stages.Add(new Types.LifecycleStage() { Name = "C3", Column = 1 });
            if (recyclingPotentialStage.Value) Stages.Add(new Types.LifecycleStage() { Name = "D", Column = 2 });

            // if a Path has been specified load Data from File
            if (this.alternativeDataSourcePath != "")
                this.LoadedMaterials = Types.Material.LoadFromFile(this.alternativeDataSourcePath, this.Stages);
            else
            {
                // Load from Quartz DB
                if (useQuartz.Value)
                    this.LoadedMaterials = Types.Material.LoadFromQuartz(this.Stages,this.Url);

                // Load from Oekobau.dat
                else
                    this.LoadedMaterials = Types.Material.LoadFromOekoBauDat(this.Stages, this.Url, lang.Value.ToLower());
            }
                

            DA.SetData("Material", this.material);
        }
        
        public void ShowEditor()
        {
            Forms.MaterialEditorForm materialEditor = new Forms.MaterialEditorForm();
            Grasshopper.GUI.GH_WindowsFormUtil.CenterFormOnCursor(materialEditor, true);
            
            if (this.material != null) materialEditor.materialEditor1.material = this.material;
            materialEditor.materialEditor1.Materials = this.LoadedMaterials;
            materialEditor.ShowDialog();
            materialEditor.materialEditor1.material.LoadData(this.Url);
            this.SetValue("material", Serialization.Utilities.Serialize(materialEditor.materialEditor1.material));
            this.ExpireSolution(true);           
        }

        // Properties
        public override Guid ComponentGuid
        {
            get
            {
                return new Guid("{5ea4aa2d-d271-3a9f-a777-4323bfeb3b1a}");
            }
        }
        protected override System.Drawing.Bitmap Internal_Icon_24x24
        {
            get
            {
                return Properties.Resources.tortuga_edit;
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

    public class MaterialLayer : GH_Component
    {
        public MaterialLayer() : base("Tortuga Material Layer", "Material Layer", "LCA Material Layer", "Tortuga", "Material") { }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Material", "M", "LCA Material", GH_ParamAccess.item);
            pManager.AddNumberParameter("Thickness", "T", "Thickness", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Layer", "L", "Tortuga Material Layer", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Tortuga.Types.Material material = null;
            DA.GetData<Tortuga.Types.Material>(0, ref material);

            GH_Number thickness = new GH_Number(0);
            DA.GetData<GH_Number>(1, ref thickness);

            Types.Layer layer = new Types.Layer(material, thickness.Value);

            DA.SetData(0, layer);
        }

        // Properties
        public override Guid ComponentGuid
        {
            get
            {
                return new Guid("{5ea4aa2d-d271-3a9f-a777-4321bfeb3b1a}");
            }
        }
        protected override System.Drawing.Bitmap Internal_Icon_24x24
        {
            get
            {
                return Properties.Resources.tortuga_edit;
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

    public class ProfileLayer : GH_Component
    {
        public ProfileLayer() : base("Tortuga Profile Layer", "Profile Layer", "LCA Profile Layer", "Tortuga", "Material") { }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Profile", "P", "LCA Profile", GH_ParamAccess.item);
            pManager.AddGenericParameter("Material", "M", "Material", GH_ParamAccess.item);
            pManager.AddNumberParameter("Distance", "D", "Distance", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Layer", "L", "Tortuga Material Layer", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Tortuga.Types.Material material = null;
            DA.GetData<Tortuga.Types.Material>(1, ref material);

            Tortuga.Types.Profile profile = null;
            DA.GetData<Tortuga.Types.Profile>(0, ref profile);

            GH_Number distance = new GH_Number(0);
            DA.GetData<GH_Number>(2, ref distance);

            Types.Layer layer = new Types.Layer(material, profile.Area / distance.Value);

            DA.SetData(0, layer);
        }

        // Properties
        public override Guid ComponentGuid
        {
            get
            {
                return new Guid("{5ea4aa2d-d271-3a9f-a117-4321bfeb1b1a}");
            }
        }
        protected override System.Drawing.Bitmap Internal_Icon_24x24
        {
            get
            {
                return Properties.Resources.tortuga_edit;
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

    public class StackedMaterialLayer : GH_Component
    {
        public StackedMaterialLayer() : base("Tortuga Stacked Material Layer", "Stacked Material Layer", "LCA Stacked Material Layer", "Tortuga", "Material") { }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Materials", "M", "LCA Materials", GH_ParamAccess.list);
            pManager.AddNumberParameter("Widths", "W", "Stacked Widths", GH_ParamAccess.list);
            pManager.AddNumberParameter("Thickness", "T", "Overall Thickness", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Layers", "L", "Tortuga Material Layers", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Tortuga.Types.Material> materials = new List<Types.Material>();
            DA.GetDataList<Tortuga.Types.Material>(0, materials);

            GH_Number thickness = new GH_Number(0);
            DA.GetData<GH_Number>(2, ref thickness);

            List<GH_Number> widths = new List<GH_Number>();
            DA.GetDataList<GH_Number>(1, widths);

            double overall = 0;
            foreach (GH_Number width in widths) overall += width.Value;

            List<Types.Layer> layers = new List<Types.Layer>();

            if (widths.Count == materials.Count)
            {

                for (int i = 0; i < widths.Count; i++)
                {
                    double width = widths[i].Value;
                    double percentage = width / overall;

                    layers.Add(new Types.Layer(materials[i], thickness.Value * percentage));

                }

            }


            DA.SetDataList(0, layers);
        }

        // Properties
        public override Guid ComponentGuid
        {
            get
            {
                return new Guid("{5ea4aa2d-d271-3a9f-a121-4321bfeb3b1a}");
            }
        }
        protected override System.Drawing.Bitmap Internal_Icon_24x24
        {
            get
            {
                return Properties.Resources.tortuga_edit;
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

    public class MaterialAssembly : GH_Component
    {
        public MaterialAssembly() : base("Tortuga Material Assembly", "Material Assembly", "LCA Material Assembly", "Tortuga", "Material") { }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Layer", "L", "LCA Material Layers", GH_ParamAccess.list);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Assembly", "A", "Tortuga Material Assembly", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Tortuga.Types.Layer> layers = new List<Types.Layer>();
            DA.GetDataList<Tortuga.Types.Layer>(0, layers);

            Types.Assembly assembly = new Types.Assembly();
            assembly.Layers = layers;

            DA.SetData(0, assembly);
        }

        // Properties
        public override Guid ComponentGuid
        {
            get
            {
                return new Guid("{5ea4aa2d-d221-3a9f-a777-4321bfeb3b1a}");
            }
        }
        protected override System.Drawing.Bitmap Internal_Icon_24x24
        {
            get
            {
                return Properties.Resources.tortuga_edit;
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


    public class TortugaComponentAttributes : Grasshopper.Kernel.Attributes.GH_ComponentAttributes
    {
        public TortugaComponentAttributes(IGH_Component MaterialEditor) : base(MaterialEditor) { }

        public override Grasshopper.GUI.Canvas.GH_ObjectResponse RespondToMouseDoubleClick(Grasshopper.GUI.Canvas.GH_Canvas sender, Grasshopper.GUI.GH_CanvasMouseEvent e)
        {
            ((MaterialEditor)Owner).ShowEditor();
            return Grasshopper.GUI.Canvas.GH_ObjectResponse.Handled;
        }
    }

/*
    public class Layer : GH_Component
    {
        public Layer() : base("Tortuga Material Layer", "Material Layer", "LCA Material Layer", "Tortuga", "Material") { }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Material", "M", "Tortuga Material", GH_ParamAccess.item);
            pManager.AddNumberParameter("Thickness", "T", "Thickness in m", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Layer", "L", "Tortuga Material Layer", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GH_Number thickness = new GH_Number(0);
            DA.GetData<GH_Number>("Thickness", ref thickness);

            Types.Material material = new Types.Material();
            DA.GetData<Types.Material>("Material", ref material);

            Types.Layer layer = new Types.Layer(material,thickness.Value);

            DA.SetData("Layer", layer);
        }

        // Properties
        public override Guid ComponentGuid
        {
            get
            {
                return new Guid("{5ea4aa2d-d252-3a9f-a777-4323bfeb3b1a}");
            }
        }
        protected override System.Drawing.Bitmap Internal_Icon_24x24
        {
            get
            {
                return Properties.Resources.tortuga_edit;
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


    public class Assembly : GH_Component
    {
        public Assembly() : base("Tortuga Material Assembly", "Material Assembly", "LCA Material Assembly", "Tortuga", "Material") { }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Layer", "L", "Tortuga Material Layer", GH_ParamAccess.list);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Assembly", "A", "Tortuga Material Assembly", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Types.Layer> layers = new List<Types.Layer>();
            DA.GetDataList<Types.Layer>("Layer", layers);

            Types.Assembly assembly = new Types.Assembly();
            foreach (Types.Layer layer in layers) assembly.Layers.Add(layer);

            DA.SetData("Assembly", assembly);
        }

        // Properties
        public override Guid ComponentGuid
        {
            get
            {
                return new Guid("{5ea4aa2d-d252-3a9f-a717-4123bfeb3b1a}");
            }
        }
        protected override System.Drawing.Bitmap Internal_Icon_24x24
        {
            get
            {
                return Properties.Resources.tortuga_edit;
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
    */


}