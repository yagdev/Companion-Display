using DiscordRPC.Message;
using DiscordRPC;
using Microsoft.UI.Xaml;
using SpotifyAPI.Web.Auth;
using SpotifyAPI.Web;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using Windows.Foundation;

namespace CompanionDisplayWinUI
{
    class PlayerSpotify
    {
        private string Toki, TokiA, TokiO, TokiRefresh, TokiRefresh2, Authy, Authy2, Toki2, TitleSongOffline, ReleaseDate, LyricCache, AlbumName, SongTitleReloadedAlgo, AlbumCoverBase2, ArtistBase2, Lyrics, SongID, SongIDCache, SongIDCache2, timestamp, timestamp2, StringDetail, LastLyric;
        string LyricsProvider = "";
        private TimeSpan ts, ts2;
        private int currenttimestamp = 0, endtimestamp = 0, startshit = 0;
        public const byte VK_MEDIA_PLAY_PAUSE = 179, VK_MEDIA_NEXT = 176, VK_MEDIA_PREV = 177;
        public DiscordRpcClient client;
        private readonly Uri BaseUri = new("http://localhost:5543/callback");
        private readonly PlayerCurrentlyPlayingRequest request2 = new();
        public void PreStart()
        {
            if (startshit == 1)
            {
                startshit = 0;
            }
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
                Thread thread2 = new(RefreshToken);
                thread2.Start();
                Thread thread = new(Initialize);
                thread.Start();
            }
            catch
            {
                //File.AppendAllText("ErrorLog.crlh", ex.Message);
                Thread thread = new(PerformLyricShit);
                thread.Start();
            }
        }
        public static string GenerateHex(int length)
        {
            Random random = new ();
            byte[] buffer = new byte[length / 2];
            random.NextBytes(buffer);
            string result = string.Concat(buffer.Select(x => x.ToString("X2")).ToArray());
            if (length % 2 == 0)
                return result;
            return result + random.Next(16).ToString("X");
        }
        public async void Page_Loaded()
        {
            Globals.sessionManager = await GlobalSystemMediaTransportControlsSessionManager.RequestAsync();
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
            try
            {
                using(HttpClient client2 = new(new SocketsHttpHandler
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
            }
            Thread thread5 = new(PreStart);
            thread5.Start();
        }
        public async Task OfflinePlayer()
        {
            try
            {
                if(client.CurrentPresence != null)
                {
                    client.ClearPresence();
                }
            }
            catch
            {
            }
            try
            {
                if (Globals.IsSpotify)
                {
                    Globals.IsSpotify = false;
                    Globals.sessionManager.CurrentSessionChanged -= OnCurrentSessionChanged;
                    Globals.sessionManager.CurrentSessionChanged += OnCurrentSessionChanged;
                }
                else
                {
                    TimeSpan timeSpan = TimeSpan.FromMilliseconds(Globals.sessionManager.GetCurrentSession().GetTimelineProperties().Position.TotalMilliseconds);
                    TimeSpan timeSpan2 = TimeSpan.FromMilliseconds(Globals.sessionManager.GetCurrentSession().GetTimelineProperties().EndTime.TotalMilliseconds);
                    Globals.SongTime = timeSpan.ToString(@"mm\:ss");
                    Globals.SongEnd = timeSpan2.ToString(@"mm\:ss");
                    Globals.SongProgress = Globals.sessionManager.GetCurrentSession().GetTimelineProperties().Position.TotalMilliseconds / Globals.sessionManager.GetCurrentSession().GetTimelineProperties().EndTime.TotalMilliseconds * 100.0;
                }
            }
            catch
            {
            }
        }
        private JObject responsedecode;
        private SpotifyClient spotify;
        private async void OnCurrentSessionChanged(GlobalSystemMediaTransportControlsSessionManager sender, CurrentSessionChangedEventArgs args)
        {
            try
            {
                var currentSession = sender.GetCurrentSession();
                if (currentSession != null)
                {
                    Globals.songInfo = await Globals.sessionManager.GetCurrentSession().TryGetMediaPropertiesAsync();
                    if (TitleSongOffline != Globals.songInfo.Title)
                    {
                        Globals.BackgroundChanged = true;
                        TitleSongOffline = Globals.songInfo.Title;
                    }
                    Globals.SongName = Globals.songInfo.Title;
                    Globals.SongDetails = Globals.songInfo.Artist;
                    Globals.SongLyrics = Globals.songInfo.AlbumTitle;
                }
            }
            catch { }
            
        }
        private async Task ActuallyDoShit()
        {
            System.Runtime.GCSettings.LargeObjectHeapCompactionMode = System.Runtime.GCLargeObjectHeapCompactionMode.CompactOnce;
            if (startshit == 1)
            {
                try
                {
                    LyricCache = "";
                    if (spotify == null)
                    {
                        spotify = new(Toki);
                    }
                    spotify.Player.GetCurrentlyPlaying(request2);
                    try
                    {
                        using (HttpClient client2 = new(new SocketsHttpHandler() { ConnectTimeout = TimeSpan.FromSeconds(2.0), KeepAlivePingTimeout = TimeSpan.FromSeconds(5.0), EnableMultipleHttp2Connections = false }))
                        {
                            string url = "https://api.spotify.com/v1/me/player/currently-playing";
                            client2.DefaultRequestHeaders.Clear();
                            client2.DefaultRequestHeaders.Add("Authorization", "Bearer " + Toki);
                            using (Task<string> response = client2.GetStringAsync(url))
                            {
                                try
                                {
                                    if (responsedecode != null)
                                    {
                                        responsedecode.RemoveAll();
                                    }
                                    if (response.Result.ToString().Contains("https://api.spotify.com/v1/tracks/") && response.Result.ToString().Contains("\"is_playing\" : true"))
                                    {
                                        Globals.IsSpotify = true;
                                        Globals.sessionManager.CurrentSessionChanged -= OnCurrentSessionChanged;
                                        responsedecode = JObject.Parse(response.Result.ToString());
                                        SongID = responsedecode["item"]["id"].ToString();
                                        SongTitleReloadedAlgo = responsedecode["item"]["name"].ToString();
                                        timestamp2 = responsedecode["item"]["duration_ms"].ToString();
                                        timestamp = responsedecode["progress_ms"].ToString();
                                        Globals.SongID = responsedecode["item"]["external_ids"]["isrc"].ToString();
                                        AlbumCoverBase2 = responsedecode["item"]["album"]["images"][0]["url"].ToString();
                                        AlbumName = responsedecode["item"]["album"]["name"].ToString();
                                        ReleaseDate = responsedecode["item"]["album"]["release_date"].ToString();
                                        ArtistBase2 = responsedecode["item"]["artists"][0]["name"].ToString();
                                        currenttimestamp = int.Parse(timestamp);
                                        Globals.currenttimestamp = int.Parse(timestamp);
                                        ts = TimeSpan.FromMilliseconds(currenttimestamp);
                                        endtimestamp = int.Parse(timestamp2);
                                        ts2 = TimeSpan.FromMilliseconds(endtimestamp);
                                        if (SongID == SongIDCache)
                                        {
                                            try
                                            {
                                                if (LyricsProvider == "Spotify")
                                                {
                                                    timestamp = "";
                                                    if (Lyrics.Contains("startTimeMs"))
                                                    {
                                                        using (var reader2 = new StringReader(Lyrics))
                                                        {
                                                        startchicanery:
                                                            string Lyrics2 = reader2.ReadLine();
                                                            if (Lyrics2.Contains("startTimeMs"))
                                                            {
                                                                Lyrics2 = Lyrics2.Remove(0, 15);
                                                                Lyrics2 = Lyrics2.Remove(Lyrics2.Length - 2, 2);
                                                                int lyrictimestamp = int.Parse(Lyrics2);
                                                                Lyrics2 = reader2.ReadLine();
                                                                Lyrics2 = Lyrics2.Remove(0, 9);
                                                                Lyrics2 = Lyrics2.Remove(Lyrics2.Length - 2, 2);
                                                                if (Lyrics2.Contains('♪'))
                                                                {
                                                                    Lyrics2 += "♪";
                                                                }
                                                                if (lyrictimestamp < currenttimestamp)
                                                                {
                                                                    LyricCache = Lyrics2;
                                                                    LyricCache = System.Text.RegularExpressions.Regex.Unescape(LyricCache);
                                                                    Lyrics2 = reader2.ReadLine();
                                                                    goto startchicanery;
                                                                }
                                                            }
                                                            else
                                                            {
                                                                goto startchicanery;
                                                            }
                                                        }
                                                    }
                                                }
                                                if (LyricsProvider == "MXM")
                                                {
                                                    using (StringReader readerlyric = new(Lyrics))
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
                                                                    //File.AppendAllText("ErrorLog.crlh", ex.Message);
                                                                    s = TimeSpan.FromSeconds(Double.Parse(line.Replace(".", ","))).TotalMilliseconds;
                                                                }
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
                                                }
                                            }
                                            catch
                                            {
                                                LyricCache = "";
                                            }
                                        }
                                        else if (!response.Result.ToString().Contains("\"progress_ms\" : 0"))
                                        {
                                            try
                                            {
                                                var url2 = "https://spclient.wg.spotify.com/color-lyrics/v2/track/" + SongID;
                                                client2.DefaultRequestHeaders.Clear();
                                                client2.DefaultRequestHeaders.Add("Authorization", "Bearer " + Toki2);
                                                client2.DefaultRequestHeaders.Add("accept", "application/json");
                                                client2.DefaultRequestHeaders.Add("app-platform", "Win32");
                                                var response2 = client2.GetStringAsync(url2);
                                                Lyrics = response2.Result.ToString();
                                                timestamp = "";
                                                if (Lyrics.Contains("startTimeMs"))
                                                {
                                                    Lyrics = Lyrics.Replace("},", "\n},\n");
                                                    Lyrics = Lyrics.Replace("\",", "\",\n");
                                                    Lyrics = Lyrics.Replace("{", "{\n");
                                                    using (var reader2 = new StringReader(Lyrics))
                                                    {
                                                        string Lyrics2 = reader2.ReadLine();
                                                    startchicanery:
                                                        if (Lyrics2.Contains("startTimeMs"))
                                                        {
                                                            Lyrics2 = Lyrics2.Remove(0, 15);
                                                            Lyrics2 = Lyrics2.Remove(Lyrics2.Length - 2, 2);
                                                            int lyrictimestamp = int.Parse(Lyrics2);
                                                            Lyrics2 = reader2.ReadLine();
                                                            Lyrics2 = Lyrics2.Remove(0, 9);
                                                            Lyrics2 = Lyrics2.Remove(Lyrics2.Length - 2, 2);
                                                            if (Lyrics2.Contains('♪'))
                                                            {
                                                                Lyrics2 += "♪";
                                                            }
                                                            SongIDCache = SongID;
                                                            if (lyrictimestamp < currenttimestamp)
                                                            {
                                                                Lyrics2 = Lyrics2.Replace("\\u0027", "'");
                                                                Lyrics2 = Lyrics2.Replace("\\\"", "\"");
                                                                LyricCache = Lyrics2;
                                                                Lyrics2 = reader2.ReadLine();
                                                                goto startchicanery;
                                                            }
                                                            else
                                                            {
                                                                reader2.Close();
                                                            }
                                                        }
                                                        else
                                                        {
                                                            if (Lyrics2 != "")
                                                            {
                                                                Lyrics2 = reader2.ReadLine();
                                                                goto startchicanery;
                                                            }
                                                        }
                                                        LyricsProvider = "Spotify";
                                                    }
                                                }
                                                else
                                                {
                                                    SongIDCache = SongID;
                                                }
                                            }
                                            catch (Exception nn)
                                            {
                                                if (nn.Message.Contains("401"))
                                                {
                                                    Thread thread4 = new(RefreshToken);
                                                    thread4.Start();
                                                }
                                                try
                                                {
                                                    string toHash0 = "https://apic.musixmatch.com/ws/1.1/track.get?track_isrc=" + Globals.SongID + "&usertoken=" + Globals.MusixMatchToken + "&app_id=android-player-v1.0" + DateTime.Now.ToString("yyyyMMdd");
                                                    string key = "967Pn4)N3&R_GBg5$b('";
                                                    byte[] keyBytes = Encoding.UTF8.GetBytes(key);
                                                    byte[] toHashBytes0 = Encoding.UTF8.GetBytes(toHash0);
                                                    using (HMACSHA1 hmac = new(keyBytes))
                                                    {
                                                        byte[] signatureBytes = hmac.ComputeHash(toHashBytes0);
                                                        string signaturekey = Convert.ToBase64String(signatureBytes).Replace('+', '-').Replace('/', '_').TrimEnd('=');
                                                        toHash0 = toHash0.Remove(toHash0.Length - 8, 8) + "&signature=" + signaturekey + "&signature_protocol=sha1";
                                                    }
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
                                                            LyricsProvider = "MXM";
                                                            SongIDCache = SongID;
                                                        }
                                                    }
                                                }
                                                catch (MusixMatchToken)
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
                                                    LyricsProvider = "Spotify";
                                                    SongIDCache = SongID;
                                                }
                                                catch (Exception ex2)
                                                {
                                                    if (ex2.Message.Contains("401"))
                                                    {
                                                        Thread thread4 = new(RefreshToken);
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
                                        try
                                        {
                                            DateTime dt = DateTime.Now.ToUniversalTime().Add(-ts);
                                            DateTime dt2 = DateTime.Now.ToUniversalTime().Add(-ts + ts2);
                                            if (LyricCache.Length > 128)
                                            {
                                                LyricCache = LyricCache.Remove(125, LyricCache.Length - 125) + "...";
                                            }
                                            StringDetail = SongTitleReloadedAlgo + " by " + ArtistBase2 + " · " + ReleaseDate;
                                            if (StringDetail.Length > 128)
                                            {
                                                StringDetail = StringDetail.Remove(125, StringDetail.Length - 125) + "...";
                                            }
                                            if (AlbumName.Length > 128)
                                            {
                                                AlbumName = AlbumName.Remove(125, AlbumName.Length - 125) + "...";
                                            }
                                            Globals.SongName = SongTitleReloadedAlgo;
                                            Globals.SongDetails = ArtistBase2 + " · " + ReleaseDate;
                                            Globals.SongLyrics = LyricCache;
                                            Globals.SongProgress = decimal.ToDouble(decimal.Divide(this.currenttimestamp, endtimestamp) * 100m);
                                            Globals.SongTime = ts.ToString("mm\\:ss");
                                            Globals.SongEnd = ts2.ToString("mm\\:ss");
                                            if (LastLyric != LyricCache || SongIDCache2 != SongID)
                                            {
                                                client.SetPresence(new RichPresence
                                                {
                                                    Details = StringDetail,
                                                    State = LyricCache,
                                                    Timestamps = new Timestamps
                                                    {
                                                        Start = dt,
                                                        End = dt2
                                                    },
                                                    Buttons =
                                                [
                                                new() {
                                            Label = "View on Spotify",
                                            Url = "https://open.spotify.com/track/" + SongID
                                        }
                                                ],
                                                    Assets = new Assets
                                                    {
                                                        LargeImageKey = AlbumCoverBase2,
                                                        LargeImageText = AlbumName,
                                                        SmallImageKey = "mini_logo"
                                                    },
                                                });
                                                LastLyric = LyricCache;
                                                Globals.SongBackground = AlbumCoverBase2;
                                            }
                                            SongIDCache2 = SongID;
                                        }
                                        catch
                                        {
                                        }
                                    }
                                    else
                                    {
                                        var t = Task.Run(() => OfflinePlayer());
                                        t.Wait();
                                    }
                                }
                                catch
                                {
                                    startshit = 1;
                                    if (Toki == TokiO && TokiA != null)
                                    {
                                        Toki = TokiA;
                                    }
                                    else
                                    {
                                        Toki = TokiO;
                                    }
                                }
                            }
                        }
                    }
                    catch
                    {
                        throw new Exception();
                    }
                    Thread.Sleep(1000);
                }
                catch
                {
                    startshit = 1;
                    if (Toki == TokiO && TokiA != null)
                    {
                        Toki = TokiA;
                    }
                    else
                    {
                        Toki = TokiO;
                    }
                    Thread.Sleep(30000);
                }
            }
            else
            {
                var t = Task.Run(() => OfflinePlayer());
                t.Wait();
                Thread.Sleep(1000);
            }
        }
        public async void PerformLyricShit()
        {
            while (Globals.StartedPlayer)
            {
                await ActuallyDoShit();
            }
        }
        public class MusixMatchToken(string message) : Exception(message)
        {
        }
        public async void Initialize()
        {
            try
            {
                if (Globals.RefreshToken != "")
                {
                    TokiRefresh = Globals.RefreshToken;
                    TokiO = (await new OAuthClient().RequestToken(new AuthorizationCodeRefreshRequest(Globals._clientId, Globals._secretId, TokiRefresh))).AccessToken;
                    Toki = TokiO;
                    Thread thread2 = new(InitializeB);
                    thread2.Start();
                }
                else
                {
                    SpotifyClientConfig config = SpotifyClientConfig.CreateDefault();
                    using(EmbedIOAuthServer server = new(new Uri("http://localhost:5543/callback"), 5543))
                    {
                        server.AuthorizationCodeReceived += async delegate (object sender, AuthorizationCodeResponse response)
                        {
                            AuthorizationCodeTokenResponse tokenResponse = await new OAuthClient(config).RequestToken(new AuthorizationCodeTokenRequest(Globals._clientId, Globals._secretId, response.Code, BaseUri));
                            await server.Stop();
                            TokiO = tokenResponse.AccessToken;
                            TokiRefresh = tokenResponse.RefreshToken;
                            Globals.RefreshToken = tokenResponse.RefreshToken;
                            File.WriteAllText(Globals.RefreshTokenPath, tokenResponse.RefreshToken);
                            if (Globals._clientId2 != "" && Globals._secretId2 != "")
                            {
                                Thread thread5 = new(InitializeB);
                                thread5.Start();
                            }
                            else
                            {
                                Thread thread3 = new(PerformLyricShit);
                                thread3.Start();
                                Thread thread4 = new(RefreshAPI);
                                thread4.Start();
                            }
                        };
                        await server.Start();
                        LoginRequest loginRequest = new(server.BaseUri, Globals._clientId, LoginRequest.ResponseType.Code)
                        {
                            Scope = ["user-read-currently-playing", "user-read-playback-state", "user-read-recently-played"]
                        };
                        BrowserUtil.Open(loginRequest.ToUri());
                    }
                }
                Toki = TokiO;
            }
            catch
            {
                Globals.SongLyrics = "Config error";
            }
        }
        public async void InitializeB()
        {
            try
            {
                if (Globals.RefreshToken2 != "")
                {
                    TokiRefresh2 = Globals.RefreshToken2;
                    TokiA = (await new OAuthClient().RequestToken(new AuthorizationCodeRefreshRequest(Globals._clientId2, Globals._secretId2, TokiRefresh2))).AccessToken;
                    Thread thread2 = new(PerformLyricShit);
                    thread2.Start();
                    Thread thread2a = new(RefreshAPI);
                    thread2a.Start();
                    Thread thread3 = new(RefreshAPI2);
                    thread3.Start();
                }
                else
                {
                    SpotifyClientConfig config = SpotifyClientConfig.CreateDefault();
                    using (EmbedIOAuthServer server = new(new Uri("http://localhost:5543/callback"), 5543))
                    {
                        server.AuthorizationCodeReceived += async delegate (object sender, AuthorizationCodeResponse response)
                        {
                            AuthorizationCodeTokenResponse tokenResponse = await new OAuthClient(config).RequestToken(new AuthorizationCodeTokenRequest(Globals._clientId2, Globals._secretId2, response.Code, BaseUri));
                            await server.Stop();
                            TokiA = tokenResponse.AccessToken;
                            TokiRefresh2 = tokenResponse.RefreshToken;
                            Globals.RefreshToken2 = tokenResponse.RefreshToken;
                            File.WriteAllText(Globals.RefreshToken2Path, tokenResponse.RefreshToken);
                            Thread thread4 = new(PerformLyricShit);
                            thread4.Start();
                            Thread thread2a2 = new(RefreshAPI);
                            thread2a2.Start();
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
            }
            catch
            {
                if (Globals._clientId2 != "" && Globals._secretId2 != "")
                {
                    Thread thread = new(InitializeB);
                    thread.Start();
                }
                else
                {
                    Thread thread4 = new(PerformLyricShit);
                    thread4.Start();
                    Thread thread2a2 = new(RefreshAPI);
                    thread2a2.Start();
                }
            }
        }
        public void RefreshToken()
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
        public async void RefreshAPI()
        {
            try
            {
                AuthorizationCodeRefreshResponse newResponse = await new OAuthClient().RequestToken(new AuthorizationCodeRefreshRequest(Globals._clientId, Globals._secretId, TokiRefresh));
                TokiO = newResponse.AccessToken;
                Thread.Sleep(newResponse.ExpiresIn * 1000);
                Thread thread3 = new(RefreshAPI);
                thread3.Start();
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
                Thread.Sleep(newResponse.ExpiresIn * 1000);
                Thread thread3 = new(RefreshAPI2);
                thread3.Start();
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
