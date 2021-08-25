using Flurl.Http;
using ForkNgaWatcher.Model;
using Newtonsoft.Json;
using System.Text;

namespace ForkNgaWatcher
{
    class Watcher
    {
        private IDConfig _idConfig;


        public Watcher()
        {
            _idConfig = LoadDataFormFile();

            MultiWatch();
        }

        public void MultiWatch()
        {
            Console.WriteLine($"创建{_idConfig.WatchUIds.Count()}个监控线程");

            foreach (var watchid in _idConfig.WatchUIds)
            {
                Watch(watchid);
                Thread.Sleep(5000);
            }
        }

        public void Watch(string singleWatchID)
        {
            Console.WriteLine($"开始监控用户{singleWatchID}");

            int LastPostPid = 0;

            Task.Run(() =>
            {
                while (true)
                {
                    Thread.Sleep(30000);

                    var fc = new FlurlClient();

                    var resp = "https://bbs.nga.cn"
                    .WithClient(fc)
                    .AppendPathSegment("thread.php")
                    .SetQueryParams(new { authorid = singleWatchID, searchpost = "1", page = "1", lite = "js", noprefix = "" })
                    .WithCookies(new { ngaPassportUid = _idConfig.NgaPassportUID, ngaPassportCid = _idConfig.NgaPassportCID })
                    .GetStreamAsync().Result;

                    #region 注册GBK编码
                    System.Text.EncodingProvider provider = System.Text.CodePagesEncodingProvider.Instance;
                    Encoding.RegisterProvider(provider);
                    #endregion

                    var reader = new StreamReader(resp, Encoding.GetEncoding("gbk"));

                    var json = JsonConvert.DeserializeObject<Post>(reader.ReadToEnd());

                    try
                    {
                        string error = json.data.__T[0].error;

                        if (error != null)
                        {
                            Console.WriteLine($"服务器返回错误：{error}");
                        }
                    }
                    catch (Exception)
                    {

                    }

                    int pid = json.data.__T[0].__P.pid;

                    if (pid > LastPostPid)
                    {
                        string subject = json.data.__T[0].subject;
                        string content = json.data.__T[0].__P.content;
                        string tpcurl = "https://bbs.nga.cn/read.php?pid=" + json.data.__T[0].__P.pid + "&opt=128";
                        content = content + "\n" + tpcurl;
                        Console.WriteLine(subject);
                        Console.WriteLine(content);
                        SendMessageToSCT(subject, content);
                        LastPostPid = pid;
                    }

                }
            });
        }

        private IDConfig LoadDataFormFile()
        {
            if (!File.Exists("IDConfig"))
            {
                _idConfig = new IDConfig();
                //File.Create("IDConfig");
                File.WriteAllText("IDConfig", JsonConvert.SerializeObject(_idConfig));
                Console.WriteLine("已生成配置文件，请自行修改");
                Console.ReadLine();
                System.Environment.Exit(0);
                return null;
            }
            var _tempObj = JsonConvert.DeserializeObject<IDConfig>(File.ReadAllText("IDConfig"));
            if (_tempObj != null)
            {
                return _tempObj;
            }
            else
            {
                return new IDConfig();
            }
        }



        private void SendMessageToSCT(string subject, string content)
        {
            _ = "https://sctapi.ftqq.com"
                .WithClient(new FlurlClient())
                //将下面一行修改为自己的server酱的APIKey.send
                .AppendPathSegment("")
                .SetQueryParams(new { title = subject, desp = content })
                .GetAsync().Result;
        }
    }
}
