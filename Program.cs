//Written for Captain Lycop : Invasion of the Heters. https://store.steampowered.com/app/525070
using System;
using System.IO;
using System.IO.Compression;

namespace Lycop_Extractor
{
    class Program
    {
        static BinaryReader br;
        static int size, n;
        static string path, type;
        static void Main(string[] args)
        {
            br = new BinaryReader(File.OpenRead(args[0]));
            br.BaseStream.Position = 20;

            n = 0;
            path = Path.GetDirectoryName(args[0]) + "\\" + Path.GetFileNameWithoutExtension(args[0]);
            Directory.CreateDirectory(path);   
            while (br.BaseStream.Position < br.BaseStream.Length)
            {
                var variable = br.ReadBytes(4);
                Array.Reverse(variable);
                size = BitConverter.ToInt32(variable, 0);

                type = GetType();
                if (type == ".zlib")
                {
                    variable = br.ReadBytes(4);
                    Array.Reverse(variable);
                    int sizeUncompressed = BitConverter.ToInt32(variable, 0);

                    br.ReadInt16();
                    MemoryStream ms = new();
                    using (var ds = new DeflateStream(new MemoryStream(br.ReadBytes(size - 6)), CompressionMode.Decompress))
                        ds.CopyTo(ms);

                    long position = br.BaseStream.Position;
                    size = sizeUncompressed;

                    br = new(ms);
                    br.BaseStream.Position = 0;
                    type = GetType();
                    WriteFile();
                    br = new BinaryReader(File.OpenRead(args[0]));
                    br.BaseStream.Position = position;
                }
                else
                    WriteFile();
                n++;
            }
        }

        static void WriteFile()
        {
            BinaryWriter bw = new(File.Create(path + "\\" + n + type));
            bw.Write(br.ReadBytes(size));
            bw.Close();
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

                    br.BaseStream.Position -= 6;
                    return ".zlib";
            }
        }
    }
}
