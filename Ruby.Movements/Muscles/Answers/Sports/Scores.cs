using Ruby.Internal;
using Swiss;
using Swiss.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Ruby.Muscle
{
    internal class Scores : Answer
    {
        public string[] GetMLBTeams()
        {
            return new string[]{
            "Arizona DiamondBacks",
            "Atlanta Braves",
            "Baltimore Orioles",
            "Boston Red-Sox",
            "x x Chicago Cubs",
            "x x Chicago Sox",
            "Cincinnati Reds",
            "Cleveland Indians",
            "Colorado Rockies",
            "Detroit Tigers",
            "Houston Astros",
            "Kansas City Royals",
            "x x LA Dodgers",
            "x x LA Angels",
            "Miami Marlins",
            "Milwaukee Brewers",
            "Minnesota Twins",
            "x x NY Yankees",
            "x x NY Mets",
            "Oakland Athletics",
            "Philadelphia Phillies",
            "Pittsburgh Pirates",
            "San Diego Padres",
            "San Francisco Giants",
            "Seattle Mariners",
            "St. Louis Cardinals",
            "Tampa Bay Rays",
            "Texas Rangers",
            "Toronto Blue-Jays",
            "Washington Nationals" };
        }

        private Dictionary<string, string> MLBTeams;

        public Scores() : base()
        {
            MLBTeams = new Dictionary<string, string>();

            Name = "Scores";
            Key = "scores";
            ID = "156";
        }

        public override void Initialize()
        {
            MapMLBTeams();
            Spec = "future";
        }

        public override void EstablishRecognizers()
        {
            List<string> MLBNames = new List<string>();

            foreach (string team in GetMLBTeams())
            {
                var bits = team.SplitOnWhiteSpace();

                MLBNames.Add(bits[0]);
                MLBNames.Add(bits[1]);
            }

            Recognizers = new Dictionary<string, string[]>()
            {
                { "do the {} play", MLBNames.ToArray() },
                { "who pitches for the {}", MLBNames.ToArray() },
                { "where are the {} playing", MLBNames.ToArray() },
                { "what time do the {} play", MLBNames.ToArray() },
                { "what time is the {} game", MLBNames.ToArray() },
                { "who do the {} play", MLBNames.ToArray() },
            };
        }

        public override bool Stretch()
        {
            GetTodaysMLBGames();

            Spec = "current";

            string finishedGame = "1=colorado 3   ^ny mets 12 (final)&mlb_s_right1_1=w: syndergaard l: butler&mlb_s_right1_count=1&mlb_s_url1=http://sports.espn.go.com/mlb/boxscore?gameid=350813121&mlb_s_";
            string futureGame = "9=texas at minnesota (8:10 pm et)&mlb_s_right9_1=gallardo vs. gibson&mlb_s_right9_count=1&mlb_s_url9=http://sports.espn.go.com/mlb/scoreboard&mlb_s_";
            string currentGame = "3=arizona 0   cincinnati 0 (bot 1st)&mlb_s_right3_1=%ab         2 outs&mlb_s_right3_count=1&mlb_s_url3=http://sports.espn.go.com/mlb/gamecast?gameid=350823117&mlb_s_";

            string[] current_values = new string[] { "reds", "diamondbacks", "0", "0", "tied", "bottom", "1st" };
            Dictionary<string, string> current_result = DisectCurrentGame(currentGame, "cincinnati");

            string[] finished_values = new string[] { "mets", "rockies", "12", "3", "yes" };
            Dictionary<string, string> finished_result = DisectFinishedGame(finishedGame, "ny mets");

            string[] future_values = new string[] { "7:10 pm", "rangers", "gibson", "gallardo", "twins", "at home" };
            Dictionary<string, string> future_result = DisectFutureGame(futureGame, "minnesota");

            bool[] results = new bool[3];

            results[0] = !current_values.Any(val => !current_result.Values.Contains(val));
            results[1] = !future_values.Any(val => !future_result.Values.Contains(val));
            results[2] = !finished_values.Any(val => !finished_result.Values.Contains(val));

            return results.Any(res => res == false);
        }

        public override void WarmUp() { }

        public override void SetPrimarySpecs()
        {
            PrimarySpecs = new Dictionary<string, string[]>();
        }

        public override void SetSecondarySpecs()
        {
            SecondarySpecs = new Dictionary<string, string[]>()
            {
                { "pitch", new string[] { "pitching", "pitches", "mound", "pitcher" } },
                { "time", new string[] { "when", "what time", "time" } },
                { "place", new string[] { "where" } },
                { "opp", new string[] { "who do the", "play today", "opponent" } },
                { "basic", new string[] { "how are the", "doing", "score of the", "what is the score" } },
            };
        }

        public override void SetDefaultSpec()
        {
            DefaultSpec = string.Empty;
        }

        public override void GatherValues()
        {
            string input = Core.External.UserInput;

            Dictionary<string, string> results = new Dictionary<string, string>();
            string[] split = StringExtensions.SplitOnWhiteSpace(Core.External.UserInput);

            KeyValuePair<string, string> match = MLBTeams.FirstOrDefault(pair => split.Contains(pair.Key) || split.Contains(pair.Value));

            string[] pitcher = { "pitching", "pitches", "mound", "pitcher" };
            string[] gametime = { "when", "what time", "time" };
            string[] place = { "where" };
            string[] opponent = { "who do the", "play today", "opponent" };
            string[] basic = { "how are the", "doing", "score of the", "what is the score" };
            
            if(match.Key == null)
            {
                SpeechUtility.SpeakText("That team does not exist", Core.Internal.isQuiet);
            }
            else
            {
                List<string> MLBGames = GetTodaysMLBGames();
                string team = match.Key;
                string game = MLBGames.FirstOrDefault(gm => gm.Contains(team));

                if(game == null && !MLBGames.First().Contains("time"))
                {
                    Values = new Dictionary<string, string>()
                    {
                        {"Default", "The " + MLBTeams[team] + " do not appear to play today" }
                    };
                }
                else if(game.Contains("final"))
                {
                    Spec = "finished";
                    Values = DisectFinishedGame(game, team);

                    Spec += Values["won"].Equals("yes") ? "_winner" : "_loser";
                }
                else if(game.Contains("bot") || game.Contains("top"))
                {
                    Spec = "current";
                    Values = DisectCurrentGame(game, team);

                    if(Values["state"].Equals("tied")){ Spec += "_tied"; }
                    else if (Values["state"].Equals("winning")) { Spec += "_winning"; }
                    else
                    {
                        Spec += "_losing";
                    }
                }
                else
                {
                    Spec = "future" + Spec;
                    Values = DisectFutureGame(game, team);
                }
            }     
        }

        //"1=colorado 3   ^ny mets 12 (final)&mlb_s_right1_1=w: syndergaard l: butler&mlb_s_right1_count=1&mlb_s_url1=http://sports.espn.go.com/mlb/boxscore?gameid=350813121&mlb_s_";
        //"3=arizona 0   cincinnati 0 (bot 1st)&mlb_s_right3_1=%ab         2 outs&mlb_s_right3_count=1&mlb_s_url3=http://sports.espn.go.com/mlb/gamecast?gameid=350823117&mlb_s_";
        //1=cleveland 3   ny yankees 1 (bot 5th)&mlb_s_right1_1=%a2         0 outs&mlb_s_right1_count=1&mlb_s_url1=http://sports.espn.go.com/mlb/gamecast?gameid=350823110&mlb_s_
        private void MapMLBTeams()
        {
            foreach (string team in GetMLBTeams())
            {
                string[] splitter = team.ToLower().Split(' ');

                if (splitter.Length == 2)
                {
                    MLBTeams.Add(splitter[0], splitter[1]);
                }
                else if (splitter.Length == 3)
                {
                    MLBTeams.Add(splitter[0] + " " + splitter[1], splitter[2]);
                }
                else
                {
                    MLBTeams.Add(splitter[2] + " " + splitter[3], splitter[3]);
                }
            }

            MLBTeams["boston"] = MLBTeams["boston"].Replace("-", " ");
            MLBTeams["toronto"] = MLBTeams["toronto"].Replace("-", " ");
        }
        private List<string> GetTodaysMLBGames()
        {
            List<string> games = new List<string>();

            string url = "http://sports.espn.go.com/mlb/bottomline/scores";
            string res = InternetUtility.MakeWebRequest(url, 5000);

            res = res.Replace("%20", " ");

            foreach (string str in Regex.Split(res, "left"))
            {
                games.Add(str.ToLower());
            }

            return games;
        }
        private Dictionary<string,string> DisectCurrentGame(string game, string team)
        {
            string[] splitter = Regex.Split(game, "[()]");
            string mtchup = splitter[0].Split('=')[1];
            string inning = splitter[1];

            string[] teams = Regex.Split(mtchup.Trim(), "   ");

            string myTeamScore, oppScore, oppTeam;

            if(teams[0].Contains(team))
            {
                myTeamScore = teams[0].Split(' ').Last();
                oppScore = teams[1].Split(' ').Last();
                oppTeam = teams[1].Replace(oppScore, "").Trim();
            }
            else
            {
                myTeamScore = teams[1].Split(' ').Last();
                oppScore = teams[0].Split(' ').Last();
                oppTeam = teams[0].Replace(oppScore, "").Trim();
            }

            string stateOfInning = inning.Split(' ')[0].Equals("bot") ? "bottom" : "top";

            bool winning, losing, tied;

            winning = Convert.ToInt16(myTeamScore) > Convert.ToInt16(oppScore);
            losing = !winning;
            tied = myTeamScore.Equals(oppScore);

            string state = string.Empty;

            if (winning) { state = "winning"; }
            else if (tied) { state = "tied"; }
            else { state = "losing"; }

            return new Dictionary<string, string>()
            {
                { "team", MLBTeams[team] },
                { "opponent", MLBTeams[oppTeam] },
                { "theScore", myTeamScore },
                { "oppScore", oppScore },
                { "state", state},
                { "stInning", stateOfInning },
                { "inning", inning.Split(' ')[1] }
            };
        }
        private Dictionary<string,string> DisectFinishedGame(string game, string team)
        {
            string[] sp = game.Split('=');
            string matchup = sp[1].Split('(')[0].Trim();
            string pitchers = sp[2];

            string[] splitOnWinner = Regex.Split(matchup, "   ");
            int indexWinner = splitOnWinner[0].StartsWith("^") ? 0 : 1;
            int indexLoser = indexWinner == 0 ? 1 : 0;

            string[] winner = splitOnWinner[indexWinner].Substring(1).Split(' ');
            string[] loser = splitOnWinner[indexLoser].Split(' ');

            string winningTeam = winner.Length == 2 ? winner[0] : winner[0] + " " + winner[1];
            string winnerScore = winner.Last();

            string losingTeam = loser.Length == 2 ? loser[0] : loser[0] + " " + loser[1];
            string loserScore = loser.Last();

            string won = winningTeam.Equals(team) ? "yes" : "no";

            return new Dictionary<string, string>()
            {
                { "winner", MLBTeams[winningTeam] },
                { "loser", MLBTeams[losingTeam] },
                { "wScore", winnerScore },
                { "lScore", loserScore },
                { "won", won}
            };
        }
        private Dictionary<string, string> DisectFutureGame(string game, string team)
        {
            string[] sp = game.Split('=');
            string[] matchup = Regex.Split(sp[1], @"\bat\b|[()]");
            string visitor = matchup[0].Trim();
            string home = matchup[1].Trim();

            string timeOfGame = matchup[2].Trim();
            timeOfGame = (Convert.ToInt16(timeOfGame.Split(':')[0]) - 1).ToString() + ":" + timeOfGame.Split(':')[1].Substring(0, 5);

            string place = home.Equals(team) ? "at home" : "away";
            string opp = home.Equals(team) ? visitor : home;
            int pitcherIndex = home.Equals(team) ? 1 : 0;
            int opposingIndex = pitcherIndex == 0 ? 1 : 0;

            string[] pitching = Regex.Split(sp[2].Substring(0, sp[2].IndexOf("&")), "vs.");
            string todaysPitcher = pitching[pitcherIndex].Trim();
            string opposingPitcher = pitching[opposingIndex].Trim();

            return new Dictionary<string, string>()
            {
                { "time", timeOfGame },
                { "opponent", MLBTeams[opp] },
                { "pitcher", todaysPitcher },
                { "oppPitcher", opposingPitcher },
                { "team", MLBTeams[team] },
                { "place", place }
            };
        }
    }
}
