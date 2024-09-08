using System;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using System.Threading;

namespace JwglqProMax_Frontend
{
    public partial class QrcodeLogin : Form
    {
        public QrcodeLogin()
        {
            InitializeComponent();
        }
        private NetWorkService request = new NetWorkService();
        private Thread backgroundThread;

        private void RefreshQrCode()
        {
            try
            {
                this.timer1.Enabled = false;
                // 获取random token
                JObject res = JObject.Parse(NetWorkService.Post(Configration.BASICURL + "/randToken/"));

                Configration.Token = (string)res["data"];
                // 获取二维码
                res = JObject.Parse(NetWorkService.Post(Configration.BASICURL + "/glht/Logon.do/"));
                // 创建token全局对象
                RandowToken.CreateRandowToken(res);
                string imgUrl = Configration.JwSisBaseUrl + "/connect/qrimg?sid=" + RandowToken.sid;
                // 下载二维码图片
                this.pictureBox2.LoadAsync(imgUrl);
                // 开始检测扫描状态
                this.timer1.Enabled = true;
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
            }
        }

        // 刷新二维码
        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.RefreshQrCode();
        }

        private void QrcodeLogin_Load(object sender, EventArgs e)
        {

        }

        private void QrcodeLogin_Shown(object sender, EventArgs e)
        {
            // 刷新二维码
            this.RefreshQrCode();
        }

        // 判断扫码的情况
        private void checkQrcodeState()
        {
            JObject res = JObject.Parse(NetWorkService.Post(Configration.BASICURL + "/connect/state/", RandowToken.GetPostParams()));
            if ( (int)res["code"] == 500)
            {
                // MessageBox.Show(res.ToString());
                this.label1.Text = (string)res["msg"];
                return;
            }
            // 登录成功
            // 设置登录成功的参数
            int affectedRows= Student.SetLoginParames(res);
            MessageBox.Show("登录成功！\n\n\t姓名："+Student.name+"\n\t班级："+Student.personlnfo_class+"\n\t年级："+Student.personlnfo_grade+"\n\t专业："+Student.personlnfo_major);            
            if (affectedRows > 0)
            {
                // 进入新的窗体
                this.timer1.Enabled = false;
                this.Close();
                //this.Hide();
                //MainWindow m = new MainWindow();
                //m.ShowDialog();
                //Application.Exit();
            }
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            // 检查是否有任务正在执行
            if ((backgroundThread != null && backgroundThread.IsAlive))
                return; // 上一个周期的任务还未完成，直接返回
            // 创建一个后台线程执行任务
            backgroundThread = new Thread(checkQrcodeState);
            backgroundThread.IsBackground = true;
            backgroundThread.Start();
        }
    }
}
