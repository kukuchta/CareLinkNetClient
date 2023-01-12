using System.Globalization;
using System.Net;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Web;
using CareLinkNetClient.Model;
using Newtonsoft.Json;

namespace CareLinkNetClient
{
    public class CareLinkClient
    {

        protected static readonly string CarelinkConnectServerEu = "carelink.minimed.eu";
        protected static readonly string CarelinkConnectServerUs = "carelink.minimed.com";
        protected static readonly string CarelinkLanguageEn = "en";
        protected static readonly string CarelinkLocaleEn = "en";
        protected static readonly string CarelinkAuthTokenCookieName = "auth_tmp_token";
        protected static readonly string CarelinkTokenValidtoCookieName = "c_token_valid_to";
        protected static readonly int AuthExpireDeadlineMinutes = 1;

        //Authentication data
        protected string CarelinkUsername;
        protected string CarelinkPassword;
        protected string CarelinkCountry;

        //Session info
        protected bool LoggedIn;

        public bool IsLoggedIn()
        {
            return LoggedIn;
        }

        protected User SessionUser;

        public User GetSessionUser()
        {
            return GetClone(SessionUser);
        }

        protected Profile SessionProfile = new();

        public Profile GetSessionProfile()
        {
            return GetClone(SessionProfile);
        }

        protected CountrySettings SessionCountrySettings = new();

        public CountrySettings GetSessionCountrySettings()
        {
            return GetClone(SessionCountrySettings);
        }

        protected MonitorData SessionMonitorData;

        public MonitorData GetSessionMonitorData()
        {
            return GetClone(SessionMonitorData);
        }

        protected T? GetClone<T>(T source)
        {
            if (ReferenceEquals(source, null)) return default;

            // initialize inner objects individually
            // for example in default constructor some list property initialized with some values,
            // but in 'source' these items are cleaned -
            // without ObjectCreationHandling.Replace default constructor values will be added to result
            var deserializeSettings = new JsonSerializerSettings
            { ObjectCreationHandling = ObjectCreationHandling.Replace };

            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(source), deserializeSettings);
        }

        //Communication info
        protected HttpClient httpClient = null;
        protected CookieContainer cookieContainer;
        protected bool loginInProcess = false;
        protected string lastResponseBody;

        protected void SetLastResponseBody(HttpResponseMessage response)
        {
            try
            {
                // this.lastResponseBody = response.body().ToString();
                this.lastResponseBody = response.Content.ReadAsStringAsync().Result;
            }
            catch (Exception ex)
            {
            }
        }

        protected void SetLastResponseBody(String responseBody)
        {
            this.lastResponseBody = responseBody;
        }

        public String getLastResponseBody()
        {
            return lastResponseBody;
        }

        protected int lastResponseCode;

        public int getLastResponseCode()
        {
            return lastResponseCode;
        }

        protected bool lastLoginSuccess;

        public bool getLastLoginSuccess()
        {
            return lastLoginSuccess;
        }

        protected bool lastDataSuccess;

        public bool getLastDataSuccess()
        {
            return lastDataSuccess;
        }

        protected String lastErrorMessage;

        public String getLastErrorMessage()
        {
            return lastErrorMessage;
        }

        protected String lastStackTraceString;

        public String getLastStackTraceString()
        {
            return lastStackTraceString;
        }

        protected enum RequestType
        {
            HtmlGet,
            HtmlPost,
            Json
        }


        public CareLinkClient(String carelinkUsername, String carelinkPassword, String carelinkCountry)
        {

            this.CarelinkUsername = carelinkUsername;
            this.CarelinkPassword = carelinkPassword;
            this.CarelinkCountry = carelinkCountry;

            // Create main http client with CookieJar
            cookieContainer = new CookieContainer();
            HttpClientHandler handler = new HttpClientHandler();
            //handler.PooledConnectionIdleTimeout = TimeSpan.FromMinutes(10);
            handler.MaxConnectionsPerServer = 5;
            handler.AllowAutoRedirect = true;
            handler.UseCookies = true;
            handler.CookieContainer = cookieContainer;
            this.httpClient = new HttpClient(handler);

            //HttpResponseMessage response = client.GetAsync("http://google.com").Result;

            //Uri uri = new Uri("http://google.com");
            //IEnumerable<Cookie> responseCookies = cookies.GetCookies(uri).Cast<Cookie>();
            //foreach (Cookie cookie in responseCookies)
            //    Console.WriteLine(cookie.Name + ": " + cookie.Value);

            //Console.ReadLine();
        }

        /*
         *  WRAPPER DATA RETRIEVAL METHODS
         */
        public RecentData GetRecentData()
        {

            // Force login to get basic info
            if (GetAuthorizationToken() != null)
            {
                if (CountryUtils.isUS(CarelinkCountry) || SessionMonitorData.IsBle())
                    return this.GetConnectDisplayMessage(this.SessionProfile.Username, this.SessionUser.GetUserRole(),
                        SessionCountrySettings.BlePereodicDataEndpoint);
                else
                    return this.GetLast24Hours();
            }
            else
            {
                return null;
            }


        }

        // Get server URL
        protected string CareLinkServer()
        {
            return this.CarelinkCountry.Equals("us") ? CarelinkConnectServerUs : CarelinkConnectServerEu;
        }


        // Authentication methods
        public bool login()
        {
            if (!this.LoggedIn)
            {
                this.ExecuteLoginProcedure();
            }

            return this.LoggedIn;
        }

        protected bool ExecuteLoginProcedure()
        {

            HttpResponseMessage loginSessionResponse = null;
            HttpResponseMessage doLoginResponse = null;
            HttpResponseMessage consentResponse = null;

            lastLoginSuccess = false;
            loginInProcess = true;
            lastErrorMessage = null;

            try
            {
                // Clear cookies
                cookieContainer.GetAllCookies().ToList().ForEach(c => c.Expired = true);

                // Clear basic infos
                this.SessionUser = null;
                this.SessionProfile = null;
                this.SessionCountrySettings = null;
                this.SessionMonitorData = null;

                // Open login (get SessionId and SessionData)
                loginSessionResponse = this.GetLoginSession();
                this.lastResponseCode = (int)loginSessionResponse.StatusCode;

                // Login
                doLoginResponse = this.DoLogin(loginSessionResponse);
                this.lastResponseCode = (int)doLoginResponse.StatusCode;
                SetLastResponseBody(loginSessionResponse);
                //loginSessionResponse.close();

                // Consent
                consentResponse = this.DoConsent(doLoginResponse);
                SetLastResponseBody(doLoginResponse);
                //doLoginResponse.close();
                this.lastResponseCode = (int)consentResponse.StatusCode;
                SetLastResponseBody(consentResponse);
                //consentResponse.close();

                // Get sessions infos if required
                if (this.SessionUser == null)
                    this.SessionUser = this.GetMyUser();
                if (this.SessionProfile == null)
                    this.SessionProfile = this.GetMyProfile();
                if (this.SessionCountrySettings == null)
                    this.SessionCountrySettings = this.GetCountrySettings(this.CarelinkCountry, CarelinkLanguageEn);
                if (this.SessionMonitorData == null)
                    this.SessionMonitorData = this.GetMonitorData();
                // Set login success if everything was ok:
                if (this.SessionUser != null && this.SessionProfile != null && this.SessionCountrySettings != null &&
                    this.SessionMonitorData != null)
                {
                    lastLoginSuccess = true;
                    RecentData data = GetLast24Hours();
                }

            }
            catch (Exception e)
            {
                lastErrorMessage = e.ToString();
                // lastStackTraceString = Log.getStackTraceString(e);
            }

            loginInProcess = false;
            LoggedIn = lastLoginSuccess;
            return lastLoginSuccess;
        }

        protected HttpResponseMessage GetLoginSession()
        {
            var url = new Uri(
                @$"https://{this.CareLinkServer()}/patient/sso/login?country={this.CarelinkCountry}&lang={CarelinkLanguageEn}");
            var request = new HttpRequestMessage(HttpMethod.Get, url);

            this.AddHttpHeaders(request, RequestType.HtmlGet);
            //request.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,*/*;q=0.8");
            //request.Headers.Add("Accept-Language", "pl,en-US;q=0.7,en;q=0.3");
            //request.Headers.Add("Connection", "keep-alive");
            //request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:108.0) Gecko/20100101 Firefox/108.0");
            return this.httpClient.Send(request);
        }

        protected HttpResponseMessage DoLogin(HttpResponseMessage loginSessionResponse)
        {
            var queryDictionary =
                System.Web.HttpUtility.ParseQueryString(loginSessionResponse.RequestMessage.RequestUri.Query);

            var form = new Dictionary<string, string>
            {
                { "sessionID", queryDictionary["sessionID"] ?? "" },
                { "sessionData", queryDictionary["sessionData"] ?? "" },
                { "locale", CarelinkLocaleEn },
                { "action", "login" },
                { "username", this.CarelinkUsername },
                { "password", this.CarelinkPassword },
                { "actionButton", "Log in" }
            };

            var content = new FormUrlEncodedContent(form);
            var url = new Uri(
                @$"https://mdtlogin.medtronic.com/mmcl/auth/oauth/v2/authorize/login?locale={CarelinkLocaleEn}&country={this.CarelinkCountry}");
            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = content
            };

            this.AddHttpHeaders(request, RequestType.HtmlGet);
            //request.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,*/*;q=0.8");
            //request.Headers.Add("Accept-Language", "pl,en-US;q=0.7,en;q=0.3");
            //request.Headers.Add("Connection", "keep-alive");
            //request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:108.0) Gecko/20100101 Firefox/108.0");
            return this.httpClient.Send(request);
        }

        protected HttpResponseMessage DoConsent(HttpResponseMessage doLoginResponse)
        {
            string doLoginRespBody = doLoginResponse.Content.ReadAsStringAsync().Result;

            Regex consentUrlPattern = new Regex("form action=\\\"(?<consentUrlPattern>.+)\\\" method");
            Regex consentSessionDataPattern =
                new Regex(
                    "<input type=\\\"hidden\\\" name=\\\"sessionData\\\" value=\\\"(?<consentSessionData>.+)\\\">");
            Regex consentSessionIdPattern =
                new Regex("<input type=\\\"hidden\\\" name=\\\"sessionID\\\" value=\\\"(?<consentSessionId>.+)\\\">");

            Match consentUrlMatch = consentUrlPattern.Match(doLoginRespBody);
            Match consentSessionDataMatch = consentSessionDataPattern.Match(doLoginRespBody);
            Match consentSessionIdMatch = consentSessionIdPattern.Match(doLoginRespBody);

            string consentUrl = consentUrlMatch.Groups["consentUrlPattern"].Value;
            string consentSessionData = consentSessionDataMatch.Groups["consentSessionData"].Value;
            string consentSessionId = consentSessionIdMatch.Groups["consentSessionId"].Value;

            var form = new Dictionary<string, string>
            {
                { "action", "consent" },
                { "sessionID", consentSessionId },
                { "sessionData", consentSessionData },
                { "response_type", "code" },
                { "response_mode", "query" }
            };

            var content = new FormUrlEncodedContent(form);
            var url = new Uri(consentUrl);
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Content = content;

            this.AddHttpHeaders(request, RequestType.HtmlPost);
            //request.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,*/*;q=0.8");
            //request.Headers.Add("Accept-Language", "pl,en-US;q=0.7,en;q=0.3");
            //request.Headers.Add("Connection", "keep-alive");
            //request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:108.0) Gecko/20100101 Firefox/108.0");
            return this.httpClient.Send(request);
        }

        protected string? GetAuthorizationToken()
        {

            // New token is needed:
            // a) no token or about to expire => execute authentication
            // b) last response 401

            bool authTokenCookiePresent = cookieContainer.GetAllCookies()
                .Any(cookie => cookie.Name.Equals(CarelinkAuthTokenCookieName));
            bool tokenValidToCookiePresent = cookieContainer.GetAllCookies()
                .Any(cookie => cookie.Name.Equals(CarelinkTokenValidtoCookieName));
            bool isUnauthorized = this.lastResponseCode == 401;

            Cookie? tokenValidToCookie = cookieContainer.GetAllCookies()
                .FirstOrDefault(cookie => cookie.Name.Equals(CarelinkTokenValidtoCookieName));
            DateTime validToTime = DateTime.ParseExact(tokenValidToCookie.Value, "ddd MMM dd HH:mm:ss UTC yyyy", CultureInfo.InvariantCulture);
            bool isAboutToExpire = validToTime <= DateTime.UtcNow.AddMilliseconds(AuthExpireDeadlineMinutes * 60000);

            if (!authTokenCookiePresent
                || !tokenValidToCookiePresent
                || isAboutToExpire
                || isUnauthorized
               )
            {
                //execute new login process | null, if error OR already doing login
                if (this.loginInProcess || !this.ExecuteLoginProcedure())
                    return null;

            }

            // there can be only one
            return "Bearer" + " " + cookieContainer.GetAllCookies()
                .FirstOrDefault(cookie => cookie.Name.Equals(CarelinkAuthTokenCookieName))?.Value;
        }

        ///*
        // * CareLink APIs
        // */

        // My user
        public User? GetMyUser()
        {
            return this.GetData<User>(this.CareLinkServer(), "patient/users/me", null, null);
        }

        // My profile
        public Profile? GetMyProfile()
        {
            return this.GetData<Profile>(this.CareLinkServer(), "patient/users/me/profile", null, null);
        }

        // Monitoring data
        public MonitorData? GetMonitorData()
        {
            return this.GetData<MonitorData>(this.CareLinkServer(), "patient/monitor/data", null, null);
        }

        // Country settings
        public CountrySettings? GetCountrySettings(String country, String language)
        {

            var queryParams = new Dictionary<string, string>
            {
                { "countryCode", country },
                { "language", language }
            };

            return this.GetData<CountrySettings>(this.CareLinkServer(), "patient/countries/settings", queryParams, null);
        }

        // Old last24hours webapp data
        public RecentData? GetLast24Hours()
        {
            //Last24Hour getLast24Hours() {

            var queryParams = new Dictionary<string, string>
            {
                { "cpSerialNumber", "NONE" },
                { "msgType", "last24hours" },
                { "requestTime", DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString()}

            };

            return this.GetData<RecentData>(this.CareLinkServer(), "patient/connect/data", queryParams, null);

        }

        // Periodic data from CareLink Cloud
        public RecentData? GetConnectDisplayMessage(String username, String role, String endpointUrl)
        {

            HttpContent requestBody = null;

            // Build user json for request
            JsonObject userJson = new JsonObject();
            userJson.Add("username", username);
            userJson.Add("role", role);

            requestBody = new StringContent(userJson.ToJsonString(), Encoding.UTF8, "application/json");

            RecentData? recentData = this.GetData<RecentData>(new Uri(endpointUrl), requestBody);
            if (recentData != null)
                CorrectTimeInRecentData(recentData);
            return recentData;

        }


        // Helper methods
        // Response parsing
        //protected String ExtractResponseData(String respBody, String groupRegex, int groupIndex)
        //    {

        //        //String responseData = null;

        //        //Matcher responseDataMatcher = Pattern.compile(groupRegex).matcher(respBody);
        //        //if (responseDataMatcher.find())
        //        //    responseData = responseDataMatcher.group(groupIndex);

        //        return ""; //responseData;

        //    }

        // Http header builder for requests
        protected void AddHttpHeaders(HttpRequestMessage request, RequestType type)
        {

            //Add common browser headers
            request.Headers.Add("Accept-Language", "en;q=0.9, *;q=0.8");
            request.Headers.Add("Connection", "keep-alive");
            request.Headers.Add("sec-ch-ua",
                "\"Google Chrome\";v=\"87\", \" Not;A Brand\";v=\"99\", \"Chromium\";v=\"87\"");
            request.Headers.Add("User-Agent",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.88 Safari/537.36");

            //Set media type based on request type
            switch (type)
            {
                case RequestType.Json:
                    request.Headers.Add("Accept", "application/json, text/plain, */*");
                    request.Content?.Headers.Remove("Content-Type");
                    request.Content?.Headers.Add("Content-Type", "application/json; charset=utf-8");
                    break;
                case RequestType.HtmlGet:
                    request.Headers.Add("Accept",
                        "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
                    break;
                case RequestType.HtmlPost:
                    request.Headers.Add("Accept",
                        "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
                    request.Content?.Headers.Remove("Content-Type");
                    request.Content?.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                    break;
            }

        }

        //Data request for API calls
        protected T? GetData<T>(Uri url, HttpContent? content)
        {

            T? data = default(T);

            this.lastDataSuccess = false;
            this.lastErrorMessage = null;

            // Get auth token
            string? authToken = this.GetAuthorizationToken();

            if (authToken != null)
            {
                HttpRequestMessage request = new HttpRequestMessage
                {
                    RequestUri = url
                };
                request.Headers.Add("Authorization", authToken);

                // Add header
                if (content == null)
                {
                    request.Method = HttpMethod.Get;
                    this.AddHttpHeaders(request, RequestType.Json);
                }
                else
                {
                    request.Method = HttpMethod.Post;
                    request.Content = content;
                    this.AddHttpHeaders(request, RequestType.HtmlPost);
                }

                // Send request
                try
                {
                    var response = this.httpClient.Send(request);
                    this.lastResponseCode = (int)response.StatusCode;
                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = response.Content.ReadAsStringAsync().Result;
                        SetLastResponseBody(responseBody);
                        data = JsonConvert.DeserializeObject<T>(responseBody);
                        this.lastDataSuccess = true;
                    }

                }
                catch (Exception e)
                {
                    lastErrorMessage = e.Message;
                }

            }

            return data;
        }

        protected T? GetData<T>(string host, string path, Dictionary<string, string>? queryParams, HttpContent? content)
        {
            var uriBuilder = new UriBuilder();
            uriBuilder.Scheme = "https";
            uriBuilder.Host = host;
            uriBuilder.Path = path;

            if (queryParams != null)
            {
                var query = HttpUtility.ParseQueryString(uriBuilder.Query);
                foreach (var entry in queryParams)
                {
                    query[entry.Key] = entry.Value;
                }
                uriBuilder.Query = query.ToString();
            }

            return this.GetData<T>(uriBuilder.Uri, content);
        }

        protected void CorrectTimeInRecentData(RecentData recentData)
        {

            if (recentData.sMedicalDeviceTime != null && recentData.LastMedicalDeviceDataUpdateServerTime > 1)
            {

                //Calc time diff between event time and actual local time (timezone is wrong in CareLink data)
                int diffInHour =
                    (int)Math.Round(((recentData.LastMedicalDeviceDataUpdateServerTime -
                                      ((DateTimeOffset)DateTime.SpecifyKind(recentData.sMedicalDeviceTime, DateTimeKind.Utc)).ToUnixTimeMilliseconds()) / 3600000D));

                //Correct times if server <> device time differs in hours
                if (diffInHour != 0 && diffInHour < 26)
                {
                    recentData.MedicalDeviceTimeAsString = recentData.MedicalDeviceTimeAsString.AddHours(diffInHour);
                    recentData.sMedicalDeviceTime = recentData.sMedicalDeviceTime.AddHours(diffInHour);
                    recentData.LastConduitDateTime = recentData.LastConduitDateTime.AddHours(diffInHour);
                    recentData.LastSensorTsAsString = recentData.LastSensorTsAsString.AddHours(diffInHour);
                    recentData.sLastSensorTime = recentData.sLastSensorTime.AddHours(diffInHour);
                    //Sensor
                    if (recentData.Sgs != null)
                    {
                        foreach (SensorGlucose sg in recentData.Sgs)
                        {
                            sg.Datetime = sg.Datetime.AddHours(diffInHour);
                        }
                    }

                    //Markers
                    if (recentData.Markers != null)
                    {
                        foreach (Marker marker in recentData.Markers)
                        {
                            marker.Datetime = marker.Datetime.AddHours(diffInHour);
                        }
                    }

                    //Notifications
                    if (recentData.NotificationHistory != null)
                    {
                        if (recentData.NotificationHistory.ClearedNotifications != null)
                        {
                            foreach (ClearedNotification notification in recentData.NotificationHistory.ClearedNotifications)
                            {
                                notification.Datetime = notification.Datetime.Date.AddHours(diffInHour);
                                notification.TriggeredDateTime = notification.TriggeredDateTime.AddHours(diffInHour);
                            }
                        }

                        if (recentData.NotificationHistory.ActiveNotifications != null)
                        {
                            foreach (ActiveNotification notification in recentData.NotificationHistory.ActiveNotifications)
                            {
                                notification.Datetime = notification.Datetime.AddHours(diffInHour);
                            }
                        }
                    }
                }
            }

        }

    }
}

public class CountryUtils
{

    public static string[] supportedCountryCodes = {
            "dz",
            "ba",
            "eg",
            "za",
            "ca",
            "cr",
            "mx",
            "ma",
            "pa",
            "pr",
            "us",
            "ar",
            "br",
            "cl",
            "co",
            "ve",
            "hk",
            "in",
            "id",
            "il",
            "jp",
            "kw",
            "lb",
            "my",
            "ph",
            "qa",
            "sa",
            "sg",
            "kr",
            "tw",
            "th",
            "tn",
            "tr",
            "ae",
            "vn",
            "at",
            "be",
            "bg",
            "hr",
            "cz",
            "dk",
            "ee",
            "fi",
            "fr",
            "de",
            "gr",
            "hu",
            "is",
            "ie",
            "it",
            "lv",
            "lt",
            "lu",
            "nl",
            "no",
            "pl",
            "pt",
            "ro",
            "ru",
            "rs",
            "sk",
            "si",
            "es",
            "se",
            "ch",
            "ua",
            "gb",
            "au",
            "nz",
            "bh",
            "om",
            "cn",
            "cy",
            "al",
            "am",
            "az",
            "bs",
            "bb",
            "by",
            "bm",
            "bo",
            "kh",
            "do",
            "ec",
            "sv",
            "ge",
            "gt",
            "hn",
            "ir",
            "iq",
            "jo",
            "xk",
            "ly",
            "mo",
            "mk",
            "mv",
            "mt",
            "mu",
            "yt",
            "md",
            "me",
            "na",
            "nc",
            "ni",
            "ng",
            "pk",
            "py",
            "mf",
            "sd",
            "uy",
            "aw",
            "ky",
            "cw",
            "pe"
    };

    public static bool IsSupportedCountry(String countryCode)
    {
        return supportedCountryCodes.Contains(countryCode);
    }

    public static bool isUS(String countryCode)
    {
        return countryCode.Equals("us");
    }

    public static bool isOutsideUS(String countryCode)
    {
        return !isUS(countryCode);
    }

}


