using System;

namespace huffArch {
    class Program {
        static void Main(string[] args) {
            Arch.Compress("input.txt", "output.huff");
            Arch.Decompress("output.huff", "output.txt");
        }
    }
}
