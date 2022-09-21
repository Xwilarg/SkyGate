using SkyGate.Music;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace SkyGate.Game
{
    public class GridManager : MonoBehaviour
    {
        public static GridManager Instance { get; private set; }

        [SerializeField]
        private PlayerLine[] _lines;

        [SerializeReference]
        private GameObject _horizontalLinePrefab, _lightHorizontalLinePrefab;

        [SerializeField]
        private GameObject _notePrefab;

        [SerializeField]
        private Transform _gridContainer;

        [SerializeField]
        private AudioSource _sfxPlayer;

        [SerializeField]
        private AudioClip _hitSound;

        private readonly List<RectTransform> _horizontalLines = new();
        private readonly List<GridNoteData> _notes = new();

        private const int _sublineCount = 8;
        private const float _yNoteSize = 20f;

        public UnityEvent OnGridReset = new();

        private float GlobalTime => MusicManager.Instance.TimeElapsed * MusicManager.Instance.BPM;

        public IEnumerable<NoteData> GetNotes()
        {
            foreach (var note in _notes)
            {
                yield return note.NoteData;
            }
        }

        private void Awake()
        {
            Instance = this;
        }

        private void SpawnNote(NoteData note)
        {
            var noteGo = Instantiate(_notePrefab, _lines[note.Line].transform);
            var noteRT = (RectTransform)noteGo.transform;
            noteRT.sizeDelta = new(noteRT.sizeDelta.x, _yNoteSize);
            noteRT.anchoredPosition = new(0f, note.Y * MusicManager.Instance.BPM - GlobalTime);
            _notes.Add(new() { NoteData = note, RectTransform = noteRT });
        }

        public void UpdatePosition(NoteData note)
        {
            var noteData = _notes.FirstOrDefault(x => x.NoteData == note);
            if (noteData != null)
            {
                noteData.RectTransform.SetParent(_lines[noteData.NoteData.Line].transform);
                noteData.RectTransform.anchoredPosition = new(0f, note.Y * MusicManager.Instance.BPM - GlobalTime);
            }
            else
            {
                SpawnNote(note);
            }
        }

        public void ResetGrid()
        {
            // Clean
            foreach (var line in _horizontalLines)
            {
                Destroy(line.gameObject);
            }
            _horizontalLines.Clear();
            foreach (var line in _notes)
            {
                Destroy(line.RectTransform.gameObject);
            }
            _notes.Clear();

            NoteData.ID = 0;

            var timeElapsed = MusicManager.Instance.TimeElapsed; // Time elapsed since the start of the song
            var globalTime = timeElapsed * MusicManager.Instance.BPM; // Total scroll to go to the current song position
            var relativeTime = globalTime % MusicManager.Instance.BPM; // Relative position

            // Spawn horizontal lines
            for (int i = 0; i < (Screen.height / MusicManager.Instance.BPM) + MusicManager.Instance.BPM; i++)
            {
                var go = Instantiate(_horizontalLinePrefab, _gridContainer);
                var goRT = (RectTransform)go.transform;
                goRT.anchoredPosition = new Vector2(0f, i * MusicManager.Instance.BPM - relativeTime);
                _horizontalLines.Add(goRT);

                for (int j = 0; j < _sublineCount - 1; j++)
                {
                    var goLight = Instantiate(_lightHorizontalLinePrefab, _gridContainer);
                    var goLightRT = (RectTransform)goLight.transform;
                    goLightRT.anchoredPosition = new Vector2(0f, (i * MusicManager.Instance.BPM) + ((j + 1) * MusicManager.Instance.BPM / _sublineCount) - relativeTime);
                    _horizontalLines.Add(goLightRT);
                }
            }

            foreach (var note in MusicManager.Instance.Notes)
            {
                SpawnNote(note);
            }

            OnGridReset.Invoke();
        }

        private void Update()
        {
            if (MusicManager.Instance.IsPlaying)
            {
                for (int i = 0; i < _horizontalLines.Count; i++)
                {
                    var previous = i == 0 ? _horizontalLines.Last() : _horizontalLines[i - 1];
                    var rTransform = _horizontalLines[i];
                    rTransform.anchoredPosition = new(0f, rTransform.anchoredPosition.y - (Time.deltaTime * MusicManager.Instance.BPM));
                    if (rTransform.anchoredPosition.y < 0f) // We move the line back to the top
                    {
                        rTransform.anchoredPosition = new(0f, ((RectTransform)previous.transform).anchoredPosition.y + MusicManager.Instance.BPM / _sublineCount);
                    }
                }

                for (int i = _notes.Count - 1; i >= 0; i--)
                {
                    var note = _notes[i].RectTransform;
                    note.anchoredPosition = new(0f, note.anchoredPosition.y - (Time.deltaTime * MusicManager.Instance.BPM));
                    if (note.anchoredPosition.y < 0f)
                    {
                        Destroy(note.gameObject);
                        _notes.RemoveAt(i);
                    }
                }
            }
        }

        public float GetGridIndex(float y)
        {
            var timeElapsed = MusicManager.Instance.TimeElapsed; // Time elapsed since the start of the song
            var globalTime = timeElapsed * MusicManager.Instance.BPM; // Total scroll to go to the current song position

            return Mathf.Floor((y + globalTime) / MusicManager.Instance.BPM * _sublineCount) / _sublineCount;
        }

        public int GetLine(float x)
        {
            for (int i = 0; i < _lines.Length; i++)
            {
                var line = _lines[i];
                var rTransform = (RectTransform)line.transform;
                var demSize = rTransform.sizeDelta.x / 2f;
                if (x >= rTransform.position.x - demSize && x <= rTransform.position.x + demSize)
                {
                    return i;
                }
            }
            return -1;
        }

        public float LineWidth => ((RectTransform)_lines[0].transform).sizeDelta.x;

        private void PressLine(InputAction.CallbackContext value, int id)
        {
            if (value.phase == InputActionPhase.Started)
            {
                _lines[id].ShowMark(true);
                var targetNote = _notes.Where(x => x.NoteData.Line == id).OrderBy(x => x.NoteData.Y).FirstOrDefault();
                if (targetNote != null)
                {
                    var distance = Mathf.Abs(targetNote.RectTransform.anchoredPosition.y - _lines[id].YPos);
                    int score = 0;
                    if (distance < _yNoteSize)
                    {
                        score = 100;
                    }
                    else if (distance < _yNoteSize * 2f)
                    {
                        score = 50;
                    }
                    else if (distance < _yNoteSize * 4f)
                    {
                        score = 25;
                    }
                    if (score > 0)
                    {
                        Destroy(targetNote.RectTransform.gameObject);
                        _notes.RemoveAll(x => x.NoteData == targetNote.NoteData);
                    }
                }
                _sfxPlayer.clip = _hitSound;
                _sfxPlayer.Play();
            }
            else if (value.phase == InputActionPhase.Canceled)
            {
                _lines[id].ShowMark(false);
            }
        }

        public void OnFirstLine(InputAction.CallbackContext value) => PressLine(value, 0);
        public void OnSecondLine(InputAction.CallbackContext value) => PressLine(value, 1);
        public void OnThirdLine(InputAction.CallbackContext value) => PressLine(value, 2);
        public void OnFourthLine(InputAction.CallbackContext value) => PressLine(value, 3);
        public void OnFifthLine(InputAction.CallbackContext value) => PressLine(value, 4);
        public void OnSixthLine(InputAction.CallbackContext value) => PressLine(value, 5);
    }
}
