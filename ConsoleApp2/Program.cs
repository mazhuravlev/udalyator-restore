using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

class Program
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        var path = @"\\SA\es8\D\удаленное";
        var files = GetVaildFilesInDirectory(path);
        Console.WriteLine("found files: " + files.Count);
        Console.WriteLine("copynig files..");
        foreach(var filePath in files)
        {
            var copyPath = MakeCopyPath(filePath);
            var dir = Path.GetDirectoryName(copyPath);
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            File.Copy(filePath, copyPath);
        }
        Console.WriteLine("done!");
        Console.ReadKey();
    }

    static List<String> GetVaildFilesInDirectory(string path)
    {
        var extensions = new List<String> { ".pdf", ".jpg", ".doc", ".docx", ".dwg" };
        var files = Directory.GetFiles(path)
               .Where(f => extensions.Contains(Path.GetExtension(f)))
               .Where(f => IsValidFile(f))
               .ToList();
        var dirs = Directory.GetDirectories(path);
        if(dirs.Length > 0)
        {
            foreach(var dir in dirs)
            {
                Console.Write(">");
                files.AddRange(GetVaildFilesInDirectory(dir));
            }
        }
        return files;
    }

    static string MakeCopyPath(string path)
    {
        return @"\\SA\es8\filtered" + path.Substring(8);
    }

    static bool IsValidFile(string fileName)
    {
        var extension = Path.GetExtension(fileName);
        switch (extension)
        {
            case ".pdf":
                return FileStartsWithString(fileName, "%PDF");
            case ".dwg":
                return FileStartsWithString(fileName, "AC10");
            case ".docx":
                return FileStartsWithString(fileName, "PK");
            case ".doc":
                return FileStartsWithByteArray(fileName, new byte[] {208,207,17,224,161,177,26,225});
            case ".jpg":
                return FileStartsWithByteArray(fileName, new byte[] {255, 216, 255});
            default:
                throw new Exception("file type not supported");
        }
    }

    static bool FileStartsWithString(string fileName, string s)
    {
        try
        {
            using (FileStream fs = File.Open(fileName, FileMode.Open))
            {
                byte[] array = new byte[s.Length];
                var n = fs.Read(array, 0, s.Length);
                if (n < s.Length) return false;
                var isValid = System.Text.Encoding.UTF8.GetString(array).Equals(s);
                Console.Write(isValid ? "o" : ".");
                return isValid;
            }
        }
        catch (System.IO.IOException)
        {
            Console.Write("x");
            return false;
        }
    }

    static bool FileStartsWithByteArray(string fileName, byte[] a)
    {
        try
        {
            using (FileStream fs = File.Open(fileName, FileMode.Open))
            {
                byte[] array = new byte[a.Length];
                var n = fs.Read(array, 0, a.Length);
                if (n != a.Length) return false;
                var isValid = a.SequenceEqual(array);
                Console.Write(isValid ? "o" : ".");
                return isValid;
            }
        } catch(System.IO.IOException)
        {
            Console.Write("x");
            return false;
        }
    }
}