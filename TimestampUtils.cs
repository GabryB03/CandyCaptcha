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

public static class TimestampUtils
{
    public static long GetTimestamp()
    {
        return ((DateTimeOffset)DateTime.UtcNow.ToUniversalTime()).ToUniversalTime().ToUnixTimeSeconds();
    }

    public static long GetDiscordTimestamp()
    {
        return ((DateTimeOffset)DateTime.UtcNow.ToUniversalTime()).ToUniversalTime().ToUnixTimeMilliseconds();
    }

    public static string GetDiscordTime()
    {
        string[] splitted = Microsoft.VisualBasic.Strings.Split(((DateTimeOffset)DateTime.UtcNow.ToUniversalTime()).ToUniversalTime().ToString("o"), "+");
        string formatted = splitted[0];

        while (formatted.Length > 26)
        {
            formatted = formatted.Substring(0, formatted.Length - 1);
        }

        return formatted + "+" + splitted[1];
    }

    public static bool IsDateValid(string value)
    {
        DateTime tempDate;

        if (DateTime.TryParseExact(value, "yyyy-MM-dd", DateTimeFormatInfo.InvariantInfo, DateTimeStyles.None, out tempDate))
        {
            return true;
        }

        return false;
    }

    public static DateTime ParseDate(string value)
    {
        DateTime tempDate;
        DateTime.TryParseExact(value, "yyyy-MM-dd", DateTimeFormatInfo.InvariantInfo, DateTimeStyles.None, out tempDate);

        return tempDate;
    }

    public static int GetAge(DateTime birthDate, DateTime now)
    {
        int age = now.Year - birthDate.Year;

        if (now.Month < birthDate.Month || (now.Month == birthDate.Month && now.Day < birthDate.Day))
        {
            age--;
        }

        return age;
    }
}