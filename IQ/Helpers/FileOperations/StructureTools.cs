using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace IQ.Helpers.FileOperations
{
    public class StructureTools
    {
        #region IQXFile
        public static Structures.IQXFile UserDataStoreToIQX(string[] DataStore)
        {
            Structures.IQXFile file = new Structures.IQXFile(); //Create New .IQX File

            //Convert Values To File Structure
            file.ServerName = DataStore[0];
            file.PortNumber = ushort.Parse(DataStore[1]);
            file.DatabaseName = DataStore[2];
            file.Username = DataStore[3];
            file.Password = ulong.Parse(DataStore[4]);
            file.ConnectionString = DataStore[5];

            //Header
            file.iqxFileVersion = 1;
            char[] Header = { 'I', 'Q', 'X' };
            file.iqxHeader = Header;
            //Return .IQX File
            return file;
        }

        public static byte[] IQXFileToBytes(Structures.IQXFile file)
        {
            MemoryStream memoryStream = new MemoryStream(); //Create Memory Stream
            BinaryWriter writer = new BinaryWriter(memoryStream); //Make Binary Writer to Write to Stream

            //Write Data to .IQXFile
            writer.Write(file.iqxHeader);
            writer.Write(file.iqxFileVersion);
            writer.Write(file.ServerName);
            writer.Write(file.PortNumber);
            writer.Write(file.DatabaseName);
            writer.Write(file.Username);
            writer.Write(file.Password);
            writer.Write(file.ConnectionString);

            return memoryStream.ToArray();
        }

        public static Structures.IQXFile BytesToIQXFile(byte[] file)
        {
            MemoryStream memoryStream = new MemoryStream(file); //Create Memory Stream
            BinaryReader reader = new BinaryReader(memoryStream); //Make Binary Writer to Write to Stream
            Structures.IQXFile iQXFile = new Structures.IQXFile();

            //Read Data from .IQXFile
            iQXFile.iqxHeader = reader.ReadChars(3);
            iQXFile.iqxFileVersion = reader.ReadUInt16();
            iQXFile.ServerName = reader.ReadString();
            iQXFile.PortNumber = reader.ReadUInt16();
            iQXFile.DatabaseName = reader.ReadString();
            iQXFile.Username = reader.ReadString();
            iQXFile.Password = reader.ReadUInt64();
            iQXFile.ConnectionString = reader.ReadString();

            return iQXFile;
        }
        #endregion
    }
}
