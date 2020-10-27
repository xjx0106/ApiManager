using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace ApiManagePrj
{
    public partial class Form1 : Form
    {
        public string projectPath = ""; // 前端项目的地址

        // 公共操作
        /// <summary>
        /// 遍历该路径下的文件夹
        /// </summary>
        /// <param name="path">要遍历的文件夹路径</param>
        /// <returns>返回一个string[]</returns>
        public string[] foreachFolders(string path)
        {
            try
            {
                string[] folders = Directory.GetDirectories(path, "*.*", SearchOption.TopDirectoryOnly);
                return folders;
            }
            catch
            {
                MessageBox.Show("没有找到该文件夹");
                return null;
            }
        }

        /// <summary>
        /// 读取文件流
        /// </summary>
        /// <param name="path">要读取的文件地址</param>
        /// <returns></returns>
        public string readTextFile(string path)
        {
            using (FileStream fs = File.Open(path, FileMode.Open))
            {
                StreamReader br = new StreamReader(fs, Encoding.UTF8);
                return br.ReadToEnd(); //Replace("\n", "")
                // Console.WriteLine(apiListToString);
            }
        }

        //================================================================================
        //================================================================================
        /// <summary>
        /// 窗口初始化
        /// </summary>
        public Form1()
        {
            InitializeComponent();
            projectPath = textBox1.Text; // 获取前端项目的地址
        }

        /// <summary>
        /// 点击按钮，解析前端项目，在listBox1添加所检索到的api项目名
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            // string[] files = Directory.GetFiles(projectPath,"*.*", SearchOption.TopDirectoryOnly);
            listBox1.Items.Clear(); // 清除api项目列表
            string[] rootFolders = foreachFolders(projectPath); // 遍历项目里的子文件夹（为了寻找出node_modules）
            foreach (string folder in rootFolders) // 遍历根目录找出node_modules文件夹
            {
                if (folder.EndsWith("node_modules")) // 选择出node_modules文件夹
                {
                    string[] folderInAtDataexa = foreachFolders(projectPath + @"\node_modules\@dataexa");
                    for (int i = 0; i < folderInAtDataexa.Length; i++) // 遍历node_modules\@dataexa
                    {
                        int index = folderInAtDataexa[i].LastIndexOf(@"\");
                        folderInAtDataexa[i] = folderInAtDataexa[i].Substring(index + 1);
                        if (folderInAtDataexa[i].StartsWith("api"))
                        {
                            listBox1.Items.Add(folderInAtDataexa[i]); // 检测出以api开头的文件夹，添加。
                        }
                    }
                    
                }
            }

            listBox4.Items.Clear();
            string[] providersFolders = Directory.GetFiles(projectPath + @"\src\providers", "*.js", SearchOption.TopDirectoryOnly);
            foreach (string item in providersFolders)
            {
                listBox4.Items.Add(item.Substring(item.LastIndexOf(@"\")+1));
            }

            listBox5.Items.Clear();
            string[] servicesFolders = Directory.GetFiles(projectPath + @"\src\services", "*.js", SearchOption.TopDirectoryOnly);
            foreach (string item in servicesFolders)
            {
                listBox5.Items.Add(item.Substring(item.LastIndexOf(@"\") + 1));
            }
        }

        /// <summary>
        /// 点击listBox1里的项目，遍历该Api项目的api类名
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string apiFolderPath = textBox1.Text + @"\node_modules\@dataexa\" + listBox1.SelectedItem.ToString();
            string apiClassListFilePath = apiFolderPath + @"\src\client\index.ts"; // 要读取Api类名的目标文件
            string apiClassListRead; // 所读取到的api项目里的包含api类名的那个ts文件，返回带有回车和tab的string
            string apiListClassToString; // 所读取到的api项目里的包含api类名的那个ts文件，去掉了回车空格之类的

            apiClassListRead = readTextFile(apiClassListFilePath);
            apiListClassToString = apiClassListRead.Replace("\n", "").Replace(" ", "").Replace("\t", "").Replace("\r", "");

            string start = "import{";
            string end = "}from";
            Regex rg = new Regex("(?<=(" + start + "))[.\\s\\S]*?(?=(" + end + "))", RegexOptions.Multiline | RegexOptions.Singleline);
            // Console.WriteLine("apis: " + rg.Match(apiListToString).Value);

            string[] apiClasses = rg.Match(apiListClassToString).Value.Split(','); // 将正则所匹配到的"WordApi,WordNetApi,StrategyApi,"匹配逗号分离开，存入apiClasses
            listBox2.Items.Clear();
            foreach (string item in apiClasses)
            {
                if (item != "")
                {
                    listBox2.Items.Add(item);
                }
            }

            string selected1 = listBox1.SelectedItem.ToString();
            int indexJump = -1;
            int indexToSelected = -1;
            foreach(var item in listBox4.Items)
            {
                Console.WriteLine(item);
                indexJump++;
                if(item.ToString().StartsWith(selected1))
                {
                    Console.WriteLine("Yeah");
                    indexToSelected = indexJump;
                }
            }
            listBox4.SelectedIndex = indexToSelected;
        }

        /// <summary>
        /// listBox2项被点击的事件触发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBox3.Items.Clear();
            listBox6.Items.Clear();
            string path = projectPath + @"\node_modules\@dataexa\"+ listBox1.SelectedItem + @"\src\api\api.ts"; 
            // Console.WriteLine(path);
            string apiListFileRead = readTextFile(path);

            // 正则的规则
            // pattern1 用于匹配出api.ts里的整个API类，匹配出来的东西里面包含了该Api类所有的api接口。
            // pattern2
            // pattern3 用于匹配出每个api接口名字。
            string[] pattern1 = { "" + listBox2.SelectedItem.ToString()+ " - axios parameter creator", listBox2.SelectedItem.ToString() + " - functional programming interface" }; // 用于识别WordApi
            string pattern2 = @"/\n\s*\b\w*\b[(]"; // \n\s*\b\w*\b[(] new Regex(@"\n\s*\b\w*\b[(]")
            string[] pattern3 = { "localVarPath = `", "`" }; // api的请求地址

            // 正则的声明
            Regex rg1 = new Regex("(?<=(\\s" + pattern1[0] + "))[.\\s\\S]*?(?=(" + pattern1[1] + "))", RegexOptions.Multiline | RegexOptions.Singleline);
            Regex rg2 = new Regex(pattern2, RegexOptions.Multiline);
            Regex rg3 = new Regex("(?<=(" + pattern3[0] + "))[.\\s\\S]*?(?=(" + pattern3[1] + "))", RegexOptions.Multiline | RegexOptions.Singleline);

            // 正则的使用
            string resultClass = rg1.Match(apiListFileRead).ToString(); // 匹配出整个api类
            string _resultClass = resultClass; // 复制api类用于操作
 
            while(rg2.IsMatch(_resultClass)) // 判断剩余的字符串里是否还有匹配的接口名
            {
                var apiNameGet = rg2.Match(_resultClass); // "/\n remove("              带有空格和回车的api名
                string apiName = apiNameGet.Value.Replace("\n", "").Replace(" ", "").Replace("\t", "").Replace("\r", ""); // "/remove("
                string _apiName = apiName.Substring(1, apiName.Length - 2); // "remove"
                listBox3.Items.Add(_apiName); // "remove"

                _resultClass = _resultClass.Substring(apiNameGet.Length + apiNameGet.Index);

                var apiPathGet = rg3.Match(_resultClass);
                string apiPath = apiPathGet.Value.Replace("\n", "").Replace(" ", "").Replace("\t", "").Replace("\r", ""); // "/remove("
                string _apiPath = apiPath.Substring(0); // "remove"
                listBox6.Items.Add(_apiPath); // "remove"
                Console.WriteLine(_apiPath);
            }
        }

        /// <summary>
        /// 项目地址输入框文字更变的事件触发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            projectPath = textBox1.Text;
        }

        /// <summary>
        /// 点击api名或者path名两边对等
        /// </summary>
        private void apiNameAndPathEqual(object sender, EventArgs e)
        {
            ListBox currentListbox = (ListBox)sender; // 注册事件触发
            if (currentListbox.Name == "listBox3") 
            {
                listBox6.SelectedIndex = listBox3.SelectedIndex;
            }else if (currentListbox.Name == "listBox6")
            {
                listBox3.SelectedIndex = listBox6.SelectedIndex;
            }
            textBox3.Text = "params";
            textBox2.Text = listBox6.SelectedItem.ToString();
            Regex rg = new Regex(@"{.*}", RegexOptions.Multiline|RegexOptions.Singleline);
            if (rg.IsMatch(textBox2.Text))
            {
                string res = rg.Match(textBox2.Text).Value;
                textBox3.Text = res.Substring(1,res.Length-2);
            }
        }

        /// <summary>
        /// 点击生成provider和services里的文字
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            if (listBox4.SelectedIndex == -1 || listBox5.SelectedIndex == -1) // 检测有没有选择对应的providers和services
            {
                MessageBox.Show("请选择Providers和Services");
                return;
            }
            string apiPrjName = listBox1.SelectedItem.ToString();
            string apiClassName = listBox2.SelectedItem.ToString();
            string apiName = listBox3.SelectedItem.ToString();
            string apiParameterName = textBox3.Text;
            string servicesFileName = listBox5.Text;




            new Form2(apiPrjName, apiClassName, apiName, apiParameterName, servicesFileName).Show();

        }
    }
}
