using System;

namespace KindleNoteExtractor // Note: actual namespace depends on the project name.
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //TODO: remove me!
            args = new string[1] { "C:\\Users\\Cole\\Desktop\\My Clippings.txt" };


            if (args.Length == 0) {
                Console.WriteLine("Please supply the full path to a clippings file. Example usage:");
                Console.WriteLine("    > KindleNoteExtractor.exe \"C:\\Users\\Me\\Desktop\\My Clippings.txt\"");
            } else {
                var extractor = new Extractor(args[0]);
                extractor.Execute();
            }
        }
    }
}