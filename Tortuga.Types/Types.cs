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
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace Tortuga.Types
{

    public class Result
    {
        public UnitDouble<LCA.CO2e> GlobalWarmingPotential;
        public UnitDouble<LCA.kgCFC11> DepletionOfOzoneLayer;
        public UnitDouble<LCA.kgSO2> Acidification;
        public UnitDouble<LCA.kgPhostphate> Eutrophication;
        public UnitDouble<LCA.kgNOx> FormationTroposphericOzone;
        public UnitDouble<LCA.MJ> DepletionOfNonrenewbles;

        public override string ToString()
        {
            return string.Format("Tortuga Result ({0} CO2e)", this.GlobalWarmingPotential.Value);
        }
    }

    public class Material
    {
        [DataMember]
        public string Name;

        [DataMember]
        public Guid GUID;

        [DataMember]
        public UnitDouble<LCA.CO2e> GlobalWarmingPotential;

        [DataMember]
        public UnitDouble<LCA.kgCFC11> DepletionOfOzoneLayer;

        [DataMember]
        public UnitDouble<LCA.kgSO2> Acidification;

        [DataMember]
        public UnitDouble<LCA.kgPhostphate> Eutrophication;

        [DataMember]
        public UnitDouble<LCA.kgNOx> FormationTroposphericOzone;

        [DataMember]
        public UnitDouble<LCA.MJ> DepletionOfNonrenewbles;

        public Material() 
        {

        }

        public ListViewItem Draw()
        {
            ListViewItem layerItem = new ListViewItem();
            layerItem.Margin = new Thickness(1);
            if (Layer.contextMenu == null) Layer.BuildContextMenu();
            layerItem.Background = Brushes.White;




            layerItem.Tag = this;

            StackPanel panel = new StackPanel()
            {
                Orientation = Orientation.Horizontal,
                Height = 20
            };


            layerItem.BorderBrush = Brushes.White;
            layerItem.BorderThickness = new Thickness(1);


            TextBlock title = new TextBlock()
            {
                Text = this.Name,
                VerticalAlignment = System.Windows.VerticalAlignment.Center,
                Margin = new Thickness(5)
            };


            panel.Children.Add(title);
            panel.Margin = new Thickness(2);

            layerItem.Content = panel;

            return layerItem;

        }
    }

    [DataContract]
    [XmlSerializerFormat]
    [KnownType(typeof(Layer))]
    [KnownType(typeof(Material))]
    [KnownType(typeof(UnitDouble<LCA.CO2e>))]
    [KnownType(typeof(LCA.CO2e))]
    [KnownType(typeof(UnitDouble<LCA.kgCFC11>))]
    [KnownType(typeof(LCA.kgCFC11))]
    [KnownType(typeof(UnitDouble<LCA.kgSO2>))]
    [KnownType(typeof(LCA.kgSO2))]
    [KnownType(typeof(UnitDouble<LCA.kgPhostphate>))]
    [KnownType(typeof(LCA.kgPhostphate))]
    [KnownType(typeof(UnitDouble<LCA.kgNOx>))]
    [KnownType(typeof(LCA.kgNOx))]
    [KnownType(typeof(UnitDouble<LCA.MJ>))]
    [KnownType(typeof(LCA.MJ))]
    [KnownType(typeof(LCA))]
    public class Assembly
    {
        [DataMember]
        public List<Layer> Layers;

        public override string ToString()
        {
            string result = "Tortuga Material Assembly";
            foreach (Layer layer in this.Layers) result += string.Format(", {0}", layer.Material.Name);
            return result;
        }
        
        public Assembly(){this.Layers = new List<Layer>();}

        public double Width
        {
            get
            {
                double overall = 0;
                foreach (Layer layer in this.Layers) overall += layer.Width;
                return overall;
            }
        }

        public UnitDouble<LCA.kgCFC11> DepletionOfOzoneLayer
        {
            get
            {
                UnitDouble<LCA.kgCFC11> overall = new UnitDouble<LCA.kgCFC11>(0);
                foreach (Layer layer in this.Layers) overall += layer.DepletionOfOzoneLayer;
                return overall;
            }
        }

        public UnitDouble<LCA.kgSO2> Acidification
        {
            get
            {
                UnitDouble<LCA.kgSO2> overall = new UnitDouble<LCA.kgSO2>(0);
                foreach (Layer layer in this.Layers) overall += layer.Acidification;
                return overall;
            }
        }

        public UnitDouble<LCA.kgPhostphate> Eutrophication
        {
            get
            {
                UnitDouble<LCA.kgPhostphate> overall = new UnitDouble<LCA.kgPhostphate>(0);
                foreach (Layer layer in this.Layers) overall += layer.Eutrophication;
                return overall;
            }
        }

        public UnitDouble<LCA.kgNOx> FormationTroposphericOzone
        {
            get
            {
                UnitDouble<LCA.kgNOx> overall = new UnitDouble<LCA.kgNOx>(0);
                foreach (Layer layer in this.Layers) overall += layer.FormationTroposphericOzone;
                return overall;
            }
        }

        public UnitDouble<LCA.MJ> DepletionOfNonrenewbles
        {
            get
            {
                UnitDouble<LCA.MJ> overall = new UnitDouble<LCA.MJ>(0);
                foreach (Layer layer in this.Layers) overall += layer.DepletionOfNonrenewbles;
                return overall;
            }
        }

        public UnitDouble<LCA.CO2e> GlobalWarmingPotential
        {
            get
            {
                UnitDouble<LCA.CO2e> overall = new UnitDouble<LCA.CO2e>(0);
                foreach (Layer layer in this.Layers) overall += layer.GlobalWarmingPotential;
                return overall;
            }
        }

        public void AddLayer(ListView assemblyList, Layer layer)
        {
            this.Layers.Add(layer);
            assemblyList.Items.Add(layer.Draw());
        }

        public void Draw(ListView assemblyList)
        {
            foreach (Layer layer in this.Layers)
            assemblyList.Items.Add(layer.Draw());
        }


        public static Assembly operator +(Assembly first, Assembly second)
        {
            Assembly assembly = new Assembly();

            foreach (Layer layer in first.Layers) assembly.Layers.Add(layer);
            foreach (Layer layer in second.Layers) assembly.Layers.Add(layer);

            return assembly;
        }

        public static Assembly operator *(Assembly first, double factor)
        {
            Assembly assembly = new Assembly();

            foreach (Layer layer in first.Layers)
            {
                Layer lay = new Layer(layer.Material, layer.Width * factor);                
                assembly.Layers.Add(lay);
            }
       

            return assembly;
        }

    }

    [DataContract]
    [XmlSerializerFormat]
    public class Layer
    {
        [DataMember]
        public Material Material;

        [DataMember]
        public double Width;

        public Layer(Material material) { this.Width = 0.02; this.Material = material; }

        public Layer(Material material, double width) { this.Width = width; this.Material = material; }

        private static double heightMax = 0.2;
        private static double heightMin = 0.02;

        public ListViewItem Draw()
        {
            ListViewItem layerItem = new ListViewItem();
            layerItem.Margin = new Thickness(1);
            if (Layer.contextMenu == null) Layer.BuildContextMenu();

            layerItem.ContextMenu = Layer.contextMenu;
            

            
            layerItem.Tag = this;

            double width = this.Width;
            if (width < Layer.heightMin) width = Layer.heightMin;
            else if (width > Layer.heightMax) width = Layer.heightMax;

            StackPanel panel = new StackPanel()
            {
                Orientation = Orientation.Horizontal,
                Height = width * 1000
            };

            layerItem.Background = new SolidColorBrush(Colors.White);
            layerItem.BorderBrush = Brushes.White;
            layerItem.BorderThickness = new Thickness(1);

            TextBox inputWidth = new TextBox()
            {
                Text = this.Width.ToString(),
                Height = 20,
                Width = 50,
                Tag = this
            };

            inputWidth.TextChanged += inputWidth_TextChanged;

            TextBlock unitLabel = new TextBlock()
            {
                Text = "m",
                VerticalAlignment = System.Windows.VerticalAlignment.Center,
                Margin = new Thickness(5)
            };

            TextBlock title = new TextBlock()
            {
                Text = this.Material.Name,
                VerticalAlignment = System.Windows.VerticalAlignment.Center,
                Margin = new Thickness(5)
            };

            panel.Children.Add(inputWidth);
            panel.Children.Add(unitLabel);
            panel.Children.Add(title);
            panel.Margin = new Thickness(2);

            layerItem.Content = panel;

            return layerItem;

        }

        public UnitDouble<LCA.kgCFC11> DepletionOfOzoneLayer
        {
            get
            {
                return new UnitDouble<LCA.kgCFC11>(this.Material.DepletionOfOzoneLayer.Value * this.Width);
            }
        }

        public UnitDouble<LCA.kgSO2> Acidification
        {
            get
            {
                return new UnitDouble<LCA.kgSO2>(this.Material.Acidification.Value * this.Width);
            }
        }

        public UnitDouble<LCA.kgPhostphate> Eutrophication
        {
            get
            {
                return new UnitDouble<LCA.kgPhostphate>(this.Material.Eutrophication.Value * this.Width);
            }
        }

        public UnitDouble<LCA.kgNOx> FormationTroposphericOzone
        {
            get
            {
                return new UnitDouble<LCA.kgNOx>(this.Material.FormationTroposphericOzone.Value * this.Width);
            }
        }

        public UnitDouble<LCA.MJ> DepletionOfNonrenewbles
        {
            get
            {
                return new UnitDouble<LCA.MJ>(this.Material.DepletionOfNonrenewbles.Value * this.Width);
            }
        }

        public UnitDouble<LCA.CO2e> GlobalWarmingPotential
        {
            get
            {
                return new UnitDouble<LCA.CO2e>(this.Material.GlobalWarmingPotential.Value * this.Width);
            }
        }

        public static ContextMenu contextMenu;

        public static void BuildContextMenu()
        {
            contextMenu = new ContextMenu();
            MenuItem moveUp = new MenuItem() { Header = "move up" }; moveUp.Click += move_Click;
            MenuItem moveDown = new MenuItem() { Header = "move down" }; moveDown.Click += move_Click;
            MenuItem moveTop = new MenuItem() { Header = "move top" }; moveTop.Click += move_Click;
            MenuItem moveBottom = new MenuItem() { Header = "move bottom" }; moveBottom.Click += move_Click;

            contextMenu.Items.Add(moveUp);
            contextMenu.Items.Add(moveDown);
            contextMenu.Items.Add(moveTop);
            contextMenu.Items.Add(moveBottom);
        
        }



        static void move_Click(object sender, RoutedEventArgs e)
        {
            MenuItem mnu = (MenuItem)sender;
            ContextMenu m = (ContextMenu)mnu.Parent;
            ListViewItem item = (ListViewItem)m.PlacementTarget;
            ListView lv = (ListView)item.Parent;
            int index = lv.SelectedIndex;

            switch (mnu.Header.ToString())
            {
                case "move up":                   
                    if (index > 0)
                    {
                        lv.Items.RemoveAt(index);
                        lv.Items.Insert(index - 1, item);
                    }
                    break;

                case "move down":
                    if (index < lv.Items.Count - 1)
                    {
                        lv.Items.RemoveAt(index);
                        lv.Items.Insert(index + 1, item);
                    }
                    break;

                case "move top":
                    lv.Items.RemoveAt(lv.SelectedIndex);
                    lv.Items.Insert(0, item);
                    break;

                case "move bottom":
                    lv.Items.RemoveAt(lv.SelectedIndex);
                    lv.Items.Add(item);
                    break;

                default: break;
            }
            

            
        }

        void inputWidth_TextChanged(object sender, TextChangedEventArgs e)
        {

            TextBox textBox = (TextBox)sender;

            double value = Layer.heightMin;

            if (double.TryParse(textBox.Text, out value))
            {
                Layer layer = (Layer)textBox.Tag;
                layer.Width = value;


                if (value < Layer.heightMin) value = Layer.heightMin;
                else if (value > Layer.heightMax) value = Layer.heightMax;

                StackPanel panel = (StackPanel)textBox.Parent;
                panel.Height = value * 1000;


            }
        }

    }


}


