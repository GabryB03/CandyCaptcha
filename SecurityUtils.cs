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

public class SecurityUtils
{
    public static bool IsBrowserValid(string userAgent, string superProperties)
    {
        string properties = Utils.Base64Decode(superProperties);
        dynamic jss = JObject.Parse(properties);

        if (jss.browser_user_agent != userAgent)
        {
            if (userAgent.Contains("OPR/"))
            {
                string[] splitted1 = Strings.Split(userAgent, " OPR/");

                if (jss.browser_user_agent != splitted1[0])
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        string browser = jss.browser;

        if (!userAgent.Contains(browser + "/"))
        {
            return false;
        }

        string[] splitted = Strings.Split(userAgent, browser + "/");
        string browserVersion = splitted[1];

        if (browserVersion.Contains(" "))
        {
            browserVersion = browserVersion.Split(' ')[0];
        }

        if (jss.browser_version != browserVersion)
        {
            if (userAgent.Contains("OPR/"))
            {
                splitted = Strings.Split(userAgent, "OPR/");
                browserVersion = splitted[1];

                if (jss.browser_version != browserVersion)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        if (!userAgent.Contains("(" + jss.os))
        {
            return false;
        }

        if (!JSONUtils.IsJsonOrderValid(properties, new string[] { "os", "browser", "device", "system_locale", "browser_user_agent", "browser_version", "os_version", "referrer", "referring_domain", "referrer_current", "referring_domain_current", "release_channel", "client_build_number", "client_event_source" }))
        {
            return false;
        }

        return true;
    }

    public static bool CheckSuperProperties(string superProperties, dynamic ws)
    {
        string properties = Utils.Base64Decode(superProperties);
        dynamic jss = JObject.Parse(properties);

        if (jss.os != ws.d.properties.os || jss.browser != ws.d.properties.browser || jss.device != ws.d.properties.device || jss.system_locale != ws.d.properties.system_locale || jss.browser_user_agent != ws.d.properties.browser_user_agent || jss.browser_version != ws.d.properties.browser_version || jss.os_version != ws.d.properties.os_version || jss.referrer != ws.d.properties.referrer || jss.referring_domain != ws.d.properties.referring_domain || jss.referrer_current != ws.d.properties.referrer_current || jss.referring_domain_current != ws.d.properties.referring_domain_current || jss.release_channel != ws.d.properties.release_channel || jss.client_build_number != ws.d.properties.client_build_number || jss.client_event_source != ws.d.properties.client_event_source)
        {
            return false;
        }

        return true;
    }

    public static bool CheckDiscordIdentity(string discordIdentity, string userAgent)
    {
        string identity = Utils.Base64Decode(discordIdentity);
        dynamic jss = JObject.Parse(identity);

        if (!JSONUtils.IsJsonOrderValid(identity, new string[] { "r", "c", "u", "t", "s" }))
        {
            return false;
        }

        int r = (int)jss.r;
        string c = (string)jss.c;
        string u = (string)jss.u;
        int t = (int)jss.t;

        if (jss.s != null)
        {
            return false;
        }

        if (!(r >= 80 && r <= 162131))
        {
            return false;
        }

        if (c != "OPEN_CORD")
        {
            return false;
        }

        if (!(t >= 127831 && t <= 372133))
        {
            return false;
        }

        if (userAgent.Contains("OPR/") && !u.Contains(" OPR/"))
        {
            string realAgent = Strings.Split(userAgent, " OPR/")[0];

            if (realAgent != u)
            {
                return false;
            }
        }
        else
        {
            if (userAgent != u)
            {
                return false;
            }
        }

        return true;
    }

    public static string CheckAndGetBrowser(string browserInformations, string discordIdentity, string userAgent, string sec_ch_ua, string browserValidity, string browserIdentification, string browserProperties)
    {
        if (userAgent.ToLower().Contains("postman"))
        {
            return "NULL";
        }

        if (!CheckDiscordIdentity(discordIdentity, userAgent))
        {
            return "NULL";
        }

        string browser = "NULL", informations = Utils.Base64Decode(browserInformations);

        if (!JSONUtils.IsJsonOrderValid(informations, new string[] { "e", "t", "c", "i", "m", "d", "h", "k", "o" }))
        {
            return "NULL";
        }

        dynamic j_informations = JObject.Parse(informations);

        int e = (int)j_informations.e;
        int t = (int)j_informations.t;
        int i = (int)j_informations.i;
        int d = (int)j_informations.d;
        int h = (int)j_informations.h;
        int k = (int)j_informations.k;
        string c = (string)j_informations.c;

        if (c != "OPEN_CORD")
        {
            return "NULL";
        }

        if (j_informations.m != null)
        {
            return "NULL";
        }

        if (j_informations.o != null)
        {
            return "NULL";
        }

        if (e >= 60 && e <= 1283)
        {
            browser = "Chromedriver";
        }
        else if (t >= 3521 && t <= 4827)
        {
            browser = "Opera";
        }
        else if (i >= 7102 && i <= 8391)
        {
            browser = "Firefox";
        }
        else if (d >= 9129 && d <= 12712)
        {
            browser = "Brave";
        }
        else if (h >= 16382 && h <= 18321)
        {
            browser = "Chrome";
        }

        if (browser == "Chrome")
        {
            if (sec_ch_ua.Contains("\"(Not(A: Brand\";"))
            {
                return "NULL";
            }
        }
        else if (browser == "Chromedriver" || browser == "NULL")
        {
            return "NULL";
        }

        string validity = Utils.Base64Decode(browserValidity);
        dynamic j_validity = JObject.Parse(validity);

        if (!JSONUtils.IsJsonOrderValid(validity, new string[] { "e", "c", "i", "u", "r", "l", "p", "f", "n", "t", "k", "b" }))
        {
            return "NULL";
        }

        if (j_validity.c != "OPEN_CORD")
        {
            return "NULL";
        }

        ulong j_i = (ulong)j_validity.i;

        if (!(j_i >= 900000000000000000L && j_i <= 980000000000000000L))
        {
            return "NULL";
        }

        ulong j_r = (ulong)j_validity.r;

        if (!(j_r >= 1246576837 && j_r <= 1846576879))
        {
            return "NULL";
        }

        if (j_validity.b != null)
        {
            return "NULL";
        }

        if (j_validity.p != null)
        {
            return "NULL";
        }

        ulong j_t = (ulong)j_validity.t;
        ulong j_e = (ulong)j_validity.e;

        if ((j_t - 1504587432L) != j_e)
        {
            return "NULL";
        }

        ulong browserID = (ulong)j_validity.l;
        string realBrowserName = "NULL";

        if (browserID >= 920000000000000000L && browserID <= 930000000000000000L)
        {
            realBrowserName = "Opera";
        }
        else if (browserID >= 940000000000000000 && browserID <= 950000000000000000L)
        {
            realBrowserName = "Firefox";
        }
        else if (browserID >= 960000000000000000 && browserID <= 970000000000000000L)
        {
            realBrowserName = "Brave";
        }
        else if (browserID >= 980000000000000000L && browserID <= 990000000000000000L)
        {
            realBrowserName = "Chrome";
        }

        if (realBrowserName != browser)
        {
            return "NULL";
        }

        string u = (string)j_validity.u;

        if (userAgent.Contains("OPR/") && !u.Contains("OPR/"))
        {
            string realAgent = Strings.Split(userAgent, " OPR/")[0];

            if (realAgent != u)
            {
                return "NULL";
            }
        }
        else
        {
            if (userAgent != u)
            {
                return "NULL";
            }
        }

        List<object> numbers = JObject.Parse(validity).GetValue("n").ToObject<List<object>>();

        if (numbers.Count != 15)
        {
            return "NULL";
        }

        List<object> ids = JObject.Parse(validity).GetValue("k").ToObject<List<object>>();

        if (ids.Count != 15)
        {
            return "NULL";
        }

        for (int j = 0; j < numbers.Count; j++)
        {
            var element = numbers[j];
            ulong elm = 0;

            if (element != null)
            {
                elm = ulong.Parse(element.ToString());
            }

            if (j == 0)
            {
                if (!(elm >= 48739872L && elm <= 83720192L))
                {
                    return "NULL";
                }
            }
            else if (j == 1)
            {
                if (!(elm >= 984372L && elm <= 732456453L))
                {
                    return "NULL";
                }
            }
            else if (j == 2)
            {
                if (!(elm >= 56473L && elm <= 7483921L))
                {
                    return "NULL";
                }
            }
            else if (j == 3)
            {
                if (element != null)
                {
                    return "NULL";
                }
            }
            else if (j == 4)
            {
                if (!(elm >= 4832L && elm <= 827111L))
                {
                    return "NULL";
                }
            }
            else if (j == 5)
            {
                if (element != null)
                {
                    return "NULL";
                }
            }
            else if (j == 6)
            {
                if (!(elm >= 483212L && elm <= 5463728L))
                {
                    return "NULL";
                }
            }
            else if (j == 7)
            {
                if (elm != j_t - 1404567432L)
                {
                    return "NULL";
                }
            }
            else if (j == 8)
            {
                if (!(elm >= 83294702L && elm <= 5483759432L))
                {
                    return "NULL";
                }
            }
            else if (j == 9)
            {
                if (!(elm >= 743212L && elm <= 8473564732L))
                {
                    return "NULL";
                }
            }
            else if (j == 10)
            {
                if (!(elm >= 57434L && elm <= 7564732L))
                {
                    return "NULL";
                }
            }
            else if (j == 11)
            {
                if (element != null)
                {
                    return "NULL";
                }
            }
            else if (j == 12)
            {
                if (!(elm >= 8473621L && elm <= 84756437L))
                {
                    return "NULL";
                }
            }
            else if (j == 13)
            {
                if (!(elm >= 5746L && elm <= 7584932L))
                {
                    return "NULL";
                }
            }
            else if (j == 14)
            {
                if (!(elm >= 46321L && elm <= 4859435768L))
                {
                    return "NULL";
                }
            }
        }

        for (int j = 0; j < ids.Count; j++)
        {
            var element = ids[j];
            ulong elm = 0;

            if (element != null)
            {
                elm = ulong.Parse(element.ToString());
            }

            if (j == 5)
            {
                if (elm.ToString().Substring(0, 13) != (browserID + 7483929382L).ToString().Substring(0, 13))
                {
                    return "NULL";
                }
            }
            else if (j == 9)
            {
                if (elm.ToString().Substring(0, 13) != (browserID + j_t).ToString().Substring(0, 13))
                {
                    return "NULL";
                }
            }
            else
            {
                if (!(elm >= 900000000000000000L && elm <= 980000000000000000L))
                {
                    return "NULL";
                }
            }
        }

        ulong actualTimestamp = (ulong)TimestampUtils.GetTimestamp();
        ulong difference = actualTimestamp - j_t;

        if (difference > 10)
        {
            return "NULL";
        }

        if ((string)j_validity.f != CryptoUtils.GetMD5("MRJKWERHKJWEHRKJWHERKJHWEKRJHWEJKRHKWEJHRKJWEHRKJWEHR" + (j_t - 1518594939L).ToString() + "KWEKRJHWEHRJWERKJWEKJRHWKEJRHKJWEHRJKWHERKJHWERKJHWEKJRHWEKJHRKJWEHR"))
        {
            return "NULL";
        }

        string theProperties = Utils.Base64Decode(browserProperties);
        dynamic j_uproperties = JObject.Parse(theProperties);

        if (!JSONUtils.IsJsonOrderValid(theProperties, new string[] { "u", "h", "i", "m", "k", "l" }))
        {
            return "NULL";
        }

        string deviceUUID = Utils.Base64Decode((string)j_uproperties.u);

        if (((string)j_uproperties.h) != CryptoUtils.GetMD5("ERKJHEJHRJERJKHWEJRKWJHERKJWHEKRJHWEKJRH" + j_uproperties.u + "HEJRHKJERHKJHERKJHWEKJRHWKEJHRKJWEHRKJHEWR"))
        {
            return "NULL";
        }

        string requestUUID = j_uproperties.i;

        if (j_uproperties.m != CryptoUtils.GetMD5("UWERJHWEJKRHJKWEHRJKHWERJKHWEKJRHKWJEHR" + requestUUID + "JERHEJHRJKEHWRKJHWEJKRHWEJKHRKJWEHRKJHWERJH"))
        {
            return "NULL";
        }

        if (!(j_uproperties.k >= 20000 && j_uproperties.k <= 50000))
        {
            return "NULL";
        }

        if (CryptoUtils.GetMD5("JERJHWEJRKWEJHRKJWHERJKHWEJKRHWKEJHRJKWHERKJHWER" + ((string)j_uproperties.k) + "OEWRKJWERHJHWERJKHKJWEHRKJWEHRKJHWEKRJHWEKJRH") != (string)j_uproperties.l)
        {
            return "NULL";
        }

        string identification = Utils.Base64Decode(browserIdentification);
        dynamic j_identification = JObject.Parse(identification);

        if (!JSONUtils.IsJsonOrderValid(identification, new string[] { "t", "e", "r", "y", "o", "k", "m", "f", "j" }))
        {
            return "NULL";
        }

        if ((string)j_identification.e != CryptoUtils.GetMD5("EJHRHWEJRKJWHER" + j_t.ToString() + "WKREJWEKRJWEKRJEWR" + discordIdentity))
        {
            return "NULL";
        }

        if ((string)j_identification.y != CryptoUtils.GetMD5("KERHWEJRHJWERKJWHERKJHWER" + j_t.ToString() + "IEWRJHWEJRHJWEHRJHEWR" + browserInformations))
        {
            return "NULL";
        }

        if ((string)j_identification.k != CryptoUtils.GetMD5("MEJHJRHJWHERKJWEKRJH" + j_t.ToString() + "OERKHEJRHKJWEHRJHEWRJKHWER" + browserValidity))
        {
            return "NULL";
        }

        if ((string)j_identification.f != CryptoUtils.GetMD5("KJWEKRJKWEJRKLWJERLKJWERKLJWER" + j_t.ToString() + "LERKHWEKRJHWJEHRKJHWERKJHWEKJHRKWJRHE" + browserProperties))
        {
            return "NULL";
        }

        if ((ulong)j_identification.t != j_t)
        {
            return "NULL";
        }

        if ((int)j_identification.r != discordIdentity.Length)
        {
            return "NULL";
        }

        if ((int)j_identification.o != browserInformations.Length)
        {
            return "NULL";
        }

        if ((int)j_identification.m != browserValidity.Length)
        {
            return "NULL";
        }

        if ((int)j_identification.j != browserProperties.Length)
        {
            return "NULL";
        }

        return browser + "|" + j_t.ToString() + "|" + deviceUUID + "|" + requestUUID;
    }

    public static bool CheckBody(string body)
    {
        dynamic jss = JObject.Parse(body);
        int movements = 0;
        int screenWidth = jss.sw, screenHeight = jss.sh;
        int lastPosX = 0, lastPosY = 0;

        foreach (var coso in jss.mp)
        {
            movements++;
            int incrementalX = coso[2] + 35212;
            int incrementalY = coso[3] + 12732;
            string hash = coso[4];
            int posX = coso[0] - incrementalX;
            int posY = coso[1] - incrementalY;
            int windowWidth = coso[5], windowHeight = coso[6];

            if (hash != CryptoUtils.GetMD5("WERHJWERKJWEHRKJHWERKJHWEKJHR" + incrementalX.ToString() + incrementalY.ToString() + posX.ToString() + posY.ToString() + "KJLERJKWEJRLKJWELKRJWLKEJRLWEKJR"))
            {
                return false;
            }

            if (posX < 0)
            {
                return false;
            }

            if (posY < 0)
            {
                return false;
            }

            if (posX > screenWidth)
            {
                return false;
            }

            if (posY > screenHeight)
            {
                return false;
            }

            if (posX > windowWidth)
            {
                return false;
            }

            if (posY > windowHeight)
            {
                return false;
            }
        }

        if (movements < 2)
        {
            return false;
        }

        if (movements > 200)
        {
            return false;
        }

        long startTimestamp = jss.st, endTimestamp = jss.et;

        if (endTimestamp - startTimestamp <= 0)
        {
            return false;
        }

        if (endTimestamp - startTimestamp >= 15)
        {
            return false;
        }

        return true;
    }
}