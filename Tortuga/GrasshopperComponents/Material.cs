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
        public MaterialEditor() : base("Tortuga Material Editor", "Material Editor", "LCA Material Editor", "Tortuga", "Tortuga") { }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            int a = pManager.AddTextParameter("Path", "P", "Optional: Filepath if you want to load your own CSV data", GH_ParamAccess.item);
            pManager[a].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Material", "M", "Tortuga Material", GH_ParamAccess.item);
        }

        private Types.Assembly assembly;
        private string alternativeDataSourcePath;

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
            this.alternativeDataSourcePath = path.Value;



            if (assembly != null) this.SetValue("assembly", Serialization.Utilities.Serialize(this.assembly));

            DA.SetData("Material", assembly);
        }

        public void ShowEditor()
        {

            Forms.MaterialEditorForm materialEditor = new Forms.MaterialEditorForm();

            Grasshopper.GUI.GH_WindowsFormUtil.CenterFormOnCursor(materialEditor, true);

            materialEditor.materialEditor1.alternativeDataSourcePath = this.alternativeDataSourcePath;
            if (this.assembly != null) materialEditor.materialEditor1.assembly = this.assembly;

            materialEditor.ShowDialog();

            this.assembly = materialEditor.materialEditor1.assembly;
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
                return Properties.Resources.report_stack;
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

}