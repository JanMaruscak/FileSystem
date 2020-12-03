using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FileSystem.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace FileSystem.Data
{
    public class FileSystemService
    {
        private MongoClient _client;
        private IMongoDatabase db;
        public FileSystemService()
        {
            _client = new MongoClient();
            db = _client.GetDatabase("FileSystem");
        }
        public void InsertDisk(Disk disk)
        {
            var collection = db.GetCollection<Disk>("disk");
            collection.InsertOne(disk);
        }
        public void InsertFile(File file)
        {
            var collection = db.GetCollection<File>("file");
            collection.InsertOne(file);
        }
        public Task<List<Disk>> GetDisksAsync()
        {
            var disks = db.GetCollection<Disk>("disk");
            return Task.FromResult(disks.Find(t=>true).ToList());


            // var rng = new Random();
            // return Task.FromResult(Enumerable.Range(1, 5).Select(index => new WeatherForecast
            // {
            //     Date = startDate.AddDays(index),
            //     TemperatureC = rng.Next(-20, 55),
            //     Summary = Summaries[rng.Next(Summaries.Length)]
            // }).ToArray());
        }
        public Task<List<File>> GetFilesAsync(ObjectId diskId)
        {
            var disks = db.GetCollection<Disk>("disk");
            var disk = disks.Find(t => t.Id == diskId).First();
            var files = db.GetCollection<File>("file").Find(t => disk.Files.Contains(t.Id));
            return Task.FromResult(files.ToList());
        }
    }
}