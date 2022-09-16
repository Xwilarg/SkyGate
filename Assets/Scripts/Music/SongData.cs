using System.IO;
using UnityEngine;

namespace SkyGate.Music
{
    public class SongData
    {
        private SongData(string name, int bpm)
        {
            Name = name;
            BPM = bpm;
        }

        public static SongData FromFileInfo(FileInfo file)
        {
            return new SongData(
                name: file.Name[..^file.Extension.Length],
                bpm: 200
            );
        }

        public static SongData FromFile(string path)
        {
            using FileStream fi = new(path, FileMode.Open, FileAccess.Read);
            using BinaryReader reader = new(fi);
            reader.ReadInt32();
            string name = reader.ReadString();
            int bpm = reader.ReadInt32();
            reader.Close();
            fi.Close();
            return new SongData(
                name: name,
                bpm: bpm
            );
        }

        public void Save()
        {
            var savePath = $"{Application.persistentDataPath}/{Name}.bin";
            using FileStream fi = new(savePath, FileMode.OpenOrCreate, FileAccess.Write);
            using BinaryWriter writer = new(fi);
            writer.Write(_version);
            writer.Write(Name);
            writer.Write(BPM);
            writer.Flush();
            writer.Close();
            fi.Close();
            Debug.Log($"File saved to {savePath}");
        }

        public int BPM { private set; get; }
        public string Name { private set; get; }

        private const byte _version = 1;
    }
}
