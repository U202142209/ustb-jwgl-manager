using System;

using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;

namespace JwglqProMax_Frontend
{
    public partial class MainWindow : Form
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private DatabaseManager databaseManager;
        private NetWorkService request = new NetWorkService();
        private Thread backgroundThread;
        private bool hideCloseDialog = false;

        // 窗体关闭
        private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Configration.Logined && hideCloseDialog==false)
            {
                DialogResult result = MessageBox.Show("你确定要退出程序吗，退出后所有的抢课任务将被迫终止", "提示信息", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                if (result == DialogResult.OK)
                {
                    e.Cancel = false;
                    // 这是最彻底的退出方式，不管什么线程都被强制退出，把程序结束的很干净
                    // System.Environment.Exit(0);
                    // Application.Exit();
                }
                else
                {
                    e.Cancel = true;
                }
            }
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            // 窗体加载事件
            //允许线程间通信
            System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;
            // 数据库操作
            this.databaseManager = new DatabaseManager();
            //// 设置datagridview的行高
            //dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            //dataGridView1.RowTemplate.Height = 100;

            // 默认不开启抢课
            this.button3.Enabled = false;
            // 在窗体加载时将其最大化
            this.WindowState = FormWindowState.Maximized;
        }

        private void 使用课程编号添加ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void 我的已选课程ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.getSelectedCourses();
        }

        /// <summary>
        /// 查看已经选择的课程信息
        /// </summary>
        private void getSelectedCourses()
        {
            SelectedCourses sc = new SelectedCourses();
            sc.ShowDialog();
        }

        private void 数据中心ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(Configration.BASICURL + "/stu-info/" + Configration.Cookie);
        }

        private void button13_Click(object sender, EventArgs e)
        {
            this.getSelectedCourses();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            new AddCourseForm1().ShowDialog();
            // 刷新页面
            this.RefreshTaskQueue();
        }

        private void button12_Click(object sender, EventArgs e)
        {
            // 已选课程
            System.Diagnostics.Process.Start(Configration.BASICURL + "/stu-info/" + Configration.Cookie);
            // 数据中心
            // System.Diagnostics.Process.Start(Configration.BASICURL + "/stu-info/" + Configration.Cookie);
        }

        private void MainWindow_Shown(object sender, EventArgs e)
        {
            // 检查是否登录
            new Form1().ShowDialog();
            if (!Configration.Logined)
            {
                this.Close();
            }
            
            this.label1.Text = "姓名：" + Student.name + " 班级：" + Student.personlnfo_class + " 专业：" + Student.personlnfo_major;

            // 从数据库中获取数据并渲染
            this.getCourseLogFromDatabase();
            // 渲染
            this.RefreshTaskQueue();
        }

        /// <summary>
        /// 从数据库中获取数据并渲染
        /// </summary>
        private void getCourseLogFromDatabase()
        {
            Course[] cs = databaseManager.GetAllCourseLogs();
            CourseTaskQueue.ClearCourseDictionary();
            string msg = "";
            foreach (Course c in cs)
            {
                CourseTaskQueue.AddCourse(c, ref msg);
            }
        }

        /// <summary>
        /// 刷新抢课任务列表
        /// </summary>
        private void RefreshTaskQueue()
        {
            dataGridView1.Columns.Clear(); dataGridView1.Rows.Clear();
            dataGridView1.Columns.Add(new DataGridViewButtonColumn()
            {
                Name = "操作",
                HeaderText = "操作",
                Text = "移出抢课队列",
                UseColumnTextForButtonValue = true
            });
            string[] tags = new string[] { "序号", "课程号", "通知单编号", "课程名称", "主讲教师", "限选人数", "课余量", "课程性质", "学分" };
            for (int i = 0; i < tags.Length; i++)
            {
                dataGridView1.Columns.Add(tags[i], tags[i]);
            }
            // 设置列宽
            dataGridView1.Columns["操作"].Width = 100;
            dataGridView1.Columns["序号"].Width = 40;
            dataGridView1.Columns["课程号"].Width = 80;
            dataGridView1.Columns["通知单编号"].Width = 100;
            dataGridView1.Columns["课程名称"].Width = 160;
            dataGridView1.Columns["主讲教师"].Width = 85;
            dataGridView1.Columns["限选人数"].Width = 80;
            dataGridView1.Columns["课余量"].Width = 70;
            dataGridView1.Columns["课程性质"].Width = 80;
            dataGridView1.Columns["学分"].Width = 55;

            /// 遍历数据
            int idx = 1;
            foreach (Course c in CourseTaskQueue.getCoursesQueue().Values)
            {

                dataGridView1.Rows.Add(
                    "移出抢课队列",
                    (idx++).ToString(),
                    c.courseNumber,
                    c.NotificationNumber,
                    c.CourseName,
                    c.MainTeacher,
                    c.EnrollmentLimit,
                    c.CourseAvailability,
                    c.CourseType,
                    c.Credits
                    );
            }
            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dataGridView1.DefaultCellStyle.WrapMode = DataGridViewTriState.True; // 设置自动换行
        }

        private void button11_Click(object sender, EventArgs e)
        {
            this.RefreshTaskQueue();
        }

        // 移除抢课任务
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == dataGridView1.Columns["操作"].Index && e.RowIndex >= 0)
            {
                string NotificationNumber = (string)dataGridView1.Rows[e.RowIndex].Cells["通知单编号"].Value;
                string name = (string)dataGridView1.Rows[e.RowIndex].Cells["课程名称"].Value;
                DialogResult result = MessageBox.Show("你确定将课程: " + name + " 移出抢课队列吗？", "确定", MessageBoxButtons.OKCancel);
                if (result == DialogResult.OK)
                {
                    // 用户选择了确定按钮
                    // Deleting a course based on its notification number
                    string msg = "";
                    // 测试邮箱发送提醒功能，到时候设置为false
                    if (CourseTaskQueue.RemoveCourse(NotificationNumber, ref msg, false))
                    //  if (CourseTaskQueue.RemoveCourse(NotificationNumber, ref msg,true))
                    {
                        // 从数据库中删除
                        if (databaseManager.DeleteCourseLog(NotificationNumber) < 0)
                            MessageBox.Show("从数据库中删除失败...");
                        // 刷新页面
                        this.RefreshTaskQueue();
                        // 判断抢课队列是否为空
                        if (CourseTaskQueue.getCoursesQueue().Count == 0)
                        {
                            // 暂停 暂停抢课任务
                            return;
                        }
                    }
                    else
                    {
                        MessageBox.Show($"课程:{name}已移除抢课队列。");
                    }
                }
                else if (result == DialogResult.Cancel)
                {
                    // 用户选择了取消按钮
                }
            }
        }

        //开启抢课任务
        private void button4_Click(object sender, EventArgs e)
        {
            // 判断抢课列表是否为空
            if (CourseTaskQueue.Count() == 0)
            {
                MessageBox.Show("当前抢课任务队列为空，请添加任务再启动抢课。");
                return;
            }
            richTextBox1.Text += "开始抢课\n";
            timer1.Enabled = true;
            this.暂停抢课任务ToolStripMenuItem.Enabled = true;
            this.开启抢课任务ToolStripMenuItem.Enabled = false;
            this.button3.Enabled = true;
            this.button4.Enabled = false;
            this.添加任务ToolStripMenuItem.Enabled = false;
            this.设置ToolStripMenuItem.Enabled = false;
        }


        /// <summary>
        /// 抢课
        /// </summary>
        private void getClassWork()
        {
            Parallel.ForEach(CourseTaskQueue.getCoursesQueue(), kvp =>
            {
                try
                {
                    JObject res = request.getClass(kvp.Value.NotificationNumber, kvp.Value.courseNumber);
                    richTextBox1.Invoke(new Action(() =>
                    {
                        if (res["message"].ToString().Contains("选课成功"))
                        {
                            richTextBox1.Text += "\n " + "抢课成功!!!!!";
                            string msg="";
                            // 移除抢课任务
                            CourseTaskQueue.RemoveCourse(kvp.Value.NotificationNumber,ref msg,true);
                            // 从数据库中删除
                            if (databaseManager.DeleteCourseLog(kvp.Value.NotificationNumber) < 0)
                                MessageBox.Show("从数据库中删除失败...");
                            RefreshTaskQueue();// 刷新页面
                            // 判断抢课队列是否为空
                            if (CourseTaskQueue.Count() == 0)
                            {
                                // 暂停
                                button3.PerformClick();
                                return;
                            }
                        }
                        richTextBox1.Text += "\n " + kvp.Value.CourseName + "；" + res["message"];
                    }));
                }
                catch (Exception error)
                {
                    richTextBox1.Text += "\n抢课的过程中发生了错误：" + error.Message;
                }
            });
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            // 检查是否有任务正在执行
            if (Configration.waitingtStop && (backgroundThread != null && backgroundThread.IsAlive))
                return; // 上一个周期的任务还未完成，直接返回
            // 创建一个后台线程执行任务
            backgroundThread = new Thread(getClassWork);
            backgroundThread.IsBackground = true;
            backgroundThread.Start();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            richTextBox1.Text += "\n暂停抢课\n";
            timer1.Enabled = false;
            this.button3.Enabled = false;
            this.button4.Enabled = true;
            this.开启抢课任务ToolStripMenuItem.Enabled = true;
            this.暂停抢课任务ToolStripMenuItem.Enabled = false;
            this.添加任务ToolStripMenuItem.Enabled = true;
        }

        private void button14_Click(object sender, EventArgs e)
        {
            richTextBox1.Text = "";
        }
        // 从我的教务系统添加
        private void button1_Click(object sender, EventArgs e)
        {
            new AddCourseForm2().ShowDialog();
            this.RefreshTaskQueue();
        }

        // 保存抢课日志
        private void button6_Click(object sender, EventArgs e)
        {
            try
            {
                FolderBrowserDialog dialog = new FolderBrowserDialog();
                dialog.Description = "请选择文件需要保存的文件夹";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string savePath = dialog.SelectedPath + "\\" + DateTime.Now.ToString("yyyyMMddHHmmssffff") + ".txt";
                    System.IO.File.WriteAllText(savePath, richTextBox1.Text + "\n\n\n" + DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss"));
                    MessageBox.Show("抢课日志已保存至：\n：" + savePath);
                }
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
            }
        }
        
        // 开通消息提醒
        private void button5_Click(object sender, EventArgs e)
        {
            new EmailMessage().ShowDialog();
        }

        // 抢课参数设置button7
        private void button7_Click(object sender, EventArgs e)
        {
            new SettingsForm().ShowDialog();
            // 设置抢课时间间隔
            this.timer1.Interval = Configration.interval;
        }

        private void 设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            button7.PerformClick();
        }

        private void 退出系统ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        // 系统使用说明文档
        private void button10_Click(object sender, EventArgs e)
        {
            string url = "https://www.yuque.com/zhao314159/rthvzo/zyzv233i7v28kcq4";
            System.Diagnostics.Process.Start(url);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            // https://www.yuque.com/zhao314159/rthvzo/zyzv233i7v28kcq4#ZHv8x
            MessageBox.Show("请添加微信：safeseaa 联系技术支持");
        }

        // 反馈问题
        private void button8_Click(object sender, EventArgs e)
        {
            string url = "https://www.yuque.com/zhao314159/rthvzo/zyzv233i7v28kcq4#aWj74";
            System.Diagnostics.Process.Start(url);
        }

        private void 保存抢课日志ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            button6.PerformClick();
        }

        private void 开通消息提醒ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new EmailMessage().ShowDialog();
        }

        private void 抢课参数设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.button7.PerformClick();
        }

        private void 退出登录ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("你确定退出登录吗？将删除所有的缓存数据", "确定", MessageBoxButtons.OKCancel);
            if (result == DialogResult.OK)
            {
                this.hideCloseDialog = true;
                // 清理登录缓存
                databaseManager.DeleteAllLoginLog();
                Application.Exit();
            }
        }

        private void 系统使用说明ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            button10.PerformClick();
        }

        private void 联系技术支持ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            button9.PerformClick();
        }

        private void 反馈问题ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            button8.PerformClick();
        }
    }
}
