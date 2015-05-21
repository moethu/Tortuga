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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Tortuga.Types;

namespace Tortuga.Controls
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class MaterialEditor : UserControl
    {
        public MaterialEditor()
        {
            InitializeComponent();
        }

        private TextBlock infoScreen;
        public Assembly assembly;
        private ListView materialComposer;
        private ListView MaterialSelector;

        public string alternativeDataSourcePath;

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {

            if (System.IO.File.Exists(this.alternativeDataSourcePath))
            {
                string[] data = System.IO.File.ReadAllLines(this.alternativeDataSourcePath);

                foreach (string row in data)
                {
                    string[] fields = row.Split(';');

                    if (fields.Length == 7)
                    {
                        Tortuga.Types.Material material = new Material()
                        {
                            Name = fields[0],

                            GlobalWarmingPotential = new UnitDouble<Types.LCA.CO2e>(double.Parse(fields[1])),
                            Acidification = new UnitDouble<Types.LCA.kgSO2>(double.Parse(fields[2])),
                            DepletionOfNonrenewbles = new UnitDouble<Types.LCA.MJ>(double.Parse(fields[3])),
                            DepletionOfOzoneLayer = new UnitDouble<Types.LCA.kgCFC11>(double.Parse(fields[4])),
                            Eutrophication = new UnitDouble<Types.LCA.kgPhostphate>(double.Parse(fields[5])),
                            FormationTroposphericOzone = new UnitDouble<Types.LCA.kgNOx>(double.Parse(fields[6])),

                        };

                        MaterialSelector.Items.Add(material.Draw());
                    
                    }
                }
            
            }


            MaterialSelector = new ListView();
            materialSelection.Children.Add(MaterialSelector);
            MaterialSelector.Background = Brushes.WhiteSmoke;
            MaterialSelector.BorderBrush = Brushes.WhiteSmoke;


            Tortuga.Types.Material material1 = new Material()
            {
                Name = "TestMaterial",
                GlobalWarmingPotential = new UnitDouble<Types.LCA.CO2e>(1),
                Acidification = new UnitDouble<Types.LCA.kgSO2>(2),
                DepletionOfNonrenewbles = new UnitDouble<Types.LCA.MJ>(3),
                DepletionOfOzoneLayer = new UnitDouble<Types.LCA.kgCFC11>(4),
                Eutrophication = new UnitDouble<Types.LCA.kgPhostphate>(5),
                FormationTroposphericOzone = new UnitDouble<Types.LCA.kgNOx>(6)
            };



            MaterialSelector.Items.Add(material1.Draw());

            MaterialSelector.Height = 500;









            materialComposer = new ListView();
            materialAssemblyHost.Children.Add(materialComposer);
            materialComposer.Height = 500;
            
            materialComposer.SelectionChanged += materialComposer_SelectionChanged;
            materialComposer.Background = Brushes.WhiteSmoke;
            materialComposer.BorderBrush = Brushes.WhiteSmoke;

            infoScreen = new TextBlock();
            infoScreen.Margin = new Thickness(5);
            materialAssemblyHost.Children.Add(infoScreen);

            if (this.assembly == null) this.assembly = new Assembly();
            else this.assembly.Draw(this.materialComposer);


        }

        void materialComposer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            infoScreen.Text = String.Format("Width: {0} mm, {1} CO2e/m3 = {2} CO2e/m2", new object[] { assembly.Width, assembly.GlobalWarmingPotential.Value, assembly.GlobalWarmingPotential.Value * assembly.Width });
        }

        private ListViewItem AddListViewItem(string name)
        {
            ListViewItem lvi = new ListViewItem();

            TextBlock tb = new TextBlock();
            tb.Text = name;

            lvi.Content = tb;

            return lvi;
        
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (MaterialSelector.SelectedItem != null)
            {
                ListViewItem lvi = (ListViewItem)MaterialSelector.SelectedItem;
                Material mat = (Material)lvi.Tag;
                Layer lay = new Layer(mat);
                assembly.AddLayer(materialComposer, lay);

            }
            

            
        }





    }
}
