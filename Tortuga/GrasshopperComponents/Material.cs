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

    public class MaterialEditor : GH_Component
    {
        public MaterialEditor() : base("Tortuga Material Editor", "Material Editor", "LCA Material Editor", "Tortuga", "Material") { }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            List<int> optionals = new List<int>();

            optionals.Add(pManager.AddTextParameter("Path", "P", "Optional: Filepath if you want to load your own CSV data", GH_ParamAccess.item));
            optionals.Add(pManager.AddBooleanParameter("Percentual", "%", "Optional: Percentual values instead of metric [false]", GH_ParamAccess.item));

            optionals.Add(pManager.AddBooleanParameter("Production", "A1-A3", "Optional: Production Stage (A1-A3) [true]", GH_ParamAccess.item));
            optionals.Add(pManager.AddBooleanParameter("Waste Processing", "C3", "Optional: Waste processing Stage (C3) [true]", GH_ParamAccess.item));
            optionals.Add(pManager.AddBooleanParameter("Recycling Potential", "D", "Optional: Recycling Potential Stage (D) [true]", GH_ParamAccess.item));

            foreach (int i in optionals) pManager[i].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Material", "M", "Tortuga Material", GH_ParamAccess.item);
        }

        private Types.Assembly assembly;
        private string alternativeDataSourcePath;
        private bool isPercentual;

        private List<Types.LifecycleStage> Stages;

        public override void CreateAttributes()
        {
            m_attributes = new TortugaComponentAttributes(this);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string serializedData = this.GetValue("assembly", "");
            
            if (serializedData != "")
            {
                this.assembly = (Types.Assembly)Serialization.Utilities.Deserialize(serializedData, typeof(Types.Assembly));
            }


            GH_String path = new GH_String("");
            DA.GetData<GH_String>("Path", ref path);


            GH_Boolean percentual = new GH_Boolean(false);
            if (!DA.GetData<GH_Boolean>("Percentual", ref percentual)) percentual.Value = false;

            GH_Boolean productionStage = new GH_Boolean(true);
            GH_Boolean wasteProcessingStage = new GH_Boolean(true);
            GH_Boolean recyclingPotentialStage = new GH_Boolean(true); 

            if (!DA.GetData<GH_Boolean>("Production", ref productionStage)) productionStage.Value = true;
            if (!DA.GetData<GH_Boolean>("Waste Processing", ref wasteProcessingStage)) wasteProcessingStage.Value = true;
            if (!DA.GetData<GH_Boolean>("Recycling Potential", ref recyclingPotentialStage)) recyclingPotentialStage.Value = true;

            Stages = new List<Types.LifecycleStage>();
            if (productionStage.Value) Stages.Add(new Types.LifecycleStage() { Name = "Production", Column = 0 });
            if (wasteProcessingStage.Value) Stages.Add(new Types.LifecycleStage() { Name = "Waste Processing", Column = 1 });
            if (recyclingPotentialStage.Value) Stages.Add(new Types.LifecycleStage() { Name = "Recycling Potential", Column = 2 });

            this.isPercentual = percentual.Value;
            this.alternativeDataSourcePath = path.Value;

            Types.Material.LoadFrom(this.alternativeDataSourcePath, this.Stages);

            if (this.assembly != null)
            {
                foreach (Types.Layer layer in this.assembly.Layers)
                {
                    if (Types.Material.LoadedMaterials.ContainsKey(layer.Material.Name))
                    {
                        layer.Material.CopyFrom(Types.Material.LoadedMaterials[layer.Material.Name]);
                    }
                }
            }

            //if (assembly != null) this.SetValue("assembly", Serialization.Utilities.Serialize(this.assembly));

            DA.SetData("Material", assembly);
        }

        public void ShowEditor()
        {

            Forms.MaterialEditorForm materialEditor = new Forms.MaterialEditorForm();

            Grasshopper.GUI.GH_WindowsFormUtil.CenterFormOnCursor(materialEditor, true);

            materialEditor.materialEditor1.alternativeDataSourcePath = this.alternativeDataSourcePath;
            materialEditor.materialEditor1.isPercentual = this.isPercentual;
            materialEditor.materialEditor1.Stages = this.Stages;


            if (this.assembly != null) materialEditor.materialEditor1.assembly = this.assembly;

            materialEditor.ShowDialog();

           // this.assembly = materialEditor.materialEditor1.assembly;

            this.SetValue("assembly", Serialization.Utilities.Serialize(materialEditor.materialEditor1.assembly));

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