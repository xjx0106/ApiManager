using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
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
            string[] rootFolders = foreachFolders(projectPath); // 遍历项目里的子文件夹（为了寻找出node_modules）
            listBox1.Items.Clear(); // 清除api项目列表
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
        }

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

        }


        /// <summary>
        /// listBox2项被点击的事件触发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            string path = projectPath + @"\node_modules\@dataexa\"+ listBox1.SelectedItem + @"\src\api\api.ts"; 
            Console.WriteLine(path);
            string apiListFileRead = readTextFile(path);

            string pattren1 = listBox2.SelectedItem.ToString(); // WordApi
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
    }
}
