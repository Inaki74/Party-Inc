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
                public List<string> assets;
                public Dictionary<string, object> character;
                public Dictionary<string, object> data;
                public Dictionary<string, object> stats;
                public Dictionary<string, object> goals;

                public FSPlayer()
                {
                    achievements = new List<string>();
                    assets = new List<string>();
                    character = new FSCharacter().ToDictionary();
                    data = new FSData().ToDictionary();
                    stats = new FSStats().ToDictionary();
                    goals = new Dictionary<string, object>();
                }

                public FSPlayer(List<string> achievements, List<string> assets, FSCharacter character, FSData data, FSStats stats)
                {
                    this.achievements = achievements;
                    this.assets = assets;
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
                    public int currentoutfit;
                    public Dictionary<string, object> face;
                    public Dictionary<string, object> outfits;

                    public FSCharacter()
                    {
                        currentoutfit = 0;
                        face = new FSFace().ToDictionary();
                        outfits = new Dictionary<string, object>();
                    }

                    public FSCharacter(int currentoutfit, FSFace face)
                    {
                        this.currentoutfit = currentoutfit;
                        this.face = face.ToDictionary();
                        this.outfits = new Dictionary<string, object>();
                    }

                    public Dictionary<string, object> ToDictionary()
                    {
                        Dictionary<string, object> dic = new Dictionary<string, object>();

                        dic.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_CHARACTER_CURRENTOUTFIT, currentoutfit);
                        dic.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_CHARACTER_FACE, face);
                        dic.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_CHARACTER_OUTFITS, outfits);

                        return dic;
                    }

                    public void AddOutfit(string outfitId,
                                              string backid, string earid, string facialhairid, string glassid,
                                              string hairid, string jawid, string pantsid, string shirtid, string socksid
                            )
                    {
                        Dictionary<string, object> toAdd = new Dictionary<string, object>();

                        toAdd.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_CHARACTER_OUTFITS_BACK, backid);
                        toAdd.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_CHARACTER_OUTFITS_EAR, earid);
                        toAdd.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_CHARACTER_OUTFITS_FACIALHAIR, facialhairid);
                        toAdd.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_CHARACTER_OUTFITS_GLASS, glassid);
                        toAdd.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_CHARACTER_OUTFITS_HAIR, hairid);
                        toAdd.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_CHARACTER_OUTFITS_JAW, jawid);
                        toAdd.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_CHARACTER_OUTFITS_PANTS, pantsid);
                        toAdd.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_CHARACTER_OUTFITS_SHIRT, shirtid);
                        toAdd.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_CHARACTER_OUTFITS_SOCKS, socksid);
                        //etc

                        outfits.Add(outfitId, toAdd);
                    }

                    public class FSFace : IFb_FirestoreMapStructure
                    {
                        public string browcolor;
                        public string browid;
                        public Dictionary<string, object> eyesockets;
                        public string facemarkid;
                        public string makeupcolor;
                        public string makeupid;
                        public Dictionary<string, object> mouth;

                        public FSFace()
                        {
                            browcolor = null;
                            browid = null;
                            eyesockets = new FSEyesockets().ToDictionary();
                            facemarkid = null;
                            makeupcolor = null;
                            makeupid = null;
                            mouth = new FSMouth().ToDictionary();
                        }

                        public FSFace(string browcolor, string browid, FSEyesockets eyesockets, string facemarkid, string makeupcolor, string makeupid, FSMouth mouth)
                        {
                            this.browcolor = browcolor;
                            this.browid = browid;
                            this.eyesockets = eyesockets.ToDictionary();
                            this.facemarkid = facemarkid;
                            this.makeupcolor = makeupcolor;
                            this.makeupid = makeupid;
                            this.mouth = mouth.ToDictionary();
                        }

                        public Dictionary<string, object> ToDictionary()
                        {
                            Dictionary<string, object> dic = new Dictionary<string, object>();

                            dic.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_CHARACTER_FACE_BROWCOLOR, browcolor);
                            dic.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_CHARACTER_FACE_BROW, browid);
                            dic.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_CHARACTER_FACE_EYESOCKETS, eyesockets);
                            dic.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_CHARACTER_FACE_FACEMARK, facemarkid);
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

                    public void AddGameStats(string gamename, int matchesplayed, int matcheswon, int recordscore, object recorddate)
                    {
                        Dictionary<string, object> toAdd = new Dictionary<string, object>();

                        toAdd.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_STATS_GAMESTATS_MATCHESPLAYED, matchesplayed);
                        toAdd.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_STATS_GAMESTATS_MATCHESWON, matcheswon);
                        toAdd.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_STATS_GAMESTATS_RECORDSCORE, recordscore);
                        toAdd.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_STATS_GAMESTATS_RECORDDATE, recorddate);

                        gamestats.Add(gamename, toAdd);
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
            public class FSSessions
            {
                public bool isRanked;
                public object date;
                public List<Dictionary<string, object>> matches;
                public List<Dictionary<string, object>> players;

                public FSSessions()
                {
                    this.isRanked = false;
                    this.date = FieldValue.ServerTimestamp;
                    this.matches = new List<Dictionary<string, object>>();//FSMatches().ToDictionary();
                    this.players = new List<Dictionary<string, object>>();//FSSessionPlayers().ToDictionary();
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

                public class FSSessionMatchPlayer
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

            public class FSGoal
            {

            }

            public class FSTeam
            {

            }

            public class FSAsset
            {

            }

            public class FSPlayerSocial
            {

            }

            public class FSChatsGroup
            {

            }

            public class FSChatsSingular
            {

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


