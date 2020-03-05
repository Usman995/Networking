using System;
using System.Net.Sockets;
using System.IO;
using System.Collections.Generic;
using System.Linq;

public class Whois
{

    static string servername = "whois.net.dcs.hull.ac.uk";
    static int port = 43;
    static string Protocols;
    static string message;
    static List<String> Items = new List<String>();

    static void Main(string[] args)
    {
        for (int i = 0; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "-h":
                    if (args[i + 1] == null)
                    {
                        break;
                    }
                    i++;
                    servername = args[i];
                    break;

                case "-p":
                    i++;
                    try
                    {
                        port = Convert.ToInt32(args[i]);
                    }
                    catch
                    {
                        Console.WriteLine("Default port will be used as no other port was given");
                        port = 43;
                    }

                    break;

                case "-h0":
                    Protocols = args[i];
                    break;

                case "-h1":
                    Protocols = args[i];
                    break;

                case "-h9":
                    Protocols = args[i];
                    break;

                default:
                    Items.Add(args[i]);

                    break;

            }
        }
        if (Items.Count == 0)
        {
            Console.WriteLine("ERROR: No username supplied");
            return;
        }
        else if (Items.Count > 2)
        {
            Console.WriteLine("ERROR: Too many items");
            return;
        }
        try
        {
            int c;
            TcpClient client = new TcpClient();
            client.Connect(servername, port);
            StreamWriter sw = new StreamWriter(client.GetStream());
            StreamReader sr = new StreamReader(client.GetStream());
            client.SendTimeout = 1000;
            client.ReceiveTimeout = 1000;
            sw.Flush();
            int contentLength = 0;

            switch (Protocols)
            {
                case "-h9":
                    if (Items.Count == 1)
                    {
                        message = "HTTP/0.9 404 Not Found\r\nContent-Type: text/plain\r\n\r\n\r\n";
                        sw.WriteLine("GET /" + Items[0]);
                        sw.Flush();
                        string temp = sr.ReadToEnd();
                        string[] stringSeparators = new string[] { "\r\n" };
                        string[] lines = temp.Split(stringSeparators, StringSplitOptions.None);
                        if (temp == message)
                        {
                            Console.WriteLine("Error: no entries found");
                        }
                        else
                        {
                            Console.WriteLine(Items[0] + " is " + lines[3]);
                        }
                    }
                    else if (Items.Count == 2)
                    {
                        message = "HTTP/0.9 200 OK\r\nContent-Type: text/plain\r\n\r\n";
                        sw.WriteLine("PUT /" + Items[0]);
                        sw.WriteLine();
                        sw.WriteLine(Items[1]);
                        sw.Flush();
                        string temp = sr.ReadToEnd();
                        string[] stringSeparators = new string[] { "\r\n" };
                        string[] lines = temp.Split(stringSeparators, StringSplitOptions.None);
                        if (temp == message)
                        {
                            // Actually OK
                            Console.WriteLine(Items[0] + " location changed to be " + Items[1]);
                        }
                        else
                        {
                            Console.WriteLine("ERROR: Server reply: " + temp);
                        }
                    }
                    break;
                case "-h0":

                    if (Items.Count == 1)
                    {
                        message = "HTTP/1.0 404 Not Found\r\nContent-Type: text/plain\r\n\r\n\r\n";
                        sw.WriteLine("GET /?" + Items[0] + " HTTP/1.0");
                        sw.WriteLine();
                        sw.Flush();
                        string[] stringSeparators = new string[] { "\r\n" };
                        string temp = sr.ReadToEnd();
                        string[] lines = temp.Split(stringSeparators, StringSplitOptions.None);


                        if (temp == message)
                        {
                            Console.WriteLine("Error: no entries found");
                        }
                        else
                        {
                            Console.WriteLine(Items[0] + " is " + lines[3]);
                        }

                    }
                    else if (Items.Count == 2)
                    {
                        message = "HTTP/1.0 200 OK\r\nContent-Type: text/plain\r\n\r\n";
                        contentLength = Items[1].Length;
                        sw.WriteLine("POST /" + Items[0] + " HTTP/1.0");
                        sw.WriteLine("Content-Length: " + contentLength);
                        sw.WriteLine();
                        sw.Write(Items[1]);
                        sw.Flush();
                        string temp = sr.ReadToEnd();
                        string[] stringSeparators = new string[] { "\r\n" };
                        string[] lines = temp.Split(stringSeparators, StringSplitOptions.None);
                        if (temp == message)
                        {
                            // Actually OK
                            Console.WriteLine(Items[0] + " location changed to be " + Items[1]);
                        }
                        else
                        {
                            Console.WriteLine("ERROR: Server reply: " + temp);
                        }

                    }
                    break;

                case "-h1":
                    if (Items.Count == 1)
                    {

                        message = "HTTP/1.1 404 Not Found\r\nContent-Type: text/plain\r\n\r\n\r\n";
                        sw.WriteLine("GET /?name=" + Items[0] + " HTTP/1.1");
                        sw.WriteLine("Host: " + servername);
                        sw.WriteLine();
                        sw.Flush();
                        string temp = "";
                        string location = "";
                        List<string> lines = new List<string>();
                        try
                        {
                            while ((temp = sr.ReadLine()) != null)
                            {
                                lines.Add(temp);
                            }
                        }
                        catch (IOException)
                        {

                        }
                        if (lines[0] == message)
                        {
                            Console.WriteLine("Error: " + message);
                        }
                        else
                        {

                            if (lines.Contains(""))
                            {
                                int i = lines.IndexOf("");
                                for (int x = i + 1; x < lines.Count; x++)
                                {
                                    location += lines[x] + "\r\n";

                                }
                            }
                            else
                            {
                                location = lines[lines.Count - 1];
                            }
                            Console.WriteLine(Items[0] + " is " + location);
                        }


                    }
                    else if (Items.Count == 2)
                    {
                        message = "HTTP/1.1 200 OK\r\nContent-Type: text/plain\r\n\r\n";
                        contentLength = Items[0].Length + Items[1].Length + 15;
                        sw.WriteLine("POST / HTTP/1.1");
                        sw.WriteLine("Host: " + servername);
                        sw.WriteLine("Content-Length: " + contentLength);
                        sw.WriteLine();
                        sw.Write("name=" + Items[0] + "&location=" + Items[1] + "\r\n");
                        sw.Flush();
                        string temp = sr.ReadToEnd();
                        string[] stringSeparators = new string[] { "\r\n" };
                        string[] lines = temp.Split(stringSeparators, StringSplitOptions.None);
                        if (temp == message)
                        {
                            // Actually OK
                            Console.WriteLine(Items[0] + " location changed to be " + Items[1]);
                        }
                        else
                        {
                            Console.WriteLine("ERROR: Server reply: " + temp);
                        }

                    }
                    break;
                default:
                    if (Items.Count > 2)
                    {
                        Console.WriteLine("input not valid");
                    }
                    if (Items.Count == 1)
                    {

                        sw.WriteLine(Items[0]);
                        sw.Flush();
                        Console.WriteLine(Items[0] + " is " + sr.ReadToEnd());
                    }
                    else if (Items.Count == 2)
                    {
                        sw.WriteLine(Items[0] + " " + Items[1]);
                        sw.Flush();
                        string temp = sr.ReadToEnd();
                        if (temp.Equals("OK\r\n"))
                        {
                            Console.WriteLine(Items[0] + " location changed to be " + Items[1]);
                        }
                        else
                        {
                            Console.WriteLine("ERROR: Server reply: " + temp);
                        }


                    }
                    else if (Items.Count == 0)
                    {
                        Console.WriteLine("input not valid");
                    }
                    break;

            }
            


        }


        catch (Exception e)
        {
            Console.WriteLine(e);
        }

    }

}