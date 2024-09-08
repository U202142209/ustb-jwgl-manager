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
    public partial class EmailMessage : Form
    {
        public EmailMessage()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (this.textBox1.Text.Trim().Length == 0 ||
                !this.textBox1.Text.Contains("@"))
            {
                MessageBox.Show("请输入有效的邮箱地址");
                return;
            }
            // 发送邮箱信息
            Dictionary<string, string> data = new Dictionary<string, string>();
            data.Add("email", this.textBox1.Text.Trim());
            JObject res = JObject.Parse(NetWorkService.Post(Configration.BASICURL + "/sendEmailCode/", data));
            if ((int)res["code"] == 200)
            {
                // 发送成功
                this.button2.Enabled = true;
                this.textBox1.Enabled = false;
            }
            MessageBox.Show((string)res["msg"]);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            data.Add("email", this.textBox1.Text.Trim());
            data.Add("code", this.textBox2.Text.Trim());
            JObject res = JObject.Parse(NetWorkService.Post(Configration.BASICURL + "/verifyEmailCode/", data));
            MessageBox.Show((string)res["msg"]);
            if ((int)res["code"] == 200)
            {
                // 发送成功
                this.Close();
            }
        }

        private void EmailMessage_Load(object sender, EventArgs e)
        {
            // 初始参数设置
            this.button2.Enabled = false;
            // 获取用户邮箱
            this.textBox1.Text = Student.getEmailByRequest();
        }
    }
}
