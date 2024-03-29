﻿using SkyGate.Game;
using SkyGate.Music;
using TMPro;
using UnityEngine;

namespace SkyGate.SongEditor
{
    public class NoteEditorUI : MonoBehaviour
    {
        [SerializeField]
        private TMP_InputField _lineInput, _yPosInput;

        [SerializeField]
        private TMP_Text _idText;

        private NoteData _noteData;

        public void Init(NoteData note)
        {
            _noteData = note;
            _idText.text = note._id.ToString();
            _lineInput.text = note.Line.ToString();
            _yPosInput.text = note.Y.ToString();
        }

        public void OnLineEdit(string value)
        {
            if (int.TryParse(value, out int parseValue))
            {
                _noteData.Line = parseValue;
                GridManager.Instance.UpdatePosition(_noteData);
            }
        }

        public void OnYEdit(string value)
        {
            if (float.TryParse(value, out float parseValue))
            {
                _noteData.Y = parseValue;
                GridManager.Instance.UpdatePosition(_noteData);
            }
        }
    }
}
