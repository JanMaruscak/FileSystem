using System;
using System.Collections.Generic;
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
        public List<ObjectId> Folders;
        public List<ObjectId> Files;
    }
}