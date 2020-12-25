using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using AI;
using MIDIFramework;
using NAudio.Midi;

namespace Piano
{
    public partial class Form1 : Form
    {
        const string pathBase = @"..\..\Data";
        MIDISynthesizer midi;
        public Form1()
        {
            InitializeComponent();
            Init();
        }

        public void Init()
        {
            midi = new MIDISynthesizer();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var notes = new List<NoteEvent>();
            //var r = new List<KeyValuePair<string, int>>();
            //for (int i = 0; i < 128; i++)
            //{
            //    var note = new NAudio.Midi.NoteEvent(1, 1, MidiCommandCode.NoteOn, i, 100);
            //    var name = note.NoteName;
            //    var dig = int.Parse(name.Last()+"");
            //    if(name.Length == 2 && dig > 2 && dig < 7)
            //        richTextBox1.Text += $"{{ \"{note.NoteName}\", {i} }},\n";
            //    //r.Add(new KeyValuePair<string, int>(note.NoteName, note.GetAsShortMessage()));
            //}
            //return;

            //MIDIReader reader = new MIDIReader($@"{pathBase}\Пираты Карибского моря - Главная тема [MIDISTOCK.RU].mid");
            //MIDIReader reader = new MIDIReader($@"{pathBase}\Кино - Группа крови [MIDISTOCK.RU].mid");
            //MIDIReader reader = new MIDIReader($@"{pathBase}\Grand Theft Auto_ San Andreas - Intro (вступление) [MIDISTOCK.RU].mid");
            //MIDIReader reader = new MIDIReader($@"{pathBase}\Super Mario 64 - Medley.mid");
            //MIDIReader reader = new MIDIReader($@"{pathBase}\Бумер - Саундтрек из фильма [MIDISTOCK.RU].mid");

            //var channels = reader.GetNoteEventsWithRealTime();
            //var channels = reader.GetNoteEvents();
            //for (int ch = 0; ch < channels.Count; ch++)
            //    for (int i = 0; i < channels[ch].Count; i++)
            //        notes.Add(channels[ch][i]);

            int t = 0;
            int time = 50;
            foreach (var path in Directory.GetFiles(pathBase))
            {
                MIDIReader reader = new MIDIReader(path);
                var channels = reader.GetNoteEventsWithRealTime();
                notes = new List<NoteEvent>();
                for (int ch = 0; ch < channels.Count; ch++)
                    for (int i = 0; i < channels[ch].Count; i++)
                        notes.Add(channels[ch][i]);
                notes = MidiConverter.ToBaseNotes(notes);
                notes = MidiConverter.ToNewGrid(notes, time);

                var matr = MidiConverter.ToMatrix(notes, time, 0, 10000);
                Vector.SaveAsBinary($"output\\m{t++}.matr", matr.ToVector());

            }

            //notes = MidiConverter.ToNoteEvents(matr, time);

            //группируем и зырим
            //var count = notes
            //    .Select(x => int.Parse(x.NoteName.Last() + ""))
            //    .GroupBy(x => x)
            //    .OrderBy(x => x.Count())
            //    .ToList();

            //шаг смотреть
            //for (int i = 0; i < 128; i++)
            //    r.Add(new NAudio.Midi.NoteEvent(1, 1, MidiCommandCode.NoteOn, i, 100).NoteName);

            //var t = new List<int>();
            //for (int i = 1; i < r.Count; i++)
            //{
            //    t.Add(r[i] - r[i - 1]);
            //}

            //музыка играть
            //midi.SendAsync(notes);
        }
    }
}
