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


        private Tortuga.Types.Material GetMat(Mat ma, List<Tortuga.Types.LifecycleStage> Stages)
        {
            System.Net.WebClient client = new System.Net.WebClient();
            string data = client.DownloadString("http://www.oekobaudat.de/OEKOBAU.DAT/resource/processes/" + ma.ID + "?format=xml");

            XmlDocument xml = new XmlDocument();
            xml.LoadXml(data);

            XmlNamespaceManager manager = new XmlNamespaceManager(xml.NameTable);
            manager.AddNamespace("p", "http://lca.jrc.it/ILCD/Process");
            manager.AddNamespace("epd", "http://www.iai.kit.edu/EPD/2013");
            manager.AddNamespace("common", "http://lca.jrc.it/ILCD/Common");
            manager.AddNamespace("xsi", "http://www.w3.org/2001/XMLSchema-instance");

            Tortuga.Types.Material mat = new Tortuga.Types.Material() { Name = ma.Name, GUID = new Guid(ma.ID), Stages = Stages };
            mat.GlobalWarmingPotential = new UnitDouble<LCA.CO2e>(0);
            mat.Acidification = new UnitDouble<LCA.kgSO2>(0);
            mat.DepletionOfNonrenewbles = new UnitDouble<LCA.MJ>(0);
            mat.DepletionOfOzoneLayer = new UnitDouble<LCA.kgCFC11>(0);
            mat.Eutrophication = new UnitDouble<LCA.kgPhostphate>(0);
            mat.FormationTroposphericOzone = new UnitDouble<LCA.kgNOx>(0);


            XmlNodeList xnList = xml.SelectNodes("//p:LCIAResults", manager);
            foreach (XmlNode child in xnList[0].ChildNodes)
            {
                

                if (child.ChildNodes[0].ChildNodes.Count == 1)
                {
                    Dictionary<string, double> values = new Dictionary<string, double>();

                    string Name = child.ChildNodes[0].ChildNodes[0].InnerText;


                    foreach (XmlNode inner in child.ChildNodes)
                    {

                        if (inner.Name == "common:other")
                        {
                            foreach (XmlNode vals in inner.ChildNodes)
                            {
                                if (vals.Name == "epd:amount")
                                {
                                    values.Add(vals.Attributes[0].Value, double.Parse(vals.InnerText));

                                }
                            }

                        }




                        foreach (Tortuga.Types.LifecycleStage stage in Stages)
                        {

                            double actualVal = 0;

                            if (!values.ContainsKey(stage.Name))
                            {
                                if (stage.Name == "A1-A3")
                                {
                                    if (values.ContainsKey("A1")) actualVal += values["A1"];
                                    if (values.ContainsKey("A2")) actualVal += values["A2"];
                                    if (values.ContainsKey("A3")) actualVal += values["A3"];
                                }
                            }
                            else
                            {
                                actualVal = values[stage.Name];
                            }



                            if (Name.Contains("(GWP)"))
                                mat.GlobalWarmingPotential += new UnitDouble<LCA.CO2e>(actualVal);

                            if (Name.Contains("(AP)"))
                                mat.Acidification += new UnitDouble<LCA.kgSO2>(actualVal);

                            if (Name.Contains("(ADPF)"))
                                mat.DepletionOfNonrenewbles += new UnitDouble<LCA.MJ>(actualVal);

                            if (Name.Contains("(OPD)"))
                                mat.DepletionOfOzoneLayer += new UnitDouble<LCA.kgCFC11>(actualVal);

                            if (Name.Contains("(ADPE)"))
                                mat.Eutrophication += new UnitDouble<LCA.kgPhostphate>(actualVal);

                            if (Name.Contains("(POCP)"))
                                mat.FormationTroposphericOzone += new UnitDouble<LCA.kgNOx>(actualVal);
 

                            
                        }

                    }

                }
                

            }

            

            return mat;



        }

        private void Form1_Load(object sender, EventArgs e)
        {

            System.Net.WebClient client = new System.Net.WebClient();
            string data = client.DownloadString("http://www.oekobaudat.de/OEKOBAU.DAT/resource/processes");


            XmlDocument xml = new XmlDocument();
            xml.LoadXml(data);

            XmlNamespaceManager manager = new XmlNamespaceManager(xml.NameTable);
            manager.AddNamespace("p", "http://www.ilcd-network.org/ILCD/ServiceAPI/Process");
            manager.AddNamespace("f", "http://www.ilcd-network.org/ILCD/ServiceAPI/Flow");
            manager.AddNamespace("fp", "http://www.ilcd-network.org/ILCD/ServiceAPI/FlowProperty");
            manager.AddNamespace("u", "http://www.ilcd-network.org/ILCD/ServiceAPI/UnitGroup");
            manager.AddNamespace("l", "http://www.ilcd-network.org/ILCD/ServiceAPI/LCIAMethod");
            manager.AddNamespace("s", "http://www.ilcd-network.org/ILCD/ServiceAPI/Source");
            manager.AddNamespace("c", "http://www.ilcd-network.org/ILCD/ServiceAPI/Contact");
            manager.AddNamespace("sapi", "http://www.ilcd-network.org/ILCD/ServiceAPI");

            List<Mat> mats = new List<Mat>();


            XmlNodeList xnList = xml.SelectNodes("//sapi:name", manager);
            XmlNodeList xnList2 = xml.SelectNodes("//sapi:uuid", manager);
            for (int i = 0; i < xnList.Count; i++)
            {
                mats.Add(new Mat() { Name = xnList[i].InnerText, ID = xnList2[i].InnerText });

                // get data from url
                // http://www.oekobaudat.de/OEKOBAU.DAT/resource/processes/ee4fb7c2-6119-4f00-8cb6-fc74cb66fe9a?format=xml

            }

            List<LifecycleStage> ll = new List<LifecycleStage>();
            ll.Add(new LifecycleStage() { Name = "A1-A3" });


            GetMat(mats[5],ll);

            GetMat(mats[7], ll);

            //GetMat(mats[117], ll);

        }
    }

    public class Mat
    {
        public string Name;
        public string ID;
    }
}
