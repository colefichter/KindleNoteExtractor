using System;
using System.ComponentModel.Design.Serialization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace KindleNoteExtractor;

public class Extractor
{
    private const string OUTPUT_FILE_EXTENSION = ".md";
    private const char ILLEGAL_CHAR = (char) 0xfeff;
    private const string END_OF_NOTE = "==========";
    private readonly string? _inputPath;

    public Extractor(string inputFilePath) {
        this._inputPath = inputFilePath;
    }

    public void Execute() {
        string[] notes = ReadFile(this._inputPath!);
        var destinationFolder = CreateOutputFolder();

        ProcessNotes(notes, destinationFolder);
    }

    private void ProcessNotes(string[] data, string destinationFolder) {
        var parsedNotes = Parse(data);
        WriteNotesToDisk(parsedNotes, destinationFolder);
    }

    private static void WriteNotesToDisk(Dictionary<string, List<string>> notes, string destinationFolder) {
        foreach (string k in notes.Keys) {
            var fileName = CleanFileName(k);
            var path = Path.Combine(destinationFolder, fileName + OUTPUT_FILE_EXTENSION);

            var text = string.Join("\n", notes[k]).Trim();
            text = $"{MakeHeader(k)}{text}";

            File.WriteAllText(path, text);
        }
    }

    private Dictionary<string, List<string>> Parse(string[] data) {
        // Store the notes in a dict that uses the book title as key and contains a list of all the notes for that particular book.
        var notes = new Dictionary<string, List<string>>();

        int i = 0;
        string? title = null;
        string metaData = string.Empty;
        string note = string.Empty;

        while (i < data.Length) {
            var line = data[i].Trim().Replace(ILLEGAL_CHAR.ToString(), string.Empty);

            // Each note beings with the book title on it's own line, followed by a metadata line that begins with "- ".
            if (i == 0 || title == null) {
                title = line;
                i += 1;
                metaData = data[i].Trim();
                note = string.Empty;

                // Skip meta-data line.
                i += 1;
                continue;
            }

            if (line.Length == 0) {
                i += 1;
                continue;
            }

            if (line == END_OF_NOTE) { // End of the note is demarcated with "==========".
                string formattedNote = CleanupNoteFormat(note, metaData); // note should already end with \n
                
                if (notes.ContainsKey(title)) {
                    notes[title].Add(formattedNote);
                } else {
                    var newList = new List<string>();
                    newList.Add(formattedNote);
                    notes.Add(title, newList);
                }

                title = null; // Prepare to start parsing a new note section.
            } else {
                // Append note lines as needed.
                note = note + line + "\n";
            }

            i += 1;
        }

        return notes;
    }

    private static string[] ReadFile(string path)  {
        return File.ReadLines(path).ToArray();
    }

    private static string CreateOutputFolder() {
        var path = Environment.CurrentDirectory;
        var directory = "My Kindle Notes";
        var destination = Path.Combine(path, directory);

        if (Directory.Exists(destination)) {
            Directory.Delete(destination, true);
        }

        Directory.CreateDirectory(destination);

        return destination;
    }

    private static string CleanFileName(string fileName) {
        return string.Join("_", fileName.Split(Path.GetInvalidFileNameChars()));
    }

    private static string CleanupNoteFormat(string note, string metaData) {
        return note + CondenseMetaData(metaData) + "\n";
    }

    private static string CondenseMetaData(string metaData) {
        var match = Regex.Match(metaData, @"\d+");
        return "- Page " + match.Value;
    }

    private static string MakeHeader(string title) {
        var header = new string('=', title.Length);
        var date = DateTime.Now.ToString();
        return title + "\n" + header + "\n\nExtracted automatically on " + date + "\n\n\n";
    }
}