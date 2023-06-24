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
using System.Reflection;

public class Utils
{
    public static string[] allowedLocales = new string[] { "da", "de", "en-GB", "en-US", "en-ES", "fr", "hr", "it", "lt", "hu", "nl", "no", "pl", "pt-BR", "ro", "fi", "sv-SE", "vi", "tr", "cs", "el", "bg", "ru", "uk", "hi", "th", "zh-CN", "ja", "zh-TW", "ko" };
    public static ResourceSemaphore idSemaphore = new ResourceSemaphore();
    private static char[] theNumbers = "0123456789".ToCharArray();

    public static IEnumerable<string> SplitToLines(string input)
    {
        if (input == null)
        {
            yield break;
        }

        using (System.IO.StringReader reader = new System.IO.StringReader(input))
        {
            string line;

            while ((line = reader.ReadLine()) != null)
            {
                yield return line;
            }
        }
    }

    public static string Base64Decode(string base64)
    {
        return Encoding.UTF8.GetString(Convert.FromBase64String(base64));
    }

    public static byte[] ReadFully(Stream input)
    {
        using (MemoryStream ms = new MemoryStream())
        {
            input.CopyTo(ms);
            return ms.ToArray();
        }
    }

    public static int GetSuperUserFlags()
    {
        return 1 | 2 | 4 | 8 | 16 | 32 | 64 | 128 | 256 | 512 | 1024 | 2048 | 4096 | 16384 | 32768 | 65536 | 131072 | 262144 | 524288;
    }

    public static bool IsHeadersOrderValid(string[] headers, string[] realHeaders)
    {
        if (headers.Length != realHeaders.Length)
        {
            return false;
        }

        for (int i = 0; i < headers.Length; i++)
        {
            if (headers[i] != realHeaders[i])
            {
                return false;
            }
        }

        return true;
    }

    public static void InjectResponse(HttpRequest request, HttpResponse response, string content, int statusCode = 200, string statusDescription = "OK")
    {
        response.SetStatusCode(statusCode);
        response.SetStatusDescription(statusDescription);
        response.SetBody(content);
        response.AddHeader("Content-Length", response.GetBodyLength().ToString());
        request.WriteResponse(response);
    }

    public static void InjectResponse(HttpRequest request, HttpResponse response, byte[] content, int statusCode = 200, string statusDescription = "OK")
    {
        response.SetStatusCode(statusCode);
        response.SetStatusDescription(statusDescription);
        response.SetBody(content);
        response.AddHeader("Content-Length", response.GetBodyLength().ToString());
        request.WriteResponse(response);
    }

    public static bool IsLocaleAllowed(string locale)
    {
        foreach (string aLocale in allowedLocales)
        {
            if (locale.Equals(aLocale))
            {
                return true;
            }
        }

        return false;
    }

    public static string ReplaceFirst(string text, string search, string replace)
    {
        int pos = text.IndexOf(search);

        if (pos < 0)
        {
            return text;
        }

        return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
    }

    public static Tuple<bool, string[], string[]> IsURLValid(string url, string format)
    {
        try
        {
            List<string> ids = new List<string>(), numbers = new List<string>();
            bool isAdding = false;
            string actualNumber = "";

            foreach (char c in url.ToCharArray())
            {
                bool exist = false;

                foreach (char s in theNumbers)
                {
                    if (c.Equals(s))
                    {
                        exist = true;
                        break;
                    }
                }

                if (exist)
                {
                    actualNumber += c.ToString();
                }
                else
                {
                    if (actualNumber != "")
                    {
                        if (actualNumber.Length == 18)
                        {
                            ids.Add(actualNumber);
                        }
                        else
                        {
                            numbers.Add(actualNumber);
                        }

                        actualNumber = "";
                    }
                }
            }

            if (actualNumber != "")
            {
                if (actualNumber.Length == 18)
                {
                    ids.Add(actualNumber);
                }
                else
                {
                    numbers.Add(actualNumber);
                }

                actualNumber = "";
            }

            foreach (string num in ids)
            {
                format = ReplaceFirst(format, "%ID%", num);
            }

            foreach (string num in numbers)
            {
                format = ReplaceFirst(format, "%NUM%", num);
            }

            if (url.Equals(format))
            {
                return new Tuple<bool, string[], string[]>(true, ids.ToArray(), numbers.ToArray());
            }

            return new Tuple<bool, string[], string[]>(false, ids.ToArray(), numbers.ToArray());
        }
        catch
        {
            return new Tuple<bool, string[], string[]>(false, null, null);
        }
    }
}