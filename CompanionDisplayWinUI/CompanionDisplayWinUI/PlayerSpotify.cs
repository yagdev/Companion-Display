using DiscordRPC.Message;
using DiscordRPC;
using Microsoft.UI.Xaml;
using SpotifyAPI.Web.Auth;
using SpotifyAPI.Web;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Windows.Media.Control;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography;
using System.Web;

namespace CompanionDisplayWinUI
{
    class PlayerSpotify
    {
        private string Toki, TokiA, TokiO, LyricCache, TokiRefresh, TokiRefresh2, Authy, Authy2, Toki2, TitleSongOffline, AlbumCoverBase2, Lyrics, LyricsSpotify, SongID, SongIDCache, timestamp, timestamp2;
        char lyricsProvider = 'M';
        bool changed = false;
        public TimeSpan ts, ts2;
        private int currenttimestamp = 0, endtimestamp = 0, startshit = 0;
        public const byte VK_MEDIA_PLAY_PAUSE = 179, VK_MEDIA_NEXT = 176, VK_MEDIA_PREV = 177;
        public DiscordRpcClient client;
        private readonly Uri BaseUri = new("http://localhost:5543/callback");
        private readonly PlayerCurrentlyPlayingRequest request2 = new();
        // Media rework effort
        // Start Logic
        public async void Page_Loaded()
        {
            Thread thread = new(startThread);
            thread.Start();
        }
        private async void startThread()
        {
            loadConfigs();
            getMMToken();
            preStart();
        }
        public void preStart()
        {
            try
            {
                loadDiscordRPC();
                refreshToken();
                initialize();
            }
            catch
            {
                Thread thread = new(PerformLyricShit);
                thread.Start();
            }
        }
        // End Of Start Logic
        // Start Music Info
        public async void PerformLyricShit()
        {
            while (Globals.StartedPlayer)
            {
                await ActuallyDoShit();
                Thread.Sleep(1000);
            }
        }
        // Musixmatch Logic
        public static string GenerateHex(int length)
        {
            Random random = new();
            byte[] buffer = new byte[length / 2];
            random.NextBytes(buffer);
            string result = string.Concat(buffer.Select(x => x.ToString("X2")).ToArray());
            if (length % 2 == 0)
                return result;
            return result + random.Next(16).ToString("X");
        }
        private async void musixmatchLyricReadExisting()
        {
            try
            {
                using (StringReader readerlyric = new(Lyrics))
                {
                    string line;
                    int i = 0;
                    while ((line = readerlyric.ReadLine()) != null)
                    {
                        if (line.Contains("text"))
                        {
                            string combined;
                            combined = System.Text.RegularExpressions.Regex.Unescape(line.Remove(line.Length - 2, 2).Remove(0, 8));
                            line = readerlyric.ReadLine();
                            if (line.StartsWith(' '))
                            {
                                combined += line.Remove(line.Length - 1, 1);
                                line = readerlyric.ReadLine();
                            }
                            if (line.EndsWith(','))
                            {
                                line = line.Remove(line.Length - 1, 1).Remove(0, 15);
                            }
                            double s;
                            try
                            {
                                s = TimeSpan.FromSeconds(Double.Parse(line)).TotalMilliseconds;
                            }
                            catch
                            {
                                s = TimeSpan.FromSeconds(Double.Parse(line.Replace(".", ","))).TotalMilliseconds;
                            }
                            if (s <= ts.TotalMilliseconds)
                            {
                                i++;
                                if (combined == "")
                                {
                                    combined = "♪♪";
                                }
                                LyricCache = combined;
                            }
                            else
                            {
                                Globals.currentLyric = i - 1;
                                Globals.SongLyrics = LyricCache;
                            }
                        }
                    }
                }
            }
            catch
            {
                Globals.currentLyric = -1;
            }
            
        }
        private async void musixmatchGetLyrics()
        {
            string toHash0 = "https://apic.musixmatch.com/ws/1.1/track.get?track_isrc=" + Globals.SongID + "&usertoken=" + Globals.MusixMatchToken + "&app_id=android-player-v1.0" + DateTime.Now.ToString("yyyyMMdd");
            using (HttpClient client2 = new(new SocketsHttpHandler() { ConnectTimeout = TimeSpan.FromSeconds(2.0), KeepAlivePingTimeout = TimeSpan.FromSeconds(5.0), EnableMultipleHttp2Connections = false }))
            {
                try
                {
                    string key = "967Pn4)N3&R_GBg5$b('";
                    byte[] keyBytes = Encoding.UTF8.GetBytes(key);
                    byte[] toHashBytes0 = Encoding.UTF8.GetBytes(toHash0);
                    using (HMACSHA1 hmac = new(keyBytes))
                    {
                        byte[] signatureBytes = hmac.ComputeHash(toHashBytes0);
                        string signaturekey = Convert.ToBase64String(signatureBytes).Replace('+', '-').Replace('/', '_').TrimEnd('=');
                        toHash0 = toHash0.Remove(toHash0.Length - 8, 8) + "&signature=" + signaturekey + "&signature_protocol=sha1";
                    }
                    string a = client2.GetStringAsync(toHash0).Result;
                    using (Task<string> responselyrics = client2.GetStringAsync(toHash0))
                    {
                        string idresult = responselyrics.Result;
                        if (idresult.Contains("\"status_code\":401"))
                        {
                            throw new MusixMatchToken(string.Format("Error"));
                        }
                        string songidmxm = JObject.Parse(idresult)["message"]["body"]["track"]["track_id"].ToString();
                        string toHash = "https://apic.musixmatch.com/ws/1.1/macro.subtitles.get?tags=playing&f_subtitle_length_max_deviation=1&subtitle_format=mxm&page_size=1&track_id=" + songidmxm + "&usertoken=" + Globals.MusixMatchToken + "&app_id=android-player-v1.0&country=pt&language_iso_code=1&format=json" + DateTime.Now.ToString("yyyyMMdd");
                        byte[] toHashBytes = Encoding.UTF8.GetBytes(toHash);
                        using (HMACSHA1 hmac = new(keyBytes))
                        {
                            byte[] signatureBytes = hmac.ComputeHash(toHashBytes);
                            string signaturekey = Convert.ToBase64String(signatureBytes).Replace('+', '-').Replace('/', '_').TrimEnd('=');
                            toHash = toHash.Remove(toHash.Length - 8, 8) + "&signature=" + signaturekey + "&signature_protocol=sha1";
                        }
                        using (Task<string> response2 = client2.GetStringAsync(toHash))
                        {
                            string lyricsformat = response2.Result.Replace("track.subtitles.get", "track_subtitles_get").Replace("track.lyrics.get", "track_lyrics_get").Replace("track.snippet.get", "track_snippet_get");
                            responsedecode = null;
                            responsedecode = JObject.Parse(lyricsformat);
                            string totalstring = responsedecode["message"]["body"]["macro_calls"]["track_subtitles_get"]["message"]["body"]["subtitle_list"][0]["subtitle"]["subtitle_body"].ToString().Replace("\",", "\",\n").Replace("[", "").Replace("]", "").Replace("{", "").Replace("}", "").Replace(",\"", ",\n\"");
                            Lyrics = totalstring.ToString();
                            using (StringReader readerlyric = new(totalstring.ToString()))
                            {
                                string line;
                                while ((line = readerlyric.ReadLine()) != null)
                                {
                                    if (line.Contains("text"))
                                    {
                                        string combined;
                                        combined = System.Text.RegularExpressions.Regex.Unescape(line.Remove(line.Length - 2, 2).Remove(0, 8));
                                        line = readerlyric.ReadLine();
                                        if (line.StartsWith(' '))
                                        {
                                            combined += line.Remove(line.Length - 1, 1);
                                            line = readerlyric.ReadLine();
                                        }
                                        if (line.EndsWith(',') && line.Contains("\"time\":\"total\""))
                                        {
                                            line = line.Remove(line.Length - 1, 1).Remove(0, 15);
                                        }
                                        double s = TimeSpan.FromSeconds(Double.Parse(line.Replace(".", ","))).TotalMilliseconds;
                                        if (s <= Globals.currenttimestamp)
                                        {
                                            if (combined == "")
                                            {
                                                combined = "♪♪";
                                            }
                                            LyricCache = combined;
                                        }
                                    }
                                }
                            }
                            SongIDCache = SongID;
                        }
                    }
                    StringReader readerlyric0 = new(Lyrics);
                    int LyricsAmount = readerlyric0.ReadToEnd().Split("text").Length;
                    using (StringReader readerlyric = new(Lyrics))
                    {
                        string line;
                        string[] lyricList = new string[LyricsAmount];
                        int i = 0;
                        while ((line = readerlyric.ReadLine()) != null)
                        {
                            if (line.Contains("text"))
                            {
                                string combined;
                                combined = System.Text.RegularExpressions.Regex.Unescape(line.Remove(line.Length - 2, 2).Remove(0, 8));
                                line = readerlyric.ReadLine();
                                if (line.StartsWith(' '))
                                {
                                    combined += line.Remove(line.Length - 1, 1);
                                    line = readerlyric.ReadLine();
                                }
                                if (line.EndsWith(','))
                                {
                                    line = line.Remove(line.Length - 1, 1).Remove(0, 15);
                                }
                                lyricList[i] = combined;
                                i++;
                            }
                        }
                        Globals.Lyrics = lyricList;
                        Globals.currentLyric = 0;
                    }
                }
                catch (MusixMatchToken)
                {
                    try
                    {
                        string toHash = "https://apic.musixmatch.com/ws/1.1/token.get?adv_id=" + Guid.NewGuid().ToString() + "&referal=utm_source%3Dgoogle-play%26utm_medium%3Dorganic&root=0&sideloaded=0&build_number=2024050901&guid=" + Guid.NewGuid().ToString() + "&lang=en_US&model=manufacturer%2FGoogle%2FPixel%204%20XL" + HttpUtility.UrlEncode(Guid.NewGuid().ToString()) + "%2FAP1A.240505.005&timestamp=" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ") + "&app_id=android-player-v1.0&format=json" + DateTime.Now.ToString("yyyyMMdd");
                        string key = "967Pn4)N3&R_GBg5$b('";
                        byte[] keyBytes = Encoding.UTF8.GetBytes(key);
                        byte[] toHashBytes = Encoding.UTF8.GetBytes(toHash);
                        string link;
                        using (HMACSHA1 hmac = new(keyBytes))
                        {
                            byte[] signatureBytes = hmac.ComputeHash(toHashBytes);
                            string signaturekey = Convert.ToBase64String(signatureBytes).Replace('+', '-').Replace('/', '_').TrimEnd('=');
                            link = toHash.Remove(toHash.Length - 8, 8) + "&signature=" + signaturekey + "&signature_protocol=sha1";
                            using (Task<string> responselyrics = client2.GetStringAsync(link))
                            {
                                string idresult = responselyrics.Result;
                                Globals.MusixMatchToken = (string)JObject.Parse(idresult)["message"]["body"]["user_token"];
                            }
                        }
                        SongIDCache = SongID;
                    }
                    catch { } 
                }
                catch (Exception ex2)
                {
                    Globals.Lyrics = null;
                    if (ex2.Message.Contains("401"))
                    {
                        Thread thread4 = new(refreshToken);
                        thread4.Start();
                        ex2.Data.Clear();
                    }
                    Exception nn2 = ex2;
                    nn2.Data.Clear();
                    LyricCache = "";
                    Lyrics = "";
                    SongIDCache = SongID;
                }
            }
        }
        // Musixmatch Logic End
        private async Task ActuallyDoShit()
        {
            System.Runtime.GCSettings.LargeObjectHeapCompactionMode = System.Runtime.GCLargeObjectHeapCompactionMode.CompactOnce;
            bool isSpotify = true;
            clientSpotifyLyrics();
            if (!Globals.IsSpotify)
            {
                isSpotify = false;
                if (!changed)
                {
                    musixmatchLyricReadExisting();
                }
                else
                {
                    changed = false;
                    musixmatchGetLyrics();
                }
            }
            pushPresenceDiscord(presenceBuilder(isSpotify));
        }
        // End Music Info
        // Spotify Token Logic
        public async void initialize()
        {
            try
            {
                if (Globals.RefreshToken != "")
                {
                    TokiRefresh = Globals.RefreshToken;
                    TokiO = (await new OAuthClient().RequestToken(new AuthorizationCodeRefreshRequest(Globals._clientId, Globals._secretId, TokiRefresh))).AccessToken;
                    Toki = TokiO;
                    Thread thread2 = new(initializeSecondToken);
                    thread2.Start();
                }
                else
                {
                    SpotifyClientConfig config = SpotifyClientConfig.CreateDefault();
                    EmbedIOAuthServer server = new(new Uri("http://localhost:5543/callback"), 5543);
                    server.AuthorizationCodeReceived += async delegate (object sender, AuthorizationCodeResponse response)
                    {
                        AuthorizationCodeTokenResponse tokenResponse = await new OAuthClient(config).RequestToken(new AuthorizationCodeTokenRequest(Globals._clientId, Globals._secretId, response.Code, BaseUri));
                        TokiO = tokenResponse.AccessToken;
                        TokiRefresh = tokenResponse.RefreshToken;
                        Globals.RefreshToken = tokenResponse.RefreshToken;
                        File.WriteAllText(Globals.RefreshTokenPath, tokenResponse.RefreshToken);
                        await server.Stop();
                        if (Globals._clientId2 != "" && Globals._secretId2 != "")
                        {
                            Thread thread5 = new(initializeSecondToken);
                            thread5.Start();
                        }
                        Thread thread4 = new(RefreshAPI);
                        thread4.Start();
                    };
                    await server.Start();
                    LoginRequest loginRequest = new(server.BaseUri, Globals._clientId, LoginRequest.ResponseType.Code)
                    {
                        Scope = ["user-read-currently-playing", "user-read-playback-state", "user-read-recently-played"]
                    };
                    BrowserUtil.Open(loginRequest.ToUri());
                }
                Toki = TokiO;
                initSessionManager();
                Thread thread1 = new(PerformLyricShit);
                thread1.Start();
            }
            catch
            {
                initSessionManager();
                Thread thread1 = new(PerformLyricShit);
                thread1.Start();
                Globals.SongLyrics = "Config error";
            }
        }
        public async void initializeSecondToken()
        {
            try
            {
                if (Globals.RefreshToken2 != "")
                {
                    TokiRefresh2 = Globals.RefreshToken2;
                    TokiA = (await new OAuthClient().RequestToken(new AuthorizationCodeRefreshRequest(Globals._clientId2, Globals._secretId2, TokiRefresh2))).AccessToken;
                    Thread thread3 = new(RefreshAPI2);
                    thread3.Start();
                }
                else
                {
                    SpotifyClientConfig config = SpotifyClientConfig.CreateDefault();
                    EmbedIOAuthServer server = new(new Uri("http://localhost:5543/callback"), 5543);
                    server.AuthorizationCodeReceived += async delegate (object sender, AuthorizationCodeResponse response)
                    {
                        AuthorizationCodeTokenResponse tokenResponse = await new OAuthClient(config).RequestToken(new AuthorizationCodeTokenRequest(Globals._clientId2, Globals._secretId2, response.Code, BaseUri));
                        await server.Stop();
                        TokiA = tokenResponse.AccessToken;
                        TokiRefresh2 = tokenResponse.RefreshToken;
                        Globals.RefreshToken2 = tokenResponse.RefreshToken;
                        File.WriteAllText(Globals.RefreshToken2Path, tokenResponse.RefreshToken);
                        Thread thread5 = new(RefreshAPI2);
                        thread5.Start();
                    };
                    await server.Start();
                    LoginRequest loginRequest = new(server.BaseUri, Globals._clientId2, LoginRequest.ResponseType.Code)
                    {
                        Scope = ["user-read-currently-playing", "user-read-playback-state", "user-read-recently-played"]
                    };
                    BrowserUtil.Open(loginRequest.ToUri());
                }
            }
            catch
            {
                if (Globals._clientId2 != "" && Globals._secretId2 != "")
                {
                    Thread.Sleep(30000);
                    initializeSecondToken();
                }
            }
        }
        public void refreshToken()
        {
            try
            {
                using (HttpClient client2 = new(new SocketsHttpHandler
                {
                    ConnectTimeout = TimeSpan.FromSeconds(5.0),
                    KeepAlivePingTimeout = TimeSpan.FromSeconds(5.0),
                    EnableMultipleHttp2Connections = false
                }))
                {
                    string url = "https://open.spotify.com/get_access_token?reason=transport&productType=web_player";
                    client2.DefaultRequestHeaders.Clear();
                    client2.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/101.0.0.0 Safari/537.36");
                    client2.DefaultRequestHeaders.Add("App-platform", "WebPlayer");
                    client2.DefaultRequestHeaders.Add("cookie", "sp_dc=" + Globals.SP_DC);
                    using (Task<string> response = client2.GetStringAsync(url))
                    {
                        Authy = response.Result.ToString();
                    }
                    Authy = Authy.Replace("{", "{\n");
                    Authy = Authy.Replace("false", "\nfalse\n");
                    Authy = Authy.Replace("\",", "\",\n");
                    Authy = Authy.Remove(0, 50);
                    using (StringReader reader2 = new(Authy))
                    {
                        Authy2 = reader2.ReadLine();
                        Authy2 = Authy2.Remove(0, 14);
                        Authy2 = Authy2.Remove(Authy2.Length - 2, 2);
                        Toki2 = Authy2;
                        reader2.Close();
                    }
                }
            }
            catch
            {
            }
        }
        // Spotify Token Logic End
        // Spotify Check
        private void getLyricsSpotify()
        {
            using (HttpClient client2 = new(new SocketsHttpHandler() { ConnectTimeout = TimeSpan.FromSeconds(2.0), KeepAlivePingTimeout = TimeSpan.FromSeconds(5.0), EnableMultipleHttp2Connections = false }))
            {
                var url2 = "https://spclient.wg.spotify.com/color-lyrics/v2/track/" + SongID;
                client2.DefaultRequestHeaders.Clear();
                client2.DefaultRequestHeaders.Add("Authorization", "Bearer " + Toki2);
                client2.DefaultRequestHeaders.Add("accept", "application/json");
                client2.DefaultRequestHeaders.Add("app-platform", "Win32");
                var response2 = client2.GetStringAsync(url2);
                LyricsSpotify = response2.Result.ToString();
                JObject lyricsSpotify = JObject.Parse(LyricsSpotify);
                string[] lyricCollection = new string[lyricsSpotify["lyrics"]["lines"].Children().Count()];
                for(int i = 0;  i < lyricCollection.Length; i++)
                {
                    lyricCollection[i] = lyricsSpotify["lyrics"]["lines"][i]["words"].ToString();
                    Globals.Lyrics = lyricCollection;
                    Globals.currentLyric = 0;
                }
            }
        }
        private void parseLyricsSpotify()
        {
            try
            {
                int i = 0;
                bool found = false;
                JObject lyricsSpotify = JObject.Parse(LyricsSpotify);
                string str = lyricsSpotify["lyrics"]["lines"][i]["startTimeMs"].ToString();
                while (!found)
                {
                    if (int.Parse(lyricsSpotify["lyrics"]["lines"][i]["startTimeMs"].ToString()) >= int.Parse(timestamp))
                    {
                        Globals.SongLyrics = lyricsSpotify["lyrics"]["lines"][i - 1]["words"].ToString().Replace("♪", "♪♪");
                        Globals.currentLyric = i - 1;
                        found = true;
                    }
                    i++;
                }
            }
            catch 
            {
                Globals.currentLyric = -1;
            }
        }
        private void clientSpotifyLyrics()
        {
            using (HttpClient client0 = new(new SocketsHttpHandler() { ConnectTimeout = TimeSpan.FromSeconds(2.0), KeepAlivePingTimeout = TimeSpan.FromSeconds(5.0), EnableMultipleHttp2Connections = false }))
            {
                bool cache = Globals.IsSpotify;
                try
                {
                    string url = "https://api.spotify.com/v1/me/player/currently-playing";
                    client0.DefaultRequestHeaders.Add("Authorization", "Bearer " + Toki);
                    using (Task<string> response = client0.GetStringAsync(url))
                    {
                        if ((string)JObject.Parse(response.Result)["is_playing"] == "True")
                        {
                            Globals.IsSpotify = true;
                            responsedecode = JObject.Parse(response.Result.ToString());
                            SongID = responsedecode["item"]["id"].ToString();
                            Globals.SongName = responsedecode["item"]["name"].ToString();
                            Globals.SongEnd = TimeSpan.FromMilliseconds(double.Parse(responsedecode["item"]["duration_ms"].ToString())).ToString(@"m\:ss");
                            timestamp = responsedecode["progress_ms"].ToString();
                            timestamp2 = responsedecode["item"]["duration_ms"].ToString();
                            Globals.SongTime = TimeSpan.FromMilliseconds(double.Parse(responsedecode["progress_ms"].ToString())).ToString(@"m\:ss");
                            Globals.SongID = responsedecode["item"]["external_ids"]["isrc"].ToString();
                            Globals.AlbumName = responsedecode["item"]["album"]["name"].ToString();
                            Globals.SongBackground = responsedecode["item"]["album"]["images"][0]["url"].ToString();
                            AlbumCoverBase2 = responsedecode["item"]["album"]["images"][0]["url"].ToString();
                            Globals.SongDetails = responsedecode["item"]["artists"][0]["name"].ToString() + " · " + responsedecode["item"]["album"]["release_date"].ToString();
                            Globals.SongProgress = (double.Parse(timestamp) / double.Parse(timestamp2)) * 100;
                            currenttimestamp = int.Parse(timestamp);
                            Globals.currenttimestamp = int.Parse(timestamp);
                            ts = TimeSpan.FromMilliseconds(currenttimestamp);
                            endtimestamp = int.Parse(timestamp2);
                            ts2 = TimeSpan.FromMilliseconds(endtimestamp);
                            LyricCache = "";
                            if (SongID != SongIDCache)
                            {
                                SongIDCache = SongID;
                                Globals.SongLyrics = "";
                                LyricCache = "";
                                LyricsSpotify = "";
                                try
                                {
                                    getLyricsSpotify();
                                    lyricsProvider = 'S';
                                }
                                catch(Exception ex)
                                {
                                    if (ex.InnerException != null && ex.InnerException.Message.Contains("401"))
                                    {
                                        refreshToken();
                                    }
                                    requestSongID();
                                    musixmatchGetLyrics();
                                    lyricsProvider = 'M';
                                }
                                SongIDCache = SongID;
                            }
                            if (lyricsProvider == 'S')
                            {
                                parseLyricsSpotify();
                            }
                            else
                            {
                                musixmatchLyricReadExisting();
                            }
                        }
                        else
                        {
                            Globals.IsSpotify = false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Globals.IsSpotify = false;
                    if (ex.InnerException != null && ex.InnerException.Message.Contains("401"))
                    {
                        RefreshAPI();
                        RefreshAPI2();
                    }
                    if (Toki == TokiO && TokiA != null)
                    {
                        Toki = TokiA;
                    }
                    else
                    {
                        Toki = TokiO;
                    }
                }
                if(cache == true && Globals.IsSpotify == false)
                {
                    SongIDCache = "";
                    Globals.SongLyrics = "";
                    LyricCache = "";
                    updateInfo(Globals.currentSession, null);
                }
            }
        }
        // Spotify Check End
        // Spotify DB
        private async void requestSongID()
        {
            try
            {
                if (Toki != null)
                {
                    using (HttpClient client2 = new(new SocketsHttpHandler() { ConnectTimeout = TimeSpan.FromSeconds(2.0), KeepAlivePingTimeout = TimeSpan.FromSeconds(5.0), EnableMultipleHttp2Connections = false }))
                    {
                        string query = Globals.SongName + " " + Globals.SongDetails.Split(" —")[0] + Globals.AlbumName;
                        string url = "https://api.spotify.com/v1/search?q=" + HttpUtility.UrlEncode((query).ToString().Replace("—", "")).Replace("!", "%21").ToString().ToLower() + "&type=track&market=US&include_external=audio";
                        client2.DefaultRequestHeaders.Clear();
                        client2.DefaultRequestHeaders.Add("Authorization", "Bearer " + Toki);
                        using (Task<string> response = client2.GetStringAsync(url))
                        {
                            try
                            {
                                int i = 0;
                                string idresult = response.Result;
                                JToken array = JObject.Parse(response.Result)["tracks"]["items"];
                                int count = array.Count();
                                int test = int.Parse(array[i]["duration_ms"].ToString());
                                int testa = int.Parse(TimeSpan.ParseExact(Globals.SongEnd, @"m\:ss", null).TotalMilliseconds.ToString());
                                while (Math.Abs((int.Parse(array[i]["duration_ms"].ToString()) - int.Parse(TimeSpan.ParseExact(Globals.SongEnd, @"m\:ss", null).TotalMilliseconds.ToString()))) > 2000)
                                {
                                    i++;
                                }
                                Globals.SongID = (string)array[i]["external_ids"]["isrc"];
                                Globals.AlbumName = (string)array[i]["album"]["name"];
                                AlbumCoverBase2 = (string)array[i]["album"]["images"][0]["url"];
                                Globals.SongBackground = (string)array[i]["album"]["images"][0]["url"];
                                Globals.SongDetails = (string)array[i]["artists"][0]["name"] + " · " + (string)array[i]["album"]["release_date"];
                            }
                            catch (Exception ex)
                            {
                                if (ex.InnerException.Message.Contains("401"))
                                {
                                    RefreshAPI();
                                    RefreshAPI2();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ee)
            {

            }
        }
        // Spotify DB
        private async void loadDiscordRPC()
        {
            try
            {
                client = new DiscordRpcClient(Globals.DiscordID);
                client.OnReady += delegate (object sender, ReadyMessage e)
                {
                    Console.WriteLine("Received Ready from user {0}", e.User.Username);
                };
                client.OnPresenceUpdate += delegate (object sender, PresenceMessage e)
                {
                    Console.WriteLine("Received Update! {0}", e.Presence);
                };
                startshit = 1;
                client.Initialize();
            }
            catch
            {
            }
        }
        private async void loadConfigs()
        {
            if (File.Exists(Globals.MediaConfigFile))
            {
                string config = File.ReadAllText(Globals.MediaConfigFile);
                using StringReader readerconfig = new(config);
                {
                    Globals.SP_DC = readerconfig.ReadLine();
                    Globals._clientId = readerconfig.ReadLine();
                    Globals._secretId = readerconfig.ReadLine();
                    Globals._clientId2 = readerconfig.ReadLine();
                    Globals._secretId2 = readerconfig.ReadLine();
                    Globals.DiscordID = readerconfig.ReadLine();
                }
            }
            if (File.Exists(Globals.RefreshTokenPath))
            {
                Globals.RefreshToken = File.ReadAllText(Globals.RefreshTokenPath);
            }
            if (File.Exists(Globals.RefreshToken2Path))
            {
                Globals.RefreshToken2 = File.ReadAllText(Globals.RefreshToken2Path);
            }
        }
        private async void getMMToken()
        {
            try
            {
                using (HttpClient client2 = new(new SocketsHttpHandler
                {
                    ConnectTimeout = TimeSpan.FromSeconds(5.0),
                    KeepAlivePingTimeout = TimeSpan.FromSeconds(5.0),
                    EnableMultipleHttp2Connections = false
                }))
                {
                    string toHash = "https://apic.musixmatch.com/ws/1.1/token.get?adv_id=" + Guid.NewGuid().ToString() + "&referal=utm_source%3Dgoogle-play%26utm_medium%3Dorganic&root=0&sideloaded=0&build_number=2024050901&guid=" + Guid.NewGuid().ToString() + "&lang=en_US&model=manufacturer%2FGoogle%2FPixel%204%20XL%2FAP1A.240505.005&timestamp=" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ") + "&app_id=android-player-v1.0&format=json" + DateTime.Now.ToString("yyyyMMdd");
                    string key = "967Pn4)N3&R_GBg5$b('";
                    byte[] keyBytes = Encoding.UTF8.GetBytes(key);
                    byte[] toHashBytes = Encoding.UTF8.GetBytes(toHash);
                    string link;
                    using HMACSHA1 hmac = new(keyBytes);
                    byte[] signatureBytes = hmac.ComputeHash(toHashBytes);
                    string signaturekey = Convert.ToBase64String(signatureBytes).Replace('+', '-').Replace('/', '_').TrimEnd('=');
                    link = toHash.Remove(toHash.Length - 8, 8) + "&signature=" + signaturekey + "&signature_protocol=sha1";
                    using (Task<string> responselyrics = client2.GetStringAsync(link))
                    {
                        string idresult = responselyrics.Result;
                        Globals.MusixMatchToken = (string)JObject.Parse(idresult)["message"]["body"]["user_token"];
                    }
                }
            }
            catch
            {
                Lyrics = "";
            }
        }
        // End Start Logic
        // Discord Presence Interactions
        private RichPresence presenceBuilder(bool isSpotify)
        {
            DateTime dt = DateTime.Now.ToUniversalTime().Add(-ts);
            DateTime dt2 = DateTime.Now.ToUniversalTime().Add(-ts + ts2);
            string details = Globals.SongName + " · " + Globals.SongDetails;
            if (details.Length > 128)
            {
                details = details.Remove(125, details.Length - 125) + "...";
            }
            RichPresence presence = new()
            {
                Details = Globals.SongName,
                State = Globals.SongDetails,
                Timestamps = new Timestamps
                {
                    Start = dt,
                    End = dt2
                },
                Assets = new Assets
                {
                    LargeImageKey = AlbumCoverBase2,
                    LargeImageText = Globals.AlbumName,
                    SmallImageKey = "mini_logo"
                },
            };
            if(Globals.SongLyrics != null)
            {
                if (details.Length >= 128)
                {
                    details = details.Remove(124, details.Length - 124) + "...";
                }
                if (Globals.SongName != null && Globals.SongName.Length >= 128)
                {
                    Globals.SongName = Globals.SongName.Remove(124, Globals.SongName.Length - 124) + "...";
                }
                try
                {
                    presence.Details = details + "";
                }
                catch
                {
                    presence.Details = details.Remove(50, details.Length - 50) + "...";
                }
                try
                {
                    presence.State = Globals.SongLyrics;
                }
                catch
                {
                    if(details != null && details.Length >= 50)
                    {
                        presence.State = Globals.SongLyrics.Remove(50, details.Length - 50) + "...";
                    }
                    else
                    {
                        presence.State = "";
                    }
                }
            }
            if (isSpotify)
            {
                
                presence.Buttons =
                [
                    new() {
                        Label = "View on Spotify",
                        Url = "https://open.spotify.com/track/" + SongID
                    }
                ];
            }
            return presence;
        }
        private void pushPresenceDiscord(RichPresence presence)
        {
            if ((client != null && Globals.playbackInfo != null && Globals.playbackInfo.PlaybackStatus == GlobalSystemMediaTransportControlsSessionPlaybackStatus.Playing) || Globals.IsSpotify)
            {
                client.SetPresence(presence);
            }
            else if(client != null)
            {
                client.ClearPresence();
            }
        }
        // Discord Presence Interactions End
        // Offline Player Interactions
        private async void initSessionManager()
        {
            Globals.sessionManager = await GlobalSystemMediaTransportControlsSessionManager.RequestAsync();
            Globals.sessionManager.SessionsChanged += updateSessions;
            updateSessions(Globals.sessionManager, null);
        }
        private async void updateSessions(GlobalSystemMediaTransportControlsSessionManager sender, SessionsChangedEventArgs args)
        {
            try
            {
                Globals.currentSession = sender.GetCurrentSession();
                Globals.currentSession.MediaPropertiesChanged += updateInfo;
                Globals.currentSession.TimelinePropertiesChanged += updateTiming;
                Globals.playbackInfo = Globals.currentSession.GetPlaybackInfo();
                updateInfo(Globals.currentSession, null);
                updateTiming(Globals.currentSession, null);
            }
            catch { }
        }
        private void updateTiming(GlobalSystemMediaTransportControlsSession sender, TimelinePropertiesChangedEventArgs args)
        {
            if (!Globals.IsSpotify)
            {
                try
                {
                    Globals.timelineInfo = sender.GetTimelineProperties();
                    TimeSpan timeSpan = TimeSpan.FromMilliseconds(Globals.timelineInfo.Position.TotalMilliseconds);
                    ts = timeSpan;
                    TimeSpan timeSpan2 = TimeSpan.FromMilliseconds(Globals.timelineInfo.EndTime.TotalMilliseconds);
                    ts2 = timeSpan2;
                    Globals.SongTime = timeSpan.ToString(@"m\:ss");
                    Globals.SongEnd = timeSpan2.ToString(@"m\:ss");
                    Globals.SongProgress = Globals.timelineInfo.Position.TotalMilliseconds / Globals.timelineInfo.EndTime.TotalMilliseconds * 100.0;
                }
                catch { }
            }
        }
        private async void updateInfo(GlobalSystemMediaTransportControlsSession sender, MediaPropertiesChangedEventArgs args)
        {
            if(sender != null && !Globals.IsSpotify)
            {
                try
                {
                    Globals.songInfo = await sender.TryGetMediaPropertiesAsync();
                    if (TitleSongOffline != Globals.songInfo.Title)
                    {
                        Globals.BackgroundChanged = true;
                        TitleSongOffline = Globals.songInfo.Title;
                    }
                    Globals.SongName = Globals.songInfo.Title;
                    if(Globals.songInfo.Artist != "")
                    {
                        Globals.SongDetails = Globals.songInfo.Artist;
                    }
                    else
                    {
                        Globals.SongDetails = Globals.songInfo.AlbumArtist;
                    }
                    Globals.AlbumName = Globals.songInfo.AlbumTitle;
                    requestSongID();
                    musixmatchGetLyrics();
                    Lyrics = "";
                    Globals.SongLyrics = "";
                }
                catch { }
                changed = true;
                Globals.BackgroundChanged = true;
            }
        }
        // Offline Player Interactions End
        // Media rework effort end
        private JObject responsedecode;
        public static SpotifyClient spotify;
        public class MusixMatchToken(string message) : Exception(message)
        {
        }
        public async void RefreshAPI()
        {
            try
            {
                AuthorizationCodeRefreshResponse newResponse = await new OAuthClient().RequestToken(new AuthorizationCodeRefreshRequest(Globals._clientId, Globals._secretId, TokiRefresh));
                TokiO = newResponse.AccessToken;
            }
            catch
            {
                Thread.Sleep(30000);
                Thread thread2 = new(RefreshAPI);
                thread2.Start();
            }
        }
        public async void RefreshAPI2()
        {
            try
            {
                AuthorizationCodeRefreshResponse newResponse = await new OAuthClient().RequestToken(new AuthorizationCodeRefreshRequest(Globals._clientId2, Globals._secretId2, TokiRefresh2));
                TokiA = newResponse.AccessToken;
            }
            catch
            {
                Thread.Sleep(30000);
                Thread thread2 = new(RefreshAPI2);
                thread2.Start();
            }
        }
    }
}
