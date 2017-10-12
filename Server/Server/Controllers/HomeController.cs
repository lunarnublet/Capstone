using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.IO;

namespace Server.Controllers
{
    public class HomeController : Controller
    {
        public string Index()
        {
            return AllPhones() + AllDesktops();
        }

        public string NewPhone()
        {
            using (var ctx = new EasyUploadEntities())
            {
                ctx.Phones.Add(new Phone() { Ip = Request.Headers.Get("ip") });
                ctx.SaveChanges();
            }

            return "Phone Added.";
        }

        public string UploadPicture()
        {
            StreamReader reader = new StreamReader(Request.InputStream);
            string body = reader.ReadToEnd();

            using (var ctx = new EasyUploadEntities())
            {
                Phone phone = ctx.Phones.Where(s => s.Id.ToString() == Request.Headers.Get("id")).SingleOrDefault();

                foreach (Desktop desktop in phone.Desktops)
                {
                    ctx.Pictures.Add(new Picture() { DesktopId = desktop.Id, AsString = body });
                }
                ctx.SaveChanges();
            }
            return "Picture Queued.";
        }

        public string PhoneAddConnection()
        {
            using (var ctx = new EasyUploadEntities())
            {
                Desktop desktop = ctx.Desktops.Where(s => s.Id.ToString() == Request.Headers.Get("desktopid")).SingleOrDefault();
                Phone phone = ctx.Phones.Where(s => s.Id.ToString() == Request.Headers.Get("phoneid")).SingleOrDefault();
                phone.Desktops.Add(desktop);
                ctx.SaveChanges();
            }
            return "Connection Added.";
        }

        public string PhoneIPChange()
        {
            using (var ctx = new EasyUploadEntities())
            {
                var phone = ctx.Phones.Where(s => s.Id.ToString() == Request.Headers.Get("id") && s.Ip == Request.Headers.Get("oldip")).SingleOrDefault();
                phone.Ip = Request.Headers.Get("newip");
                ctx.SaveChanges();
            }
            return "Phone IP Updated.";
        }

        public string NewDesktop()
        {
            using (var ctx = new EasyUploadEntities())
            {
                ctx.Desktops.Add(new Desktop() { Ip = Request.Headers.Get("ip") });
                ctx.SaveChanges();
            }
            return "Desktop Added.";
        }

        public string DesktopAlive()
        {
            string response = "";
            using (var ctx = new EasyUploadEntities())
            {
                var pictures = ctx.Pictures.Where(s => s.DesktopId.ToString() == Request.Headers.Get("id"));
                foreach(Picture picture in pictures)
                {
                    response += picture.AsString;
                    if (pictures.Last() != picture)
                    {
                        response += ",";
                    }
                    ctx.Pictures.Remove(picture);
                }
                ctx.SaveChanges();
            }
            return response;
        }

        public string DesktopIPChange()
        {
            using (var ctx = new EasyUploadEntities())
            {
                var desktop = ctx.Desktops.Where(s => s.Id.ToString() == Request.Headers.Get("id") && s.Ip == Request.Headers.Get("oldip")).SingleOrDefault();
                desktop.Ip = Request.Headers.Get("newip");
                ctx.SaveChanges();
            }
            return "Desktop IP Updated.";
        }

        private string AllPhones()
        {
            string s = "";
            using (var ctx = new EasyUploadEntities())
            {
                var phones = ctx.Phones.ToList();
                foreach (Phone phone in phones)
                {
                    s += "P" + phone.Id + ": " + phone.Ip + " ";
                }
            }
            return s;
        }

        private string AllDesktops()
        {
            string s = "";
            using (var ctx = new EasyUploadEntities())
            {
                var desktops = ctx.Desktops.ToList();
                foreach (Desktop desktop in desktops)
                {
                    s += "D"  + desktop.Id + ": " + desktop.Ip + " ";
                }
            }
            return s;
        }
    }
}