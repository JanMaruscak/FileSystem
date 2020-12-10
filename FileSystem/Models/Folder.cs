using System;
using System.Collections.Generic;
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
        public ObjectId Parent { get; set; }
        public List<ObjectId> Subfolders;
        public List<ObjectId> Files;
        public bool IsParentFolder;
    }
}