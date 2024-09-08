using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;

namespace JwglqProMax_Frontend
{
    public partial class Form1 : Form
    {
        private NetWorkService request = new NetWorkService();
        private DatabaseManager databaseManager;
        private bool isFirstShow = true;
        public Form1()
        {
            InitializeComponent();

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //允许线程间通信
            System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;
            // 数据库操作
            this.databaseManager = new DatabaseManager();
            // 当前软件版本
            this.lversion.Text = "当前软件版本:" + Configration.version;
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            // 检测自动更新
            try
            {
                JObject res = request.get(Configration.BASICURL + "/info/");
                // 初始化项目配置信息
                Configration.jx0502zbid = (string)res["CourseConfig"]["dqjx0502zbid"];
                this.Text = (string)res["info"]["name"];
                // 判断是否为最新版本
                if (Configration.version != (string)res["info"]["version"])
                {
                    string msg = $"当前软件版本:{Configration.version}\n目前的最新版本:{(string)res["info"]["version"]}\n\n新版本说明:{(string)res["info"]["desc"] }";
                    MessageBox.Show(msg, "发现新版本");
                }
                if (this.isFirstShow)
                {
                    this.isFirstShow = false;
                    // 使用sqlite读取数据库中的cookie信息并实现自动登录
                    LoginLog lastLoginLog = this.databaseManager.GetLatestLoginLog();
                    if (lastLoginLog == null)
                    {
                        return;// 没有读取到登录日志
                    }
                    Configration.Token = lastLoginLog.Token;
                    res = JObject.Parse(NetWorkService.Post(Configration.BASICURL + "/auto-login/"));
                    if ((int)res["code"] == 500)
                    {
                        return;// 登录失败
                    }
                    // 登录成功，设置登录的参数
                    Student.SetLoginParames(res);
                    // 打开新窗体
                    this.Close();
                    //new MainWindow().ShowDialog();
                    // this.Show();
                }
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
            }
        }

        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            //// 检测自动更新
            //try
            //{
            //    JObject res = request.get(Configration.BASICURL + "/info/");
            //    string msg = "软件信息\n\t名称：" + (string)res["info"]["name"] + "\n\t最新版本：" + (string)res["info"]["version"] + "\n\t当前版本：" + Configration.version + "\n\t备注：" + (string)res["info"]["desc"] + "\n\n作者信息\n\tauthor：" + (string)res["author"]["author"] + "\n\tcontact：" + (string)res["author"]["contact"] + "\n\tQQ：" + (string)res["author"]["qq"] + "\n\twx：" + (string)res["author"]["wx"] + "\n\tgithub：" + (string)res["author"]["github"] + "\n\tblog：" + (string)res["author"]["blog"] + "\n\n当前服务学期\n\t学期名称：" + (string)res["CourseConfig"]["dqjx0502zbid_name"] + "\n\t学期ID：" + (string)res["CourseConfig"]["dqjx0502zbid"];
            //    MessageBox.Show(msg);

            //}
            //catch (Exception error)
            //{
            //    MessageBox.Show(error.Message);
            //}
            string url = "https://www.yuque.com/zhao314159/rthvzo/zyzv233i7v28kcq4#aWj74";
            System.Diagnostics.Process.Start(url);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            QrcodeLogin q = new QrcodeLogin();
            this.Hide();
            q.ShowDialog();
            if (Configration.Logined == false)
                this.Show();
            else
                this.Close();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!Configration.Logined)
            {
                DialogResult result = MessageBox.Show("确定要退出程序吗，您还没有登录", "提示信息", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                if (result == DialogResult.OK)
                {
                    e.Cancel = false;
                    // 这是最彻底的退出方式，不管什么线程都被强制退出，把程序结束的很干净
                    // System.Environment.Exit(0);
                    // Application.Exit();
                    // this.Close();
                }
                else
                {
                    e.Cancel = true;
                }
            }
        }

        // 系统使用文档
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string url = "https://www.yuque.com/zhao314159/rthvzo/zyzv233i7v28kcq4#AUmts";
            System.Diagnostics.Process.Start(url);
        }

        // 用户隐私政策与服务条款
        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string url = "https://www.yuque.com/zhao314159/rthvzo/zyzv233i7v28kcq4#jD2f0";
            System.Diagnostics.Process.Start(url);
        }
    }
}
