namespace ForkNgaWatcher.Model
{
    class IDConfig
    {
        public IDConfig()
        {
            if (this.NgaPassportCID == null)
                this.NgaPassportCID = "CID";

            if (this.NgaPassportUID == null)
                this.NgaPassportUID = "UID";

            if (this.WatchUIds == null)
                this.WatchUIds = new List<string>();
        }
        public string NgaPassportCID { get; set; }

        public string NgaPassportUID { get; set; }

        public List<string> WatchUIds { get; set; }
    }
}
