using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace SkyGate.Music
{
    public class SongData
    {
        private SongData(string name, int bpm, string currentPath, string musicFileExtension, NoteData[] notes)
        {
            Name = name;
            BPM = bpm;
            _currentPath = currentPath;
            MusicFileExtension = musicFileExtension;
            _notes = notes.ToList();
        }

        public static SongData FromMusicFile(FileInfo file)
        {
            var extension = file.Extension[1..];
            var targetName = file.Name[..^file.Extension.Length];
            var path = $"{Application.persistentDataPath}/Songs/{Guid.NewGuid()}";
            var songData = new SongData(
                name: targetName,
                bpm: 200,
                currentPath: path,
                musicFileExtension: extension,
                notes: Array.Empty<NoteData>()
            );

            if (!Directory.Exists($"{Application.persistentDataPath}/Songs"))
            {
                Directory.CreateDirectory($"{Application.persistentDataPath}/Songs");
            }
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            File.Copy(file.FullName, $"{path}/song.{extension}");

            return songData;
        }

        public static SongData FromGameFile(FileInfo file)
        {
            using FileStream fi = new(file.FullName, FileMode.Open, FileAccess.Read);
            using BinaryReader reader = new(fi);

            reader.ReadByte(); // Version
            string name = reader.ReadString();
            string extension = reader.ReadString();
            int bpm = reader.ReadInt32();
            var notesCount = reader.ReadInt32();
            var notes = new NoteData[notesCount];
            for (int i = 0; i < notesCount; i++)
            {
                notes[i] = new NoteData()
                {
                    Line = reader.ReadInt32(),
                    Y = reader.ReadSingle()
                };
            }

            reader.Close();
            fi.Close();
            return new SongData(
                name: name,
                bpm: bpm,
                currentPath: file.Directory.FullName,
                musicFileExtension: extension,
                notes: notes
            );
        }

        public void Save()
        {
            var savePath = $"{_currentPath}/data.bin";
            using FileStream fi = new(savePath, FileMode.OpenOrCreate, FileAccess.Write);
            using BinaryWriter writer = new(fi);

            writer.Write(_version);
            writer.Write(Name);
            writer.Write(MusicFileExtension);
            writer.Write(BPM);
            writer.Write(_notes.Count);
            foreach (var note in _notes)
            {
                writer.Write(note.Line);
                writer.Write(note.Y);
            }

            writer.Flush();
            writer.Close();
            fi.Close();
            Debug.Log($"File saved to {savePath}");
        }

        public string AudioClipPath => $"{_currentPath}/song.{MusicFileExtension}";

        public int BPM { set; get; }
        public string Name { private set; get; }
        public string MusicFileExtension { private set; get; }
        private string _currentPath;
        private List<NoteData> _notes;

        private const byte _version = 1;
    }
}
