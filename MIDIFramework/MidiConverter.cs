using System;
using System.Collections.Generic;
using System.Linq;
using NAudio.Midi;

namespace MIDIFramework
{
    public class MidiConverter
    {
        /// <summary>
        /// Натягивает ноты на новую сетку с фиксированным шагом по времени
        /// </summary>
        /// <param name="notes">Музыкальные ноты</param>
        /// <param name="time">фиксированный шаг по времени(например, 50)</param>
        /// <returns></returns>
        public static List<NoteEvent> ToNewGrid(List<NoteEvent> notes, int time)
        {
            var copy = notes.Copy();
            var timeMax = notes.Max(x => x.AbsoluteTime);
            var times = Enumerable.Range(0, (int)timeMax / time + 2).Select(x => x * time).ToArray();
            for (int i = 0; i < copy.Count; i++)
            {
                var arr = times.Select(x => Math.Abs(x - copy[i].AbsoluteTime)).ToList();
                var min = arr.Min();
                int block = arr.IndexOf(min);
                copy[i].AbsoluteTime = times[block];
            }

            return copy;
        }

        /// <summary>
        /// Преобразование абсолютного времени нот в миллисекунды 
        /// </summary>
        /// <param name="midiEvents"></param>
        /// <param name="deltaTicksPerQuarterNote"></param>
        /// <param name="currentMicroSecondsPerTick">параметр midi-файла</param>
        /// <returns></returns>
        public static List<MidiEvent> ToRealTime(List<MidiEvent> midiEvents, int deltaTicksPerQuarterNote, ref decimal currentMicroSecondsPerTick)
        {

            List<decimal> eventsTimesArr = new List<decimal>();
            decimal lastRealTime = 0m;
            decimal lastAbsoluteTime = 0m;

            for (int i = 0; i < midiEvents.Count; i++)
            {
                MidiEvent midiEvent = midiEvents[i];
                TempoEvent tempoEvent = midiEvent as TempoEvent;

                if (midiEvent.AbsoluteTime > lastAbsoluteTime)
                {
                    lastRealTime += ((decimal)midiEvent.AbsoluteTime - lastAbsoluteTime) * currentMicroSecondsPerTick;
                }

                lastAbsoluteTime = midiEvent.AbsoluteTime;

                if (tempoEvent != null)
                {
                    currentMicroSecondsPerTick = (decimal)tempoEvent.MicrosecondsPerQuarterNote / (decimal)deltaTicksPerQuarterNote;
                    midiEvents.RemoveAt(i--);
                    continue;
                }

                eventsTimesArr.Add(lastRealTime);
            }

            for (int i = 0; i < midiEvents.Count; i++)
            {
                midiEvents[i].AbsoluteTime = (long)(eventsTimesArr[i] / 1000);
            }

            return midiEvents;
        }

        /// <summary>
        /// Оставляет только базовые ноты
        /// </summary>
        /// <param name="notes"></param>
        /// <returns></returns>
        public static List<NoteEvent> ToBaseNotes(List<NoteEvent> notes)
        {
            return notes.Where(x => IsDigit(x.NoteName.Last())).ToList();
        }

        private static bool IsDigit(string str)
        {
            return int.TryParse(str, out int result);
        }

        private static bool IsDigit(char sym)
        {
            return IsDigit(sym + "");
        }
    }
}
