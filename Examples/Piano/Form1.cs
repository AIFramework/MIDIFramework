using System;
using System.Collections.Generic;
using System.Windows.Forms;
using MIDIFramework;
using NAudio.Midi;

namespace Piano
{
    public partial class Form1 : Form
    {
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

            //MIDIReader reader = new MIDIReader(@"Пираты Карибского моря - Главная тема [MIDISTOCK.RU].mid");
            MIDIReader reader = new MIDIReader(@"Звёздные Войны - Imperial March (Имперский Марш) [MIDISTOCK.RU].mid");
            //var channels = reader.GetNoteEvents();//.Skip(1).ToList();
            //if(channels.Count == 17 && channels[0].Count == 0)
            //{
            //    channels = channels.Skip(1).ToList();
            //}

            var channels = reader.GetNoteEventsWithRealTime();
            //var channels = reader.GetNoteEvents();
            for (int ch = 0; ch < channels.Count; ch++)
                for (int i = 0; i < channels[ch].Count; i++)
                    notes.Add(channels[ch][i]);

            //группируем и зырим
            //var count = notes
            //    .Select(x => int.Parse(x.noteName.Last() + ""))
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

            notes = MidiConverter.ToNewGrid(notes, 50);

            //музыка играть
            midi.SendAsync(notes);
        }
    }
}
