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
        public string[] apiPrjName; // listBox1
        public string[] viewsName; // listBox7
        public string[] providersName; // listBox4
        public string[] servicesName; // listBox5
        public int listBox4SelectedIndex = -1;
        public int listBox5SelectedIndex = -1;

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

            // 输入框不选中任何文字
            textBox1.Select(0, 0);
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
        /// 点击按钮，解析前端项目，在listBox1添加所检索到的api项目名
        /// </summary>
        /// <summary>
        /// 原理是通过检索\node_modules\@dataexa里的api开头的文件夹。来检测api项目（的名字和数量）。
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

            // 在liztBox4添加所扫描到的providers里的js文件
            listBox4.Items.Clear();
            string[] providersFolders;
            try
            {
                providersFolders = Directory.GetFiles(projectPath + @"\src\providers", "*.js", SearchOption.TopDirectoryOnly);
            } catch
            {
                return;
            }
            foreach (string item in providersFolders)
            {
                listBox4.Items.Add(item.Substring(item.LastIndexOf(@"\")+1));
            }

            // 在listBox5添加所扫描到的services里的js文件
            listBox5.Items.Clear();
            string[] servicesFolders = Directory.GetFiles(projectPath + @"\src\services", "*.js", SearchOption.TopDirectoryOnly);
            foreach (string item in servicesFolders)
            {
                listBox5.Items.Add(item.Substring(item.LastIndexOf(@"\") + 1));
            }

            // 在listBox7添加所扫描到的views里的js文件
            listBox7.Items.Clear();
            string[] viewsFolders = foreachFolders(projectPath + @"\src\views");
            foreach (string item in viewsFolders)
            {
                listBox7.Items.Add(item.Substring(item.LastIndexOf(@"\") + 1));
            }

            // node-modules 和 providers联动
            apiPrjName = new string[listBox1.Items.Count];
            providersName = new string[listBox4.Items.Count + 1];
            for (int i = 0; i < listBox1.Items.Count; i++)
            {
                apiPrjName[i] = listBox1.Items[i].ToString();
            }
            for (int i = 0; i < listBox4.Items.Count; i++)
            {
                providersName[i] = listBox4.Items[i].ToString();
            }

            // views 和 services联动
            viewsName = new string[listBox7.Items.Count];
            servicesName = new string[listBox5.Items.Count + 1];
            for (int i = 0; i < listBox7.Items.Count; i++)
            {
                viewsName[i] = listBox7.Items[i].ToString();
            }
            for (int i = 0; i < listBox5.Items.Count; i++)
            {
                servicesName[i] = listBox5.Items[i].ToString();
            }
        }

        /// <summary>
        /// 点击listBox1里的项目，遍历该Api项目的api类名
        /// </summary>
        /// <summary>
        /// 拼接路径，匹配.../src/client/index.ts里面的import {} from 中中括号里的东西，这是api类名，将这些api类名加入listbox2里。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem == null)
            {
                return;
            }
            string apiFolderPath = textBox1.Text + @"\node_modules\@dataexa\" + listBox1.SelectedItem.ToString(); // node_modules里的@dataexa里的api-maya-resource
            string apiClassListFilePath = apiFolderPath + @"\src\client\index.ts"; // 要读取Api类名的目标文件
            string apiClassListRead; // 所读取到的api项目里的包含api类名的那个ts文件，返回带有回车和tab的string
            string apiListClassToString; // apiClassListRead 去掉了回车空格之类的

            apiClassListRead = readTextFile(apiClassListFilePath);
            apiListClassToString = apiClassListRead.Replace("\n", "").Replace(" ", "").Replace("\t", "").Replace("\r", "");

            string start = "import{";
            string end = "}from";
            Regex rg = new Regex("(?<=(" + start + "))[.\\s\\S]*?(?=(" + end + "))", RegexOptions.Multiline | RegexOptions.Singleline); // 匹配到了很多个逗号连在一起的api类名
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
            /*
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
            
             */
            string selectedItemOfListbox1 = listBox1.SelectedItem.ToString(); // api-maya-resources

            providersName[providersName.Length - 1] = "";
            
            for (int i = 0;i < providersName.Length;i++)
            {
                if(listBox4.Items.Count == providersName.Length)
                {
                    listBox4.Items.RemoveAt(providersName.Length-1);
                }
                if (providersName[i].StartsWith(selectedItemOfListbox1))
                {
                    Console.WriteLine("有匹配");
                    listBox4.SelectedIndex = i;
                    break;
                }else
                {
                    listBox4.SelectedIndex = -1;
                }
                // 到了最后一个的时候
                if(i == providersName.Length - 1)
                {
                    if(listBox4.Items.Count == i)
                    {
                        listBox4.Items.Add(selectedItemOfListbox1 + "   (NEW)");
                    } else if (listBox4.Items.Count == providersName.Length)
                    {
                        listBox4.Items[providersName.Length-1] = selectedItemOfListbox1 + "   (NEW)";
                    }
                    listBox4.SelectedIndex = providersName.Length - 1;
                }
                
            }
            listBox4SelectedIndex = listBox4.SelectedIndex;


        }

        /// <summary>
        /// listBox2项被点击的事件触发
        /// </summary>
        /// <summary>
        /// 通过listbox1所选中的api项目，去该项目的\src\api\api.ts里，拿lsitbox2（api类名）去匹配该api类，找到里面的所有方法。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(listBox2.SelectedItem == null)
            {
                return;
            }
            textBox2.Text = "";
            listBox3.Items.Clear();
            listBox6.Items.Clear();
            listBox8.Items.Clear();
            listBox9.Items.Clear();
            string path = projectPath + @"\node_modules\@dataexa\"+ listBox1.SelectedItem + @"\src\api\api.ts"; 
            // Console.WriteLine(path);
            string apiListFileRead = readTextFile(path);

            // 正则的规则
            // pattern1 用于匹配出api.ts里的整个API类，匹配出来的东西里面包含了该Api类所有的api接口。
            // pattern2 用于匹配出每个api的方法名
            // pattern3 用于匹配出每个api的请求地址
            string[] pattern1 = { "" + listBox2.SelectedItem.ToString()+ " - axios parameter creator", listBox2.SelectedItem.ToString() + " - functional programming interface" }; // 用于识别WordApi
            string pattern2 = @"/\n\s*\b\w*\b[(]"; // \n\s*\b\w*\b[(] new Regex(@"\n\s*\b\w*\b[(]")
            string[] pattern3 = { "localVarPath = `", "`" }; // api的请求地址

            // 正则的声明
            Regex rg1 = new Regex("(?<=(\\s" + pattern1[0] + "))[.\\s\\S]*?(?=(" + pattern1[1] + "))", RegexOptions.Multiline | RegexOptions.Singleline);
            Regex rg2 = new Regex(pattern2, RegexOptions.Multiline);
            Regex rg3 = new Regex(".*[)]", RegexOptions.Multiline | RegexOptions.Singleline);
            Regex rg4 = new Regex("(?<=(" + pattern3[0] + "))[.\\s\\S]*?(?=(" + pattern3[1] + "))", RegexOptions.Multiline | RegexOptions.Singleline);
            
            // 正则的使用
            string resultClass = rg1.Match(apiListFileRead).ToString(); // 匹配出整个api类
            string _resultClass = resultClass; // 复制api类用于操作
 
            while(rg2.IsMatch(_resultClass)) // 判断剩余的字符串里是否还有匹配的接口名
            {
                // 匹配出首个api接口名，添加进listbox3
                var apiNameGet = rg2.Match(_resultClass); // "/\n remove(" 带有空格和回车的api名
                string apiName = apiNameGet.Value.Replace("\n", "").Replace(" ", "").Replace("\t", "").Replace("\r", ""); // "/remove("
                string _apiName = apiName.Substring(1, apiName.Length - 2); // "remove"
                listBox3.Items.Add(_apiName); // "remove"

                _resultClass = _resultClass.Substring(apiNameGet.Length + apiNameGet.Index); // 删除掉已经用掉的一届（截止到方法名）

                // Console.WriteLine(_resultClass.IndexOf(")"));
                string originParamGet = _resultClass.Substring(0, _resultClass.IndexOf(")"));
                Console.WriteLine(originParamGet);
                listBox8.Items.Add(originParamGet);

                var apiPathGet = rg4.Match(_resultClass);
                string apiPath = apiPathGet.Value.Replace("\n", "").Replace(" ", "").Replace("\t", "").Replace("\r", ""); // "/remove("
                string _apiPath = apiPath.Substring(0); // "remove"
                listBox6.Items.Add(_apiPath); // "remove"
                // Console.WriteLine(_apiPath);
            }
        }

        /// <summary>
        /// listBox3或listBox6点击
        /// </summary>
        /// <summary>
        /// 点击api名或者path名两边对等
        /// </summary>
        private void apiNameAndPathEqual(object sender, EventArgs e)
        {
            // 同步两边的选项
            ListBox currentListbox = (ListBox)sender; // 注册事件触发
            if (currentListbox.Name == "listBox3") 
            {
                if (listBox3.SelectedItem == null)
                {
                    return;
                }
                listBox6.SelectedIndex = listBox3.SelectedIndex;
            }else if (currentListbox.Name == "listBox6")
            {
                if (listBox6.SelectedItem == null)
                {
                    return;
                }
                listBox3.SelectedIndex = listBox6.SelectedIndex;
            }
            textBox2.Text = listBox6.SelectedItem.ToString(); // 存放当前请求路径的textBox

            string oriParams = listBox8.Items[listBox3.SelectedIndex].ToString().Replace(" ", "").Replace("?", ""); // propKey:string,rid:number,options:any={}
            string[] oriParamsSplit = oriParams.Split(','); // ["propKey:string", "rid:number", "options:any={}"]\
            // 这个for循环用于将上面一行的冒号和冒号后面的类型去掉
            for (int i = 0; i < oriParamsSplit.Length; i++)
            {
                int ooindex = oriParamsSplit[i].IndexOf(":");
                oriParamsSplit[i] = oriParamsSplit[i].Substring(0, ooindex); // { "id", "wordUpdateDTO", "options" }
            }
            listBox9.Items.Clear(); // 清除存放请求参数的listBox9
            bool hasParam = false;
            foreach(string item in oriParamsSplit)
            {
                if(item.EndsWith("DTO") && hasParam == false) // 把DTO转化为params
                {
                    listBox9.Items.Add(item);
                    hasParam = true;
                }
                else if(item.StartsWith("option")) // 跳过options
                {
                    continue;
                }
                else
                {
                    listBox9.Items.Add(item);
                }
            }
        }

        /// <summary>
        /// 点击生成provider和services里的文字
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex == -1)
            {
                label11.Text = "没选择项目名\n（没选择Providers）";
                tipChooseComplete();
                return;
            }
            else if (listBox2.SelectedIndex == -1)
            {
                label11.Text = "没选择api类名";
                tipChooseComplete();
                return;
            }
            else if (listBox3.SelectedIndex == -1)
            {
                label11.Text = "没选择api接口名";
                tipChooseComplete();
                return;
            }
            else if (listBox7.SelectedIndex == -1)
            {
                label11.Text = "没选择页面名（没选择Services）";
                tipChooseComplete();
                return;
            }

            string apiPrjNameToForm2 = listBox1.SelectedItem.ToString(); // "api-maya-resource"
            string apiClassNameToForm2 = listBox2.SelectedItem.ToString(); // "RulesLibraryApi"
            string apiNameToForm2 = listBox3.SelectedItem.ToString(); // "page"
            string apiParameterNameToForm2 = ""; // "id1,id2,id3"
            string providersFileNameToForm2 = listBox4.Text; // "api-maya-resource.js"
            string servicesFileNameToForm2 = listBox5.Text; // "rule-library-api.js"
            string viewsNameToForm2 = listBox7.Text; // "rule-library"
            string prjPathToForm2 = textBox1.Text; // C:\\Users\\xxxxxx\\xxxxxxx\\rule-library

            foreach(string item in listBox9.Items)
            {
                apiParameterNameToForm2 += item + ",";
            }
            apiParameterNameToForm2 = apiParameterNameToForm2.Substring(0, apiParameterNameToForm2.Length-1);
            new Form2(
                apiPrjNameToForm2,
                apiClassNameToForm2,
                apiNameToForm2,
                apiParameterNameToForm2,
                servicesFileNameToForm2,
                prjPathToForm2,
                providersFileNameToForm2,
                viewsNameToForm2
            ).Show();
            
        }

        /// <summary>
        /// 选择页面的名字
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listBox7_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox7.SelectedItem == null)
            {
                return;
            }

            string selectedItemOfListbox2 = listBox7.SelectedItem.ToString(); // api-maya-resources

            servicesName[servicesName.Length - 1] = "";

            for (int i = 0; i < servicesName.Length; i++)
            {
                if (listBox5.Items.Count == servicesName.Length)
                {
                    listBox5.Items.RemoveAt(servicesName.Length - 1);
                }
                if (servicesName[i].StartsWith(selectedItemOfListbox2))
                {
                    Console.WriteLine("有匹配");
                    listBox5.SelectedIndex = i;
                    break;
                }
                else
                {
                    listBox5.SelectedIndex = -1;
                }
                // 到了最后一个的时候
                if (i == servicesName.Length - 1)
                {
                    if (listBox5.Items.Count == i)
                    {
                        listBox5.Items.Add(selectedItemOfListbox2 + "   (NEW)");
                    }
                    else if (listBox5.Items.Count == servicesName.Length)
                    {
                        listBox5.Items[servicesName.Length - 1] = selectedItemOfListbox2 + "   (NEW)";
                    }
                    listBox5.SelectedIndex = servicesName.Length - 1;

                }

            }
            listBox5SelectedIndex = listBox5.SelectedIndex;
        }

        /// <summary>
        /// listbox4(providers)被点击了
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listBox4_Click(object sender, EventArgs e)
        {
            listBox4.SelectedIndex = listBox4SelectedIndex;
            tipOnlyLook();
        }

        /// <summary>
        /// listbox5(services)被点击了
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listBox5_Click(object sender, EventArgs e)
        {
            listBox5.SelectedIndex = listBox5SelectedIndex;
            tipOnlyLook();
        }


        public void tipOnlyLook()
        {
            timer1.Start();
            label10.Visible = true;
        }

        public void tipChooseComplete()
        {
            timer1.Start();
            label11.Visible = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            label10.Visible = false;
            label11.Visible = false;
            timer1.Stop();
        }
    }
}
