﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FridayNightFunkin.Json;
using Newtonsoft.Json;

namespace FridayNightFunkin
{
    public class FNFSong
    {
        private Song.Root dataRoot { get; }
        public List<FNFSection> Sections { get; set; }

        public string SongName
        {
            get => dataRoot.song.SongSong; 
            set => dataRoot.song.SongSong = value;
        }
        
        public long Bpm
        {
            get => dataRoot.song.Bpm;
            set => dataRoot.song.Bpm = value;
        }

        public long Speed
        {
            get => dataRoot.song.Speed;
            set => dataRoot.song.Speed = value;
        }

        public bool NeedVoices
        {
            get => dataRoot.song.NeedsVoices;
            set => dataRoot.song.NeedsVoices = value;
        }

        public string Player1
        {
            get => dataRoot.song.Player1;
            set => dataRoot.song.Player1 = value;
        }

        public string Player2
        {
            get => dataRoot.song.Player2;
            set => dataRoot.song.Player2 = value;
        }
        
        public FNFSong(string songPath)
        {
            dataRoot= JsonConvert.DeserializeObject<Song.Root>(File.ReadAllText(songPath));
            Sections = new List<FNFSection>();
            foreach (Note n in dataRoot.song.Notes)
            {
                Sections.Add(new FNFSection(n));
            }
        }

        public FNFSong()
        {
            dataRoot = new Song.Root();
            dataRoot.bpm = 0;
            dataRoot.notes = new List<Note>();
            dataRoot.sections = 0;
            dataRoot.song = new Song("dad", "bf",1,false,"noAduio",0);
            dataRoot.song.Notes = new List<Note>();
        }

        public void AddSection(FNFSection section)
        {
            // reset sections but keep them lol
            Note n = new Note();

            n.Bpm = Bpm;
            n.ChangeBpm = false;
            n.LengthInSteps = 16;
            n.MustHitSection = section.MustHitSection;
            n.TypeOfSection = 0;
            n.sectionNotes = new List<List<decimal>>();
                
            foreach(FNFNote note in section.Notes)
                n.sectionNotes.Add(note.ConvertToNote());
            Sections.Add(section);
            dataRoot.song.Notes.Add(n);
        }
        
        public void SaveSong(string savePath)
        {
            Console.WriteLine("Compiling song with " + Sections.Count + " sections.");
            for (int i = 0; i < dataRoot.song.Notes.Count; i++)
            {
                try
                {
                    Console.WriteLine("Section " + i);
                    FNFSection section = Sections[i];
                        dataRoot.song.Notes[i] = section.dataNote;
                    
                }
                catch (Exception e)
                {
                    Console.WriteLine("Failed on section " + i + "\n" + e);
                }
            }
            Console.WriteLine("Compiled! Saving to " + savePath);
            File.WriteAllText(savePath, JsonConvert.SerializeObject(dataRoot));
        }
        
        public class FNFSection
        {
            private const int ValidSectionCapacity = 3;
            public Note dataNote { get; }
            public List<FNFNote> Notes { get; set; }
            public bool MustHitSection { get => dataNote.MustHitSection; set => dataNote.MustHitSection = value; }

            public FNFNote ModifyNote(FNFNote toModify, FNFNote newProperties)
            {
                if (toModify == null)
                    throw new Exception("ToModify is null.");
                int index = Notes.IndexOf(toModify);
                dataNote.sectionNotes[index] = newProperties.ConvertToNote();
                Notes[index] = newProperties;
                return newProperties;
            }

            public void AddNote(FNFNote newNote)
            {
                if (newNote == null)
                    throw new Exception("NewNote is null.");
                dataNote.sectionNotes.Add(newNote.ConvertToNote());
                Notes.Add(newNote);
            }

            public void RemoveNote(FNFNote removeNote)
            {
                if (removeNote == null)
                    throw new Exception("RemoveNote is null.");
                List<decimal> toRemove = null;
                foreach (List<decimal> n in dataNote.sectionNotes)
                {
                    if (n[0] != removeNote.ConvertToNote()[0] && n[1] != removeNote.ConvertToNote()[1])
                        continue;
                    toRemove = n;
                }
                if (toRemove == null)
                    throw new Exception("Note not found!");
                dataNote.sectionNotes.Remove(toRemove);
                Notes.Remove(removeNote);
            }
            
            public FNFSection(Note prvDataNote)
            {
                if (prvDataNote == null)
                    throw new Exception("Song Root is null.");
                dataNote = prvDataNote;
                Notes = new List<FNFNote>();
                foreach (List<decimal> n in dataNote.sectionNotes)
                {
                    // if some notes are missing
                    while (n.Count != ValidSectionCapacity)
                    {
                        if (n.Count < ValidSectionCapacity)
                            n.Add(0);
                        if (n.Count > ValidSectionCapacity)
                            n.RemoveAt(n.Count - 1);
                    }
                    Notes.Add(new FNFNote(n[0],n[1],n[2]));
                }
            }
        }

        public class FNFNote
        {
            public List<decimal> ConvertToNote()
            {
                List<decimal> list = new List<decimal>();
                list.Add(Time);
                list.Add((decimal)Type);
                list.Add(Length);
                return list;
            }

            public FNFNote(decimal time, decimal noteType, decimal length)
            {
                Time = time;
                Type = (NoteType)noteType;
                Length = length;
            }
            
            public decimal Time { get; set; } // Time in milliseconds
            public NoteType Type { get; set; } // Type of note, left, down, up, right, etc.
            public decimal Length { get; set; } // Length of note, for holding sliders.
        }

        public enum NoteType
        {
            Left = 0,
            Down = 1,
            Up = 2,
            Right = 3,
            RLeft = 4,
            RDown = 5,
            RUp = 6,
            RRight = 7
        }
    }
}