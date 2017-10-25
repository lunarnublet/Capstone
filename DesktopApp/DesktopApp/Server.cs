using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;

namespace DesktopApp
{
     class Server
     {
          string url;
          string filename;
          string code;
          Thread aliveCalls;

          public Server() 
          {
               filename = "myfile.txt";
               code = "";
               url = "http://easyupload-server.azurewebsites.net/";
               //url = "http://localhost:18606/";
               aliveCalls = new Thread(MakeAliveCalls);

          }

          public void OnStart() 
          {
               FileStream file = new FileStream(filename, FileMode.OpenOrCreate);
               StreamReader reader = new StreamReader(file);

               code = reader.ReadToEnd();
               if (code == "") 
               {
                    code = MakeNewDesktopCall();
                    StreamWriter writer = new StreamWriter(file);
                    writer.Write(code);
                    writer.Flush();
                    writer.Close();
               }

               StartAliveCalls();
          }

          private void StartAliveCalls()
          {
               aliveCalls.Start();
          }

          public void EndCalls()
          {
               aliveCalls.Abort();
          }

          private void WriteBitmap(string path, string base64)
          {
               byte[] bytes = Convert.FromBase64String(base64);
               Bitmap bitmap;
               using (var ms = new MemoryStream(bytes))
               {
                    bitmap = new Bitmap(ms);
                    string filepath = path + DateTime.Now.ToFileTime() + ".jpg";
                    bitmap.Save(filepath, ImageFormat.Jpeg);
               }
          }

          private void MakeAliveCalls() 
          {
               Dictionary<string, string> headers = new Dictionary<string, string>();
               headers.Add("code", code);
               while (true) 
               {
                    string photoas64 = Call(url + "desktopalive", headers, "");
                    if (photoas64 != "") 
                    {
                         WriteBitmap("D:\\test\\", photoas64);
                    }
                    Thread.Sleep(1000);
               }
          }

          public string MakeNewDesktopCall() 
          {
               Dictionary<string, string> headers = new Dictionary<string, string>();
               return Call(url + "newdesktop", headers, "");
          }

          public string Call(string url, Dictionary<string, string> headers, string body = "")
          {
               HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(new Uri(url));
               request.Method = "GET";
               request.ContentType = "application/x-www-form-urlencoded";
               foreach (var key in headers.Keys)
               {
                    string value;
                    headers.TryGetValue(key, out value);
                    request.Headers[key] = value;
               }

               using (WebResponse response = request.GetResponse())
               {
                    using (Stream responseStream = response.GetResponseStream())
                    {
                         StreamReader reader = new StreamReader(responseStream);
                         string output = reader.ReadToEnd();

                         return output;
                    }
               }
          }
     }
}
