using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.IO;
using System.Text;
using System.Security.Cryptography;

namespace Server.Controllers
{
     public class HomeController : Controller
     {
          public string Index()
          {
               using (var dataContext = new EasyUploadEntities())
               using (var transaction = dataContext.Database.BeginTransaction())
               {

                    dataContext.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[Photos] ON");
                    transaction.Commit();
               }
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

          public string UploadPhoto()
          {
               string body = "";
               foreach (string key in Request.Form.AllKeys)
               {
                    body += Request.Form[key];
               }
               string photocode = GenerateCode(10);
               using (var ctx = new EasyUploadEntities())
               {
                    string code = Request.Headers.Get("code");
                    string isFinished = Request.Headers.Get("isfinished");
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
                         Photo photo = new Photo();
                         photo.IsFinished = (isFinished == "1");
                         photo.DesktopId = desktop.Id;
                         photo.FileName = DateTime.Now.ToFileTime().ToString();
                         string filepath = Server.MapPath("~/" + photo.FileName);
                         photo.Code = photocode;
                         ctx.Photos.Add(photo);

                         WriteFile(filepath + "temp.txt", body);
                         if (photo.IsFinished) 
                         {
                              EncryptFile(filepath + ".txt", filepath + "temp.txt");
                              DeleteFile(filepath + "temp.txt");
                         }
                    }
                    ctx.SaveChanges();
               }
               return photocode;
          }

          public string AppendPhoto()
          {
               string body = "";
               foreach (string key in Request.Form.AllKeys)
               {
                    body += Request.Form[key];
               }
               using (var ctx = new EasyUploadEntities())
               {
                    string photoCode = Request.Headers.Get("photocode");
                    string isFinished = Request.Headers.Get("isfinished");
                    var photos = ctx.Photos.Where(s => s.Code == photoCode);

                    if (photos.Count() == 0)
                    {
                         return "Could not find any photos with that code.";
                    }
                    foreach (var photo in photos)
                    {
                         string filepath = Server.MapPath("~/" + photo.FileName);
                         WriteFile(filepath + "temp.txt", body);
                         photo.IsFinished = (isFinished == "1");

                         if (photo.IsFinished)
                         {
                              EncryptFile(filepath + ".txt", filepath + "temp.txt");
                              DeleteFile(filepath + "temp.txt");
                         }
                    }

                    ctx.SaveChanges();
               }
               return "Photo Appended.";
          }

          public string GetPhoto()
          {
               Photo photo = new Photo();
               using (var ctx = new EasyUploadEntities())
               {
                    string photoId = Request.Headers.Get("photoid");
                    photo = ctx.Photos.Where(s => s.Id.ToString() == photoId).SingleOrDefault();

                    if (photo == null)
                    {
                         return "Could not find photo with that Id.";
                    }
                    string filepath = Server.MapPath("~/" + photo.FileName);
                    return ReadFile(filepath);

               }
               return "idk";
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
                    string code = Request.Headers.Get("code");
                    var desktop = ctx.Desktops.Where(s => s.Code == code).SingleOrDefault();
                    if (desktop == null)
                    {
                         response = "Could not find desktop with that code.";
                    }
                    else
                    {
                         var photo = ctx.Photos.Where(s => s.DesktopId == desktop.Id && s.IsFinished).FirstOrDefault();
                         if (photo != null)
                         {
                              string filepath = Server.MapPath("~/" + photo.FileName) + ".txt";
                              response = ReadFile(filepath);
                              ctx.Photos.Remove(photo);
                              DeleteFile(filepath);
                         }
                         ctx.SaveChanges();
                    }
               }
               return response;
          }

          private void DeleteFile(string filepath)
          {
               if (System.IO.File.Exists(filepath))
               {
                    System.IO.File.Delete(filepath);
               }
          }

          private string ReadFile(string filepath)
          {
               byte[] bytes = DecryptFile(filepath);
               string content = Encoding.ASCII.GetString(bytes);
               return content;
          }

          private void WriteFile(string filepath, string content)
          {

               FileStream filestream = new FileStream(filepath, FileMode.Append);
               StreamWriter writer = new StreamWriter(filestream);
               writer.Write(content);
               writer.Flush();
               writer.Close();
               filestream.Close();
               
          }

          private byte[] DecryptFile(string filepath)
          {
               string password = @"myKey123"; // Your Key Here

               UnicodeEncoding UE = new UnicodeEncoding();
               byte[] key = UE.GetBytes(password);

               FileStream fsCrypt = new FileStream(filepath, FileMode.Open);

               RijndaelManaged RMCrypto = new RijndaelManaged();

               CryptoStream cs = new CryptoStream(fsCrypt,
                   RMCrypto.CreateDecryptor(key, key),
                   CryptoStreamMode.Read);

               int data;
               List<byte> bytes = new List<byte>();
               while ((data = cs.ReadByte()) != -1)
               {
                    bytes.Add((byte)data);
               }

               cs.Close();
               fsCrypt.Close();
               return bytes.ToArray();

          }

          private void EncryptFile(string encrypted, string file)
          {

               string password = @"myKey123"; // Your Key Here
               UnicodeEncoding UE = new UnicodeEncoding();
               byte[] key = UE.GetBytes(password);

               string cryptFile = encrypted;
               FileStream fsCrypt = new FileStream(cryptFile, FileMode.OpenOrCreate);

               RijndaelManaged RMCrypto = new RijndaelManaged();

               CryptoStream cs = new CryptoStream(fsCrypt,
                   RMCrypto.CreateEncryptor(key, key),
                   CryptoStreamMode.Write);

               FileStream fileToEncrypt = new FileStream(file, FileMode.Open);

               int data;
               while((data = fileToEncrypt.ReadByte()) != -1)
               {
                    cs.WriteByte((byte)data);
               }

               fileToEncrypt.Close();
               cs.Close();
               fsCrypt.Close();
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
                         int numPhotos = ctx.Photos.Where(s => s.Code == code).ToArray().Length;
                         if (numPhones == 0 && numDesktops == 0 && numPhotos == 0)
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