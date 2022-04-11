using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace Explorer.Models
{
    public class CreateDB
    {
        private byte[] data;

        // иконки расширений файлов для БД
        public string StringData { get; set; }
        public byte[] ByteData
        {
            set
            {
                data = value;
                StringData = string.Join(".", data.Select(p => p.ToString()));
            }
        }

        private readonly ExplorerEntities context;
        private int foldId;
        private int typeId;

        readonly Dictionary<string, int> extCoList = new Dictionary<string, int>();
        readonly Dictionary<string, int> foldList = new Dictionary<string, int>();
        
        public CreateDB(string rootDir)
        {
            context = new ExplorerEntities();
            AddDataDb(rootDir);
        }

        private void AddDataDb (string startDir)
        {
            var di = new DirectoryInfo(startDir);
            var fsItems = di.GetFileSystemInfos()
                .OrderBy(f => f.Name)
                .ToList();

            foreach (var fsItem in fsItems)
            {
                if (fsItem.IsDirectory())
                {
                    AddFolder(fsItem);
                    AddDataDb(fsItem.FullName);
                }
                else
                {
                    AddExtension(fsItem);
                    AddFile(fsItem);
                }
            }
        }

        private void AddExtension(FileSystemInfo fsItem)
        {

            string ext = fsItem.Extension;
            var ico = Icon.ExtractAssociatedIcon(fsItem.FullName);
            var sz = ico.Size;
            ByteData = PictureToBytes(ico);

            if (!extCoList.ContainsKey(ext))
            {
                FileExtentions newFileExtension = new FileExtentions()
                {
                    Type = ext,
                    Icon = StringData
                };

                context.FileExtentions.Add(newFileExtension);
                context.SaveChanges();

                typeId = newFileExtension.TypeFileId;
                extCoList.Add(ext, typeId);
                return;
            }

            extCoList.TryGetValue(ext, out typeId);

        }

        private void AddFile(FileSystemInfo fsItem)
        {
            string parent = new DirectoryInfo(fsItem.FullName).Parent?.Name;

            Files newFile = new Files()
            {
                Name = fsItem.FullName,
                FolderId = foldId,
                TypeFileId = typeId,
                Description = string.Format($"Файл {fsItem.Extension}"),
                Content = ""
            };

            context.Files.Add(newFile);
            context.SaveChanges();

            newFile.FolderId = CheckParent(parent, newFile.FileId);
            context.SaveChanges();

        }

        private void AddFolder(FileSystemInfo fsItem)
        {
            string parent = new DirectoryInfo(fsItem.FullName).Parent?.Name;

            Folders newFolder = new Folders()
                {
                    Name = fsItem.Name
                };

                context.Folders.Add(newFolder);
                context.SaveChanges();

                foldId = newFolder.FolderId;
                Folders fld = context.Folders
                    .FirstOrDefault(p => p.Name == fsItem.Name);

                fld.ParentFolderId = CheckParent(parent, foldId);

                context.SaveChanges();

                foldId = newFolder.FolderId;
                foldList.Add(fsItem.Name, foldId);
        }
        // erwewe
        private int CheckParent(string parent, int defId)
        {
            if (!foldList.ContainsKey(parent))
            {
                return defId;
            }

            foldList.TryGetValue(parent, out int idParent);
            return idParent;
        }

        private static byte[] PictureToBytes(Icon icon)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                icon.Save(ms);
                return ms.ToArray();
            }
        }
    }

    public static class FileSystemInfoExtensions
    {
        public static bool IsDirectory(this FileSystemInfo fsItem)
        {
            return (fsItem.Attributes & FileAttributes.Directory) == FileAttributes.Directory;
        }
    }
}