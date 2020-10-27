using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ApiManagePrj
{
    public partial class Form2 : Form
    {
        public Form2(string apiPrjName, string apiClassName, string apiName, string apiParameterName, string servicesFileName)
        {
            InitializeComponent();

            // 组装providers的文字
            string providersApiText = "\t" + apiName + "(" + apiParameterName + "){\n\t\treturn HttpClient." + apiClassName + "." + apiName + "(" + apiParameterName + ");\n\t}";
            string providersText = apiClassName + ":{\n" + providersApiText + "\n}";

            // 展示providers的文字
            label1.Text = label1.Text + "         在  " + apiPrjName + ".js  里输入";
            textBox1.Text = providersText.Replace("\n", "\r\n").Replace("\t", "  ");

            // 组装services的文字
            string[] servicesApiPrjName = apiPrjName.Split('-');
            for(int i = 1;i < servicesApiPrjName.Length;i++)
            {
                servicesApiPrjName[i] = servicesApiPrjName[i].Substring(0, 1).ToUpper() + servicesApiPrjName[i].Substring(1);
            }
            string _servicesApiPrjName="";
            foreach (string item in servicesApiPrjName)
            {
                _servicesApiPrjName = _servicesApiPrjName + item;
            }
            string servicesApiText = 
                "const { " + apiClassName + " } = providers." + _servicesApiPrjName + ";\n"+
                "export function "+ apiName + "("+ apiParameterName + ") {\n"+
                "\treturn "+ apiClassName + "."+ apiName + "(" + apiParameterName + ");\n" + 
                "}";

            //展示services的文字
            label2.Text = label2.Text + "         在  " + servicesFileName.Replace(" ","") + ".js  里输入";
            textBox2.Text = servicesApiText.Replace("\n", "\r\n").Replace("\t","  ");

        }
    }
}
