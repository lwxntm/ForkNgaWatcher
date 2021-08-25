namespace ForkNgaWatcher.Model
{
    public class Post
    {


        public Data data { get; set; }
        public string encode { get; set; }
        public int time { get; set; }


        public class Data
        {
            public __CU __CU { get; set; }
            public __GLOBAL __GLOBAL { get; set; }
            public string __ROWS { get; set; }
            public Dictionary<int, DirectionaryT> __T { get; set; }
            public int __T__ROWS { get; set; }
            public int __T__ROWS_PAGE { get; set; }
            public int __R__ROWS_PAGE { get; set; }
            public __F __F { get; set; }
        }

        public class __CU
        {
            public int uid { get; set; }
            public int group_bit { get; set; }
            public string admincheck { get; set; }
            public int rvrc { get; set; }
        }

        public class __GLOBAL
        {
            public string _ATTACH_BASE_VIEW { get; set; }
        }



        public class DirectionaryT
        {
            public int tid { get; set; }
            public int fid { get; set; }
            public int quote_from { get; set; }
            public string quote_to { get; set; }
            public string titlefont { get; set; }
            public string topic_misc { get; set; }
            public string author { get; set; }
            public int authorid { get; set; }
            public string subject { get; set; }
            public int type { get; set; }
            public int postdate { get; set; }
            public int lastpost { get; set; }
            public string lastposter { get; set; }
            public int replies { get; set; }
            public int lastmodify { get; set; }
            public int recommend { get; set; }
            public __P __P { get; set; }
            public string tpcurl { get; set; }
            public string error { get; set; }
        }

        public class __P
        {
            public int tid { get; set; }
            public int pid { get; set; }
            public int authorid { get; set; }
            public int type { get; set; }
            public int postdate { get; set; }
            public string subject { get; set; }
            public string content { get; set; }
        }

        public class __F
        {
        }

    }
}
