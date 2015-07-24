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

namespace Tortuga.Types
{
    public class Profile
    {
        public string Name;
        public double Area;
        public string Category;

        public Profile(string name, double area, string category)
        {
            this.Name = name;
            this.Area = area;
            this.Category = category;
        }

        public static List<Profile> Profiles;

        public static string profilePath = String.Format(@"{0}\Profiles", AssemblyDirectory);

        public static void LoadProfiles()
        {
            Profiles = new List<Profile>();

            foreach (string filename in System.IO.Directory.GetFiles(profilePath))
            {
                ReadFile(filename);
            }        
        }

        public static string AssemblyDirectory
        {
            get
            {
                string fullPath = System.Reflection.Assembly.GetAssembly(typeof(Profile)).Location;
                return System.IO.Path.GetDirectoryName(fullPath);
            }
        }


        private static void ReadFile(string path)
        {
            string category = System.IO.Path.GetFileNameWithoutExtension(path);

            foreach (string line in System.IO.File.ReadAllLines(path))
            { 
                string[] data = line.Split(':');
                if (data.Length == 2)
                {
                    double area = 0;
                    if (double.TryParse(data[1],out area))
                        Profiles.Add(new Profile(data[0], area, category));
                }
            }

        }

        public override string ToString()
        {
            return String.Format("Category:{0}, Profile:{1}, Area:{2}", new string[] { this.Category, this.Name, this.Area.ToString() });
        }
    }


}
