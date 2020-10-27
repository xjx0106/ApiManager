using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ApiManagePrj
{
    public partial class Form2 : Form
    {
        public string apiPrjName,apiClassName,apiName,apiParameterName,servicesFileName,prjPath;
        public string apiPrjNameUpper;

        public Form2(string _apiPrjName, string _apiClassName, string _apiName, string _apiParameterName, string _servicesFileName, string _prjPath)
        {
            InitializeComponent();

            apiPrjName = _apiPrjName; // "api-maya-system"
            apiClassName = _apiClassName; // "BaseResourceAuthorityApi"
            apiName = _apiName; // "remove"
            apiParameterName = _apiParameterName; // "id"
            servicesFileName = _servicesFileName; // "element-management-api.js"
            prjPath = _prjPath; // "C:\\Users\\xxxxxx\\xxxxxx\\xxxxxx\\xxxxxx\\xxxxxx\\rule-library"

            string[] _apiPrjNameUpper = apiPrjName.Split('-');
            for(int i = 1;i < _apiPrjNameUpper.Length; i++)
            {
                _apiPrjNameUpper[i] = _apiPrjNameUpper[i].Substring(0, 1).ToUpper() + _apiPrjNameUpper[i].Substring(1);
            }
            foreach(string item in _apiPrjNameUpper)
            {
                apiPrjNameUpper = apiPrjNameUpper + item;
            }

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
            label2.Text = label2.Text + "         在  " + servicesFileName.Replace(" ","") + "  里输入";
            textBox2.Text = servicesApiText.Replace("\n", "\r\n").Replace("\t","  ");


        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 将该接口写入services
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            StreamReader sr = new StreamReader(prjPath + "\\src\\services\\" + servicesFileName);
            string servicesRead = sr.ReadToEnd();

            string pattern1 = "import providers from \"@/providers\";";
            string pattern2 = "const { " + apiClassName + " } = providers." + apiPrjNameUpper + ";";

            Regex rg1 = new Regex(pattern1, RegexOptions.Multiline | RegexOptions.Singleline);
            Regex rg2 = new Regex(pattern2, RegexOptions.Multiline | RegexOptions.Singleline);
            Regex rg3 = new Regex(@"function\s.*\(");
            if (rg1.IsMatch(servicesRead))
            {
                Console.WriteLine("有import");
            }
            if (rg2.IsMatch(servicesRead))
            {
                Console.WriteLine("有const");
            }
            if (true)
            {
                Console.WriteLine("有接口");
            }


        }
    }
}
