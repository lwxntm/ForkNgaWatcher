using Flurl.Http;
using Microsoft.Toolkit.Uwp.Notifications;
using System.Text;
using System.Windows.Forms;

namespace NGA_WATCH
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            //初始化界面
            InitializeComponent();

            //执行爬虫程序
            WatchDog();
        }
        /// <summary>
        /// 初始化，创建文件
        /// </summary>
        protected void init()
        {
            LoadUid();
            LoadCid();
            LoadPid();
            LoadWatchId();
        }

        #region 工具方法

        /// <summary>
        /// 从文件获取UID
        /// </summary>
        /// <returns></returns>
        protected String LoadUid()
        {
            if (!File.Exists("ngaPassportUid"))
            {
                FilePutContent("ngaPassportUid", "ngaPassportUid");
            }

            return FileGetContent("ngaPassportUid");
        }

        /// <summary>
        /// 从文件获取watchId
        /// </summary>
        /// <returns></returns>
        protected String LoadWatchId()
        {
            if (!File.Exists("watchId"))
            {
                FilePutContent("watchId", "42149372");
            }

            return FileGetContent("watchId");
        }

        /// <summary>
        /// 从文件获取CId
        /// </summary>
        /// <returns></returns>
        protected String LoadCid()
        {
            if (!File.Exists("ngaPassportCid"))
            {
                FilePutContent("ngaPassportCid", "ngaPassportCid");
            }

            return FileGetContent("ngaPassportCid");
        }

        /// <summary>
        /// 从文件获取Pid
        /// </summary>
        /// <returns></returns>
        protected String LoadPid()
        {
            if (!File.Exists("pid"))
            {
                PutPid("0");
            }
            return FileGetContent("pid");
        }
        /// <summary>
        /// 把Pid写入文件
        /// </summary>
        /// <param name="pid"></param>
        protected void PutPid(string pid)
        {
            FilePutContent("pid", pid);
        }

        #endregion

        protected void WatchDog()
        {
            init();

            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    FlurlClient fc = new FlurlClient();

                    var response = "https://bbs.nga.cn"
                    .WithClient(fc)
                    .AppendPathSegment("thread.php")
                    .SetQueryParams(new { authorid = LoadWatchId(), searchpost = "1", page = "1", lite = "js", noprefix = "" })
                    .WithCookies(new { ngaPassportUid = LoadUid(), ngaPassportCid = LoadCid() })
                    .GetStreamAsync().Result;

                    System.Text.EncodingProvider provider = System.Text.CodePagesEncodingProvider.Instance;
                    Encoding.RegisterProvider(provider);

                    var stream = new StreamReader(response, Encoding.GetEncoding("gbk"));

                    string json = stream.ReadToEnd();

                    Posts data = Newtonsoft.Json.JsonConvert.DeserializeObject<Posts>(json);

                    string error = data.data.__T[0].error;

                    if (error != null)
                    {
                        new ToastContentBuilder()
                       .AddText("NGA WATCH 错误提示")
                       .AddText(error)
                       .Show();
                    }

                    int pid = data.data.__T[0].__P.pid;

                    if (pid > Int32.Parse(LoadPid()))
                    {
                        string subject = data.data.__T[0].subject;
                        string content = data.data.__T[0].__P.content;
                        string tpcurl = "https://bbs.nga.cn/read.php?pid=" + data.data.__T[0].__P.pid + "&opt=128";

                        new ToastContentBuilder()
                       .AddText(subject)
                       .AddText(content)
                       .SetProtocolActivation(new Uri(tpcurl))
                       .Show();
                    }

                    FilePutContent("pid", data.data.__T[0].__P.pid.ToString());

                    Thread.Sleep(10000);
                }
            });
        }

        /// <summary>
        /// 把内容写入到文件里
        /// </summary>
        /// <param name="filename">文件名</param>
        /// <param name="contents">文件内容</param>
        /// <returns></returns>
        protected bool FilePutContent(string filename, string contents)
        {
            File.WriteAllText(filename, contents);
            return true;
        }

        /// <summary>
        /// 获取文件名里的内容
        /// </summary>
        /// <param name="filename">文件名</param>
        /// <returns></returns>
        protected string FileGetContent(string filename)
        {
            return File.ReadAllText(filename);
        }

        protected string GB2312ToUtf8(string gb2312String)
        {
            byte[] fromBytes = Encoding.GetEncoding("GB2312").GetBytes(gb2312String);
            byte[] toBytes = Encoding.Convert(Encoding.GetEncoding("GB2312"), Encoding.UTF8, fromBytes);

            string toString = Encoding.UTF8.GetString(toBytes);
            return toString;
        }
        //双击托盘图标退出程序
        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Application.Exit();
        }
    }
}
