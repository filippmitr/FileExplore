//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Explorer
{
    using System;
    using System.Collections.Generic;
    
    public partial class Files
    {
        public int FileId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Content { get; set; }
        public int FolderId { get; set; }
        public int TypeFileId { get; set; }
    
        public virtual FileExtentions FileExtentions { get; set; }
        public virtual Folders Folders { get; set; }
    }
}
