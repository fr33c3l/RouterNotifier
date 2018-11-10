using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RestSharp;
using System.Reflection;

namespace RouterNotifier
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.ShowInTaskbar = false;
            this.Hide();
            notifyIcon.Icon = new Icon("Icons/" + GetContext() + ".ico");

            notifyIcon.ContextMenuStrip = contextMenuStrip;
        }


        private int GetContext()
        {
            var client = new RestClient("http://192.168.1.1");
            client.CookieContainer = new System.Net.CookieContainer();
            var request = new RestRequest("/authenticate?username=admin&password=", Method.POST);
            IRestResponse response = client.Execute(request);
            var content = SimpleJson.DeserializeObject<RootContext>(response.Content);
            request = new RestRequest("/sysbus/NeMo/Intf/data:getMIBs", Method.POST);
            request.AddParameter("X-Context", content.data.contextID, ParameterType.HttpHeader);

            response = client.Execute(request);
            int oMycustomclassname = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(response.Content).status.ppp.ppp_data.LastChange;

            TimeSpan t = TimeSpan.FromSeconds(oMycustomclassname);

            return t.Hours;
        }



        public class Context
        {
            public string contextID { get; set; }
        }

        public class RootContext
        {
            public int status { get; set; }
            public Context data { get; set; }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            notifyIcon.Icon = new Icon("Icons/" + GetContext() + ".ico");
        }

        private void restartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var client = new RestClient("http://192.168.1.1");
            client.CookieContainer = new System.Net.CookieContainer();
            var request = new RestRequest("/authenticate?username=admin&password=", Method.POST);
            IRestResponse response = client.Execute(request);
            var content = SimpleJson.DeserializeObject<RootContext>(response.Content);
            request = new RestRequest("/sysbus/NMC:reboot", Method.POST);
            request.AddParameter("X-Context", content.data.contextID, ParameterType.HttpHeader);
            response = client.Execute(request);
        }
    }
}
