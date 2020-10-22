using System;
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

namespace ApiManagePrj
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string projectPath = textBox1.Text;
            // string[] files = Directory.GetFiles(projectPath,"*.*", SearchOption.TopDirectoryOnly);
            string[] rootFolders = foreachFolders(projectPath);
            string[] apiFolders;
            foreach (string folder in rootFolders)
            {
                if (folder.EndsWith("node_modules"))
                {
                    apiFolders = foreachFolders(projectPath + @"\node_modules\@dataexa");
                    for (int i = 0; i < apiFolders.Length; i++)
                    {
                        int index = apiFolders[i].LastIndexOf(@"\");
                        apiFolders[i] = apiFolders[i].Substring(index + 1);
                        if (apiFolders[i].StartsWith("api"))
                        {
                            listBox1.Items.Add(apiFolders[i]);
                        }
                    }
                }
            }
        }

        public string[] foreachFolders(string path)
        {
            string[] folders = Directory.GetDirectories(path, "*.*", SearchOption.TopDirectoryOnly);
            return folders;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string apiListRead;
            string apiListToString;
            string apiFolderPath = textBox1.Text + @"\node_modules\@dataexa\" + listBox1.SelectedItem.ToString();
            //\src\client\index.ts
            string apiListFilePath = apiFolderPath + @"\src\client\index.ts";
            using (FileStream fs = File.Open(apiListFilePath, FileMode.Open))
            {
                StreamReader br = new StreamReader(fs, Encoding.UTF8);
                apiListRead = br.ReadToEnd(); //Replace("\n", "")
                apiListToString = apiListRead.Replace("\n","").Replace(" ", "").Replace("\t", "").Replace("\r", "");
                //Console.WriteLine(apiListToString);
            }
            string start = "import{";
            string end = "}from";
            Regex rg = new Regex("(?<=(" + start + "))[.\\s\\S]*?(?=(" + end + "))", RegexOptions.Multiline | RegexOptions.Singleline);
            Console.WriteLine("apis: " + rg.Match(apiListToString).Value);

        }
    }
}
