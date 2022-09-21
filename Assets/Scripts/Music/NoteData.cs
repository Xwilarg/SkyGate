using System;

namespace SkyGate.Music
{
    public class NoteData
    {
        public NoteData(int line, float y)
        {
            Line = line;
            Y = y;
            _id = ID++;
        }

        public int Line { set; get; }
        public float Y { set; get; }
        public int _id { private set; get; }

        public static int ID = 0;

        public static bool operator ==(NoteData a, NoteData b)
        {
            if (a is null)
            {
                return b is null;
            }
            if (b is null)
            {
                return false;
            }
            return a._id == b._id;
        }
        public static bool operator !=(NoteData a, NoteData b) => !(a == b);

        public override bool Equals(object obj)
            => obj is NoteData data && _id == data._id;

        public override int GetHashCode()
        {
            return HashCode.Combine(_id);
        }
    }
}
