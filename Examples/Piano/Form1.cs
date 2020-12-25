using System;
using System.Collections.Generic;
using System.Globalization;
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
        int time = 50;
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
            //clear
            if (!Directory.Exists("output"))
            {
                Directory.CreateDirectory("output");
            }
            else
            {
                foreach (var path in Directory.GetFiles("output"))
                {
                    File.Delete(path);
                }
            }

            var notes = new List<NoteEvent>();

            int t = 0;
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

                int n = 10000;
                int l = (int)notes.Max(x => x.AbsoluteTime);
                for (int i = 0; i < l - n; i += n/2)
                {
                    var matr = MidiConverter.ToMatrix(notes, time, i, n);
                    if (matr.H != 28 || matr.W != 200)
                        throw new Exception("Error dim");
                    Vector.SaveAsBinary($"output\\m{t++}.matr", matr.ToVector());
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var path = @"C:\Users\marat\Downloads\aaaaaaaa.matr";
            var vec = new Vector(File.ReadAllLines(path).Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToArray());
            var matr = new Matrix(28, 200);
            matr.Matr = vec;
            var min = vec.Min();
            var max = vec.Max();

            matr = (matr - min) / (max - min);

            var notes = MidiConverter.ToNoteEvents(matr, time);

            midi.SendAsync(notes);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 128; i++)
            {
                var note = new NoteEvent(1, 1, MidiCommandCode.NoteOn, i, 100);
                var name = note.NoteName;
                var dig = int.Parse(name.Last() + "");
                if (name.Length == 2 && dig > 2 && dig < 7)
                    richTextBox1.Text += $"{{ \"{note.NoteName}\", {i} }},\n";
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            MIDIReader reader = new MIDIReader($@"{pathBase}\Ласковый май - Белые розы [MIDISTOCK.RU].mid");
            var channels = reader.GetNoteEventsWithRealTime();
            var notes = new List<NoteEvent>();
            for (int ch = 0; ch < channels.Count; ch++)
                for (int i = 0; i < channels[ch].Count; i++)
                    notes.Add(channels[ch][i]);

            notes = MidiConverter.ToBaseNotes(notes);
            notes = MidiConverter.ToNewGrid(notes, time);

            midi.SendAsync(notes);
        }
    }
}
