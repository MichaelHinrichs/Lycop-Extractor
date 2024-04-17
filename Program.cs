//Written for Captain Lycop : Invasion of the Heters. https://store.steampowered.com/app/525070
using System;
using System.IO;

namespace Lycop_Extractor
{
    class Program
    {
        static void Main(string[] args)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(args[0]));
            br.BaseStream.Position = 20;

            int n = 0;
            string path = Path.GetDirectoryName(args[0]) + "\\" + Path.GetFileNameWithoutExtension(args[0]);
            Directory.CreateDirectory(path);   
            while (br.BaseStream.Position < br.BaseStream.Length)
            {
                var variable = br.ReadBytes(4);
                Array.Reverse(variable);
                int size = BitConverter.ToInt32(variable, 0);

                BinaryWriter bw = new(File.Create(path + "\\" + n));

                bw.Write(br.ReadBytes(size));
                bw.Close();
                n++;
            }
        }
    }
}
