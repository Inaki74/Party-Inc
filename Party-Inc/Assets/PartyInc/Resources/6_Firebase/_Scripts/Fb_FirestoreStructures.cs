using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Firestore;

namespace PartyInc
{
    namespace PartyFirebase.Firestore
    {
        public static class Fb_FirestoreStructures
        {
            /// <summary>
            /// The database object of every Firestore document in the Players Collection translated to a C# class template.
            /// The PLAYER structure:
            ///
            /// Has 5 elements:
            ///     assets -> The list of assets it owns.
            ///     character -> The dictionary that contains information about this players character.
            ///     data -> The dictionary that contains information about this players personal data.
            ///     stats -> The dictionary that contains information about this players game statistics.
            ///     tasks -> The dictionary that contains information about this players current tasks.
            ///
            /// All 5 of these elements have a static maximum amount of information (with exception of assets), are frequently accessed with the player and are
            /// essential to the player experience. Hence the structure given.
            /// </summary>
            ///
            public class FSPlayer : IFb_FirestoreMapStructure
            {
                public CollectionReference sessions;
                public List<string> achievements;
                public List<Dictionary<string, object>> assets;
                public Dictionary<string, object> character;
                public Dictionary<string, object> data;
                public Dictionary<string, object> stats;
                public Dictionary<string, object> goals;

                public FSPlayer()
                {
                    achievements = new List<string>();
                    assets = new List<Dictionary<string, object>>();
                    character = new FSCharacter().ToDictionary();
                    data = new FSData().ToDictionary();
                    stats = new FSStats().ToDictionary();
                    goals = new Dictionary<string, object>();
                }

                public FSPlayer(List<string> achievements, FSCharacter character, FSData data, FSStats stats)
                {
                    this.achievements = achievements;
                    this.assets = new List<Dictionary<string, object>>();
                    this.character = character.ToDictionary();
                    this.data = data.ToDictionary();
                    this.stats = stats.ToDictionary();
                    this.goals = new Dictionary<string, object>();
                }

                public void AddGoal(string goalid, double progress)
                {
                    Dictionary<string, object> goal = new Dictionary<string, object>();

                    goal.Add(goalid, progress);

                    goals.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_TASKS, goal);
                }

                public void AddAsset(string assetId, int assetType, List<string> variations)
                {
                    Dictionary<string, object> ass = new Dictionary<string, object>();

                    ass.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_ASSETS_TYPE, assetType);
                    ass.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_ASSETS_ID, assetId);
                    ass.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_ASSETS_VARIATIONS, variations);

                    assets.Add(ass);
                }

                public Dictionary<string, object> ToDictionary()
                {
                    Dictionary<string, object> dic = new Dictionary<string, object>();

                    dic.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_ASSETS, assets);
                    dic.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_CHARACTER, character);
                    dic.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_DATA, data);
                    dic.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_STATS, stats);
                    dic.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_TASKS, goals);

                    return dic;
                }

                public class FSCharacter : IFb_FirestoreMapStructure
                {
                    public int currentloadout;
                    public Dictionary<string, object> face;
                    public Dictionary<int, object> loadouts;

                    public FSCharacter()
                    {
                        currentloadout = 0;
                        face = new FSFace().ToDictionary();
                        loadouts = new Dictionary<int, object>();
                    }

                    public FSCharacter(int currentloadout, FSFace face)
                    {
                        this.currentloadout = currentloadout;
                        this.face = face.ToDictionary();
                        this.loadouts = new Dictionary<int, object>();
                    }

                    public Dictionary<string, object> ToDictionary()
                    {
                        Dictionary<string, object> dic = new Dictionary<string, object>();

                        dic.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_CHARACTER_CURRENTLOADOUT, currentloadout);
                        dic.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_CHARACTER_FACE, face);
                        dic.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_CHARACTER_LOADOUTS, loadouts);

                        return dic;
                    }

                    public void AddLoadout(int loadoutId,
                                           string backid, string earid, string facialhairid, string glassid, string wallpaperid,
                                           string hairid, string pantsid, string shirtid, string socksid, string shoesid,
                                           string emoteHappyid, string emoteSadid, string emoteAngryid, string emoteLaughid, string emoteSurpriseid, string celebrationid,
                                           string tuneid
                            )
                    {
                        Dictionary<string, object> toAdd = new Dictionary<string, object>();

                        toAdd.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_CHARACTER_LOADOUTS_SHOES, shoesid);
                        toAdd.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_CHARACTER_LOADOUTS_BACK, backid);
                        toAdd.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_CHARACTER_LOADOUTS_EAR, earid);
                        toAdd.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_CHARACTER_LOADOUTS_FACIALHAIR, facialhairid);
                        toAdd.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_CHARACTER_LOADOUTS_GLASS, glassid);
                        toAdd.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_CHARACTER_LOADOUTS_HAIR, hairid);
                        //toAdd.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_CHARACTER_LOADOUTS_JAW, jawid);
                        toAdd.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_CHARACTER_LOADOUTS_PANTS, pantsid);
                        toAdd.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_CHARACTER_LOADOUTS_SHIRT, shirtid);
                        toAdd.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_CHARACTER_LOADOUTS_SOCKS, socksid);
                        toAdd.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_CHARACTER_LOADOUTS_WALLPAPER, wallpaperid);
                        toAdd.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_CHARACTER_LOADOUTS_EMOTEHAPPY, emoteHappyid);
                        toAdd.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_CHARACTER_LOADOUTS_EMOTESAD, emoteSadid);
                        toAdd.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_CHARACTER_LOADOUTS_EMOTEANGRY, emoteAngryid);
                        toAdd.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_CHARACTER_LOADOUTS_EMOTELAUGH, emoteLaughid);
                        toAdd.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_CHARACTER_LOADOUTS_EMOTESURPRISE, emoteSurpriseid);
                        toAdd.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_CHARACTER_LOADOUTS_CELEBRATION, celebrationid);
                        toAdd.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_CHARACTER_LOADOUTS_TUNE, tuneid);

                        loadouts.Add(loadoutId, toAdd);
                    }

                    public class FSFace : IFb_FirestoreMapStructure
                    {
                        public string skinColor;
                        public string noseId; 
                        public string browcolor; // One asset? Two? One and color? We will see
                        public string browid;
                        public Dictionary<string, object> eyesockets;
                        public string wrinklesId;
                        public string beautyMarkId; // TODO: Height and width? Scale and stretch?
                        public string makeupcolor;
                        public string makeupid;
                        public Dictionary<string, object> mouth;

                        public FSFace()
                        {
                            skinColor = null;
                            noseId = null;
                            beautyMarkId = null;
                            browcolor = null;
                            browid = null;
                            eyesockets = new FSEyesockets().ToDictionary();
                            wrinklesId = null;
                            makeupcolor = null;
                            makeupid = null;
                            mouth = new FSMouth().ToDictionary();
                        }

                        public FSFace(string skinColor, string noseId, string beautyMarkId, string browcolor, string browid, FSEyesockets eyesockets, string wrinklesId, string makeupcolor, string makeupid, FSMouth mouth)
                        {
                            this.skinColor = skinColor;
                            this.noseId = noseId;
                            this.beautyMarkId = beautyMarkId;
                            this.browcolor = browcolor;
                            this.browid = browid;
                            this.eyesockets = eyesockets.ToDictionary();
                            this.wrinklesId = wrinklesId;
                            this.makeupcolor = makeupcolor;
                            this.makeupid = makeupid;
                            this.mouth = mouth.ToDictionary();
                        }

                        public Dictionary<string, object> ToDictionary()
                        {
                            Dictionary<string, object> dic = new Dictionary<string, object>();

                            dic.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_CHARACTER_FACE_SKINCOLOR, skinColor);
                            dic.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_CHARACTER_FACE_NOSE, noseId);
                            dic.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_CHARACTER_FACE_BEAUTYMARKS, beautyMarkId);
                            dic.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_CHARACTER_FACE_BROWCOLOR, browcolor);
                            dic.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_CHARACTER_FACE_BROW, browid);
                            dic.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_CHARACTER_FACE_EYESOCKETS, eyesockets);
                            dic.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_CHARACTER_FACE_WRINKLES, wrinklesId);
                            dic.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_CHARACTER_FACE_MAKEUPCOLOR, makeupcolor);
                            dic.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_CHARACTER_FACE_MAKEUP, makeupid);
                            dic.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_CHARACTER_FACE_MOUTH, mouth);

                            return dic;
                        }

                        public class FSEyesockets : IFb_FirestoreMapStructure
                        {
                            public string eyecolor;
                            public string eyeid;
                            public double height;
                            public double rotation;
                            public double scale;
                            public double separation;
                            public double squash;

                            public FSEyesockets()
                            {
                                eyecolor = null;
                                eyeid = null;
                                height = 0d;
                                rotation = 0d;
                                scale = 0d;
                                separation = 0d;
                                squash = 0d;
                            }

                            public FSEyesockets(string eyecolor, string eyeid, double height, double rotation, double scale, double squash, double separation)
                            {
                                this.eyecolor = eyecolor;
                                this.eyeid = eyeid;
                                this.height = height;
                                this.rotation = rotation;
                                this.scale = scale;
                                this.separation = separation;
                                this.squash = squash;
                            }

                            public Dictionary<string, object> ToDictionary()
                            {
                                Dictionary<string, object> dic = new Dictionary<string, object>();

                                dic.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_CHARACTER_FACE_EYESOCKETS_EYECOLOR, eyecolor);
                                dic.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_CHARACTER_FACE_EYESOCKETS_EYE, eyeid);
                                dic.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_CHARACTER_FACE_EYESOCKETS_HEIGHT, height);
                                dic.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_CHARACTER_FACE_EYESOCKETS_ROTATION, rotation);
                                dic.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_CHARACTER_FACE_EYESOCKETS_SCALE, scale);
                                dic.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_CHARACTER_FACE_EYESOCKETS_SEPARATION, separation);
                                dic.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_CHARACTER_FACE_EYESOCKETS_SQUASH, squash);

                                return dic;
                            }
                        }

                        public class FSMouth : IFb_FirestoreMapStructure
                        {
                            public double height;
                            public string mouthcolor;
                            public string mouthid;
                            public double scale;
                            public double squash;

                            public FSMouth()
                            {
                                mouthcolor = null;
                                mouthid = null;
                                height = 0d;
                                squash = 0d;
                                scale = 0d;
                            }

                            public FSMouth(double height, string mouthcolor, string mouthid, double scale, double squash)
                            {
                                this.mouthcolor = mouthcolor;
                                this.mouthid = mouthid;
                                this.height = height;
                                this.squash = squash;
                                this.scale = scale;
                            }

                            public Dictionary<string, object> ToDictionary()
                            {
                                Dictionary<string, object> dic = new Dictionary<string, object>();

                                dic.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_CHARACTER_FACE_MOUTH_HEIGHT, height);
                                dic.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_CHARACTER_FACE_MOUTH_MOUTHCOLOR, mouthcolor);
                                dic.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_CHARACTER_FACE_MOUTH_MOUTHID, mouthid);
                                dic.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_CHARACTER_FACE_MOUTH_SCALE, scale);
                                dic.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_CHARACTER_FACE_MOUTH_SQUASH, squash);

                                return dic;
                            }
                        }
                    }
                }

                public class FSData : IFb_FirestoreMapStructure
                {
                    public object birthdate;
                    public string city;
                    public string country;
                    public Dictionary<string, object> currencies;
                    public string language;
                    public string nickname;
                    public string picture;
                    public object regdate;
                    public string wallpaperid;

                    public FSData()
                    {
                        birthdate = null;
                        city = null;
                        country = null;
                        currencies = new FSCurrencies().ToDictionary();
                        language = null;
                        nickname = null;
                        picture = null;
                        regdate = null;
                        wallpaperid = null;
                    }

                    public FSData(object birthdate, string city, string country, FSCurrencies currency, string language, string nickname, string picture, string wallpaperid)
                    {
                        this.birthdate = birthdate;
                        this.city = city;
                        this.country = country;
                        this.currencies = currency.ToDictionary();
                        this.language = language;
                        this.nickname = nickname;
                        this.picture = picture;
                        this.regdate = FieldValue.ServerTimestamp;
                        this.wallpaperid = wallpaperid;
                    }

                    public Dictionary<string, object> ToDictionary()
                    {
                        Dictionary<string, object> dictionary = new Dictionary<string, object>();

                        dictionary.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_DATA_BIRTHDATE, birthdate);
                        dictionary.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_DATA_CITY, city);
                        dictionary.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_DATA_COUNTRY, country);
                        dictionary.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_DATA_CURRENCIES, currencies);
                        dictionary.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_DATA_LANGUAGE, language);
                        dictionary.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_DATA_NICKNAME, nickname);
                        dictionary.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_DATA_PICTURE, picture);
                        dictionary.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_DATA_REGDATE, regdate);
                        dictionary.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_DATA_WALLPAPER, wallpaperid);

                        return dictionary;
                    }

                    public class FSCurrencies : IFb_FirestoreMapStructure
                    {
                        public int baseC;
                        public int premium;
                        public int lives;

                        public FSCurrencies()
                        {
                            baseC = 0;
                            premium = 0;
                            lives = 0;
                        }

                        public FSCurrencies(int baseC, int premium, int lives)
                        {
                            this.baseC = baseC;
                            this.premium = premium;
                            this.lives = lives;
                        }

                        public Dictionary<string, object> ToDictionary()
                        {
                            Dictionary<string, object> dictionary = new Dictionary<string, object>();

                            dictionary.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_DATA_CURRENCIES_BASE, baseC);
                            dictionary.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_DATA_CURRENCIES_PREMIUM, premium);
                            dictionary.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_DATA_CURRENCIES_LIVES, lives);

                            return dictionary;
                        }
                    }
                }

                public class FSStats : IFb_FirestoreMapStructure
                {
                    public Dictionary<string, object> gamestats;
                    public int experience;
                    public int level;
                    public int matchesplayed;
                    public int matcheswon;
                    public int mmr;

                    public FSStats()
                    {
                        experience = 0;
                        gamestats = new Dictionary<string, object>();
                        level = 0;
                        matchesplayed = 0;
                        matcheswon = 0;
                        mmr = 0;
                    }

                    public FSStats(int level, int matchesplayed, int matcheswon, int mmr, int experience)
                    {
                        this.level = level;
                        this.matchesplayed = matchesplayed;
                        this.matcheswon = matcheswon;
                        this.mmr = mmr;
                        this.experience = experience;
                    }

                    public static void AddGameStatsInt(Dictionary<string, object> output, string gamename, int matchesplayed, int matcheswon, int recordscore, object recorddate)
                    {
                        Dictionary<string, object> toAdd = new Dictionary<string, object>();

                        toAdd.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_STATS_GAMESTATS_MATCHESPLAYED, matchesplayed);
                        toAdd.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_STATS_GAMESTATS_MATCHESWON, matcheswon);
                        toAdd.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_STATS_GAMESTATS_RECORDSCORE, recordscore);
                        toAdd.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_STATS_GAMESTATS_RECORDDATE, recorddate);

                        if (output.ContainsKey(gamename))
                        {
                            output.Remove(gamename);
                        }
                        output.Add(gamename, toAdd);
                    }

                    public static void AddGameStatsFloat(Dictionary<string, object> output, string gamename, int matchesplayed, int matcheswon, float recordscore, object recorddate)
                    {
                        Dictionary<string, object> toAdd = new Dictionary<string, object>();

                        toAdd.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_STATS_GAMESTATS_MATCHESPLAYED, matchesplayed);
                        toAdd.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_STATS_GAMESTATS_MATCHESWON, matcheswon);
                        toAdd.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_STATS_GAMESTATS_RECORDSCORE, recordscore);
                        toAdd.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_STATS_GAMESTATS_RECORDDATE, recorddate);

                        if (output.ContainsKey(gamename))
                        {
                            output.Remove(gamename);
                        }
                        output.Add(gamename, toAdd);
                    }

                    public Dictionary<string, object> ToDictionary()
                    {
                        Dictionary<string, object> dictionary = new Dictionary<string, object>();

                        dictionary.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_STATS_GAMESTATS, gamestats);
                        dictionary.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_STATS_XP, experience);
                        dictionary.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_STATS_LEVEL, level);
                        dictionary.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_STATS_MATCHESPLAYED, matchesplayed);
                        dictionary.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_STATS_MATCHESWON, matcheswon);
                        dictionary.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_STATS_MMR, mmr);

                        return dictionary;
                    }
                }
            }

            /// <summary>
            /// The class in charge of the Sessions sub-collection of the Players collection structure.
            /// </summary>
            public class FSSessions : IFb_FirestoreMapStructure
            {
                public bool isRanked;
                public object date;
                public List<Dictionary<string, object>> matches;
                public List<Dictionary<string, object>> players;

                public FSSessions()
                {
                    isRanked = false;
                    date = FieldValue.ServerTimestamp;
                    matches = new List<Dictionary<string, object>>();//FSMatches().ToDictionary();
                    players = new List<Dictionary<string, object>>();//FSSessionPlayers().ToDictionary();
                }

                public FSSessions(bool isRanked, object date, List<Dictionary<string, object>> matches, List<Dictionary<string, object>> players)
                {
                    this.isRanked = isRanked;
                    this.date = date;
                    this.matches = matches;
                    this.players = players;
                }

                public void AddPlayer(string uid, int mmr, string name)
                {
                    Dictionary<string, object> plyr = new Dictionary<string, object>();

                    plyr.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_SESSIONS_PLAYERS_ID, uid);
                    plyr.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_SESSIONS_PLAYERS_MMR, mmr);
                    plyr.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_SESSIONS_PLAYERS_NAME, name);

                    players.Add(plyr);
                }

                public void AddMatch(string gameName, List<FSSessionMatchPlayer> players)
                {
                    Dictionary<string, object> mtch = new Dictionary<string, object>();
                    List<Dictionary<string, object>> plyrsDic = new List<Dictionary<string, object>>();

                    foreach(FSSessionMatchPlayer p in players)
                    {
                        plyrsDic.Add(p.ToDictionary());
                    }

                    mtch.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_SESSIONS_MATCHES_GAMENAME, gameName);
                    mtch.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_SESSIONS_MATCHES_PLAYERS, plyrsDic);

                    matches.Add(mtch);
                }

                public Dictionary<string, object> ToDictionary()
                {
                    Dictionary<string, object> dic = new Dictionary<string, object>();

                    dic.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_SESSIONS_ISRANKED, isRanked);
                    dic.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_SESSIONS_DATE , date);
                    dic.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_SESSIONS_MATCHES, matches);
                    dic.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_SESSIONS_PLAYERS, players);

                    return dic;
                }

                public class FSSessionMatchPlayer : IFb_FirestoreMapStructure
                {
                    public string nickname;
                    public int score;
                    public string uid;

                    public FSSessionMatchPlayer()
                    {
                        nickname = null;
                        score = 0;
                        uid = null;
                    }

                    public FSSessionMatchPlayer(string nickname, string uid, int score)
                    {
                        this.nickname = nickname;
                        this.uid = uid;
                        this.score = score;
                    }

                    public Dictionary<string, object> ToDictionary()
                    {
                        Dictionary<string, object> dic = new Dictionary<string, object>();

                        dic.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_SESSIONS_MATCHES_PLAYERS_NICKNAME, nickname);
                        dic.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_SESSIONS_MATCHES_PLAYERS_SCORE, score);
                        dic.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_SESSIONS_MATCHES_PLAYERS_UID, uid);

                        return dic;
                    }
                }
            }

            /// <summary>
            /// The class in charge of the Goals collection structure.
            /// Might not use it much, since Goals are only added by developers.
            /// </summary>
            public class FSGoal : IFb_FirestoreMapStructure
            {
                public string description;
                public string name;
                public int scoreneeded;

                public FSGoal()
                {
                    description = null;
                    name = null;
                    scoreneeded = 0;
                }

                public FSGoal(string description, string name, int scoreneeded)
                {
                    this.description = description;
                    this.name = name;
                    this.scoreneeded = scoreneeded;
                }

                public Dictionary<string, object> ToDictionary()
                {
                    Dictionary<string, object> dic = new Dictionary<string, object>();

                    dic.Add(Fb_Constants.FIRESTORE_KEY_GOALS_DESCRIPTION, description);
                    dic.Add(Fb_Constants.FIRESTORE_KEY_GOALS_NAME, name);
                    dic.Add(Fb_Constants.FIRESTORE_KEY_GOALS_SCORENEEDED, scoreneeded);

                    return dic;
                }
            }

            /// <summary>
            /// The class in charge of the Teams collection structure.
            /// </summary>
            public class FSTeam : IFb_FirestoreMapStructure
            {
                public string name;
                public List<Dictionary<string, object>> members;
                public string tagline;

                public FSTeam()
                {
                    name = null;
                    members = new List<Dictionary<string, object>>();
                    tagline = null;
                }

                public FSTeam(string name, List<Dictionary<string, object>> members, string tagline)
                {
                    this.name = name;
                    this.members = members;
                    this.tagline = tagline;
                }

                public void AddMember(FSTeamMembers member)
                {
                    members.Add(member.ToDictionary());
                }

                public Dictionary<string, object> ToDictionary()
                {
                    Dictionary<string, object> dic = new Dictionary<string, object>();

                    dic.Add(Fb_Constants.FIRESTORE_KEY_TEAM_NAME, name);
                    dic.Add(Fb_Constants.FIRESTORE_KEY_TEAM_MEMBERS, members);
                    dic.Add(Fb_Constants.FIRESTORE_KEY_TEAM_TAGLINE, tagline);

                    return dic;
                }

                public class FSTeamMembers : IFb_FirestoreMapStructure
                {
                    public string name;
                    public string uid;
                    public int mmr;
                    public bool isadmin;

                    public FSTeamMembers()
                    {
                        name = null;
                        uid = null;
                        mmr = 0;
                        isadmin = false;
                    }

                    public FSTeamMembers(string name, string uid, int mmr, bool isadmin)
                    {
                        this.name = name;
                        this.uid = uid;
                        this.mmr = mmr;
                        this.isadmin = isadmin;
                    }

                    public Dictionary<string, object> ToDictionary()
                    {
                        Dictionary<string, object> dic = new Dictionary<string, object>();

                        dic.Add(Fb_Constants.FIRESTORE_KEY_TEAM_MEMBER_NAME, name);
                        dic.Add(Fb_Constants.FIRESTORE_KEY_TEAM_MEMBER_UID, uid);
                        dic.Add(Fb_Constants.FIRESTORE_KEY_TEAM_MEMBER_MMR, mmr);
                        dic.Add(Fb_Constants.FIRESTORE_KEY_TEAM_MEMBER_ISADMIN, isadmin);

                        return dic;
                    }
                }
            }

            /// <summary>
            /// The class in charge of the Asset collection structure.
            /// Might not use it much, since Assets are only added by developers.
            /// </summary>
            public class FSAsset : IFb_FirestoreMapStructure
            {
                public string achievement;
                public int baseprice;
                public int premiumprice;
                public string name;
                public int type;
                public int rarity;
                public List<string> variations;

                public FSAsset()
                {
                    achievement = null;
                    baseprice = 0;
                    premiumprice = 0;
                    name = null;
                    type = 0;
                    rarity = 0;
                    variations = null;
                }

                public FSAsset(string achievement, int baseprice, int premiumprice, string name, int type, int rarity, List<string> variations)
                {
                    this.achievement = achievement;
                    this.baseprice = baseprice;
                    this.premiumprice = premiumprice;
                    this.name = name;
                    this.type = type;
                    this.rarity = rarity;
                    this.variations = variations;
                }

                public Dictionary<string, object> ToDictionary()
                {
                    Dictionary<string, object> dic = new Dictionary<string, object>();

                    dic.Add(Fb_Constants.FIRESTORE_KEY_ASSETS_ACHIEVEMENT, achievement);
                    dic.Add(Fb_Constants.FIRESTORE_KEY_ASSETS_BASEPRICE, baseprice);
                    dic.Add(Fb_Constants.FIRESTORE_KEY_ASSETS_PREMIUMPRICE, premiumprice);
                    dic.Add(Fb_Constants.FIRESTORE_KEY_ASSETS_NAME, name);
                    dic.Add(Fb_Constants.FIRESTORE_KEY_ASSETS_TYPE, type);
                    dic.Add(Fb_Constants.FIRESTORE_KEY_ASSETS_RARITY, rarity);
                    dic.Add(Fb_Constants.FIRESTORE_KEY_ASSETS_VARIATIONS, variations);

                    return dic;
                }
            }

            /// <summary>
            /// The class in charge of the Players-Social collection structure.
            /// </summary>
            public class FSPlayerSocial : IFb_FirestoreMapStructure
            {
                public List<Dictionary<string, object>> friends;
                public List<Dictionary<string, object>> friendrequests;
                public List<Dictionary<string, object>> teams;
                public List<Dictionary<string, object>> teamrequests;

                public FSPlayerSocial()
                {
                    friends = new List<Dictionary<string, object>>();
                    friendrequests = new List<Dictionary<string, object>>();
                    teams = new List<Dictionary<string, object>>();
                    teamrequests = new List<Dictionary<string, object>>();
                }

                public FSPlayerSocial(List<Dictionary<string, object>> teams, List<Dictionary<string, object>> teamrequests, List<Dictionary<string, object>> friends, List<Dictionary<string, object>> friendrequests)
                {
                    this.friends = friends;
                    this.friendrequests = friendrequests;
                    this.teams = teams;
                    this.teamrequests = teamrequests;
                }

                public Dictionary<string, object> ToDictionary()
                {
                    Dictionary<string, object> dictionary = new Dictionary<string, object>();

                    dictionary.Add(Fb_Constants.FIRESTORE_KEY_PLAYERSOCIAL_TEAMS, teams);
                    dictionary.Add(Fb_Constants.FIRESTORE_KEY_PLAYERSOCIAL_TEAMREQUESTS, teamrequests);
                    dictionary.Add(Fb_Constants.FIRESTORE_KEY_PLAYERSOCIAL_FRIENDS, friends);
                    dictionary.Add(Fb_Constants.FIRESTORE_KEY_PLAYERSOCIAL_FRIENDREQUESTS, friendrequests);

                    return dictionary;
                }

                public class FSPlayerSocialTeam : IFb_FirestoreMapStructure
                {
                    public string name;
                    public string teamid;
                    public string imageid;

                    public FSPlayerSocialTeam()
                    {
                        name = null;
                        teamid = null;
                        imageid = null;
                    }

                    public FSPlayerSocialTeam(string name, string teamid, string imageid)
                    {
                        this.name = name;
                        this.teamid = teamid;
                        this.imageid = imageid;
                    }

                    public Dictionary<string, object> ToDictionary()
                    {
                        Dictionary<string, object> dictionary = new Dictionary<string, object>();

                        dictionary.Add(Fb_Constants.FIRESTORE_KEY_PLAYERSOCIAL_TEAMS_TEAMID, teamid);
                        dictionary.Add(Fb_Constants.FIRESTORE_KEY_PLAYERSOCIAL_TEAMS_NAME, name);
                        dictionary.Add(Fb_Constants.FIRESTORE_KEY_PLAYERSOCIAL_TEAMS_IMAGEID, imageid);

                        return dictionary;
                    }
                }

                public class FSPlayerSocialFriend : IFb_FirestoreMapStructure
                {
                    public string name;
                    public string uid;
                    public string imageid;

                    public FSPlayerSocialFriend()
                    {
                        name = null;
                        uid = null;
                        imageid = null;
                    }

                    public FSPlayerSocialFriend(string name, string uid, string imageid)
                    {
                        this.name = name;
                        this.uid = uid;
                        this.imageid = imageid;
                    }

                    public Dictionary<string, object> ToDictionary()
                    {
                        Dictionary<string, object> dictionary = new Dictionary<string, object>();

                        dictionary.Add(Fb_Constants.FIRESTORE_KEY_PLAYERSOCIAL_FRIENDS_UID, uid);
                        dictionary.Add(Fb_Constants.FIRESTORE_KEY_PLAYERSOCIAL_FRIENDS_NAME, name);
                        dictionary.Add(Fb_Constants.FIRESTORE_KEY_PLAYERSOCIAL_FRIENDS_IMAGEID, imageid);

                        return dictionary;
                    }
                }
            }

            /// <summary>
            /// The class in charge of the Group Chats sub-collection of the Players-Social collection structure.
            /// </summary>
            public class FSChatsGroup : IFb_FirestoreMapStructure
            {
                public List<Dictionary<string, object>> members;
                public List<Dictionary<string, object>> messages;

                public FSChatsGroup()
                {
                    members = new List<Dictionary<string, object>>();
                    messages = new List<Dictionary<string, object>>();
                }

                public FSChatsGroup(List<Dictionary<string, object>> members, List<Dictionary<string, object>> messages)
                {
                    this.members = members;
                    this.messages = messages;
                }

                public Dictionary<string, object> ToDictionary()
                {
                    Dictionary<string, object> dictionary = new Dictionary<string, object>();

                    dictionary.Add(Fb_Constants.FIRESTORE_KEY_PLAYERSOCIAL_CHATSGROUP_MEMBERS, members);
                    dictionary.Add(Fb_Constants.FIRESTORE_KEY_PLAYERSOCIAL_CHATSGROUP_MESSAGES, messages);

                    return dictionary;
                }

                public class FSChatsGroupMembers : IFb_FirestoreMapStructure
                {
                    public string uid;
                    public string name;

                    public FSChatsGroupMembers()
                    {
                        uid = null;
                        name = null;
                    }

                    public FSChatsGroupMembers(string uid, string name)
                    {
                        this.uid = uid;
                        this.name = name;
                    }

                    public Dictionary<string, object> ToDictionary()
                    {
                        Dictionary<string, object> dictionary = new Dictionary<string, object>();

                        dictionary.Add(Fb_Constants.FIRESTORE_KEY_PLAYERSOCIAL_CHATSGROUP_MEMBERS_NAME, name);
                        dictionary.Add(Fb_Constants.FIRESTORE_KEY_PLAYERSOCIAL_CHATSGROUP_MEMBERS_UID, uid);

                        return dictionary;
                    }
                }

                public class FSChatsGroupMessages : IFb_FirestoreMapStructure
                {
                    public string messageid;
                    public string senderid;
                    public string message;
                    public object timestamp;

                    public FSChatsGroupMessages()
                    {
                        messageid = null;
                        senderid = null;
                        message = null;
                        timestamp = FieldValue.ServerTimestamp;
                    }

                    public FSChatsGroupMessages(string messageid, string senderid, string message)
                    {
                        this.messageid = messageid;
                        this.senderid = senderid;
                        this.message = message;
                        this.timestamp = FieldValue.ServerTimestamp;
                    }

                    public Dictionary<string, object> ToDictionary()
                    {
                        Dictionary<string, object> dictionary = new Dictionary<string, object>();

                        dictionary.Add(Fb_Constants.FIRESTORE_KEY_PLAYERSOCIAL_CHATSGROUP_MESSAGES_ID, messageid);
                        dictionary.Add(Fb_Constants.FIRESTORE_KEY_PLAYERSOCIAL_CHATSGROUP_MESSAGES_MESSAGE, message);
                        dictionary.Add(Fb_Constants.FIRESTORE_KEY_PLAYERSOCIAL_CHATSGROUP_MESSAGES_TIMESTAMP, timestamp);
                        dictionary.Add(Fb_Constants.FIRESTORE_KEY_PLAYERSOCIAL_CHATSGROUP_MESSAGES_SENDERID, senderid);

                        return dictionary;
                    }
                }
            }

            /// <summary>
            /// The class in charge of the Singular Chats sub-collection of the Players-Social collection structure.
            /// </summary>
            public class FSChatsSingular : IFb_FirestoreMapStructure
            {
                public List<Dictionary<string, object>> messages;

                public FSChatsSingular()
                {
                    messages = new List<Dictionary<string, object>>();
                }

                public FSChatsSingular(List<Dictionary<string, object>> messages)
                {
                    this.messages = messages;
                }

                public void AddMessage(FSChatsSingularMessage message)
                {
                    messages.Add(message.ToDictionary());
                }

                public Dictionary<string, object> ToDictionary()
                {
                    Dictionary<string, object> dictionary = new Dictionary<string, object>();

                    dictionary.Add(Fb_Constants.FIRESTORE_KEY_PLAYERSOCIAL_CHATSSING_MESSAGES , messages);

                    return dictionary;
                }

                public class FSChatsSingularMessage : IFb_FirestoreMapStructure
                {
                    public string message;
                    public object timestamp;
                    public bool mine;

                    public FSChatsSingularMessage()
                    {
                        message = null;
                        timestamp = FieldValue.ServerTimestamp;
                        mine = false;
                    }

                    public FSChatsSingularMessage(string message, bool mine)
                    {
                        this.message = message;
                        timestamp = FieldValue.ServerTimestamp;
                        this.mine = mine;
                    }

                    public Dictionary<string, object> ToDictionary()
                    {
                        Dictionary<string, object> dictionary = new Dictionary<string, object>();

                        dictionary.Add(Fb_Constants.FIRESTORE_KEY_PLAYERSOCIAL_CHATSSING_MESSAGES_MESSAGE, message);
                        dictionary.Add(Fb_Constants.FIRESTORE_KEY_PLAYERSOCIAL_CHATSSING_MESSAGES_TIMESTAMP, timestamp);
                        dictionary.Add(Fb_Constants.FIRESTORE_KEY_PLAYERSOCIAL_CHATSSING_MESSAGES_MINE, mine);

                        return dictionary;
                    }
                }
            }

            public class TestCollection : IFb_FirestoreMapStructure
            {
                public List<string> testList;
                public string testString;
                public int testInt;
                public float testFloat;
                public Dictionary<string, object> testDict;
                public List<Dictionary<string, object>> testMapList;
                public object testTimestamp;
                //public DocumentReference testRef;

                public TestCollection()
                {
                    testList = null;
                    testString = null;
                    testInt = 0;
                    testFloat = 0f;
                    testDict = new Dictionary<string, object>();
                    testMapList = new List<Dictionary<string, object>>();
                    testTimestamp = null;
                    //testRef = refe;
                }

                public TestCollection(List<string> l, string s, int i, float f, Dictionary<string, object> d, List<Dictionary<string, object>> ml, object tim)
                {
                    testList = l;
                    testString = s;
                    testInt = i;
                    testFloat = f;
                    testDict = d;
                    testMapList = ml;
                    testTimestamp = tim;
                    //testRef = refe;
                }

                public Dictionary<string, object> ToDictionary()
                {
                    Dictionary<string, object> dictionary = new Dictionary<string, object>();

                    dictionary.Add("testlist", testList);
                    dictionary.Add("teststring", testString);
                    dictionary.Add("testint", testInt);
                    dictionary.Add("testfloat", testFloat);
                    dictionary.Add("testdict", testDict);
                    dictionary.Add("testmaplist", testMapList);
                    dictionary.Add("testtimestamp", testTimestamp);
                    //dictionary.Add("testreference", testRef);

                    return dictionary;
                }
            }
        }
    }
}


