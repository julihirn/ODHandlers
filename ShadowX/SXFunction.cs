using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Handlers.ShadowX {
    public class SXFunction : SXObject {
        private SXVariable output;
        public SXVariable Output { 
            get => output; 
            set => output = value; 
        }
        public List<SXVariable> Inputs = new List<SXVariable>();
        public List<SXVariable> Variables = new List<SXVariable>();
    }
}
