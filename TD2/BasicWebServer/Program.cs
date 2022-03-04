using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Web;

namespace BasicServerHTTPlistener
{
    internal class Program
    {
        private static void Main(string[] args)
        {

            //if HttpListener is not supported by the Framework
            if (!HttpListener.IsSupported)
            {
                Console.WriteLine("A more recent Windows version is required to use the HttpListener class.");
                return;
            }

            // Create a listener.
            HttpListener listener = new HttpListener();

            // Add the prefixes.
            if (args.Length != 0)
            {
                foreach (string s in args)
                {
                    listener.Prefixes.Add(s);
                    // don't forget to authorize access to the TCP/IP addresses localhost:xxxx and localhost:yyyy 
                    // with netsh http add urlacl url=http://localhost:xxxx/ user="Tout le monde"
                    // and netsh http add urlacl url=http://localhost:yyyy/ user="Tout le monde"
                    // user="Tout le monde" is language dependent, use user=Everyone in english 

                }
            }
            else
            {
                Console.WriteLine("Syntax error: the call must contain at least one web server url as argument");
            }
            listener.Start();

            // get args 
            foreach (string s in args)
            {
                Console.WriteLine("Listening for connections on " + s);
            }

            // Trap Ctrl-C on console to exit 
            Console.CancelKeyPress += delegate {
                // call methods to close socket and exit
                listener.Stop();
                listener.Close();
                Environment.Exit(0);
            };


            while (true)
            {
                // Note: The GetContext method blocks while waiting for a request.
                HttpListenerContext context = listener.GetContext();
                HttpListenerRequest request = context.Request;

                string documentContents;
                using (Stream receiveStream = request.InputStream)
                {
                    using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                        documentContents = readStream.ReadToEnd();
                    }
                }
                
                // get url 
                Console.WriteLine($"Received request for {request.Url}");

                //get url protocol
                Console.WriteLine(request.Url.Scheme);
                //get user in url
                Console.WriteLine(request.Url.UserInfo);
                //get host in url
                Console.WriteLine(request.Url.Host);
                //get port in url
                Console.WriteLine(request.Url.Port);
                //get path in url 
                Console.WriteLine(request.Url.LocalPath);

                // parse path in url 
                foreach (string str in request.Url.Segments)
                {
                    Console.WriteLine(str);
                }

                //get params un url. After ? and between &

                Console.WriteLine(request.Url.Query);

                //parse params in url
                Console.WriteLine("param1 = " + HttpUtility.ParseQueryString(request.Url.Query).Get("param1"));
                Console.WriteLine("param2 = " + HttpUtility.ParseQueryString(request.Url.Query).Get("param2"));
                Console.WriteLine("param3 = " + HttpUtility.ParseQueryString(request.Url.Query).Get("param3"));
                Console.WriteLine("param4 = " + HttpUtility.ParseQueryString(request.Url.Query).Get("param4"));

                //Paramteres 
                String[] parameters =
                {
                    HttpUtility.ParseQueryString(request.Url.Query).Get("param1"),
                    HttpUtility.ParseQueryString(request.Url.Query).Get("param2"),
                    HttpUtility.ParseQueryString(request.Url.Query).Get("param3"),
                    HttpUtility.ParseQueryString(request.Url.Query).Get("param4")
                };

                //
                Console.WriteLine(documentContents);

                // Obtain a response object.
                HttpListenerResponse response = context.Response;

                // Construct a response.
                String responseString = "";

                //Obtain the adress of the local host and the exercice question wanted.
                if (request.Url.Segments.Length >= 2)
                {
                    switch(request.Url.Segments[1])
                    {
       
                        //Answer to question 1
                        case "exercice1/":
                       
                            String htmlResponse = "";

                            //Obtain the name of the method and the two parameters
                            if (request.Url.Segments.Length >= 3)
                            {

                                Type methodType = typeof(MyMethods);

                                //Obtain the method's name
                                MethodInfo method1 = methodType.GetMethod(request.Url.Segments[2]);

                                if (method1 == null)
                                {
                                    htmlResponse = "Method passed in parameters is undefined";
                                }
                                else
                                {
                                    try
                                    {
                                        String result1 = (string)methodType.GetMethod(request.Url.Segments[2]).Invoke(null, new object[] { parameters[0], parameters[1] });
                                        htmlResponse = $"The result is {result1}";
                                    }
                                    catch (TargetInvocationException)
                                    {
                                        htmlResponse = "Format error";
                                    }
                                }

                            }
                            else
                            {
                                htmlResponse = "3 parameters should be given";
                            }
                            responseString = $"<!DOCTYPE html><html><body>{htmlResponse}</body></html>";
                            break;

                        //Answer to question 2 
                        /**
                        * Tester la fonction http://localhost:8080/exercice2/substract/substract?param1=5&param2=1
                        */
                        case "exercice2/":

                            ProcessStartInfo start = new ProcessStartInfo();
                            start.FileName = "python";
                            start.Arguments = $"substract.py ";

                            if (request.Url.Segments.Length > 2)
                            {
                                start.Arguments += $"{request.Url.Segments[2]} ";
                            }

                            foreach (string param in parameters)
                            {
                                start.Arguments += ((param == null || param.Equals("")) ? "undefined " : param + " ");
                            }

                            Console.Write("Program arguments: " + start.Arguments);

                            start.UseShellExecute = false;
                            start.RedirectStandardOutput = true;

                            using (Process process = Process.Start(start))
                            {
                                using (StreamReader reader = process.StandardOutput)
                                {
                                    string _result = reader.ReadToEnd();
                                    responseString = _result;
                                }
                            }
                            break;

                        case "exercice3/":
                            Type methodsType = typeof(MyMethods);
                            MethodInfo method = methodsType.GetMethod(request.Url.Segments[2]);
                            string result = "";

                            if (method == null)
                            {
                                result = "Bad method";
                            }
                            else
                            {
                                try
                                {
                                    result = (string)methodsType.GetMethod(request.Url.Segments[2]).Invoke(null, new object[] { parameters[0] });
                                }
                                catch (TargetInvocationException)
                                {
                                    result = "Error";
                                }
                            }

                            responseString = "{\"result\": \"" + result + "\"}";
                            break;

                        //Default answer
                        default:
                            responseString = $"TIMEOUT";
                            break;


                    }

                }

                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
                // Get a response stream and write the response to it.
                response.ContentLength64 = buffer.Length;
                System.IO.Stream output = response.OutputStream;
                output.Write(buffer, 0, buffer.Length);
                // You must close the output stream.
                output.Close();
            }
            // Httplistener neither stop ... But Ctrl-C do that ...
            // listener.Stop();
        }
    }

    internal class MyMethods
    {
        /**
         * Tester la fonction http://localhost:8080/exercice1/add?param1=5&param2=1
         */
        public static String add(String param1, String param2)
        {
            int num1 = Int32.Parse(param1);
            int num2 = Int32.Parse(param2);
            return (num1 + num2).ToString();
        }
    }

}