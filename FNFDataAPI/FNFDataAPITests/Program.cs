using System;
using FridayNightFunkin;
using FridayNightFunkin.Json;

namespace FNFDataAPITests
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Song Data or Directory:");
            string data = Console.ReadLine();
            Console.WriteLine("Is this a directory (1) or JSON data (2)?");
            string option = Console.ReadLine();
            Console.WriteLine("TEST 1 - LOADING");
            try
            {
                FNFSong song;
                if (option == "1")
                    song = new FNFSong(data);
                else
                    song = new FNFSong(data, FNFSong.DataReadType.AsRawJson);
                //It would have been an "if option == 2" but it has to be an "else" or
                //else the compiler flips its shit.

                Console.WriteLine("Song Name: " + song.SongName);
                Console.WriteLine("Song BPM: " + song.Bpm);
                Console.WriteLine("Song Speed: " + song.Speed);
                Console.WriteLine("Sections: " + song.Sections.Count);
                Console.WriteLine("TEST 1 - COMPLETE");
                Console.WriteLine("TEST 2 - SECTIONS & NOTES");
                FNFSong.FNFNote noteToModify = null;
                FNFSong.FNFSection sectionToModify = null;
                foreach (FNFSong.FNFSection section in song.Sections)
                {
                    Console.WriteLine("Section: " + section.MustHitSection + "\nNotes: " + section.Notes.Count);
                    if (section.Notes.Count == 0)
                        continue;
                    foreach (FNFSong.FNFNote note in section.Notes)
                    {
                        if (noteToModify == null)
                        {
                            noteToModify = note; // select the first note to be modified
                            sectionToModify = section;
                        }

                        Console.WriteLine("Note: " + note.Type + "\nTime: " + note.Time + "\nLength: " + note.Length);
                        
                    }
                }
                Console.WriteLine("TEST 2 - COMPLETE");
                Console.WriteLine("TEST 3 - ADDING/REMOVING/MODIFYING NOTES");
                noteToModify = sectionToModify.ModifyNote(noteToModify, new FNFSong.FNFNote(noteToModify.Time, (decimal)FNFSong.NoteType.Left, 35));
                sectionToModify.AddNote(new FNFSong.FNFNote(noteToModify.Time += 1000,(decimal) FNFSong.NoteType.Right, 75));
                sectionToModify.RemoveNote(sectionToModify.Notes[3]); // the first section with less than 4 notes will fail this.
                Console.WriteLine("TEST 3 - COMPLETE");
                Console.WriteLine("TEST 4 - COMPILE");
                song.SaveSong("compiledSong.json");
                Console.WriteLine("TEST 5 - COMPLETE");
                Console.WriteLine("END");
            }
            catch (Exception e)
            {
                Console.WriteLine("FAILED TESTS.\n" + e);
            }
        }
    }
}