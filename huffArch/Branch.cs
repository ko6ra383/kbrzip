using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace huffArch {
    class Branch {
        public byte Symbol { get; set; } = 0;
        public Branch Zero { get; set; } = null;
        public Branch One { get; set; } = null;
        public int Freq { get; set; } = 0;

        
    }
    class BranchComparer : IComparer<Branch> {
        public int Compare(Branch x, Branch y) {
            if (x.Freq < y.Freq) return -1;
            else if (x.Freq > y.Freq) return 1;
            else return 0;
            
        }
    }
}
