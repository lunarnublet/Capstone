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
               return "HelloWorld";
          }

          public string NewPhone()
          {
               string code = "";
               using (var ctx = new EasyUploadEntities())
               {    
                    code = GenerateCode(10);
                    ctx.Phones.Add(new Phone() { Code = code });
                    ctx.SaveChanges();
               }

               return code;
          }

          public string UploadPicture()
          {
               StreamReader reader = new StreamReader(Request.InputStream);
               string body = reader.ReadToEnd();

               using (var ctx = new EasyUploadEntities())
               {
                    string code = Request.Headers.Get("code");
                    Phone phone = ctx.Phones.Where(s => s.Code == code).SingleOrDefault();

                    if (phone == null)
                    {
                         return "Could not find phone with that code.";
                    }
                    else if (phone.Desktops.Count == 0) 
                    {
                         return "No Desktops to send to.";
                    }
                    foreach (Desktop desktop in phone.Desktops)
                    {
                         ctx.Photos.Add(new Photo() { DesktopId = desktop.Id, AsString = body });
                    }
                    ctx.SaveChanges();
               }
               return "Picture Queued.";
          }

          public string PhoneAddConnection()
          {
               using (var ctx = new EasyUploadEntities())
               {
                    string desktopCode = Request.Headers.Get("desktopCode");
                    string phoneCode = Request.Headers.Get("phoneCode");
                    Desktop desktop = ctx.Desktops.Where(s => s.Code == desktopCode).SingleOrDefault();
                    Phone phone = ctx.Phones.Where(s => s.Code == phoneCode).SingleOrDefault();

                    if (phone == null)
                    {
                         return "Could not find phone with that code.";
                    }
                    else if (desktop == null) 
                    {
                         return "Could not find desktop with that code.";
                    }
                    phone.Desktops.Add(desktop);
                    ctx.SaveChanges();
               }
               return "Connection Added.";
          }

          public string NewDesktop()
          {
               string code = "";
               using (var ctx = new EasyUploadEntities())
               {
                    code = GenerateCode(10);
                    ctx.Desktops.Add(new Desktop() { Code = code });
                    ctx.SaveChanges();
               }

               return code;
          }

          public string DesktopAlive()
          {
               string response = "";
               using (var ctx = new EasyUploadEntities())
               {
                    string code = Request.Headers.Get("Code");
                    var desktop = ctx.Desktops.Where(s => s.Code == code).SingleOrDefault();
                    if (desktop == null)
                    {
                         response = "Could not find desktop with that code.";
                    }
                    else 
                    {
                         var photos = ctx.Photos.Where(s => s.DesktopId.ToString() == Request.Headers.Get("id"));
                         foreach (Photo photo in photos)
                         {
                              response += photo.AsString;
                              if (photos.Last() != photo)
                              {
                                   response += ",";
                              }
                              ctx.Photos.Remove(photo);
                         }

                         ctx.SaveChanges();
                    }
               }
               return response;
          }

          private string GenerateCode(int length) 
          {
               bool accepted = false;
               string code = "";
               using (var ctx = new EasyUploadEntities())
               {
                    do
                    {
                         code = NewCode(length);
                         int numDesktops = ctx.Desktops.Where(s => s.Code == code).ToArray().Length;
                         int numPhones = ctx.Phones.Where(s => s.Code == code).ToArray().Length;
                         if (numPhones == 0 && numDesktops == 0) 
                         {
                              accepted = true;
                         }
                    } while (!accepted);
               }

               return code;
          }

          private string NewCode(int length) 
          {

               Random random = new Random();
               string code = "";

               while (code.Length < length) 
               {
                    int type = random.Next(0, 3);
                    char c = ' ';
                    switch (type) 
                    {
                         case 0:
                              c = (char)random.Next(48, 57);
                              break;
                         case 1:
                              c = (char)random.Next(65, 90);
                              break;
                         case 2:
                              c = (char)random.Next(97, 122);
                              break;
                         default:
                              break;
                    }
                    code += c;
               }
               return code;
          }
     }
}