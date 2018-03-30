/// <summary>
/// author: Benjamin T. Hoang
/// Class: CS 490
/// Description: this program parses file(s) by header and footer signature.
/// </summary>
/// 

using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace DataGrepperV2
{
    class Program
    {
        //signatures
        const int empty = 0x00;
        const int J = 0x4a;
        const int O = 0x4f;
        const int S = 0x53;
        const int H = 0x48;
        const int N = 0x4e;
        const int E = 0x45;

        static void Main(string[] args)
        {
            List<Filee> filees = new List<Filee>();

            Console.WriteLine("Loading Data...");
            byte[] file = File.ReadAllBytes("jji_project.001");

            SignatureHeaderMarker(file, ref filees);
            SignatureFooterMarker(file, ref filees);
            GetBuffers(file, ref filees);
            WriteCSV(ref filees);
            WriteFile(ref filees);
        }

        public static void SignatureHeaderMarker(byte[] file, ref List<Filee> filees)
        {

            for (int i = 0; i < file.LongLength; i++)
            {
                Filee temp = new Filee();
                if (file[i] == empty
                    && file[i + 1] == J
                    && file[i + 2] == empty
                    && file[i + 3] == O
                    && file[i + 4] == empty
                    && file[i + 5] == S
                    && file[i + 6] == empty
                    && file[i + 7] == H
                    )
                {
                    temp.Start = i;
                    temp.Name = "file"+i;
                    filees.Add(temp);
                }
            }
        }

        public static void SignatureFooterMarker(byte[] file, ref List<Filee> filees)
        {
            for(int i = 0; i < filees.Count; i++)
            {
                List<byte> buffers = new List<byte>();

                for (int j = filees[i].Start; j < file.LongLength; j++)
                {
                    if (   file[j] == J
                        && file[j + 1] == empty
                        && file[j + 2] == O
                        && file[j + 3] == empty
                        && file[j + 4] == N
                        && file[j + 5] == empty
                        && file[j + 6] == E
                        && file[j + 7] == empty
                        && file[j + 8] == S
                        && file[j + 9] == empty)
                    {
                        filees[i].End = j + 9;
                        break;
                    }
                }
            }
        }

        public static void GetBuffers(byte[] file, ref List<Filee> filees)
        {
            for (int i = 0; i < filees.Count; i++)
            {
                if (filees[i].Start != -1 && filees[i].End != -1)
                {
                    

                    //assuming List<T> max size is ~2bil
                    List<byte> temp = new List<byte>();
                    for (int j = filees[i].Start; j < filees[i].End; j++)
                    {
                        temp.Add(file[j]);
                    }

                    filees[i].Buffers = temp.ToArray();
                    filees[i].Name = "file" + (i+1);

                    ///set md5
                    using (MD5 md5Hash = MD5.Create())
                    {
                        filees[i].Md5 = GetMd5Hash(md5Hash, filees[i].Buffers);
                    }
                }
                else
                {
                    //remove bad file at i
                    filees.RemoveAt(i);
                    Console.WriteLine("Removed corrupted file: file" + (i + 1));
                }
            }



        }

        /// <summary>
        ///ref https://msdn.microsoft.com/en-us/library/s02tk69a(v=vs.110).aspx 
        ///check file for MD5 hash 
        /// </summary>
        static string GetMd5Hash(MD5 md5Hash, byte[] input)
        {

            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hash.ComputeHash(input);

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

        static void WriteFile(ref List<Filee> filees)
        {
            foreach (Filee f in filees)
            {
               File.WriteAllBytes(f.Name + ".jji", f.Buffers);
               Console.WriteLine("File: " + f.Name + "   Size(bytes): "+f.Buffers.Length+"       Hash: " + f.Md5);
            }
            Console.WriteLine("Final total: " + filees.Count);
            Console.WriteLine("Press Any Key to Continue...");
            Console.ReadKey();
        }

        static void WriteCSV(ref List<Filee> filees)
        {
            using (var w = new StreamWriter("file Stats.csv"))
            {
                w.WriteLine("Md5,Size");
                foreach (Filee f in filees)
                { 
                    w.WriteLine(f.Md5 + "," + f.Buffers.Length.ToString());
                    w.Flush();
                }
            }
        }
    }
}
