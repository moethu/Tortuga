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
using System.Windows.Media.Imaging;

namespace Tortuga.Types
{
    public static class Utilities
    {
        /// <summary>
        /// Get BitmapSource from Bitmap
        /// </summary>
        /// <param name="bitmap">Bitmap (eG. Resource)</param>
        /// <returns>BitmapSource</returns>
        public static BitmapSource GetSourceFromBitmap(System.Drawing.Bitmap bitmap)
        {
            System.Drawing.Size size = bitmap.Size;

            return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                bitmap.GetHbitmap(),
                IntPtr.Zero,
                System.Windows.Int32Rect.Empty,
                BitmapSizeOptions.FromWidthAndHeight(size.Width, size.Height)
                );
        }
    }
}
