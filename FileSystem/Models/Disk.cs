using System;
using MongoDB.Bson;

namespace FileSystem.Models
{
    public class Disk
    {
        public ObjectId Id { get; set; }
        public string Name { get; set; }
        public ulong TotalSize { get; set; }
        public DateTime Created;
        public DateTime LastEdit;
        public ObjectId[] Folders;
        public ObjectId[] Files;
    }
}