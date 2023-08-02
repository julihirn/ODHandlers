using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Handlers.ShadowX {
    public class SXVariable {
        public SXVariable() {

        }
        private object value;
        public object Value { get => value; set => this.value = value; }
      
    }
}
