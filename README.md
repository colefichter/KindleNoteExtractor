Kindle Note Extractor
=====================

This program will parse the notes from your kindle into separate well-formatted files, organized by book title.

Ouput format is .md.

Pre-requisites
--------------

Requires dotnet 8.

Usage
-----

Via USB connection, find Documents\My Clippings.txt on your kindle and copy it somewhere local, e.g. your desktop.

From this folder, run:

> .\bin\Debug\net8.0\KindleNoteExtractor.exe "C:\Users\Cole\Desktop\My Clippings.txt"

All output is placed into a subfolder called "My Kindle Notes".