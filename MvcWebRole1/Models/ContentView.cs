using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcWebRole1.Models
{
    public class ContentView
    {
        public string text;
        public ContentData[] contentData;
        public int id;
        public string title;
        public string[] tags;
        public ContentView(string text, string title, ContentData[] contentData, int id, string[] tags)
        {
            this.text = text;
            this.title = title;
            this.contentData = contentData;
            this.id = id;
            this.tags = tags;
        }
        public ContentView(string text, string title, ContentData[] contentData, int id)
        {
            this.text = text;
            this.title = title;
            this.contentData = contentData;
            this.id = id;
            tags = null;
        }
    }
}