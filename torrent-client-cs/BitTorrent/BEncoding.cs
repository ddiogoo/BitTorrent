using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.IO;

namespace BitTorrent;

public static class BEncoding {
    private static byte DictionaryStart  = Encoding.UTF8.GetBytes("d")[0]; // 100
    private static byte DictionaryEnd    = Encoding.UTF8.GetBytes("e")[0]; // 101
    private static byte ListStart        = Encoding.UTF8.GetBytes("l")[0]; // 108
    private static byte ListEnd          = Encoding.UTF8.GetBytes("e")[0]; // 101
    private static byte NumberStart      = Encoding.UTF8.GetBytes("i")[0]; // 105
    private static byte NumberEnd        = Encoding.UTF8.GetBytes("e")[0]; // 101
    private static byte ByteArrayDivider = Encoding.UTF8.GetBytes(":")[0]; // 58

    public static object Decode(byte[] bytes) {
        IEnumerator<byte> enumerator = ((IEnumerable<byte>)bytes).GetEnumerator();
        enumerator.MoveNext();
        return DecodeNextObject(enumerator);
    }
    
    private static object DecodeNextObject(IEnumerator<byte> enumerator) {
        if(enumerator.Current == DictionaryStart)
            return null; // It will be implemented the DecodeDictionary(enumerator) method
        if(enumerator.Current == ListStart)
            return null; // It will be implemented the DecodeList(enumerator) method
        if(enumerator.Current == NumberStart)
            return null; // It will be implemented the DecodeNumber(enumerator) method
        return null; // It will be implemented the DecodeByteArray(enumerator) method
    }
}

