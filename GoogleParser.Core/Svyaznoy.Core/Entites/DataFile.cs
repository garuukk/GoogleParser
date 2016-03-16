namespace Svyaznoy.Core.Entites
{
    using System.IO;
    using Svyaznoy.Core.Web;

    public class DataFile
    {
        public string Name { get; set; }

        public byte[] Body { get; set; }

        public string Mime { get; set; }

        public static DataFile ReadFromFile(string file)
        {
            return new DataFile()
            {
                Body = File.ReadAllBytes(file),
                Name = Path.GetFileName(file),
                Mime = file.GetMimeType()
            };
        }
    }
}
