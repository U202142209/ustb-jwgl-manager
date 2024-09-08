using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JwglqProMax_Frontend
{
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            this.textBox2.Text = Student.getEmailByRequest();
            this.textBox1.Text = Configration.interval.ToString();
            this.checkBox1.Checked = Configration.IsEmailMessageAlert;
            this.textBox3.Text = Configration.TIMEOUT.ToString();
            this.checkBox2.Checked = Configration.waitingtStop;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                Configration.IsEmailMessageAlert = checkBox1.Checked;
                // 时间间隔
                Configration.waitingtStop = checkBox2.Checked;
                Configration.interval = Convert.ToInt32(textBox1.Text);
                Configration.TIMEOUT = Convert.ToInt32(textBox3.Text);
                if (Configration.interval <= 500)
                {
                    DialogResult result = MessageBox.Show($"您当前设置的抢课间隔为：{textBox1.Text} 毫秒发起一次抢课\n设置的太小了，可能导致程序负载过大，建议设置不小于600毫秒\n\n点击确定按钮以继续您的操作", "确定", MessageBoxButtons.OKCancel);
                    if (result == DialogResult.OK)
                    {

                    }
                    else if (result == DialogResult.Cancel)
                    {
                        // 用户选择了取消按钮
                        return;
                    }
                }
                this.Close();
            }
            catch (Exception error)
            {
                MessageBox.Show("出错了：" + error.Message);
            }
        }
    }
}
