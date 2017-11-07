using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Net.NetworkInformation;
using System.Collections.Specialized;
using System.Web;

namespace AndroidApplication
{
     class Server
     {
          string url = "http://easyupload-server.azurewebsites.net/";
          //string url = "http://localhost:18606/";
          MainActivity caller;

          public Server(MainActivity caller)
          {
               this.caller = caller;
          }

          public void OnStart()
          {
               string code = GetCode();

               if (code == "")
               {
                    Register();
               }
          }

          public void AddConnection(string connectionCode) 
          {
               string code = GetCode();
               MakeAddConnectionCall(code, connectionCode);
          }

          public async Task<String> SendPhoto(string photo, int i) 
          {
               string code = GetCode();
               int maxChars = 200000;
               int index = 0;
               string photocode = "";
               bool isfinished = false;
               int totalsections = (photo.Length / maxChars) + 1;
               int sectionon = 1;

               if (photo.Length <= maxChars)
               {
                    isfinished = true;
                    caller.SetListViewText(i, "1/1");
                    photocode = await MakeSendPhotoCall(code, photo, isfinished);

               }
               else 
               {
                    caller.SetListViewText(i, sectionon + "/" + totalsections);

                    string substring = photo.Substring(index, maxChars);
                    photocode = await MakeSendPhotoCall(code, substring, isfinished);
                    index = maxChars;

                    while (index <= photo.Length)
                    {
                         ++sectionon;
                         caller.SetListViewText(i, sectionon + "/" + totalsections);


                         if (index + maxChars < photo.Length)
                         {
                              substring = photo.Substring(index, maxChars);
                              isfinished = false;
                         }
                         else
                         {
                              substring = photo.Substring(index);
                              isfinished = true;
                         }
                         await MakeAppendPhotoCall(photocode, substring, isfinished);
                         index += maxChars;

                    }

               }
               caller.SetListViewText(i, "DONE");



               return photocode;
          }

          private string GetCode() 
          {
               string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
               string filename = Path.Combine(path, "myfile.txt");
               string code = ReadCode(filename);
               return code;
          }

          private async Task<string> Register() 
          {
               string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
               string filename = Path.Combine(path, "myfile.txt");
               string code = await MakeRegisterCall();
               WriteCode(filename, code);
               return code;
          }

          private string ReadCode(string filename)
          {
               string code = "";
               using (var streamReader = new StreamReader(filename))
               {
                    code = streamReader.ReadToEnd();
               }
               return code;
          }

          private void WriteCode(string filename, string code)
          {
               using (var streamWriter = new StreamWriter(filename))
               {
                    streamWriter.Write(code, false);
               }
          }

          private void MakeAddConnectionCall(string code, string connectionCode) 
          {
               Dictionary<string, string> headers = new Dictionary<string, string>();
               headers.Add("phoneCode", code);
               headers.Add("desktopCode", connectionCode);
               Call(url + "phoneaddconnection", headers, "");
          }

          private async Task<string> MakeSendPhotoCall(string code, string photo, bool iscompleted)
          {
               Dictionary<string, string> headers = new Dictionary<string, string>();
               headers.Add("code", code);
               headers.Add("isfinished", iscompleted ? "1" : "0");
               return await Call(url + "uploadphoto", headers, photo);
          }
          private async Task<string> MakeAppendPhotoCall(string photocode, string photo, bool iscompleted)
          {
               Dictionary<string, string> headers = new Dictionary<string, string>();
               headers.Add("photocode", photocode);
               headers.Add("isfinished", iscompleted ? "1" : "0");
               return await Call(url + "appendphoto", headers, photo);
          }

          private async Task<string> MakeRegisterCall()
          {
               Dictionary<string, string> headers = new Dictionary<string, string>();
               return await Call(url + "newphone", headers, "");
          }

          private async Task<string> Call(string url, Dictionary<string, string> headers, string body)
          {
               HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(new Uri(url));
               request.Method = "POST";
               request.ContentType = "application/x-www-form-urlencoded";
               foreach (var key in headers.Keys)
               {
                    string value;
                    headers.TryGetValue(key, out value);
                    request.Headers[key] = value;
               }

               NameValueCollection outgoingQueryString = HttpUtility.ParseQueryString(String.Empty);
               outgoingQueryString.Add("body", body);

               string postdata = outgoingQueryString.ToString();
               int v = postdata.Length;

               Stream requestStream = request.GetRequestStream();
               StreamWriter writer = new StreamWriter(requestStream);
               writer.Write(postdata);
               writer.Flush();
               writer.Close();

               using (WebResponse response = await request.GetResponseAsync())
               {
                    using (Stream responseStream = response.GetResponseStream())
                    {
                         StreamReader reader = new StreamReader(responseStream);
                         string output = reader.ReadToEnd();

                         return output;
                    }
               }
          }

          private async Task<string> Call(string url, Dictionary<string, string> headers, byte[] body)
          {
               // Create an HTTP web request using the URL:
               HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(new Uri(url));
               request.Method = "POST";
               foreach (var key in headers.Keys)
               {
                    string value;
                    headers.TryGetValue(key, out value);
                    request.Headers[key] = value;
               }


               Stream requestStream = request.GetRequestStream();
               StreamWriter writer = new StreamWriter(requestStream);
               writer.Write(body);
               writer.Flush();
               writer.Close();

               // Send the request to the server and wait for the response:
               using (WebResponse response = await request.GetResponseAsync())
               {
                    // Get a stream representation of the HTTP web response:
                    using (Stream responseStream = response.GetResponseStream())
                    {
                         StreamReader reader = new StreamReader(responseStream);
                         // Use this stream to build a JSON document object:
                         string output = reader.ReadToEnd();
                         caller.FindViewById<TextView>(Resource.Id.MyTextView).Text = output;

                         // Return the JSON document:
                         return output;
                    }
               }
          }

     }
}