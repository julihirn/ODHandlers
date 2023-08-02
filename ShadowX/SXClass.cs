using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;

namespace Handlers.ShadowX {
    public class SXClass : SXObject {
        public List<SXVariable> Variables;
        public List<SXFunction> Functions;
        public List<Enum> Enums;
        public SXClass(string Name, int BeginAt) {
            this.Name = Name;
            this.StartLine = BeginAt;
        }

        // public List<>
    }
}
