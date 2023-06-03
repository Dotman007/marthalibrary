using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarthaLibrary.Domain.Utility
{
    public class ImageDto
    {
        public Stream Content { get; set; }

        public string Name { get; set; }

        public string ContentType { get; set; }

        public string GetPathWithFileName(string uniques)
        {
            string unique = Path.GetRandomFileName();
            string shorts = Path.GetFileNameWithoutExtension(Name);
            string ext = Path.GetExtension(Name);
            string basePath = $"book_{uniques}/book/";
            return basePath + unique + "_" + shorts + ext;
        }


    }
}
