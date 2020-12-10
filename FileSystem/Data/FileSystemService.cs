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
        private readonly IMongoDatabase _db;
        public FileSystemService()
        {
            var client = new MongoClient();
            _db = client.GetDatabase("FileSystem");
        }
        public async Task InsertDiskAsync(Disk disk)
        {
            var collection = _db.GetCollection<Disk>("disk");
            await collection.InsertOneAsync(disk);
        }
        public async Task InsertFileAsync(File file)
        {
            var collection = _db.GetCollection<File>("file");
            AddFileToDiskList(file.Parent,file.Id);
            await collection.InsertOneAsync(file);
        }
        public async Task InsertFolderAsync(Folder folder)
        {
            var collection = _db.GetCollection<Folder>("folder");
            AddFolderToDiskList(folder.Parent,folder.Id);
            await collection.InsertOneAsync(folder);
            await Task.FromResult(0);
        }

        public async Task<List<Disk>> GetDisksAsync()
        {
            var disks = _db.GetCollection<Disk>("disk");
            return await Task.FromResult(disks.Find(t=>true).ToList());
        }

        public async Task<List<File>> GetFilesAsync(ObjectId parentId, ParentType parentType)
        {
            switch (parentType)
            {
                case ParentType.Disk:
                    var disks = _db.GetCollection<Disk>("disk");
                    var disk = disks.Find(t => t.Id == parentId).First();
                    var files = _db.GetCollection<File>("file").Find(t => disk.Files.Contains(t.Id));
                    return await Task.FromResult(files.ToList());
                case ParentType.Folder:
                    var allFolders = _db.GetCollection<Folder>("folder");
                    var folder = allFolders.Find(t => t.Id == parentId).First();
                    files = _db.GetCollection<File>("file").Find(t => folder.Files.Contains(t.Id));
                    return await Task.FromResult(files.ToList());
                default:
                    throw new NotSupportedException();
            }
        }
        public async Task<List<Folder>> GetFoldersAsync(ObjectId parentId, ParentType parentType)
        {
            switch (parentType)
            {
                case ParentType.Disk:
                    var disks = _db.GetCollection<Disk>("disk");
                    var disk = disks.Find(t => t.Id == parentId).First();
                    var folders = _db.GetCollection<Folder>("folder").Find(t => disk.Folders.Contains(t.Id));
                    return await Task.FromResult(folders.ToList());
                case ParentType.Folder:
                    var allFolders = _db.GetCollection<Folder>("folder");
                    var folder = allFolders.Find(t => t.Id == parentId).First();
                    var subFolders = _db.GetCollection<Folder>("folder").Find(t => folder.Subfolders.Contains(t.Id));
                    return await Task.FromResult(subFolders.ToList());
                default:
                    throw new NotSupportedException();
            }
        }

        public Disk GetDiskById(ObjectId id)
        {
            var disks = _db.GetCollection<Disk>("disk");
            // var diskId = GetIdByLocation(location, ParentType.Disk);
            var disk = disks.Find(t => t.Id == id).First();
            return disk;
        }
        public Folder GetFolderById(ObjectId id)
        {
            var folders = _db.GetCollection<Folder>("folder");
            // var folderId = GetIdByLocation(location, ParentType.Folder);
            var folder = folders.Find(t => t.Id == id).First();
            return folder;
        }
        public File GetFileById(ObjectId id)
        {
            var files = _db.GetCollection<File>("file");
            // var fileId = GetIdByLocation(location, ParentType.File);
            var file = files.Find(t => t.Id == id).First();
            return file;
        }

        private void AddFileToDiskList(ObjectId diskId, ObjectId fileId)
        {
            var disks = _db.GetCollection<Disk>("disk");

            var filter = Builders<Disk>.Filter.Where(t => t.Id == diskId);
            var update = Builders<Disk>.Update.Push(t => t.Files, fileId);
         
            disks.UpdateOneAsync(filter, update);
        }
        private void AddFolderToDiskList(ObjectId diskId, ObjectId folderId)
        {
            var disks = _db.GetCollection<Disk>("disk");

            var filter = Builders<Disk>.Filter.Where(t => t.Id == diskId);
            var update = Builders<Disk>.Update.Push(t => t.Folders, folderId);
         
            disks.UpdateOneAsync(filter, update);
        }

        public async void SaveFileContent(ObjectId id, string content)
        {
            var files = _db.GetCollection<File>("file");

            var filter = Builders<File>.Filter.Where(t => t.Id == id);
            var update = Builders<File>.Update.Set(t => t.Content, content);
         
            await files.UpdateOneAsync(filter, update);
        }

        // TODO Update Parent size
        private void UpdateParentSize(ObjectId parentId,ObjectId fileId, ulong size, ParentType parentType)
        {
            switch (parentType)
            {
                case ParentType.Disk:
                    UpdateSize(parentId, size, parentType);
                    break;
                case ParentType.Folder:
                    UpdateSize(parentId, size, ParentType.Folder);
                    var folder = GetFolderById(parentId);
                    // TODO
                    /*if (folder.IsParentFolder)
                    {
                        UpdateSize();
                    }*/
                    
                    break;
            }
        }

        private void UpdateSize(ObjectId id, ulong size, ParentType parentType)
        {
            switch (parentType)
            {
                case ParentType.Disk:
                    var disks = _db.GetCollection<Disk>("disk");
                    var filter = Builders<Disk>.Filter.Where(t => t.Id == id);
                    var update = Builders<Disk>.Update.Set(t => t.TotalSize, size);
                    disks.UpdateOne(filter, update);
                    break;
                case ParentType.File:
                    throw new NotImplementedException();
                case ParentType.Folder:
                    var folders = _db.GetCollection<Folder>("folder");
                    var folderFilter = Builders<Folder>.Filter.Where(t => t.Id == id);
                    var folderUpdate = Builders<Folder>.Update.Set(t => t.TotalSize, size);
                    folders.UpdateOne(folderFilter, folderUpdate);
                    break;
            }
        }
        private ObjectId GetIdByURL(string location,ParentType parentType)
        {
            switch (parentType)
            {
                case ParentType.Folder:
                    var split = location.Split('/');
                    var folders = _db.GetCollection<Folder>("folder");
                    var folder = folders.Find(t => t.Name == split.Last()).First();
                    return folder.Id;
                    
                case ParentType.Disk:
                    var disks = _db.GetCollection<Disk>("disk");
                    var disk = disks.Find(t => t.Name == location).First();
                    return disk.Id;
                case ParentType.File:
                    split = location.Split('/');
                    var files = _db.GetCollection<File>("file");
                    var file = files.Find(t => t.Name == split.Last()).First();
                    return file.Id;
            }
            //TODO Don't return ObjectId.Empty
            return ObjectId.Empty;
        }

        public enum ParentType
        {
            Folder,Disk,File
        }

        public void Delete(ObjectId id, ParentType parentType)
        {
            switch (parentType)
            {
                case ParentType.Disk:
                    var disks = _db.GetCollection<Disk>("disk");
                    var disk = disks.Find(t => t.Id == id).First();
                    foreach (var file in disk.Files)
                    {
                        Delete(file,ParentType.File);
                    }

                    foreach (var folder in disk.Folders)
                    {
                        
                        Delete(folder,ParentType.Folder);
                    }
                    var filter = Builders<Disk>.Filter.Where(t => t.Id == id);
                    disks.DeleteOne(filter);
                    break;
                case ParentType.File:
                    var files = _db.GetCollection<File>("file");
                    var fileFilter = Builders<File>.Filter.Where(t => t.Id == id);
                    files.DeleteOne(fileFilter);
                    break;
                case ParentType.Folder:
                    var folders = _db.GetCollection<Folder>("folder");
                    var parent = folders.Find(t => t.Id == id).First();
                    foreach (var file in parent.Files)
                    {
                        Delete(file,ParentType.File);
                    }

                    foreach (var folder in parent.Subfolders)
                    {
                        Delete(folder,ParentType.Folder);
                    }
                    var folderFilter = Builders<Folder>.Filter.Where(t => t.Id == id);
                    folders.DeleteOne(folderFilter);
                    break;
            }
        }
    }
}