using System;
using System.Net.Sockets;
using System.IO;
public class Whois
{
   
    static void Main(string[] args)
    {
        try
        {
            int c;
            TcpClient client = new TcpClient();
            client.Connect("localhost", 43);
            StreamWriter sw = new StreamWriter(client.GetStream());
            StreamReader sr = new StreamReader(client.GetStream());
           
            if(args.Length >1 )
            {
                string line = "";
                foreach(string arg in args)
                {
                    line += arg + " ";
                }
                sw.WriteLine(line);
            }
            else
            {
                sw.WriteLine(args[0]);
            }
            
            sw.Flush();
            Console.WriteLine(sr.ReadToEnd());
            
        }
        catch (IndexOutOfRangeException)
        {
            Console.WriteLine("No Arguments Supplied");
        }

    }
}