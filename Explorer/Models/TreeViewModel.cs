namespace Explorer.Models
{
    public class TreeViewModel
    {
        public string id { get; set; }
        public string parent { get; set; }
        public string text { get; set; }
        public string icon { get; set; }
        public string state { get; set; } //opened, disabled, selected
        public string li_attr { get; set; }
        public string a_attr { get; set; }
    }
}