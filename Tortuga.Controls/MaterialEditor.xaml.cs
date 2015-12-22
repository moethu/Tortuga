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


        public Material material;

        private ListView MaterialSelector;

        public System.Windows.Forms.Form ParentWindow;

        public Dictionary<string, Material> Materials;
     



        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            searchField.Text = "Search";
            searchField.Foreground = Brushes.Gray;
            

            MaterialSelector = new ListView();
            materialSelection.Children.Add(MaterialSelector);
            MaterialSelector.Background = Brushes.WhiteSmoke;
            MaterialSelector.BorderBrush = Brushes.WhiteSmoke;
            MaterialSelector.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
            MaterialSelector.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;



            foreach (Material material in Materials.Values.ToList())
            {
                ListViewItem lvi = material.Draw();
                int index = MaterialSelector.Items.Add(lvi);
                if (this.material != null)
                {
                    if (this.material.Name == material.Name) { MaterialSelector.SelectedIndex = index; }
                }

            }


            MaterialSelector.Height = 500;



            searchField.TextChanged += searchField_TextChanged;

            this.MouseDown += MaterialEditor_MouseDown;
        }

        void MaterialEditor_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Type type = this.ParentWindow.GetType();
            System.Reflection.MethodInfo meth = type.GetMethod("DragMove");

            if (e.ChangedButton == MouseButton.Left) meth.Invoke(this.ParentWindow, null);
        }



        private ListViewItem AddListViewItem(string name)
        {
            ListViewItem lvi = new ListViewItem();

            TextBlock tb = new TextBlock();
            tb.Text = name;

            lvi.Content = tb;

            return lvi;
        
        }


        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (searchField.Text == "Search")
            {
                searchField.Text = "";
                searchField.Foreground = Brushes.Black;
            }
        }

        private void searchField_LostFocus(object sender, RoutedEventArgs e)
        {
            if (searchField.Text == "")
            {
                searchField.Text = "Search";
                searchField.Foreground = Brushes.Gray;
            }
        }

        private void searchField_TextChanged(object sender, TextChangedEventArgs e)
        {
            MaterialSelector.Items.Clear();

            foreach (Material material in Materials.Values.ToList())
            {
                if (searchField.Text == "Search" || searchField.Text == "" || material.Name.ToLower().Contains(searchField.Text.ToLower()))
                    MaterialSelector.Items.Add(material.Draw());
            }
                
        }

        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            if (MaterialSelector.SelectedItem != null)
            {
                ListViewItem item = (ListViewItem)MaterialSelector.SelectedItem;
                this.material = (Types.Material)item.Tag;
            }


            ParentWindow.Close();
            
        }





    }
}
