using System.Collections.Generic;
using System.Linq;
using NAudio.Midi;
namespace MIDIFramework
{
    public class MIDIReader
    {
        private MidiFile midiFile;

        public MIDIReader(string path)
        {
            midiFile = new MidiFile(path);
        }

        public List<List<MidiEvent>> GetMidiEvents()
        {
            var channels = new List<List<MidiEvent>>();
            foreach (var channel in midiFile.Events)
            {
                channels.Add(channel.ToList());
            }

            return channels;
        }

        public List<List<NoteEvent>> GetNoteEvents()
        {
            var channels = new List<List<NoteEvent>>();
            foreach (var channel in midiFile.Events)
            {
                var events = channel
                        .Where(x => x is NoteEvent)
                        .Select(x => (NoteEvent)x)
                        .Where(x => x.CommandCode == MidiCommandCode.NoteOn)
                        //.Where(x => x.CommandCode != MidiCommandCode.NoteOff)// && x.GetAsShortMessage() <= 8323216 + 127*256 && x.GetAsShortMessage() >= 8323216)
                        //.Select(x => new NoteEvent((long)(x.AbsoluteTime*midiFile.DeltaTicksPerQuarterNote*0.001), x.Channel, x.CommandCode, x.NoteNumber, x.Velocity))
                        .Select(x => new NoteEvent(x.AbsoluteTime, x.Channel, x.CommandCode, x.NoteNumber, x.Velocity))
                        .ToList();

                if(events.Count > 0)
                    channels.Add(events);
            }

            return channels;
        }

        public List<List<NoteEvent>> GetNoteEventsWithRealTime()
        {
            var midiEvents = GetMidiEvents();
            var noteEvents = new List<List<NoteEvent>>();
            decimal currentMicroSecondsPerTick = 0m;
            foreach (var midiEvent in midiEvents)
            {
                var preproc = MidiConverter.ToRealTime(midiEvent, midiFile.DeltaTicksPerQuarterNote, ref currentMicroSecondsPerTick);
                var events = preproc
                        .Where(x => x is NoteEvent)
                        .Select(x => (NoteEvent)x)
                        .Where(x => x.CommandCode == MidiCommandCode.NoteOn)// && x.GetAsShortMessage() <= 8323216 + 127*256 && x.GetAsShortMessage() >= 8323216)
                        .Select(x => new NoteEvent(x.AbsoluteTime, x.Channel, x.CommandCode, x.NoteNumber, x.Velocity))
                        .ToList();
                if (events.Count > 0)
                    noteEvents.Add(events);
            }

            return noteEvents;
        }

        public int GetDeltaTicksPerQuarterNote()
        {
            return midiFile.DeltaTicksPerQuarterNote;
        }

        public double GetMetronomeTime()
        {
            var events = midiFile.Events.FirstOrDefault();
            if (events == null)
                return 0;
            var meta = events.Where(x => x is TimeSignatureEvent).Select(x => x as TimeSignatureEvent).FirstOrDefault();
            if (meta == null)
                return 0;

            var ticks = meta.TicksInMetronomeClick;
            var signature = meta.TimeSignature;
            var splits = signature.Split('/');
            var a = int.Parse(splits[0]);
            var b = int.Parse(splits[1]);
            return 0;
        }
    }
}
