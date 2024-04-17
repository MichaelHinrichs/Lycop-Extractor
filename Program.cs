//Written for Captain Lycop : Invasion of the Heters. https://store.steampowered.com/app/525070
using System;
using System.IO;

namespace Lycop_Extractor
{
    class Program
    {
        static BinaryReader br;
        static void Main(string[] args)
        {
            br = new BinaryReader(File.OpenRead(args[0]));
            br.BaseStream.Position = 20;

            int n = 0;
            string path = Path.GetDirectoryName(args[0]) + "\\" + Path.GetFileNameWithoutExtension(args[0]);
            Directory.CreateDirectory(path);   
            while (br.BaseStream.Position < br.BaseStream.Length)
            {
                var variable = br.ReadBytes(4);
                Array.Reverse(variable);
                int size = BitConverter.ToInt32(variable, 0);

                string type = GetType();
                BinaryWriter bw = new(File.Create(path + "\\" + n + type));

                if(type == ".zlib")
                    type -= 4;

                bw.Write(br.ReadBytes(size));
                bw.Close();
                n++;
            }
        }

        static new string GetType()
        {
            byte[] magicBytes = br.ReadBytes(4);
            br.BaseStream.Position -= 4;
            string magic = new(System.Text.Encoding.UTF7.GetString(magicBytes));
            switch (magic)
            {
                case "OggS":
                    return ".ogg";
                case "\u0089PNG":
                    return ".png";
                case "RIFF":
                    return ".wav";
                default:
                    br.BaseStream.Position += 4;
                    magicBytes = br.ReadBytes(2);
                    if(magicBytes[0] != 0x78 || magicBytes[1] != 0x9C)
                        throw new Exception("Unrecognized subfile type.");

                    br.BaseStream.Position -= 2;
                    return ".zlib";
            }
        }
    }
}
