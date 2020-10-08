using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LucaSystem
{
    public abstract class AbstractFileParser
    {
        public abstract void FileExport(string path, string outpath = null);
        public abstract void FileImport(string path, string outpath = null);
    }
}
