using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace huffArch {
    static class Arch {
        
        public static void Compress(string pathSourse, string pathOutput) {
            byte[] data = File.ReadAllBytes(pathSourse);
            Branch[] freqs = data.GroupBy(b => b)
                                .Select(b => new Branch{ Symbol = b.Key, Freq = b.Count() })
                                .OrderBy(b => b.Freq)
                                .ToArray<Branch>();
            Branch root = HuffTree(freqs);
            string[] codes = MakeCodes(root);
            byte[] bits = CompressCodes(data, codes);
            byte[] head = Header(data.Length, freqs);
            File.WriteAllBytes(pathOutput, head.Concat(bits).ToArray());
        }


        public static void Decompress(string pathSourse, string pathOutput) {
            byte[] arch = File.ReadAllBytes(pathSourse);
            ParseHeader(arch, out List<Branch> listfreqs, out int dataLength);
            Branch[] freqs = listfreqs.ToArray();
            Branch root = HuffTree(freqs);
            byte[] data = DecompressCodes(arch, root, dataLength);
            File.WriteAllBytes(pathOutput, data);
        }

        private static byte[] DecompressCodes(byte[] arch, Branch root, int dataLength) {
            int size = 0;
            Branch curr = root;
            int startIndex = 256 + 4;
            List<byte> bytes = new List<byte>();
            for (int i = startIndex; i < arch.Length; i++) {
                for (int bit = 1; bit <=128; bit<<=1) {
                    if ((arch[i] & bit) == 0) curr = curr.Zero;
                    else curr = curr.One;
                    if (curr.Zero != null || curr.One!=null) continue;
                    if (size++ < dataLength) bytes.Add(curr.Symbol);
                    curr = root;
                }
            }
            return bytes.ToArray();
        }

        private static void ParseHeader(byte[] arch, out List<Branch> listfreqs, out int dataLength) {
            dataLength = arch[0] | (arch[1] << 8) | (arch[1] << 16) | (arch[1] << 24);
            listfreqs = new List<Branch>();
            for (int i = 0; i < 256; i++) {
                if (arch[i + 4] > 0) listfreqs.Add(new Branch { Symbol = (byte)(i), Freq = arch[i + 4] });
            }
        }

        #region Compress
        private static byte[] Header(int length, Branch[] freqs) {
            List<byte> head = new List<byte>();
            head.Add((byte)length);
            head.Add((byte)(length>>8));
            head.Add((byte)(length>>16));
            head.Add((byte)(length>>24));
            var tmp = new byte[256];
            for (int i = 0; i < freqs.Length; i++) {
                tmp[freqs[i].Symbol] = (byte)freqs[i].Freq;
            }
            return head.Concat(tmp).ToArray();
        }

        private static byte[] CompressCodes(byte[] data, string[] codes) {
            List<byte> bits = new List<byte>();
            byte sum = 0;
            byte bitMask = 1;
            foreach(byte symbol in data) {
                foreach (char c in codes[symbol]) {
                    if (c == '1') sum |= bitMask;
                    if(bitMask<128)bitMask <<= 1;
                    else {
                        bitMask = 1;
                        bits.Add(sum);
                        sum = 0;
                    }
                }
            }
            if (bitMask > 1) bits.Add(sum);
            return bits.ToArray();
        }

        private static string[] MakeCodes(Branch root) {
            string[] codes = new string[256];
            Next(root, "");
            return codes;
            void Next(Branch branch,string s){
                if (branch.Zero == null && branch.One == null)
                    codes[branch.Symbol] = s;
                else {
                    Next(branch.Zero, s+"0");
                    Next(branch.One, s+"1");
                }
            }
            
        }

        private static Branch HuffTree(Branch[] freqs) {
            var que = new PriorityQueue<Branch>();
            for (int i = 0; i < freqs.Length; i++) 
                que.Enqueue(freqs[i].Freq, freqs[i]);

            while (que.Size > 1) {
                Branch zero = que.Dequeue();
                Branch one = que.Dequeue();
                Branch branch = new Branch { Freq = zero.Freq + one.Freq, One = one, Zero = zero };
                que.Enqueue(branch.Freq, branch);
            }
            return que.Dequeue();
        }

        #endregion
    }
}
