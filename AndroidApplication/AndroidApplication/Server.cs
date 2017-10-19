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

namespace AndroidApplication
{
     class Server
     {
          string url = "http://easyupload-server.azurewebsites.net/";
          Activity caller;

          public Server(Activity caller)
          {
               this.caller = caller;
          }

          public void OnStart()
          {
               string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
               string filename = Path.Combine(path, "myfile.txt");
               string currentIp = GetIPAddress().ToString();
               string oldIp = ReadOldIp(filename);

               if (oldIp != "")
               {
                    CheckIPChanged();
               }
               else
               {
                    Register(filename, currentIp);
               }
          }

          public void On

          private void CheckIPChanged() 
          {
               string oldIp = ReadOldIp(filename);
               if (oldIp != currentIp)
               {

                    MakeIPChangeCall(oldIp, currentIp);
                    WriteCurrentIP(filename, currentIp);
               }
          }

          private void Register(string filename, string currentIp) 
          {
               MakeRegisterCall(currentIp);
               WriteCurrentIP(filename, currentIp);
          }

          private string ReadOldIp(string filename)
          {
               string oldIp = "";
               using (var streamReader = new StreamReader(filename))
               {
                    oldIp = streamReader.ReadToEnd();
               }
               return oldIp;
          }

          private void WriteCurrentIP(string filename, string currentIp)
          {
               using (var streamWriter = new StreamWriter(filename))
               {
                    streamWriter.Write(currentIp, false);
               }
          }

          private void MakeAddDesktopCall(string currentIp, string desktopId) 
          {
               Dictionary<string, string> headers = new Dictionary<string, string>();
               headers.Add("phoneip", currentIp);
               headers.Add("desktopid", desktopId);
               Call(url + "newphone", headers, "");
          }
          private void MakeRegisterCall(string currentIp)
          {
               Dictionary<string, string> headers = new Dictionary<string, string>();
               headers.Add("ip", currentIp);
               Call(url + "newphone", headers, "");
          }

          private void MakeIPChangeCall(string oldIp, string currentIp)
          {
               Dictionary<string, string> headers = new Dictionary<string, string>();
               headers.Add("newip", currentIp);
               headers.Add("oldip", oldIp);
               Call(url + "phoneipchange", headers, "");
          }

          private async Task<string> Call(string url, Dictionary<string, string> headers, string body)
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
               byte[] bytes = Encoding.ASCII.GetBytes(body);
               requestStream.Write(bytes, 0, bytes.Length);

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

          public IPAddress GetIPAddress()
          {

               foreach (IPAddress adress in Dns.GetHostAddresses(Dns.GetHostName()))
               {
                    return adress;
               }

               return null;
          }


     }
}