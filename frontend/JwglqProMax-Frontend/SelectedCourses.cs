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
    public partial class SelectedCourses : Form
    {
        public SelectedCourses()
        {
            InitializeComponent();
        }

        private void SelectedCourses_Load(object sender, EventArgs e)
        {
            try
            {
                // 获取已经选择的课程信息
                JObject res = JObject.Parse(NetWorkService.Post(Configration.BASICURL + "/getSelectedClasses/"));
                if ((int)res["code"] == 200)
                {
                    // MessageBox.Show(res.ToString());
                }
                JArray selectedClasses = (JArray)res["data"];
                string[] tags = new string[] { "序号", "课程编号", "课程名称", "班级", "主讲教师", "上课时间", "上课地点", "课序号", "选课截止时间", "退课截止时间", "补选课截止时间", "课程性质", "素质拓展课程类别", "课程标签", "学年学期", "通知单编号", "学分", "修读类型", "选课状态", "优先选择" };
                for (int i = 0; i < tags.Length; i++)
                {
                    // 添加数据列
                    dataGridView1.Columns.Add(tags[i], tags[i]);
                }
                // 添加数据行
                foreach (JObject semester in selectedClasses)
                {
                    string[] content = new string[tags.Length];
                    for (int i = 0; i < tags.Length; i++)
                    {
                        content[i] = (string)semester[tags[i]];
                    }
                    dataGridView1.Rows.Add(content);
                }
                dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
                dataGridView1.DefaultCellStyle.WrapMode = DataGridViewTriState.True; // 设置自动换行
            }
            catch (Exception error)
            {
                MessageBox.Show("卧槽，程序出错了：" + error.Message);
            }
        }
    }
}
