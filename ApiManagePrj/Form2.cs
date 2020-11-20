using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Eventing.Reader;
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
        public string 
            apiPrjName, // "api-maya-resource"
            apiClassName, // "RulesLibraryApi"
            apiName, // "page"
            apiParameterName, // "id1,id2,id3"
            servicesFileName, // "rule-library   (NEW)"
            prjPath, // "C:\\Users\\xxxxxx\\xxxxxx\\xxxxxx\\xxxxxx\\xxxxxx\\rule-library"
            providersFileName, // "api-maya-resource.js"
            viewsName; // "rule-library"
        
        public string apiPrjNameUpper;

        public Form2(string _apiPrjName, string _apiClassName, string _apiName, string _apiParameterName, string _servicesFileName, string _prjPath, string _providersFileName, string _viewsName)
        {
            InitializeComponent();

            apiPrjName = _apiPrjName; // "api-maya-resource"
            apiClassName = _apiClassName; // "RulesLibraryApi"
            apiName = _apiName; // "page"
            apiParameterName = _apiParameterName; // "id,appName,appInfo"
            servicesFileName = _servicesFileName; // "rule-library   (NEW)"
            prjPath = _prjPath; // "C:\\Users\\xxxxxx\\xxxxxx\\xxxxxx\\xxxxxx\\xxxxxx\\rule-library"
            providersFileName = _providersFileName; // "api-maya-resource.js"
            viewsName = _viewsName; // "rule-library"

            string[] _apiPrjNameUpper = apiPrjName.Split('-');
            for(int i = 1;i < _apiPrjNameUpper.Length; i++)
            {
                _apiPrjNameUpper[i] = _apiPrjNameUpper[i].Substring(0, 1).ToUpper() + _apiPrjNameUpper[i].Substring(1);
            }
            foreach(string item in _apiPrjNameUpper)
            {
                apiPrjNameUpper = apiPrjNameUpper + item; // "apiMayaResource"
            }

            // 组装providers的文字（用于展示在textBox手动添加）
            string providersApiText = "\t" + apiName + "(" + apiParameterName + "){\n\t\treturn HttpClient." + apiClassName + "." + apiName + "(" + apiParameterName + ");\n\t}";
            string providersText = apiClassName + ":{\n" + providersApiText + "\n}";

            // 展示providers的文字
            label1.Text = label1.Text + "         在  " + apiPrjName + ".js  里输入";
            textBox1.Text = providersText.Replace("\n", "\r\n").Replace("\t", "  ");

            // 组装services的文字
            string servicesApiText =
                "import providers from \"@/providers\";\n" +
                "const { " + apiClassName + " } = providers." + apiPrjNameUpper + ";\n"+
                "export function "+ apiName + "("+ apiParameterName + ") {\n"+
                "\treturn "+ apiClassName + "."+ apiName + "(" + apiParameterName + ");\n" + 
                "}";

            //展示services的文字
            label2.Text = label2.Text + "         在  " + servicesFileName.Replace(" ","") + "  里输入";
            textBox2.Text = servicesApiText.Replace("\n", "\r\n").Replace("\t","  ");

            // 输入框不选中任何文字
            // textBox1.SelectionLength = 0;
            textBox1.Select(0, 0);
        }

        /// <summary>
        /// 将接口写入providers
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            string path = prjPath + @"\src\providers\" + providersFileName;

            // 没有该文件的情况
            if (File.Exists(path))
            {
                MessageBox.Show("存在文件");
                // File.Delete(dddd);//删除该文件
            } else {
                MessageBox.Show("不存在文件");
                File.Create(path);//创建该文件，如果路径文件夹不存在，则报错。
            }

            // 有该文件但是没有import的情况

            // 有该文件、有import，但是没有api类的情况

            // 有该文件、有import，有api类，但是没有方法的情况

            // 有该文件、有import，有api类，有方法的情况
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

            // 声明正则的规则
            string pattern1 = "import providers from \"@/providers\";";
            string pattern2 = "const { " + apiClassName + " } = providers." + apiPrjNameUpper + ";";
            string pattern3 = @"export function "+ apiName;

            // 声明正则
            Regex rg1 = new Regex(pattern1, RegexOptions.Multiline | RegexOptions.Singleline);
            Regex rg2 = new Regex(pattern2, RegexOptions.Multiline | RegexOptions.Singleline);
            // Regex rg3 = new Regex(@"function\s.*\(");
            Regex rg3 = new Regex(pattern3, RegexOptions.Singleline| RegexOptions.Multiline);

            // 匹配正则
            if (rg1.IsMatch(servicesRead))
            {
                Console.WriteLine("有import");
            } else
            {
                string servicesImport = "import providers from \"@/providers\";";
                Console.WriteLine("没有import，写入\n" + servicesImport);

            }
            if (rg2.IsMatch(servicesRead))
            {
                Console.WriteLine("有const");
            } else
            {
                string servicesConst = "const { " + apiClassName + " } = providers." + apiPrjNameUpper + ";";
                Console.WriteLine("没有const，写入" + servicesConst);

            }
            if (rg3.IsMatch(servicesRead))
            {
                Console.WriteLine("有接口");
            } else
            {
                string servicesApi =
                    "export function " + apiName + "(" + apiParameterName + ") {\n" +
                    "\treturn " + apiClassName + "." + apiName + "(" + apiParameterName + ");\n" +
                    "}";
                Console.WriteLine("没有接口，写入" + servicesApi);

            }


        }
    }
}
