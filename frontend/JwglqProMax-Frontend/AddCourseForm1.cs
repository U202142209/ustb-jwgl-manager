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
    public partial class AddCourseForm1 : Form
    {
        public AddCourseForm1()
        {
            InitializeComponent();
        }
        private bool prepared = false;
        private Course currentCourse = new Course();
        private void button1_Click(object sender, EventArgs e)
        {

            // 获取课程号
            string jx02id = textBox1.Text.Trim();
            if (prepared)
            {
                try
                {
                    // 添加课程
                    // DatabaseManager db = new DatabaseManager();
                    string msg="";
                    if (CourseTaskQueue.AddCourse(currentCourse, ref msg))
                    {
                        //if (db.AddCourseLog(currentCourse) < 0)
                        //{
                        //    MessageBox.Show("数据库新增数据失败；");
                        //}
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show(msg);
                    }

                }
                catch (ArgumentException err)
                {
                    MessageBox.Show(err.Message);
                }

                return;
            }
            else
            {
                Dictionary<string, string> data = new Dictionary<string, string>();
                data.Add("dqjx0502zbid", Configration.jx0502zbid);
                data.Add("jx02id", jx02id);
                JObject res = JObject.Parse(NetWorkService.Post(Configration.BASICURL + "/getClassDetail/", data));
                if ((int)res["code"] != 200)
                {
                    MessageBox.Show((string)res["msg"]);
                    return;
                }
                JArray classDetails = (JArray)res["data"];
                richTextBox1.Text = "当前匹配到的课程信息有：";
                foreach (JObject classDetail in classDetails)
                {
                    // richTextBox1.Text += classDetail.ToString();
                    richTextBox1.Text += "\n序号   :" + classDetail["序号"].ToString();
                    richTextBox1.Text += "\n课程编号:" + classDetail["课程编号"].ToString();
                    richTextBox1.Text += "\n通知单编号:" + classDetail["通知单编号"].ToString();
                    richTextBox1.Text += "\n课程名称:" + classDetail["课程名称"].ToString();
                    richTextBox1.Text += "\n主讲教师:" + classDetail["主讲教师"].ToString();
                    richTextBox1.Text += "\n限选人数:" + classDetail["限选人数"].ToString();
                    richTextBox1.Text += "\n课余量 :" + classDetail["课余量"].ToString();
                    richTextBox1.Text += "\n课程性质:" + classDetail["课程性质"].ToString();
                    richTextBox1.Text += "\n学分   :" + classDetail["学分"].ToString();
                    richTextBox1.Text += "\n选课开始时间:" + classDetail["选课开始时间"].ToString();
                    richTextBox1.Text += "\n上课时间:" + classDetail["上课时间"].ToString();
                    richTextBox1.Text += "\n上课地点:" + classDetail["上课地点"].ToString();
                    richTextBox1.Text += "\n退课截止日期:" + classDetail["退课截止日期"].ToString();
                    richTextBox1.Text += "\n补选课截止日期:" + classDetail["补选课截止日期"].ToString();
                    richTextBox1.Text += "\n学年学期    :" + classDetail["学年学期"].ToString();
                    richTextBox1.Text += "\n\n";

                    if (classDetail["通知单编号"].ToString() == textBox2.Text.Trim())
                    {
                        // 匹配成功
                        currentCourse = new Course(
                            classDetail["课程编号"].ToString(),
                            classDetail["通知单编号"].ToString(),
                            classDetail["课程名称"].ToString(),
                            classDetail["主讲教师"].ToString(),
                            classDetail["限选人数"].ToString(),
                            classDetail["课余量"].ToString(),
                            classDetail["课程性质"].ToString(),
                            classDetail["学分"].ToString());
                        prepared = true;
                        MessageBox.Show("课程信息核验通过，再次点击‘添加任务’按钮继续");
                        return;
                    }
                }
                DialogResult result = MessageBox.Show("未能找到与此课程编号匹配的通知单号，请检查是否输入正确或者使用智能填写", "使用智能填写", MessageBoxButtons.OKCancel);
                if (result == DialogResult.OK)
                {
                    // 用户选择了确定按钮,使用智能填写
                    JObject classDetail = (JObject)classDetails[0];
                    textBox2.Text = classDetail["通知单编号"].ToString();
                    richTextBox1.Text = "当前选择的课程信息如下：（点击‘添加任务’按钮继续）";

                    richTextBox1.Text += "\n\n课程编号:" + classDetail["课程编号"].ToString();
                    richTextBox1.Text += "\n通知单编号:" + classDetail["通知单编号"].ToString();
                    richTextBox1.Text += "\n课程名称:" + classDetail["课程名称"].ToString();
                    richTextBox1.Text += "\n主讲教师:" + classDetail["主讲教师"].ToString();
                    richTextBox1.Text += "\n限选人数:" + classDetail["限选人数"].ToString();
                    richTextBox1.Text += "\n课余量 :" + classDetail["课余量"].ToString();
                    richTextBox1.Text += "\n课程性质:" + classDetail["课程性质"].ToString();
                    richTextBox1.Text += "\n学分   :" + classDetail["学分"].ToString();
                    richTextBox1.Text += "\n选课开始时间:" + classDetail["选课开始时间"].ToString();
                    richTextBox1.Text += "\n上课时间:" + classDetail["上课时间"].ToString();
                    richTextBox1.Text += "\n上课地点:" + classDetail["上课地点"].ToString();
                    richTextBox1.Text += "\n退课截止日期:" + classDetail["退课截止日期"].ToString();
                    richTextBox1.Text += "\n补选课截止日期:" + classDetail["补选课截止日期"].ToString();
                    richTextBox1.Text += "\n学年学期    :" + classDetail["学年学期"].ToString();
                    richTextBox1.Text += "\n\n";
                    prepared = true;
                    // 
                    currentCourse = new Course(
    classDetail["课程编号"].ToString(),
    classDetail["通知单编号"].ToString(),
    classDetail["课程名称"].ToString(),
    classDetail["主讲教师"].ToString(),
    classDetail["限选人数"].ToString(),
    classDetail["课余量"].ToString(),
    classDetail["课程性质"].ToString(),
    classDetail["学分"].ToString());
                    MessageBox.Show("智能填写完毕，点击‘添加任务’按钮继续");
                }
                else if (result == DialogResult.Cancel)
                {
                    // 用户选择了取消按钮
                }
            }
        }

        private void AddCourseForm1_Load(object sender, EventArgs e)
        {
            textBox1.Text = "1099093";
            textBox2.Text = "202320242007105";
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            this.prepared = false;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            this.prepared = false;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MessageBox.Show("请添加微信：safeseaa 反馈问题");
        }
    }
}
