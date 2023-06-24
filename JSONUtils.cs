using System.Security.Cryptography;
using System.Text;
using System;
using System.IO;
using System.IO.Compression;
using Microsoft.VisualBasic;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Newtonsoft.Json.Linq;
using Microsoft.VisualBasic;
using System.Linq;
using System.Net;

public class JSONUtils
{
    public static string ParseJsonString(string str)
    {
        if (str == "" || str == null)
        {
            return "null";
        }

        return "\"" + str + "\"";
    }

    public static bool IsJsonOrderValid(string payload, string[] keys)
    {
        Dictionary<string, object> dictObj = JObject.Parse(payload).ToObject<Dictionary<string, object>>();

        if (dictObj.Keys.Count != keys.Length)
        {
            return false;
        }

        for (int i = 0; i < dictObj.Keys.Count; i++)
        {
            if (dictObj.Keys.ElementAt(i) != keys[i])
            {
                return false;
            }
        }

        return true;
    }

    public static bool IsJsonOrderValid(string payload, string primaryKey, string[] keys)
    {
        return IsJsonOrderValid(JObject.Parse(payload).GetValue(primaryKey).ToObject<Dictionary<string, object>>(), keys);
    }

    public static bool IsJsonOrderValid(Dictionary<string, object> dictObj, string[] keys)
    {
        if (dictObj.Keys.Count != keys.Length)
        {
            return false;
        }

        for (int i = 0; i < dictObj.Keys.Count; i++)
        {
            if (dictObj.Keys.ElementAt(i) != keys[i])
            {
                return false;
            }
        }

        return true;
    }
}