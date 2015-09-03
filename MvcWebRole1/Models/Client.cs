using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcWebRole1.Models
{
    public class Client 
    {
        [Key]
        public int ID_CL { get; set; }
        public int ID_USER { get; set; }
        public String NAME { get; set; }
        public DateTime BIRTHDAY { get; set; }
        public int TYPE { get; set; }
        public String ID_VK { get; set; }
        public String ID_FB { get; set; }
        public int SEX { get; set; }    // 2 - male, 1 - female, 0 - hz
        public DateTime DATE_COME { get; set; }
        public DateTime DATE_LEAVE { get; set; }
        public string MOBILE_NUMBER { get; set; }
        public string MAIL { get; set; }
        public Client()
        { }
        public Client(int ID_USER, String NAME, DateTime BIRTHDAY, int TYPE, String ID_VK, String ID_FB, DateTime DATE_COME, string MOBILE_NUMBER, DateTime DATE_LEAVE,int SEX)
        {
            this.ID_USER = ID_USER;
            this.NAME = NAME;
            this.BIRTHDAY = BIRTHDAY;
            this.TYPE = TYPE;
            this.ID_VK = ID_VK;
            this.ID_FB = ID_FB;
            this.DATE_COME = DATE_COME;
            this.MOBILE_NUMBER = MOBILE_NUMBER;
            this.DATE_LEAVE = DATE_LEAVE;
            this.SEX = SEX;
        }
        public Client(int ID_USER, String NAME, DateTime BIRTHDAY, int TYPE, String ID_VK, String ID_FB, DateTime DATE_COME, string MOBILE_NUMBER, DateTime DATE_LEAVE, int SEX, string mail)
        {
            this.ID_USER = ID_USER;
            this.NAME = NAME;
            this.BIRTHDAY = BIRTHDAY;
            this.TYPE = TYPE;
            this.ID_VK = ID_VK;
            this.ID_FB = ID_FB;
            this.DATE_COME = DATE_COME;
            this.MOBILE_NUMBER = MOBILE_NUMBER;
            this.DATE_LEAVE = DATE_LEAVE;
            this.SEX = SEX;
            this.MAIL = mail;
        }
    }
}
