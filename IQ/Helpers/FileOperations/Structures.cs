namespace IQ.Helpers.FileOperations
{
    public class Structures
    {
        #region iqxFile
        public struct IQXFile
        {
            /// <summary>
            /// Header for .IQX File.
            /// </summary>
            public char[] iqxHeader;
            public ushort iqxFileVersion;

            /// <summary>
            /// Data Values for .IQX File.
            /// </summary>
            public string ServerName;
            public ushort PortNumber;
            public string DatabaseName;
            public string Username;
            public ulong Password;
            public string ConnectionString;
        }
        #endregion
    }
}
