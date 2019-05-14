using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAPBotAPI.Models
{   
    public class Button
    {
        public string title { get; set; }
        public string type { get; set; }
        public string value { get; set; }
    }

    public class Element
    {
        public Element()
        {
            buttons = new List<Button>();
        }
        public string title { get; set; }
        public string imageUrl { get; set; }
        public string subtitle { get; set; }
        public List<Button> buttons { get; set; }
    }

    public class Content
    {
        public Content()
        {
            elements = new List<Element>();
        }
        public List<Element> elements { get; set; }
    }

    public class Reply
    {
        public Reply()
        {
            content = new Content();
        }
        public string type { get; set; }
        public Content content { get; set; }
    }

    public class BOTReply
    {
        public BOTReply()
        {
            replies = new List<Reply>();
        }
        public List<Reply> replies { get; set; }
    }
}