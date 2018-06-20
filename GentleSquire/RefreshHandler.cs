using System;
using System.Timers;
using System.Collections.Generic;
using System.Text;
using GentleSquire;
using Flurl.Http;
using System.IO;
using System.Linq;
using System.Globalization;
using DSharpPlus;
using DSharpPlus.Entities;

namespace GentleSquire
{
    class RefreshHandler
    {
        #region RefreshHandler Fields
        public static bool UpdateBot = false;
        public static bool doUpdate = false;
        public static int ILCounter = 0;
        public static int SpeedrunCounter = 0;
        public static ulong channelID = 413289773373849601;
        public static string ILPostLink = "http://ron.naezith.com/fetchleaderboard";
        public static string SpeedrunPostLink = "http://ron.naezith.com/fetchSpeedrunLB";
        public static string ogPath = @"C:\temp\";
        public static string JadePath = ogPath + @"Jade\";
        public static string SandPath = ogPath + @"Sand\";
        public static string MetalPath = ogPath + @"Metal\";
        public static string CrystalPath = ogPath + @"Crystal\";
        public static string SecretPath = ogPath + @"Secret\";
        public static string GeneralPath = ogPath + @"General\";

        public static Tuple<int, string, string>[] LevelInfo = {
            Tuple.Create(437, "Threshold", JadePath + @"Threshold" + ".txt"),
            Tuple.Create(383, "Ponddle", JadePath + @"Ponddle" + ".txt"),
            Tuple.Create(433, "Soggy", JadePath + @"Soggy" + ".txt"),
            Tuple.Create(406, "Sewers", JadePath + @"Sewers" + ".txt"),
            Tuple.Create(463, "Swishflush", JadePath + @"Swishflush" + ".txt"),
            Tuple.Create(404, "Lush", JadePath + @"Lush" + ".txt"),
            Tuple.Create(403, "Trempe", JadePath + @"Trempe" + ".txt"),
            Tuple.Create(157, "The Gap", JadePath + @"The Gap" + ".txt"),
            Tuple.Create(143, "Conduit", JadePath + @"Conduit" + ".txt"),
            Tuple.Create(405, "Wind Up", JadePath + @"Wind Up" + ".txt"),
            Tuple.Create(487, "Memorial", JadePath + @"Memorial" + ".txt"),
            Tuple.Create(164, "Potential", JadePath + @"Potential" + ".txt"),
            Tuple.Create(414, "Waste Pit", JadePath + @"Waste Pit" + ".txt"),
            Tuple.Create(154, "Flea", JadePath + @"Flea" + ".txt"),
            Tuple.Create(408, "Chasm", JadePath + @"Chasm" + ".txt"),
            Tuple.Create(145, "Shinespark", JadePath + @"Shinespark" + ".txt"),
            Tuple.Create(190, "Cliffside", JadePath + @"Cliffside" + ".txt"),
            Tuple.Create(156, "Classy", JadePath + @"Classy" + ".txt"),
            Tuple.Create(420, "Self Control", JadePath + @"Self Control" + ".txt"),
            Tuple.Create(149, "Plunge", JadePath + @"Plunge" + ".txt"),

            Tuple.Create(488, "Wish", SandPath + @"Wish" + ".txt"),
            Tuple.Create(121, "Roundabout", SandPath + @"Roundabout" + ".txt"),
            Tuple.Create(150, "The Swing", SandPath + @"The Swing" + ".txt"),
            Tuple.Create(331, "Buried", SandPath + @"Buried" + ".txt"),
            Tuple.Create(242, "Razorback", SandPath + @"Razorback" + ".txt"),
            Tuple.Create(193, "Sideswipe", SandPath + @"Sideswipe" + ".txt"),
            Tuple.Create(390, "Spin", SandPath + @"Spin" + ".txt"),
            Tuple.Create(202, "Yard", SandPath + @"Yard" + ".txt"),
            Tuple.Create(380, "Flipflop", SandPath + @"Flipflop" + ".txt"),
            Tuple.Create(233, "Pandora", SandPath + @"Pandora" + ".txt"),
            Tuple.Create(262, "Redmire", SandPath + @"Redmire" + ".txt"),
            Tuple.Create(160, "Palace", SandPath + @"Palace" + ".txt"),
            Tuple.Create(317, "Corrosion", SandPath + @"Corrosion" + ".txt"),
            Tuple.Create(309, "The Hill", SandPath + @"The Hill" + ".txt"),
            Tuple.Create(615, "Mist", SandPath + @"Mist" + ".txt"),
            Tuple.Create(313, "Gray Cliff", SandPath + @"Gray Cliff" + ".txt"),
            Tuple.Create(220, "Breeze", SandPath + @"Breeze" + ".txt"),
            Tuple.Create(238, "Hadrian Wall", SandPath + @"Hadrian Wall" + ".txt"),
            Tuple.Create(260, "Bloodpit", SandPath + @"Bloodpit" + ".txt"),
            Tuple.Create(616, "Sunken Caves", SandPath + @"Sunken Caves" + ".txt"),

            Tuple.Create(363, "Protos", MetalPath + @"Protos" + ".txt"),
            Tuple.Create(239, "Splinter", MetalPath + @"Splinter" + ".txt"),
            Tuple.Create(36 , "Stalactites", MetalPath + @"Stalactites" + ".txt"),
            Tuple.Create(496, "Terran Core", MetalPath + @"Terran Core" + ".txt"),
            Tuple.Create(338, "Zigzag", MetalPath + @"Zigzag" + ".txt"),
            Tuple.Create(625, "Wall of Shear", MetalPath + @"Wall of Shear" + ".txt"),
            Tuple.Create(353, "Force", MetalPath + @"Force" + ".txt"),
            Tuple.Create(243, "Escape", MetalPath + @"Escape" + ".txt"),
            Tuple.Create(325, "FTL", MetalPath + @"FTL" + ".txt"),
            Tuple.Create(304, "Hectic", MetalPath + @"Hectic" + ".txt"),
            Tuple.Create(348, "Scanner", MetalPath + @"Scanner" + ".txt"),
            Tuple.Create(340, "Plumber", MetalPath + @"Plumber" + ".txt"),
            Tuple.Create(619, "Redshift", MetalPath + @"Redshift" + ".txt"),
            Tuple.Create(318, "Tunnel", MetalPath + @"Tunnel" + ".txt"),
            Tuple.Create(336, "Floodgate", MetalPath + @"Floodgate" + ".txt"),
            Tuple.Create(211, "Vertical", MetalPath + @"Vertical" + ".txt"),
            Tuple.Create(333, "Secure", MetalPath + @"Secure" + ".txt"),
            Tuple.Create(140, "Elevator", MetalPath + @"Elevator" + ".txt"),
            Tuple.Create(641, "Techy", MetalPath + @"Techy" + ".txt"),
            Tuple.Create(144, "Shaft", MetalPath + @"Shaft" + ".txt"),

            Tuple.Create(192, "Dismemberment", CrystalPath + @"Dismemberment" + ".txt"),
            Tuple.Create(171, "Decay", CrystalPath + @"Decay" + ".txt"),
            Tuple.Create(640, "Penumbra", CrystalPath + @"Penumbra" + ".txt"),
            Tuple.Create(479, "Detour", CrystalPath + @"Detour" + ".txt"),
            Tuple.Create(646, "Surge", CrystalPath + @"Surge" + ".txt"),
            Tuple.Create(365, "Hope", CrystalPath + @"Hope" + ".txt"),
            Tuple.Create(647, "Ruins", CrystalPath + @"Ruins" + ".txt"),
            Tuple.Create(642, "Heretic", CrystalPath + @"Heretic" + ".txt"),
            Tuple.Create(670, "Ascend", CrystalPath + @"Ascend" + ".txt"),
            Tuple.Create(135, "Hall of Despair", CrystalPath + @"Hall of Despair" + ".txt"),
            Tuple.Create(635, "Metamorphosis", CrystalPath + @"Metamorphosis" + ".txt"),
            Tuple.Create(655, "Crossfire", CrystalPath + @"Crossfire" + ".txt"),
            Tuple.Create(645, "Oldway", CrystalPath + @"Oldway" + ".txt"),
            Tuple.Create(637, "Limbo", CrystalPath + @"Limbo" + ".txt"),
            Tuple.Create(559, "Bandicap", CrystalPath + @"Bandicap" + ".txt"),
            Tuple.Create(158, "Grinder", CrystalPath + @"Grinder" + ".txt"),
            Tuple.Create(357, "Couloir", CrystalPath + @"Couloir" + ".txt"),
            Tuple.Create(426, "Vertigo", CrystalPath + @"Vertigo" + ".txt"),
            Tuple.Create(341, "Tower of Joy", CrystalPath + @"Tower of Joy" + ".txt"),
            Tuple.Create(147, "Chamber", CrystalPath + @"Chamber" + ".txt"),

            Tuple.Create(132, "Void", SecretPath + @"Void" + ".txt"),
            Tuple.Create(188, "Bolt", SecretPath + @"Bolt" + ".txt"),
            Tuple.Create(227, "Cube", SecretPath + @"Cube" + ".txt"),
            Tuple.Create(314, "Boost", SecretPath + @"Boost" + ".txt"),
            Tuple.Create(376, "Victim", SecretPath + @"Victim" + ".txt"),
            Tuple.Create(429, "Area 1", SecretPath + @"Area 1" + ".txt"),
            Tuple.Create(432, "A wet level name", SecretPath + @"A wet level name" + ".txt"),
            Tuple.Create(332, "Rush", SecretPath + @"Rush" + ".txt"),
        };

        public static Tuple<int, string, string>[] SpeedrunInfo =
        {
            Tuple.Create(1, "Jade Speedrun", JadePath + @"Jade Speedrun.txt"),
            Tuple.Create(2, "Sand Speedrun", SandPath + @"Sand Speedrun.txt"),
            Tuple.Create(3, "Metal Speedrun", MetalPath + @"Metal Speedrun.txt"),
            Tuple.Create(4, "Crystal Speedrun", CrystalPath + @"Crystal Speedrun.txt"),
            Tuple.Create(5, "All Chapters Speedrun", GeneralPath + @"All Chapters Speedrun.txt"),
        };
        #endregion

        public static void StartTimer()
        {
            Directory.CreateDirectory(ogPath);
            Directory.CreateDirectory(JadePath);
            Directory.CreateDirectory(SandPath);
            Directory.CreateDirectory(MetalPath);
            Directory.CreateDirectory(CrystalPath);
            Directory.CreateDirectory(SecretPath);
            Directory.CreateDirectory(GeneralPath);

            System.Timers.Timer aTimer = new System.Timers.Timer();
            aTimer.Elapsed += new ElapsedEventHandler(RefreshILLeaderboardsAsync);
            aTimer.Interval = 3409;
            aTimer.Enabled = true;

            System.Timers.Timer SpeedrunLBTimer = new System.Timers.Timer();
            SpeedrunLBTimer.Elapsed += new ElapsedEventHandler(RefreshSpeedrunLeaderboardsAsync);
            SpeedrunLBTimer.Interval = 60000;
            SpeedrunLBTimer.Enabled = true;
        }



        public static async void RefreshILLeaderboardsAsync(object source, ElapsedEventArgs e)
        {
            int levelID = LevelInfo[ILCounter].Item1;
            string levelName = LevelInfo[ILCounter].Item2;
            string levelPath = LevelInfo[ILCounter].Item3;
            DiscordChannel channel = await Program._client.GetChannelAsync(channelID);

            string responseString = "";

            try
            {
                responseString = await ILPostLink.PostUrlEncodedAsync(new { level_id = levelID, line_count = "1", start_rank = "0" }).ReceiveString();

            }
            catch
            {
                Console.WriteLine("Could not fetch responseString");
            }

            if (responseString != "")
            {
                var NewRecord = GetRunInfo(responseString, true, "IL"); // Get new record touple (name, time, floattime, date, datetime, timespan, info)
                if (!File.Exists(levelPath))
                {
                    using (StreamWriter sw = File.CreateText(levelPath))
                    {
                        sw.WriteLine(NewRecord.Item7);
                        Console.WriteLine("Created: " + levelName + " - |" + NewRecord.Item7);
                    }
                }
                else
                {
                    var OldRecord = GetRunInfo(File.ReadLines(levelPath).Last(), false, "IL");
                    double timeDifferenceDouble = Math.Round((float.Parse(OldRecord.Item2) - float.Parse(NewRecord.Item2))/1000, 3);
                    string timeDifference = timeDifferenceDouble.ToString().Replace(",", ".");
                    TimeSpan dateDifference = (NewRecord.Item5.Date.Subtract(OldRecord.Item5.Date));
                    GetOldestRecord(levelName, responseString, false);


                    if (OldRecord.Item2 != NewRecord.Item2)
                    {
                        string[] readText = File.ReadAllLines(GeneralPath + "OldestRecord.txt");
                        foreach (string z in readText)
                        {
                            if (z.Contains(levelName))
                            {
                                string topicString = "Longest standing records:\r\n";
                                var tempFile = Path.GetTempFileName();
                                var linesToKeep = File.ReadLines(GeneralPath + "OldestRecord.txt").Where(l => !l.Contains(levelName));
                                File.WriteAllLines(tempFile, linesToKeep);
                                File.Delete(GeneralPath + "OldestRecord.txt");
                                File.Move(tempFile, GeneralPath + "OldestRecord.txt");
                                Console.WriteLine("Cleared Oldest Record Entry");
                                string[] readText2 = File.ReadAllLines(GeneralPath + "OldestRecord.txt");
                                foreach (string x in readText2)
                                {
                                    var _xlevelName = StringBuilding.getBetweenStr(x, "Level: ", " - Name:");
                                    var _xtime = StringBuilding.getBetweenStr(x, "Time: ", " | Date:");
                                    string _xconvertedTime = ConvertTime(_xtime);
                                    var _xdate = StringBuilding.getBetweenStr(x, "Date: ", " |");
                                    var _xconvertedDate = DateTime.Parse(_xdate);
                                    var _xdateDifference = (DateTime.Now.Date.Subtract(_xconvertedDate.Date));
                                    topicString = topicString + _xlevelName + " - [" + _xconvertedTime + "s] by " + StringBuilding.getBetweenStr(x, "Name: ", " | Time:") + " [" + _xdateDifference.Days.ToString() + " days]\r\n";
                                }
                                SetTopic(topicString);
                            }
                        }

                        using (StreamWriter sw = File.AppendText(levelPath))
                        {
                            sw.WriteLine(NewRecord.Item7);
                            Console.WriteLine("Added new record to " + levelName);
                            if (OldRecord.Item1 != NewRecord.Item1)
                            {
                                await Program._client.SendMessageAsync(channel, "**" + levelName + " - " + "[" + NewRecord.Item3 + "] :trophy: " + NewRecord.Item1 + "** beat **~~" + OldRecord.Item1 + "~~**'s record by **" + timeDifference + "s** (Record stood for **" + dateDifference.TotalDays.ToString() + " days!**)", false, null);
                            }
                            else
                            {
                                await Program._client.SendMessageAsync(channel, "**" + levelName + " - " + "[" + NewRecord.Item3 + "] :trophy: " + NewRecord.Item1 + "** improved their record by **" + timeDifference + "s** (Record stood for **" + dateDifference.TotalDays.ToString() + " days!**)", false, null);
                            }
                        }

                    }
                }

                Console.WriteLine("Update complete - " + levelName + " | " + DateTime.Now);
                if (ILCounter < LevelInfo.Length - 1)
                {
                    ILCounter++;
                }
                else
                {
                    if (UpdateBot == true)
                    {
                        UpdateBot = false;
                        Console.WriteLine("Bot refresh complete!");
                    }
                    ILCounter = 0;
                }
            }
        }


        public static async void RefreshSpeedrunLeaderboardsAsync(object source, ElapsedEventArgs e)
        {
            int SpeedrunID = SpeedrunInfo[SpeedrunCounter].Item1;
            string SpeedrunName = SpeedrunInfo[SpeedrunCounter].Item2;
            string SpeedrunPath = SpeedrunInfo[SpeedrunCounter].Item3;
            DiscordChannel channel = await Program._client.GetChannelAsync(channelID);

            string responseString = "";

            try
            {
                responseString = await SpeedrunPostLink.PostUrlEncodedAsync(new { type = SpeedrunID, start_rank = "0" }).ReceiveString();
            }
            catch
            {
                Console.WriteLine("Could not fetch speedrun responseString");
            }
            
            if (responseString != "")
            {
                var NewRecord = GetRunInfo(responseString, true, "Speedrun"); // Get new record touple (name, time, floattime, date, datetime, timespan, info)
                if (!File.Exists(SpeedrunPath))
                {
                    using (StreamWriter sw = File.CreateText(SpeedrunPath))
                    {
                        sw.WriteLine(NewRecord.Item7);
                        Console.WriteLine("Created: " + SpeedrunName + " - |" + NewRecord.Item7);
                    }
                }
                else
                {
                    var OldRecord = GetRunInfo(File.ReadLines(SpeedrunPath).Last(), false, "Speedrun");
                    double timeDifferenceDouble = Math.Round((float.Parse(OldRecord.Item2)- float.Parse(NewRecord.Item2))/1000, 3);
                    string timeDifference = timeDifferenceDouble.ToString().Replace(",", ".");
                    TimeSpan dateDifference = (NewRecord.Item5.Date.Subtract(OldRecord.Item5.Date));


                    if (OldRecord.Item2 != NewRecord.Item2)
                    {
                        using (StreamWriter sw = File.AppendText(SpeedrunPath))
                        {
                            sw.WriteLine(NewRecord.Item7);
                            Console.WriteLine("Added new record to " + SpeedrunName);
                            if (OldRecord.Item1 != NewRecord.Item1)
                            {
                                await Program._client.SendMessageAsync(channel, "**" + SpeedrunName + " - " + "[" + NewRecord.Item3 + "] :trophy: " + NewRecord.Item1 + "** beat **~~" + OldRecord.Item1 + "~~**'s record by **" + timeDifference + "s** (Record stood for **" + dateDifference.TotalDays.ToString() + " days!**)", false, null);
                            }
                            else
                            {
                                await Program._client.SendMessageAsync(channel, "**" + SpeedrunName + " - " + "[" + NewRecord.Item3 + "] :trophy: " + NewRecord.Item1 + "** improved their record by **" + timeDifference + "s** (Record stood for **" + dateDifference.TotalDays.ToString() + " days!**)", false, null);
                            }
                        }

                    }
                }

                Console.WriteLine("Update complete - " + SpeedrunName + " | " + DateTime.Now);
                if (SpeedrunCounter < SpeedrunInfo.Length - 1)
                {
                    SpeedrunCounter++;
                }
                else
                {
                    SpeedrunCounter = 0;
                }
            }
        }

        public static async void GetOldestRecord(string levelName, string newrecord, bool clear)
        {
            var levelPath = LevelInfo[ILCounter].Item3;
            var oldestRecordPath = GeneralPath + "OldestRecord.txt";
            var NewRecord = GetRunInfo(newrecord, true, "IL");
            DiscordChannel chan = await Program._client.GetChannelAsync(channelID);

            TimeSpan start = new TimeSpan(00, 50, 0); //10 o'clock
            TimeSpan end = new TimeSpan(00, 59, 59); //12 o'clock
            TimeSpan now = DateTime.Now.TimeOfDay;

            if ((now > start) && (now < end) && UpdateBot == false && ILCounter == 0)
            {
                SetTopic("Bot is refreshing");
                Console.WriteLine("Bot refresh is starting!");
                UpdateBot = true;
                if (File.Exists(oldestRecordPath))
                {
                    File.Delete(oldestRecordPath);
                }
            }


            if (!File.Exists(oldestRecordPath))
            {
                using (StreamWriter sw = File.CreateText(oldestRecordPath))
                {
                    sw.WriteLine("Level: " + levelName + " - " + NewRecord.Item7);
                    var topicString = "Longest standing | " + levelName + " - [" + NewRecord.Item3 + "] by " + NewRecord.Item1 + " [" + NewRecord.Item6.Days.ToString() + " days]";
                    SetTopic(topicString);
                }
            } else {
                try
                {
                    var testRekky = GetRunInfo(File.ReadLines(oldestRecordPath).Last(), false, "IL");
                } catch
                {
                    File.Delete(oldestRecordPath);
                    using (StreamWriter sw = File.CreateText(oldestRecordPath))
                    {
                        Console.WriteLine("OldestRecord.txt was empty! Deleting and recreating");
                        sw.WriteLine("Level: " + levelName + " - " + NewRecord.Item7);
                        var topicString = "Longest standing | " + levelName + " - [" + NewRecord.Item3 + "] by " + NewRecord.Item1 + " [" + NewRecord.Item6.Days.ToString() + " days]";
                        SetTopic(topicString);
                    }
                }
                var OldRecord = GetRunInfo(File.ReadLines(oldestRecordPath).Last(), false, "IL");

                if ((chan.Topic == "Bot is refreshing") && UpdateBot == false)
                {
                    doUpdate = true;
                }


                if (clear == false)
                {
                    if (NewRecord.Item6 > OldRecord.Item6)
                    {
                        File.Delete(oldestRecordPath);
                        using (StreamWriter sw = File.CreateText(oldestRecordPath))
                        {
                            sw.WriteLine("Level: " + levelName + " - " + NewRecord.Item7);
                            var topicString = "Longest standing | " + levelName + " - [" + NewRecord.Item3 + "] by " + NewRecord.Item1 + " [" + NewRecord.Item6.Days.ToString() + " days]";
                            SetTopic(topicString);
                        }
                    } else if ((NewRecord.Item6 == OldRecord.Item6) || doUpdate == true)
                    {
                        string[] readText = File.ReadAllLines(oldestRecordPath);
                        if (!readText.Contains("Level: " + levelName + " - " + NewRecord.Item7) || doUpdate == true)
                        {
                            string topicString = "Longest standing records:\r\n";
                            using (StreamWriter sw = File.AppendText(oldestRecordPath))
                            {
                                if (doUpdate == false)
                                {
                                    sw.WriteLine("Level: " + levelName + " - " + NewRecord.Item7);
                                }
                            }
                            string[] readText2 = File.ReadAllLines(oldestRecordPath);
                            foreach (string x in readText2)
                            {
                                var _xlevelName = StringBuilding.getBetweenStr(x, "Level: ", " - Name:");
                                var _xtime = StringBuilding.getBetweenStr(x, "Time: ", " | Date:");
                                string _xconvertedTime = ConvertTime(_xtime);
                                var _xdate = StringBuilding.getBetweenStr(x, "Date: ", " |");
                                var _xconvertedDate = DateTime.Parse(_xdate);
                                var _xdateDifference = (DateTime.Now.Date.Subtract(_xconvertedDate.Date));
                                topicString = topicString + _xlevelName + " - [" + _xconvertedTime + "] by " + StringBuilding.getBetweenStr(x, "Name: ", " | Time:") + " [" + _xdateDifference.Days + " days]\r\n";
                            }
                            SetTopic(topicString);
                            if(doUpdate == true)
                            {
                                doUpdate = false;
                            }
                        }
                    }
                } else {
                    if (File.Exists(oldestRecordPath))
                    {
                        File.Delete(oldestRecordPath);
                    }
                }

            }
        }

        public static async void SetTopic(string topic)
        {
            DiscordChannel chan = await Program._client.GetChannelAsync(channelID);
            if (UpdateBot == false)
            {
                try
                {
                    await chan.ModifyAsync(chan.Name, chan.Position, topic, chan.Parent);
                }
                catch
                {
                    Console.WriteLine("Exception: Failed to set topic (Permission?)");
                }
            } else
            {
                Console.WriteLine("Bot Refreshing| Can't set topic");
            }
        }

        public static Tuple<string, string, string, string, DateTime, TimeSpan, string> GetRunInfo(string rawRecord, bool newRun, string runType)
        {
            string name;
            string time;
            float timeFloat;
            string convertedTime;
            string date;
            DateTime convertedDate;
            TimeSpan dateDifference;
            string info;

            if (newRun == true)
            {
                if (runType == "IL")
                {
                    name = StringBuilding.getBetweenStr(rawRecord, "\"username\":\"", "\",\"badge\"");
                    time = StringBuilding.getBetweenStr(rawRecord, "\"time\":", ",\"replay\"");
                    convertedTime = ConvertTime(time);
                    date = StringBuilding.getBetweenStr(rawRecord, "\"update_date\":\"", "T");
                    convertedDate = DateTime.Parse(date);
                    dateDifference = (DateTime.Now.Date.Subtract(convertedDate.Date));
                    info = "Name: " + name + " | Time: " + time + " | Date: " + date + " |";

                    Tuple<string, string, string, string, DateTime, TimeSpan, string> returnData = Tuple.Create(name, time, convertedTime, date, convertedDate, dateDifference, info);

                    return returnData;
                } else if (runType == "Speedrun")
                {
                    name = StringBuilding.getBetweenStr(rawRecord, "\"username\":\"", "\",\"total_time\"");
                    time = StringBuilding.getBetweenStr(rawRecord, "\"total_time\":", ",\"update_date\"");
                    timeFloat = float.Parse(time) / 1000;
                    convertedTime = ConvertTime(time);
                    date = StringBuilding.getBetweenStr(rawRecord, "\"update_date\":\"", "T");
                    convertedDate = DateTime.Parse(date);
                    dateDifference = (DateTime.Now.Date.Subtract(convertedDate.Date));
                    info = "Name: " + name + " | Time: " + time + " | Date: " + date + " |";

                    Tuple<string, string, string, string, DateTime, TimeSpan, string> returnData = Tuple.Create(name, time, convertedTime, date, convertedDate, dateDifference, info);

                    return returnData;
                } else
                {
                    return null;
                }
            } else
            {
                info = rawRecord;
                name = StringBuilding.getBetweenStr(info, "Name: ", " | Time:");
                time = StringBuilding.getBetweenStr(info, "Time: ", " | Date:");
                convertedTime = ConvertTime(time);
                date = StringBuilding.getBetweenStr(info, "Date: ", " |");
                convertedDate = DateTime.Parse(date);
                dateDifference = (DateTime.Now.Date.Subtract(convertedDate.Date));

                Tuple<string, string, string, string, DateTime, TimeSpan, string> returnData = Tuple.Create(name, time, convertedTime, date, convertedDate, dateDifference, rawRecord);
                return returnData;
            }
        }

        public static string ConvertTime(string time)
        {
            float timeFloat;
            double hours;
            double minutes;
            double seconds;
            string convertedTime;

            timeFloat = float.Parse(time) / 1000;
            if (timeFloat >= 60.00f)
            {
                if (timeFloat >= 3600.00f)
                {
                    hours = Math.Floor(timeFloat / 60 / 60);
                    minutes = Math.Floor((timeFloat - (hours * 60 * 60)) / 60);
                    seconds = Math.Round(timeFloat - (minutes * 60),3);
                    convertedTime = hours.ToString() + "h " + minutes.ToString() + "m " + seconds.ToString() + "s";
                }
                else
                {
                    minutes = Math.Floor(timeFloat / 60);
                    seconds = Math.Round(timeFloat - (minutes * 60),3);
                    convertedTime = minutes.ToString() + "m " + seconds.ToString() + "s";
                } 
            } else {
                convertedTime = timeFloat.ToString() + "s";
            }

            convertedTime = convertedTime.Replace(",", ".");

            return convertedTime;
        }
    }
}
