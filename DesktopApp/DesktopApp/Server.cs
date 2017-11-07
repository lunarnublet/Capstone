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
using System.Collections.ObjectModel;

namespace DesktopApp
{
     class Server
     {
          private string url;
          private string filename;
          private string existingDirectoriesName;
          private string code;
          private Thread aliveCalls;
          private string savePath;
          private MainWindow host;

          public ObservableCollection<string> ExistingDirs { get; set; }

          public string SavePath
          {
               get { return savePath; }
               set 
               {
                    savePath = value;
                    if (value.Last() != '\\') 
                    {
                         savePath += '\\';
                    }
               }
          }


          public Server(MainWindow host) 
          {
               this.host = host;
               filename = "myfile.txt";
               existingDirectoriesName = "existing.txt";
               code = "";
               url = "http://easyupload-server.azurewebsites.net/";
               aliveCalls = new Thread(MakeAliveCalls);
               SavePath = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
               ExistingDirs = new ObservableCollection<string>();
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
               GetExistingDirs();
               if (ExistingDirs.Count > 0) 
               {
                    savePath = ExistingDirs[0];
               }
               StartAliveCalls();
          }

          private void GetExistingDirs()
          {
               FileStream file = new FileStream(existingDirectoriesName, FileMode.OpenOrCreate);
               StreamReader reader = new StreamReader(file);
               string temp = reader.ReadToEnd();
               if (temp.Length > 0)
               {
                    var temp2 = temp.Split(',');
                    ExistingDirs = new ObservableCollection<string>(temp2);
               }
               reader.Close();
          }

          private void SaveDirs()
          {
               FileStream file = new FileStream(existingDirectoriesName, FileMode.Truncate);
               StreamWriter writer = new StreamWriter(file);
               foreach (var item in ExistingDirs) 
               {
                    writer.Write(item);
                    if (item != ExistingDirs.Last()) 
                    {
                         writer.Write(',');
                    }
               }
               writer.Flush();
               writer.Close();
          }

          public void AddDirectory(string directory) 
          {
               if (!ExistingDirs.Contains(directory)) 
               {
                    ExistingDirs.Add(directory);
                    SaveDirs();
               }
          }

          public bool DeleteDirectory(string directory) 
          {
               if (ExistingDirs.Remove(directory)) 
               {
                    SaveDirs();
                    return true;
               }
               return false;
          }

          private void StartAliveCalls()
          {
               aliveCalls.Start();
          }

          public void EndCalls()
          {
               aliveCalls.Abort();
          }

          private string WriteBitmap(string path, string base64)
          {
               byte[] bytes = Convert.FromBase64String(base64);
               Bitmap bitmap;
               string filepath = "";
               using (var ms = new MemoryStream(bytes))
               {
                    bitmap = new Bitmap(ms);
                    filepath = path + DateTime.Now.ToFileTime() + ".jpg";
                    bitmap.Save(filepath, ImageFormat.Jpeg);
               }
               return filepath;
          }

          private void MakeAliveCalls() 
          {
               Dictionary<string, string> headers = new Dictionary<string, string>();
               headers.Add("code", code);
               while (true) 
               {
                    string photoas64 = Call(url + "desktopalive", headers);
                    if (photoas64 != "") 
                    {
                         string filepath = WriteBitmap(savePath, photoas64);
                         host.NewImage = true;
                    }
                    Thread.Sleep(1000);
               }
          }

          public string MakeNewDesktopCall() 
          {
               Dictionary<string, string> headers = new Dictionary<string, string>();
               return Call(url + "newdesktop", headers);
          }

          public string Call(string url, Dictionary<string, string> headers)
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

               try
               {
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
               catch (Exception e) 
               {
                    return "";
               }

          }
     }
}
