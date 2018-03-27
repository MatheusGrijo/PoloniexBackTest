using System;
using System.IO;
using System.Net;
using System.Text;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

public static class Http
{
    
    public static string get(String url)
    {
        try
        {
            
            String r = "";
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(new Uri(url));
            httpWebRequest.Method = "GET";
            var httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            var responseStream = httpWebResponse.GetResponseStream();
            if (responseStream != null)
            {
                var streamReader = new StreamReader(responseStream);
                r = streamReader.ReadToEnd();
            }
            if (responseStream != null) responseStream.Close();
            return r;
        }
        catch (WebException ex)
        {
            return null;
        }
    }

}