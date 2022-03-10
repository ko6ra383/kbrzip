using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace HuffLib {
    public static class Huffman {
        public static void Compress(string dataFileName, string archFileNme) {
            byte[] data = File.ReadAllBytes(dataFileName);
            byte[] arch = CompressByte(data);
            File.WriteAllBytes(archFileNme, arch);
        }

        private static byte[] CompressByte(byte[] data) {
            int[] freqs = ClulateFreq(data);
            Node root = CreateTree(freqs);
            string[] codes = CreateCodes(root);
            byte[] bits = CompressBit(data, codes);
            byte[] head = CreateHeader(data.Length, freqs);
            return head.Concat(bits).ToArray();
        }


        private static int[] ClulateFreq(byte[] data) {
            int[] freqs = new int[256];
            foreach(byte d in data) {
                freqs[d]++;
            }
            NormalizeFreqs();
            return freqs;

            void NormalizeFreqs() {
                int max = freqs.Max();
                if (max <= 255) return;
                for (int j = 0; j < 256; j++) {
                    if (freqs[j] > 0) freqs[j] = 1 + freqs[j] * 255 / (max + 1);
                }
            }
        }

        private static Node CreateTree(int[] freqs) {
            var pq = new PriorityQueue<Node>();
            for(int j = 0; j < 256; j++) 
                if (freqs[j] > 0) 
                    pq.Enqueue(freqs[j], new Node((byte)j, freqs[j]));
            while(pq.Size() > 1) {
                Node bit0 = pq.Dequeue();
                Node bit1 = pq.Dequeue();
                int freq = bit0.freq + bit1.freq;
                Node next = new Node(bit0, bit1, freq);
                pq.Enqueue(freq, next);
            }
            return pq.Dequeue();
        }

        private static string[] CreateCodes(Node root) {
            string[] codes = new string[256];
            Next(root, "");
            return codes;

            void Next(Node node, string code) {
                if (node.bit0 == null && node.bit1 == null) {
                    codes[node.symbol] = code;
                }
                else {
                    Next(node.bit0, code + "0");
                    Next(node.bit1, code + "1");
                }
            }
        }

        private static byte[] CompressBit(byte[] data, string[] codes) {
            List<byte> bits = new List<byte>();
            byte sum = 0;
            byte bit = 1;
            foreach (byte symbol in data) {
                foreach (char c in codes[symbol]) {
                    if (c == '1') sum |= bit;
                    if (bit < 128) bit <<= 1;
                    else {
                        bits.Add(sum);
                        sum = 0;
                        bit = 1;
                    }
                }
            }
            if (bit > 1) bits.Add(sum);
            return bits.ToArray();
        }

        private static byte[] CreateHeader(int datalength, int[] freqs) {
            List<byte> head = new List<byte>(); //записал int количества
            head.Add((byte)(datalength & 255));
            head.Add((byte)(datalength >> 8 & 255));
            head.Add((byte)(datalength >> 16 & 255));
            head.Add((byte)(datalength >> 24 & 255));
            for (int i = 0; i < 256; i++) {
                head.Add((byte)freqs[i]);
            }
            return head.ToArray();
        }

        public static void Decompress(string archFilename, string dataFile) {
            byte[] arch = File.ReadAllBytes(archFilename);
            byte[] data = DecompressData(arch);
            File.WriteAllBytes(dataFile, data);
        }

        private static byte[] DecompressData(byte[] arch) {
            ParseHeader(arch, out int dataLenght, out int startIndex, out int[] freq);
            Node root = CreateTree(freq);
            byte[] data = DecompressBit(arch, startIndex, dataLenght, root);
            return data;
        }

        private static void ParseHeader(byte[] arch, out int dataLenght, out int startIndex, out int[] freq) {
            dataLenght = arch[0] | (arch[1] << 8) | (arch[2] << 16) | (arch[3] << 24);
            freq = new int[256];
            for (int i = 0; i < 256; i++) 
                freq[i] = arch[i + 4];
            startIndex = 4 + 256;

        }

        private static byte[] DecompressBit(byte[] arch, int startIndex, int dataLenght, Node root) {
            int size=0;
            Node curr = root;
            List<byte> data = new List<byte>();
            for (int i = startIndex; i < arch.Length; i++) {
                for (int bit = 1; bit <= 128; bit<<=1) {
                    bool zero = (arch[i] & bit)==0;
                    if (zero) curr = curr.bit0;
                    else curr = curr.bit1;

                    if (curr.bit0 != null) continue;
                    if (size++ < dataLenght)
                        data.Add(curr.symbol);
                    curr = root;
                }
            }
            return data.ToArray();
        }

    }
}
