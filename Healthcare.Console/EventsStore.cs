using System;
using System.Collections.Generic;
using System.IO;

namespace App;

sealed class EventStore
{
    public readonly string Path;

    public EventStore(string path)
    {
        Path = path;
        EnsureFolder();
    }

    private void EnsureFolder()
    {
        string? folder = System.IO.Path.GetDirectoryName(Path);
        if (folder == null) return;
        if (folder.Length == 0) return;

        Directory.CreateDirectory(folder);
    }

    public List<string> LoadAll()
    {
        List<string> lines = new List<string>();

        if (!File.Exists(Path))
            return lines;

        string[] fileLines = File.ReadAllLines(Path);

        for (int i = 0; i < fileLines.Length; i++)
        {
            string line = fileLines[i].Trim();
            if (line.Length == 0) continue;

            lines.Add(line);
        }

        return lines;
    }

    public void Append(string line)
    {
        //Console.WriteLine("Appending to file: " + System.IO.Path.GetFullPath(Path));
        File.AppendAllText(Path, line + Environment.NewLine);
    }

}
