using System;
using System.Threading;

class Program
{
    public static HttpServer server;
    public static RequestChecker requestChecker;

    public static string[] googleChromeHeaders = new string[]
    {
        "Host", "Connection", "Content-Length", "sec-ch-ua", "X-CHK", "sec-ch-ua-mobile",
        "User-Agent", "X-IDF", "Content-type", "X-VLD", "X-BRV", "X-DID", "X-PRP", "sec-ch-ua-platform",
        "Accept", "Origin", "Sec-Fetch-Site", "Sec-Fetch-Mode", "Sec-Fetch-Dest", "Referer",
        "Accept-Encoding", "Accept-Language"
    };

    public static string[] mozillaFirefoxHeaders = new string[]
    {
        "Host", "User-Agent", "Accept", "Accept-Language", "Accept-Encoding", "Content-type",
        "X-DID", "X-BRV", "X-VLD", "X-PRP", "X-IDF", "X-CHK", "Content-Length", "Origin",
        "Connection", "Referer", "Sec-Fetch-Dest", "Sec-Fetch-Mode", "Sec-Fetch-Site"
    };

    public static string[] operaHeaders = new string[]
    {
        "Host", "Connection", "Content-Length", "sec-ch-ua", "X-CHK", "sec-ch-ua-mobile", "User-Agent",
        "X-IDF", "Content-type", "X-VLD", "X-BRV", "X-DID", "X-PRP", "sec-ch-ua-platform", "Accept",
        "Origin", "Sec-Fetch-Site", "Sec-Fetch-Mode", "Sec-Fetch-Dest", "Referer", "Accept-Encoding", "Accept-Language"
    };

    public static string[] braveHeaders = new string[]
    {
        "Host", "Connection", "Content-Length", "X-CHK", "User-Agent", "X-IDF", "Content-type", "X-VLD",
        "X-BRV", "X-PRP", "X-DID", "Accept", "Sec-GPC", "Origin", "Sec-Fetch-Site", "Sec-Fetch-Mode", "Sec-Fetch-Dest",
        "Referer", "Accept-Encoding", "Accept-Language"
    };

    public static void Main()
    {
        Console.Title = "CandyCaptcha";

        requestChecker = new RequestChecker();

        server = new HttpServer(80);
        server.Start();

        while (true)
        {
            try
            {
                HttpRequest request = server.HandleRequest();

                try
                {
                    if (request.GetURI().Equals("/") && request.GetMethodStr().Equals("GET"))
                    {
                        HttpResponse response = new HttpResponse();
                        response.SetBody(System.IO.File.ReadAllBytes("client\\index-minified.html"));
                        request.WriteResponse(response);
                    }
                    else if (request.GetURI().Equals("/background.jpg") && request.GetMethodStr().Equals("GET"))
                    {
                        HttpResponse response = new HttpResponse();
                        response.SetBody(System.IO.File.ReadAllBytes("client\\background.jpg"));
                        request.WriteResponse(response);
                    }
                    else if (request.GetURI().Equals("/verification.js") && request.GetMethodStr().Equals("GET"))
                    {
                        HttpResponse response = new HttpResponse();
                        response.SetBody(System.IO.File.ReadAllBytes("client\\verification.js"));
                        request.WriteResponse(response);
                    }
                    else if (request.GetURI().Equals("/favicon.ico") && request.GetMethodStr().Equals("GET"))
                    {
                        HttpResponse response = new HttpResponse();
                        response.SetBody(System.IO.File.ReadAllBytes("client\\favicon.ico"));
                        request.WriteResponse(response);
                    }
                    else if (request.GetURI().Equals("/verify") && request.GetMethodStr().Equals("POST"))
                    {
                        HttpResponse response = new HttpResponse();
                        int i = 0;
                        Browser browser = Browser.Chrome;

                        foreach (HttpHeader header in request.GetHeaders())
                        {
                            if (!header.GetName().Equals(googleChromeHeaders[i]))
                            {
                                browser = Browser.Unknown;
                                break;
                            }

                            i++;
                        }

                        if (browser != Browser.Unknown)
                        {
                            goto verification;
                        }

                        browser = Browser.Firefox;
                        i = 0;

                        foreach (HttpHeader header in request.GetHeaders())
                        {
                            if (!header.GetName().Equals(mozillaFirefoxHeaders[i]))
                            {
                                browser = Browser.Unknown;
                                break;
                            }

                            i++;
                        }

                        if (browser != Browser.Unknown)
                        {
                            goto verification;
                        }

                        browser = Browser.Opera;
                        i = 0;

                        foreach (HttpHeader header in request.GetHeaders())
                        {
                            if (!header.GetName().Equals(operaHeaders[i]))
                            {
                                browser = Browser.Unknown;
                                break;
                            }

                            i++;
                        }

                        if (browser != Browser.Unknown)
                        {
                            goto verification;
                        }

                        browser = Browser.Brave;
                        i = 0;

                        foreach (HttpHeader header in request.GetHeaders())
                        {
                            if (!header.GetName().Equals(braveHeaders[i]))
                            {
                                browser = Browser.Unknown;
                                break;
                            }

                            i++;
                        }

                        if (request.GetBody().Replace(" ", "").Replace('\t'.ToString(), "").Equals(""))
                        {
                            browser = Browser.Unknown;
                        }

                        verification: if (browser.Equals(Browser.Unknown) || request.GetHeaders().Count == 0)
                        {
                            response.SetBody("Non sei umano.");
                            response.SetStatusCode(400);
                            response.SetStatusDescription("Bad Request");
                        }
                        else
                        {
                            string entityBody = request.GetBody();
                            bool valid = true;

                            if (browser.Equals(Browser.Firefox))
                            {
                                if (!request.GetHeader("Host").Equals("127.0.0.1"))
                                {
                                    valid = false;
                                }
                                else if (!request.GetHeader("Accept").Equals("*/*"))
                                {
                                    valid = false;
                                }
                                else if (!request.GetHeader("Accept-Language").Equals("it-IT,it;q=0.8,en-US;q=0.5,en;q=0.3"))
                                {
                                    valid = false;
                                }
                                else if (!request.GetHeader("Accept-Encoding").Equals("gzip, deflate"))
                                {
                                    valid = false;
                                }
                                else if (!request.GetHeader("Content-type").Equals("application/json"))
                                {
                                    valid = false;
                                }
                                else if (!request.GetHeader("Content-Length").Equals(request.GetBody().Length.ToString()))
                                {
                                    valid = false;
                                }
                                else if (!request.GetHeader("Origin").Equals("http://127.0.0.1"))
                                {
                                    valid = false;
                                }
                                else if (!request.GetHeader("Connection").Equals("keep-alive"))
                                {
                                    valid = false;
                                }
                                else if (!request.GetHeader("Referer").Equals("http://127.0.0.1/"))
                                {
                                    valid = false;
                                }
                                else if (!request.GetHeader("Sec-Fetch-Dest").Equals("empty"))
                                {
                                    valid = false;
                                }
                                else if (!request.GetHeader("Sec-Fetch-Mode").Equals("cors"))
                                {
                                    valid = false;
                                }
                                else if (!request.GetHeader("Sec-Fetch-Site").Equals("same-origin"))
                                {
                                    valid = false;
                                }
                            }
                            else if (browser.Equals(Browser.Chrome) || browser.Equals(Browser.Opera))
                            {
                                if (!request.GetHeader("Host").Equals("127.0.0.1"))
                                {
                                    valid = false;
                                }
                                else if (!request.GetHeader("Connection").Equals("keep-alive"))
                                {
                                    valid = false;
                                }
                                else if (!request.GetHeader("Content-Length").Equals(request.GetBody().Length.ToString()))
                                {
                                    valid = false;
                                }
                                else if (!request.GetHeader("sec-ch-ua-mobile").Equals("?0"))
                                {
                                    valid = false;
                                }
                                else if (!request.GetHeader("Content-type").Equals("application/json"))
                                {
                                    valid = false;
                                }
                                else if (!request.GetHeader("sec-ch-ua-platform").Equals("\"Windows\""))
                                {
                                    valid = false;
                                }
                                else if (!request.GetHeader("Accept").Equals("*/*"))
                                {
                                    valid = false;
                                }
                                else if (!request.GetHeader("Origin").Equals("http://127.0.0.1"))
                                {
                                    valid = false;
                                }
                                else if (!request.GetHeader("Sec-Fetch-Site").Equals("same-origin"))
                                {
                                    valid = false;
                                }
                                else if (!request.GetHeader("Sec-Fetch-Mode").Equals("cors"))
                                {
                                    valid = false;
                                }
                                else if (!request.GetHeader("Sec-Fetch-Dest").Equals("empty"))
                                {
                                    valid = false;
                                }
                                else if (!request.GetHeader("Referer").Equals("http://127.0.0.1/"))
                                {
                                    valid = false;
                                }
                                else if (!request.GetHeader("Accept-Encoding").Equals("gzip, deflate, br"))
                                {
                                    valid = false;
                                }
                                else if (!request.GetHeader("Accept-Language").Equals("it-IT,it;q=0.9,en-US;q=0.8,en;q=0.7"))
                                {
                                    valid = false;
                                }
                            }
                            else if (browser.Equals(Browser.Brave))
                            {
                                if (!request.GetHeader("Host").Equals("127.0.0.1"))
                                {
                                    valid = false;
                                }
                                else if (!request.GetHeader("Connection").Equals("keep-alive"))
                                {
                                    valid = false;
                                }
                                else if (!request.GetHeader("Content-type").Equals("application/json"))
                                {
                                    valid = false;
                                }
                                else if (!request.GetHeader("Accept").Equals("*/*"))
                                {
                                    valid = false;
                                }
                                else if (!request.GetHeader("Sec-GPC").Equals("1"))
                                {
                                    valid = false;
                                }
                                else if (!request.GetHeader("Sec-Fetch-Site").Equals("same-origin"))
                                {
                                    valid = false;
                                }
                                else if (!request.GetHeader("Sec-Fetch-Mode").Equals("cors"))
                                {
                                    valid = false;
                                }
                                else if (!request.GetHeader("Sec-Fetch-Dest").Equals("empty"))
                                {
                                    valid = false;
                                }
                                else if (!request.GetHeader("Referer").Equals("http://127.0.0.1/"))
                                {
                                    valid = false;
                                }
                                else if (!request.GetHeader("Accept-Encoding").Equals("gzip, deflate, br"))
                                {
                                    valid = false;
                                }
                                else if (!request.GetHeader("Accept-Language").Equals("it-IT,it;q=0.9,en-US;q=0.8,en;q=0.7"))
                                {
                                    valid = false;
                                }
                            }

                            try
                            {
                                string sec_ch_ua = "";

                                try
                                {
                                    sec_ch_ua = request.GetHeader("sec-ch-ua");
                                }
                                catch
                                {

                                }

                                string otherBrowser = SecurityUtils.CheckAndGetBrowser(request.GetHeader("X-BRV"), request.GetHeader("X-DID"), request.GetHeader("User-Agent"), sec_ch_ua, request.GetHeader("X-VLD"), request.GetHeader("X-IDF"), request.GetHeader("X-PRP"));
                                string theTimestamp = otherBrowser.Split('|')[1], deviceUUID = otherBrowser.Split('|')[2], requestUUID = otherBrowser.Split('|')[3];

                                if (entityBody != "")
                                {
                                    if (request.GetHeader("X-CHK") != CryptoUtils.GetMD5("ERJHEJKRHWKJEHRKJWEHRJKEWHREJRHWKEJRHKWJEHR" + entityBody + "KJEKRJLKEJRLKWEJRLKJWERLKJWELRKJEWLKRJWELKRJWELRKJ" + theTimestamp + "WJWJWHWJKHKWJHWKJHWKJHWKJ"))
                                    {
                                        Utils.InjectResponse(request, response, "{}", 400, "Bad Request");
                                        return;
                                    }
                                }

                                if (requestChecker.IsRequestUUIDAdded(requestUUID))
                                {
                                    valid = false;
                                }
                                else
                                {
                                    requestChecker.AddRequestUUID(requestUUID);

                                    Thread thread = new Thread(() =>
                                    {
                                        Thread.Sleep(7000);
                                        requestChecker.DeleteRequestUUID(requestUUID);
                                    });

                                    thread.Priority = ThreadPriority.Highest;
                                    thread.Start();
                                }

                                otherBrowser = otherBrowser.Split('|')[0];

                                if (entityBody != "" && request.HasBody())
                                {
                                    string toDecode = entityBody.Substring(0, entityBody.Length - 32);
                                    string md5ToCheck = entityBody.Substring(toDecode.Length, entityBody.Length - toDecode.Length);

                                    if (md5ToCheck != CryptoUtils.GetMD5("EKJHRLKERKLHWEKRHKJWEHRKJHWEJKRHWEKJHRKJWEHRKJHWERKJHWEKJHRKJWEHR" + toDecode + "MEJREJERKJERKERJHERJHERHERJEJRHJERKWHRJKWREH" + theTimestamp))
                                    {
                                        valid = false;
                                    }

                                    entityBody = Utils.Base64Decode(toDecode);
                                }

                                if (!(browser.ToString().Equals("Chrome") && otherBrowser.Equals("Opera")))
                                {
                                    if (!browser.ToString().Equals(otherBrowser))
                                    {
                                        valid = false;
                                    }
                                }
                            }
                            catch
                            {
                                valid = false;
                            }

                            if (!SecurityUtils.CheckBody(entityBody))
                            {
                                valid = false;
                            }

                            if (valid)
                            {
                                response.SetBody("Sei umano.");
                            }
                            else
                            {
                                response.SetBody("Non sei umano.");
                                response.SetStatusCode(400);
                                response.SetStatusDescription("Bad Request");
                            }
                        }

                        request.WriteResponse(response);
                    }
                    else
                    {
                        HttpResponse response = new HttpResponse();

                        response.SetStatusCode(404);
                        response.SetStatusDescription("Resource Not Found");
                        response.SetBody("");

                        request.WriteResponse(response);
                    }
                }
                catch
                {

                }
            }
            catch
            {

            }
        }
    }
}