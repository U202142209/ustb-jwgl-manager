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
    public partial class AddCourseForm2 : Form
    {
        public AddCourseForm2()
        {
            InitializeComponent();
        }

        private void AddCourseForm2_Load(object sender, EventArgs e)
        {
            // 初始化参数
            // 设置下拉选项
            string[] options1 = new string[] {
                "-请选择-",
                "星期一",
                "星期二",
                "星期三",
                "星期四",
                "星期五",
                "星期六",
                "星期日"
            };
            comboBox1.Items.AddRange(options1);
            comboBox1.SelectedIndex = 0;

            string[] options2 = new string[] {
                "-请选择-",
                "第一大节",
                "第二大节",
                "第三大节",
                "第四大节",
                "第五大节",
                "第六大节"
            };
            comboBox2.Items.AddRange(options2);
            comboBox2.SelectedIndex = 0;
            // 设置搜索状态不可用
            groupBox1.Enabled = false;
            // 默认获取必修课
            this.getCoursesBase("compulsory");
        }
        private JObject resCourses;

        private void RefreshDataGridView()
        {
            dataGridView1.Columns.Clear(); dataGridView1.Rows.Clear();
            dataGridView1.Columns.Add(new DataGridViewButtonColumn()
            {
                Name = "操作",
                HeaderText = "操作",
                Text = "添加",
                UseColumnTextForButtonValue = true
            });
            string[] tags = new string[] { "序号", "课程编号", "课程名称", "课程性质", "学时", "学分", "选课状态" };
            for (int i = 0; i < tags.Length; i++)
            {
                dataGridView1.Columns.Add(tags[i], tags[i]);
            }
            // 设置列宽度
            dataGridView1.Columns["操作"].Width = 45;
            dataGridView1.Columns["序号"].Width = 40;
            dataGridView1.Columns["课程编号"].Width = 80;
            dataGridView1.Columns["课程名称"].Width = 200;
            dataGridView1.Columns["课程性质"].Width = 80;
            dataGridView1.Columns["学时"].Width = 55;
            dataGridView1.Columns["学分"].Width = 55;
            dataGridView1.Columns["选课状态"].Width = 85;

            /// 遍历数据
            foreach (JObject classDetail in (JArray)this.resCourses["data"]["data"])
            {
                dataGridView1.Rows.Add(
                    "添加",
                    classDetail["序号"].ToString(),
                    classDetail["课程编号_1"].ToString(),
                    classDetail["课程名称"].ToString(),
                    classDetail["课程性质"].ToString(),
                    classDetail["学时"].ToString(),
                    classDetail["学分"].ToString(),
                    classDetail["选课状态"].ToString()
                    );
            }
            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dataGridView1.DefaultCellStyle.WrapMode = DataGridViewTriState.True; // 设置自动换行
        }

        private void getCoursesBase(string ctype)
        {
            // 素质拓展课 搜索框设置
            if (ctype == "qualityDevelopmentCourse")
                groupBox1.Enabled = true;
            else
                groupBox1.Enabled = false;
            Dictionary<string, string> data = new Dictionary<string, string>();
            data.Add("type", ctype);
            this.getCoursesBase(data);
        }

        private void getCoursesBase(Dictionary<string, string> data, string url = "/getCoursesByType/")
        {
            // 显示加载中提示
            this.Cursor = Cursors.WaitCursor;
            this.Enabled = false;
            // 获取数据
            try
            {
                // 执行需要的操作
                this.resCourses = JObject.Parse(NetWorkService.Post(Configration.BASICURL + url, data));
                if ((int)this.resCourses["code"] != 200)
                {
                    throw new Exception((string)this.resCourses["msg"]);
                }
                // 刷新数据
                this.RefreshDataGridView();
            }
            catch (Exception ex)
            {
                MessageBox.Show("发生错误：" + ex.Message);
            }
            // 加载完成后恢复窗体操作
            this.Cursor = Cursors.Default;
            this.Enabled = true;
        }
        private void 必修课ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.getCoursesBase("compulsory");
        }

        private void 专业选修课ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.getCoursesBase("professionalElectives");
        }

        private void 素质拓展课ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.getCoursesBase("qualityDevelopmentCourse");
        }

        private void 跨专业选课ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.getCoursesBase("interdisciplinaryCourseSelection");
        }

        private void 班级课程ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.getCoursesBase("classLessons");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // 解析请求的参数
            Dictionary<string, string> data = new Dictionary<string, string>();
            data.Add("type", "qualityDevelopmentCourse");
            data.Add("kcmc", this.textBox1.Text.Trim());// 关键字
            switch (comboBox1.SelectedItem.ToString())
            {
                case "-请选择-":
                    data.Add("xq", "");
                    break;
                case "星期一":
                    data.Add("xq", "1");
                    break;
                case "星期二":
                    data.Add("xq", "2");
                    break;
                case "星期三":
                    data.Add("xq", "3");
                    break;
                case "星期四":
                    data.Add("xq", "4");
                    break;
                case "星期五":
                    data.Add("xq", "5");
                    break;
                case "星期六":
                    data.Add("xq", "6");
                    break;
                case "星期日":
                    data.Add("xq", "7");
                    break;
            }
            switch (comboBox2.SelectedItem.ToString())
            {
                case "-请选择-":
                    data.Add("jc", "");
                    break;
                case "第一大节":
                    data.Add("jc", "0102");
                    break;
                case "第二大节":
                    data.Add("jc", "0304");
                    break;
                case "第三大节":
                    data.Add("jc", "0506");
                    break;
                case "第四大节":
                    data.Add("jc", "0708");
                    break;
                case "第五大节":
                    data.Add("jc", "0910");
                    break;
                case "第六大节":
                    data.Add("jc", "1112");
                    break;
            }
            if (this.checkBox1.Checked)
                data.Add("wmjt", "1");
            this.getCoursesBase(data, "/searchqualityDevelopmentCourse/");
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == dataGridView1.Columns["操作"].Index && e.RowIndex >= 0)
            {
                // 显示加载中提示
                this.Cursor = Cursors.WaitCursor;
                this.Enabled = false;
                // 获取数据
                try
                {
                    string cnumber = (string)dataGridView1.Rows[e.RowIndex].Cells["课程编号"].Value;
                    string name = (string)dataGridView1.Rows[e.RowIndex].Cells["课程名称"].Value;
                    DataGridViewCell cell = dataGridView1.Rows[e.RowIndex].Cells[7];
                    string text = cell.Value.ToString();
                    if (text == "已选")
                    {
                        throw new Exception("你已经选择了课程：" + name + " ,无法添加。");
                    }
                    // 获取课程详细信息
                    this.getCourseDetailByCNumber(cnumber);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                // 加载完成后恢复窗体操作
                this.Cursor = Cursors.Default;
                this.Enabled = true;
            }
        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            // 检查当前行的选课状态列的值
            if (e.ColumnIndex == 7) // 替换为选课状态列的索引
            {
                if (e.Value != null && e.Value.ToString() == "已选")
                {
                    // 设置行的背景颜色为浅绿色
                    dataGridView1.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.LightGreen;
                }
            }
        }

        private void getCourseDetailByCNumber(string jx02id)
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
            // 渲染数据
            dataGridView2.Columns.Clear(); dataGridView2.Rows.Clear();
            dataGridView2.Columns.Add(new DataGridViewButtonColumn()
            {
                Name = "操作",
                HeaderText = "操作",
                Text = "加入抢课队列",
                UseColumnTextForButtonValue = true
            });
            string[] tags = new string[] { "序号", "课程编号", "课程名称", "选课开始时间", "班级", "主讲教师", "上课时间", "上课地点", "课序号", "限选人数", "课余量", "退课截上日期", "补选课截止日期", "课程性质", "素质拓展课程类别", "通知单编号", "学年学期", "学分" };
            for (int i = 0; i < tags.Length; i++)
            {
                dataGridView2.Columns.Add(tags[i], tags[i]);
            }

            JArray classDetails = (JArray)res["data"];
            richTextBox1.Text = "当前匹配到的课程信息有：";
            foreach (JObject classDetail in classDetails)
            {
                dataGridView2.Rows.Add(
                    "加入抢课队列",
                    classDetail["序号"].ToString(),
                    classDetail["课程编号"].ToString(),
                    classDetail["课程名称"].ToString(),
                    classDetail["选课开始时间"].ToString(),
                    classDetail["班级"].ToString(),
                    classDetail["主讲教师"].ToString(),
                    classDetail["上课时间"].ToString(),
                    classDetail["上课地点"].ToString(),
                    classDetail["课序号"].ToString(),
                    classDetail["限选人数"].ToString(),
                    classDetail["课余量"].ToString(),
                    classDetail["退课截止日期"].ToString(),
                    classDetail["补选课截止日期"].ToString(),
                    classDetail["课程性质"].ToString(),
                    classDetail["素质拓展课程类别"].ToString(),
                    classDetail["通知单编号"].ToString(),
                    classDetail["学年学期"].ToString(),
                    classDetail["学分"].ToString()
                    );
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
            }
            dataGridView2.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dataGridView2.DefaultCellStyle.WrapMode = DataGridViewTriState.True; // 设置自动换行
        }

        private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == dataGridView2.Columns["操作"].Index && e.RowIndex >= 0)
            {
                // 显示加载中提示
                this.Cursor = Cursors.WaitCursor;
                this.Enabled = false;
                // 获取数据
                try
                {
                    // 添加课程
                    Course currentCourse = new Course(
    (string)dataGridView2.Rows[e.RowIndex].Cells["课程编号"].Value,
    (string)dataGridView2.Rows[e.RowIndex].Cells["通知单编号"].Value,
    (string)dataGridView2.Rows[e.RowIndex].Cells["课程名称"].Value,
    (string)dataGridView2.Rows[e.RowIndex].Cells["主讲教师"].Value,
    (string)dataGridView2.Rows[e.RowIndex].Cells["限选人数"].Value,
    (string)dataGridView2.Rows[e.RowIndex].Cells["课余量"].Value,
    (string)dataGridView2.Rows[e.RowIndex].Cells["课程性质"].Value,
    (string)dataGridView1.Rows[e.RowIndex].Cells["学分"].Value);
                    // 添加课程
                    string msg = "";
                    if (CourseTaskQueue.AddCourse(currentCourse, ref msg))
                    {
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show(msg);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                // 加载完成后恢复窗体操作
                this.Cursor = Cursors.Default;
                this.Enabled = true;
            }
        }

        private void 使用说明ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string url = "https://www.yuque.com/zhao314159/rthvzo/zyzv233i7v28kcq4";
            System.Diagnostics.Process.Start(url);
        }
    }
}
