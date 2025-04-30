using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.IO;

namespace BitTorrent;

public static class BEncoding
{
    private static byte DictionaryStart = Encoding.UTF8.GetBytes("d")[0]; // 100
    private static byte DictionaryEnd = Encoding.UTF8.GetBytes("e")[0]; // 101
    private static byte ListStart = Encoding.UTF8.GetBytes("l")[0]; // 108
    private static byte ListEnd = Encoding.UTF8.GetBytes("e")[0]; // 101
    private static byte NumberStart = Encoding.UTF8.GetBytes("i")[0]; // 105
    private static byte NumberEnd = Encoding.UTF8.GetBytes("e")[0]; // 101
    private static byte ByteArrayDivider = Encoding.UTF8.GetBytes(":")[0]; // 58

    public static object Decode(byte[] bytes)
    {
        IEnumerator<byte> enumerator = ((IEnumerable<byte>)bytes).GetEnumerator();
        enumerator.MoveNext();
        return DecodeNextObject(enumerator);
    }

    private static object DecodeNextObject(IEnumerator<byte> enumerator)
    {
        if (enumerator.Current == DictionaryStart)
            return DecodeDictionary(enumerator);
        if (enumerator.Current == ListStart)
            return DecodeList(enumerator);
        if (enumerator.Current == NumberStart)
            return DecodeNumber(enumerator);
        return DecodeByteArray(enumerator);
    }

    private static object DecodeDictionary(IEnumerator<byte> enumerator)
    {
        throw new NotImplementedException("this method was not implemented");
    }

    private static object DecodeList(IEnumerator<byte> enumerator)
    {
        throw new NotImplementedException("this method was not implemented");
    }

    private static object DecodeNumber(IEnumerator<byte> enumerator)
    {
        throw new NotImplementedException("this method was not implemented");
    }

    private static object DecodeByteArray(IEnumerator<byte> enumerator)
    {
        throw new NotImplementedException("this method was not implemented");
    }
}

