using System;
using MongoDB.Bson;

namespace FileSystem.Models
{
    public class Folder
    {
        public ObjectId Id { get; set; }
        public string Name { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastEdit { get; set; }
        public ulong TotalSize { get; set; }
        public ObjectId[] Subfolders;
        public ObjectId[] Files;
    }
}