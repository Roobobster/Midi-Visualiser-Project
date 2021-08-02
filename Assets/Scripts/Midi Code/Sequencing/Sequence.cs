/*
 * Created by: Leslie Sanford
 * 
 * Contact: jabberdabber@hotmail.com
 * 
 * Last modified: 05/08/2004
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;


namespace Multimedia.Midi
{
    /// <summary>
    /// Represents a collection of Tracks.
    /// </summary>
    /// 
    public class Tempo
    {
        public int value;
        public int ticks;

        public Tempo(int value, int ticks) 
        {
            this.value = value;
            this.ticks = ticks;
        }
    }
	public class Sequence
	{
        #region Sequence Members

        #region Constants

        // Number of bits to shift for splitting division value.
        private const int DivisionShift = 8;

        #endregion

        #region Fields

        // The resolution of the sequence.
        private int division;

        // The collection of tracks for the sequence.
        private List<Track> tracks = new List<Track>();

        private List<Tempo> tempos;

        // All of the tracks for the sequence merged into one track.
        private Track mergedTrack;

        // Indicates whether or not the sequence has been changed since the 
        // last time all of the tracks were merged.
        private bool dirty = true;

        #endregion


        #region Construction

        /// <summary>
        /// Initializes a new instance of the Sequence class with the 
        /// specified division.
        /// </summary>
        /// <param name="division">
        /// The division value for the sequence.
        /// </param>
		public Sequence(int division)
		{
           
            this.division = division;
        }

        #endregion

        #region Methods

        public int GetTrackCount() 
        {
            return tracks.Count;
        }

        public Track GetTrack(int index) 
        {
            return tracks[index];
        }

        public List<Track> GetTracks() 
        {
            return tracks;
        }



        public List<Tempo> GetTempo() 
        {
            tempos = new List<Tempo>();

            foreach (MidiEvent midiEvent in tracks[0].GetMidiEvents())
            {
                if (midiEvent.Message.GetType() == typeof(MetaMessage))
                {
                    MetaMessage metaMessage = (MetaMessage) midiEvent.Message;
                    if (metaMessage.Type == MetaType.Tempo)
                    {

                        int tempoValue = MetaMessage.PackTempo(metaMessage);
                        int deltaTime = midiEvent.Ticks;
                        Tempo tempo = new Tempo(tempoValue, deltaTime);
                        tempos.Add(tempo);
                    }
                }
                    
            }

            return tempos;
        }

   

        public int[] GetTimeSignature()
        {
            int[] timeSignature = new int[2];

            foreach (Track track in tracks)
            {
                foreach (MidiEvent midiEvent in track.GetMidiEvents())
                {
                    if (midiEvent.Message.GetType() == typeof(MetaMessage))
                    {
                        MetaMessage metaMessage = (MetaMessage)midiEvent.Message;
                        if (metaMessage.Type == MetaType.TimeSignature)
                        {
                            
                            timeSignature[0] = metaMessage[0];
                            timeSignature[1] = metaMessage[1];
                            return timeSignature;
                        }
                    }

                }
            }

            return timeSignature;
        }



        /// <summary>
        /// Adds a track to the Sequence.
        /// </summary>
        /// <param name="trk">
        /// The track to add to the Sequence.
        /// </param>
        public void Add(Track trk)
        {
            tracks.Add(trk);

            // Indicate that the sequence has changed.
            dirty = true;
        }

        /// <summary>
        /// Remove the Track at the specified index.
        /// </summary>
        /// <param name="index">
        /// The zero-based index of the Track to remove. 
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if index is less than zero or greater or equal to Count.
        /// </exception>
        public void RemoveAt(int index)
        {
            // Enforce preconditions.
            if(index < 0 || index >= Count)
                throw new ArgumentOutOfRangeException("index", index,
                    "Index for removing track from sequence out of range.");

            tracks.RemoveAt(index);

            // Indicate that the sequence has changed.
            dirty = true;
        }


        /// <summary>
        /// Gets the length of the Sequence in ticks.
        /// </summary>
        /// <returns>
        /// The length of the Sequence in ticks.
        /// </returns>
        /// <remarks>
        /// The length of the Sequence is represented by the longest Track in
        /// the Sequence.
        /// </remarks>
        public int GetLength()
        {
            int length = 0;

            // For each track in the Sequence.
            foreach(Track track in tracks)
            {
                // Get the length of the track.
                int trkLength = track.GetLength();

                // If this is the longest track so far, update length value.
                if(length < trkLength)
                {
                    length = trkLength;
                }
            }

            return length;
        }

        /// <summary>
        /// Determines whether or not this is a Smpte sequence.
        /// </summary>
        /// <returns>
        /// <b>true</b> if this is a Smpte sequence; otherwise, <b>false</b>.
        /// </returns>
        public bool IsSmpte()
        {
            bool result = false;

            // The upper byte of the division value will be negative if this is
            // a Smpte sequence.
            if((sbyte)(division >> DivisionShift) < 0)
            {
                result = true;
            }

            return result;
        }


 
        #endregion
   
        #region Properties



        /// <summary>
        /// Gets the number of Tracks in the Sequence.
        /// </summary>
        public int Count
        {
            get
            {
                return tracks.Count;
            }
        }

        /// <summary>
        /// Gets the division value for the Sequence.
        /// </summary>
        public int Division
        {
            get
            {
                return division;
            }
        }

        #endregion

        #endregion
    }
}
