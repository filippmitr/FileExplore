using Explorer.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace Explorer.Controllers
{
    public class TreeModelController : Controller
    {
        public string StringData { get; set; }
        public byte[] ByteData => Array.ConvertAll(StringData.Split('.'), byte.Parse);

        // GET: TreeModel
        public async Task<ActionResult> Index()
        {
            ExplorerEntities newContainer = new ExplorerEntities();
            bool exDb = newContainer.Database.CreateIfNotExists();
            if (exDb || !newContainer.Folders.Any())
            {
                new CreateDB(AppDomain.CurrentDomain.BaseDirectory);
            }

            //Заполнение структуры дерева из БД
            IList<TreeViewModel> data = GetTreeData();

            ViewBag.Json = new JavaScriptSerializer().Serialize(data);
            return View();
        }

        private List<TreeViewModel> GetTreeData()
        {
            List<TreeViewModel> nodes = new List<TreeViewModel>();
            using (ExplorerEntities context = new ExplorerEntities())
            {
                string pathIconFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content", "images",
                    "folder_closed_16x16.png");
                var byteIconFold = PictureToBytes(new Bitmap(pathIconFolder));
                var strIconFold = "data:image/gif;base64," + Convert.ToBase64String(byteIconFold);

                int idLast = 0;
                foreach (var item in context.Folders)
                {
                    idLast = item.FolderId;
                    nodes.Add(new TreeViewModel
                    {
                        id = item.FolderId.ToString(),
                        parent = item.ParentFolderId == item.FolderId ? "#" : item.ParentFolderId.ToString(),
                        text = new DirectoryInfo(item.Name).Name,
                        icon = strIconFold,
                    });
                }

                foreach (var item in context.Files)
                {
                    var itemExt = context.FileExtentions.First(e => e.TypeFileId == item.TypeFileId);
                    StringData = itemExt.Icon;
                    var iconExt = "data:image/gif;base64," + Convert.ToBase64String(ByteData);

                    nodes.Add(new TreeViewModel
                    {
                        id = (++idLast).ToString(),
                        parent = item.FolderId == item.FileId ? "#" : item.FolderId.ToString(),
                        text = new DirectoryInfo(item.Name).Name,
                        icon = iconExt,
                    });
                }
            }

            return nodes;
        }

        public static byte[] PictureToBytes(Image img)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                img.Save(ms, ImageFormat.Gif);
                return ms.ToArray();
            }
        }

        public ActionResult Tree()
        {
            return View();
        }
    }
}