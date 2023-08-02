using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handlers.ShadowX {
    public class SXObject {
        private int startline;
        public int StartLine { get => startline; set => startline = value; }
        private int endline;
        public int EndLine { get => endline; set => endline = value; }
        public string Name { get => name; set => name = value; }

        private string name;
    }
}
