using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tortuga.Types;
using System.Xml;

namespace WindowsFormsApplication1
{



    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Query()
        {
            Tortuga.Quartz.Query query = new Tortuga.Quartz.Query("*");
            System.Console.WriteLine(query.result.hits[50].document.data.environmental.lcaResults.inUse[0].name);


        }




        private void Form1_Load(object sender, EventArgs e)
        {
            Query();
        }


    }

    public class Mat
    {
        public string Name;
        public string ID;
    }
}
