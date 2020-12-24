using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using NAudio.Midi;

namespace MIDIFramework
{
    public class MIDISynthesizer
    {
        private int handle;
        private int deviceId;
        protected MidiCallback callback;
        public MidiOutCaps caps;
        public int NumberOfDevices { get; private set; }

        protected delegate void MidiCallback(int handle, int msg, int instance, int param1, int param2);

        #region DLLs
        [DllImport("winmm.dll")]
        private static extern long mciSendString(string command, StringBuilder returnValue, int returnLength, IntPtr winHandle);

        [DllImport("winmm.dll")]
        private static extern int midiOutGetNumDevs();

        [DllImport("winmm.dll")]
        private static extern int midiOutGetDevCaps(Int32 uDeviceID, ref MidiOutCaps lpMidiOutCaps, UInt32 cbMidiOutCaps);

        [DllImport("winmm.dll")]
        private static extern int midiOutOpen(ref int handle, int deviceID,
            MidiCallback proc, int instance, int flags);

        [DllImport("winmm.dll")]
        protected static extern int midiOutShortMsg(int handle, int message);

        [DllImport("winmm.dll")]
        protected static extern int midiOutClose(int handle);
        #endregion

        public MIDISynthesizer(int deviceId = 0)
        {
            handle = 0;
            this.deviceId = deviceId;
            NumberOfDevices = midiOutGetNumDevs();

            if (NumberOfDevices == 0)
                throw new Exception("Devices not found");

            caps = new MidiOutCaps();

            var res = midiOutGetDevCaps(deviceId, ref caps, (uint)Marshal.SizeOf(caps));
            if (res != 0) throw new Exception("midiOutGetDevCaps error");

            res = midiOutOpen(ref handle, deviceId, callback, 0, 0);
            if (res != 0) throw new Exception("midiOutOpen error");
        }

        public bool Send(NoteEvent pianoNote)
        {
            int res = midiOutShortMsg(handle, pianoNote.GetAsShortMessage());
            if (res != 0) return false;

            return true;
        }

        public async Task SendAsync(List<NoteEvent> pianoNotes)
        {
            var sorted = pianoNotes.OrderBy(e => e.AbsoluteTime).ToList();
            await Task.Run(async () =>
            {
                Stopwatch timer = new Stopwatch();
                timer.Start();
                while (sorted.Count != 0)
                {
                    for (int i = 0; i < sorted.Count; i++)
                    {
                        var note = sorted[i];
                        int thres = 30;
                        var absoluteTime = note.AbsoluteTime;
                        if (Math.Abs(absoluteTime - timer.ElapsedMilliseconds) < thres)
                        {
                            Send(note);
                            sorted.RemoveAt(i--);
                        }
                        else if (timer.ElapsedMilliseconds - absoluteTime > thres) // если вдруг элемент выпал за диапазон в прошлом
                        {
                            sorted.RemoveAt(i--);
                        }
                        //else if(timer.ElapsedMilliseconds < note.absoluteTime)
                    }
                    await Task.Delay(1);
                }
                timer.Stop();
            });
        }
    }
}
