using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FileSystem.Models;
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
        public void Insert(Disk disk)
        {
            var collection = db.GetCollection<Disk>("disk");
            collection.InsertOne(disk);
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
    }
}