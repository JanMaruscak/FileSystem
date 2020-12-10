using System;
using MongoDB.Bson;
namespace FileSystem.Models
{
    public class File
    {
        public ObjectId Id { get; set; }
        public string Name { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastEdit { get; set; }
        public ObjectId Parent { get; set; }
        public string Content { get; set; }
        public bool IsParentFolder;
    }
}