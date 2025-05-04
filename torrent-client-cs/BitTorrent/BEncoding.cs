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

    #region Decode
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
        Dictionary<string, object> dict = new Dictionary<string, object>();
        List<string> keys = new List<string>();
        while (enumerator.MoveNext())
        {
            if (enumerator.Current == DictionaryEnd) break;
            string key = Encoding.UTF8.GetString(DecodeByteArray(enumerator));

            enumerator.MoveNext();
            object val = DecodeNextObject(enumerator);

            keys.Add(key);
            dict.Add(key, val);
        }
        var sortedKeys = keys.OrderBy(x => BitConverter.ToString(Encoding.UTF8.GetBytes(x)));
        if (!keys.SequenceEqual(sortedKeys))
            throw new Exception("error loading dictionary: keys not sorted");
        return dict;
    }

    private static List<object> DecodeList(IEnumerator<byte> enumerator)
    {
        List<object> list = new List<object>();
        while (enumerator.MoveNext())
        {
            if (enumerator.Current == ListEnd) break;
            list.Add(DecodeNextObject(enumerator));
        }
        return list;
    }

    private static long DecodeNumber(IEnumerator<byte> enumerator)
    {
        List<byte> bytes = new List<byte>();
        while (enumerator.MoveNext())
        {
            if (enumerator.Current == NumberEnd) break;
            bytes.Add(enumerator.Current);
        }
        string numAsString = Encoding.UTF8.GetString(bytes.ToArray());
        return Int64.Parse(numAsString);
    }

    private static byte[] DecodeByteArray(IEnumerator<byte> enumerator)
    {
        List<byte> lengthBytes = new List<byte>();
        do
        {
            if (enumerator.Current == ByteArrayDivider) break;
            lengthBytes.Add(enumerator.Current);
        } while (enumerator.MoveNext());
        string lengthString = Encoding.UTF8.GetString(lengthBytes.ToArray());

        int length;
        if (!Int32.TryParse(lengthString, out length))
            throw new Exception("unable to parse length of byte array");

        byte[] bytes = new byte[length];
        for (int i = 0; i < length; i++)
        {
            enumerator.MoveNext();
            bytes[i] = enumerator.Current;
        }
        return bytes;
    }

    public static object DecodeFile(string path)
    {
        if (!File.Exists(path))
            throw new FileNotFoundException("unable to find file " + path);
        byte[] bytes = File.ReadAllBytes(path);
        return Decode(bytes);
    }
    #endregion

    #region Encode
    public static byte[] Encode(object obj)
    {
        MemoryStream buffer = new MemoryStream();
        EncodeNextObject(buffer, obj);
        return buffer.ToArray();
    }

    public static void EncodeFile(object obj, string path)
    {
        File.WriteAllBytes(path, Encode(obj));
    }

    private static void EncodeNextObject(MemoryStream buffer, object obj)
    {
        if (obj is byte[])
            EncodeByteArray(buffer, (byte[])obj);
        else if (obj is string)
            EncodeString(buffer, (string)obj);
        else if (obj is long)
            EncodeNumber(buffer, (long)obj);
        else if (obj.GetType() == typeof(List<object>))
            EncodeList(buffer, (List<object>)obj);
        else if (obj.GetType() == typeof(Dictionary<string, object>))
            EncodeDictionary(buffer, (Dictionary<string, object>)obj);
        else
            throw new Exception("unable to encode type " + obj.GetType());
    }

    private static void EncodeNumber(MemoryStream buffer, long input)
    {
        buffer.Append(NumberStart);
        buffer.Append(Encoding.UTF8.GetBytes(Convert.ToString(input)));
        buffer.Append(NumberEnd);
    }

    private static void EncodeByteArray(MemoryStream buffer, byte[] body)
    {
        buffer.Append(Encoding.UTF8.GetBytes(Convert.ToString(body.Length)));
        buffer.Append(ByteArrayDivider);
        buffer.Append(body);
    }

    private static void EncodeString(MemoryStream buffer, string input)
    {
        EncodeByteArray(buffer, Encoding.UTF8.GetBytes(input));
    }

    private static void EncodeList(MemoryStream buffer, List<object> input)
    {
        buffer.Append(ListStart);
        foreach (var item in input)
            EncodeNextObject(buffer, item);
        buffer.Append(ListEnd);
    }

    private static void EncodeDictionary(MemoryStream buffer, Dictionary<string, object> input)
    {
        buffer.Append(DictionaryStart);
        var sortedKeys = input.Keys.ToList().OrderBy(x => BitConverter.ToString(Encoding.UTF8.GetBytes(x)));
        foreach (var key in sortedKeys)
        {
            EncodeString(buffer, key);
            EncodeNextObject(buffer, input[key]);
        }
        buffer.Append(DictionaryEnd);
    }
    #endregion
}

public static class MemoryStreamExtensions
{
    public static void Append(this MemoryStream stream, byte value)
    {
        stream.WriteByte(value);
    }

    public static void Append(this MemoryStream stream, byte[] values)
    {
        stream.Write(values, 0, values.Length);
    }
}

