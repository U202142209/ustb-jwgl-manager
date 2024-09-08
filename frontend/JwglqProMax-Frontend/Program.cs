using System;
using System.Reflection;
using System.Windows.Forms;

namespace JwglqProMax_Frontend
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            // 解决dll引用依赖的问题
            // AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            // Application.Run(new Form1());
            Application.Run(new MainWindow());
        }

        /// <summary>
        /// 解决dll引用依赖的问题
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs e)
        {
            // JwglqProMax-Frontend\Resources
            // 项目的命名空间为myapp, 嵌入dll资源在libs文件夹下，所以这里用的命名空间为： myapp.libs.
            string _resName = "JwglqProMax_Frontend.Resources." + new AssemblyName(e.Name).Name + ".dll";
            using (var _stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(_resName))
            {
                byte[] _data = new byte[_stream.Length];
                _stream.Read(_data, 0, _data.Length);
                return Assembly.Load(_data);
            }
        }
    }
}
