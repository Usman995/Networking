using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.IO;


namespace Locationserver
{
   

    class Program
    {
       static Dictionary<string, string> TheLocations = new Dictionary<string, string>();
        
       
        public static void runServer()
        {
            TcpListener listener;
            Socket connection;
            NetworkStream socketStream;
            
            try
            {
                
                listener = new TcpListener(43);
                listener.Start();
                Console.WriteLine("listening");
                while (true)
                {
                    connection = listener.AcceptSocket();
                    socketStream = new NetworkStream(connection);
                
                    doRequest(socketStream);
                    socketStream.Close();
                    connection.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        public static void doRequest(NetworkStream socketStream)
        {
            try
            {

                bool whois = true;
                socketStream.ReadTimeout = 1000;
               socketStream.WriteTimeout = 1000;
                StreamWriter sw = new StreamWriter(socketStream);
                StreamReader sr = new StreamReader(socketStream);
                string line = sr.ReadLine();
                String[] Words = line.Split(new char[] { ' ' }, 2);
                String username = null, location = "";
                List<string> OptionalLines = new List<string>();
                if (line.StartsWith("GET /"))
                {
                    whois = false;
                    // We have a lookup
                    if (line.Contains("HTTP/1.0"))
                    {
                        
                        string[] stringSeparators = new string[] { "?"," "};
                        string[] lines = line.Split(stringSeparators, StringSplitOptions.None);
                        username = lines[2];
                        if (TheLocations.ContainsKey(username))
                        {
                            // the location can be found in the dictionary
                            location = TheLocations[username];
                            sw.WriteLine("HTTP/1.0 200 OK");
                            sw.WriteLine("Content-Type: text/plain\r\n");
                            sw.WriteLine(location);
                            sw.Flush();
                        }
                        else
                        {
                            sw.Write("HTTP/1.0 404 Not Found\r\nContent-Type: text/plain\r\n\r\n\r\n");
                            sw.Flush();
                        }
                        


                    }
                    else if (line.Contains("HTTP/1.1"))
                    {
                      

                        char[] stringSeparators = { '=', ' ' };
                        string[] lines = line.Split(stringSeparators, StringSplitOptions.None);
                        username = lines[2];
                        if (TheLocations.ContainsKey(username))
                        {
                            location = TheLocations[username];

                            sw.WriteLine("HTTP/1.1 200 OK");
                            sw.WriteLine("Content-Type: text/plain");
                            sw.WriteLine();
                            sw.WriteLine(location);
                            sw.Flush();
                        }
                        else
                        {
                            sw.Write("HTTP/1.1 404 Not Found\r\nContent-Type: text/plain\r\n\r\n\r\n");
                            sw.Flush();
                        }


                    }
                    else
                    {
                        char[] stringSeparators = { '/'};
                        string[] lines = line.Split(stringSeparators, StringSplitOptions.None);
                        username = lines[1];
                        if (TheLocations.ContainsKey(username))
                        {
                            location = TheLocations[username];
                            sw.WriteLine("HTTP/0.9 200 OK");
                            sw.WriteLine("Content-Type: text/plain");
                            sw.WriteLine();
                            sw.WriteLine(location);
                            sw.Flush();
                        }
                        else
                        {
                            sw.Write("HTTP/0.9 404 Not Found\r\nContent-Type: text/plain\r\n\r\n\r\n");
                            sw.Flush();
                        }
                    }







                    }
                else if ((line.StartsWith("POST /") && line.EndsWith("1.0") || line.EndsWith("1.1")) || (line.StartsWith("PUT /") && sr.Peek() >= 0) )
                {
                    whois = false;
                    
                    if (line.Contains("PUT /"))
                    {
                        char[] stringSeparators = { '/' };
                        string[] lines = line.Split(stringSeparators, StringSplitOptions.None);
                        username = lines[1];
                        sr.ReadLine();

                        try
                        {
                            while (sr.Peek() > -1)
                            {
                                string temp = sr.ReadLine();
                                location += temp;

                            }
                        }
                        catch(IOException e)
                        {
                            Console.WriteLine(e);
                        }
                        TheLocations[username] = location;
                        sw.WriteLine("HTTP/0.9 200 OK\r\nContent-Type: text/plain\r\n");
                        sw.Flush();
                      
                    }
                    if(line.Contains("HTTP/1.0"))
                    {
                        string[] stringSeparators = new string[] { "/", " " };
                        string[] lines = line.Split(stringSeparators, StringSplitOptions.None);
                        username = lines[2];
                        string contentlength = sr.ReadLine();
                        try
                        {
                            string temp;
                            while((temp = sr.ReadLine()) != "")
                            {

                            }
                        }
                        catch
                        {

                        }
                        try
                        {
                                string[] sensible = contentlength.Split(stringSeparators, StringSplitOptions.None);
                                char[] locationchars = new char[int.Parse(sensible[1])];
                            for(int i = 0; i<locationchars.Length; i++)
                            {
                                locationchars[i] = (char)sr.Read();
                            }
                            foreach(var ch in locationchars)
                            {
                                location += ch;
                            }
                        }
                        catch (IOException e)
                        {
                            Console.WriteLine(e);
                        }
                        
                          TheLocations[username] = location;
                       
                        sw.WriteLine("HTTP/1.0 200 OK\r\nContent-Type: text/plain\r\n");
                        sw.Flush();
                    }
                    if (line.Contains("HTTP/1.1"))
                    {
                        string temp = "";
                        char[] stringSeparators = { '=', '&' };
                        while (sr.Peek() > -1)
                        {
                            temp = sr.ReadLine();

                        }
                        string[] sensible = temp.Split(stringSeparators, StringSplitOptions.None);
                        username = sensible[1];
                        TheLocations[username] = sensible[3];

                        sw.WriteLine("HTTP/1.1 200 OK\r\nContent-Type: text/plain\r\n");
                        sw.Flush();
                    }



                }



                else if (whois == true)
                {
                    username = Words[0];
                    if (Words.Length == 1)
                    {
                        if (TheLocations.ContainsKey(username))
                        {
                            // the location can be found in the dictionary
                            location = TheLocations[username];
                            sw.WriteLine(location);
                            sw.Flush();
                        }

                        else
                        {
                            // Oh no! This user is not known
                            location = "ERROR: no entries found";
                            sw.WriteLine(location);
                            sw.Flush();
                        }
                    }

                    else if (Words.Length == 2)
                    {
                        // We have an update
                        location = Words[1];
                        if (TheLocations.ContainsKey(username))
                        {// the location can be found in the dictionary
                            TheLocations[username] = location;
                        }
                        else
                        {
                            TheLocations.Add(username, location);
                        }
                            
                        sw.WriteLine("OK");
                        sw.Flush();
                    }
                    else
                    {
                        Console.WriteLine("There are no words");
                        // Something is wrong as there are no words!
                    }
                }

            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
          
        }

        static void Main(string[] args)
        {
            runServer();
        }
    }
}
