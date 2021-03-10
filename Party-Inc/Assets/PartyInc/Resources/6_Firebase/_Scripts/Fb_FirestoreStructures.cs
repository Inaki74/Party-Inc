using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Firestore;

namespace PartyInc
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
            public List<string> assets;
            public Dictionary<string, object> character;
            public Dictionary<string, object> data;
            public Dictionary<string, object> stats;
            public Dictionary<string, object> tasks;

            public FSPlayer()
            {
                assets = new List<string>();
                character = new Dictionary<string, object>();
                data = new Dictionary<string, object>();
                stats = new Dictionary<string, object>();
                tasks = new Dictionary<string, object>();
            }

            public void AddTask(string taskid, double progress)
            {
                Dictionary<string, object> task = new Dictionary<string, object>();

                task.Add(taskid, progress);

                tasks.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_TASKS, progress);
            }

            public Dictionary<string, object> ToDictionary()
            {
                Dictionary<string, object> dic = new Dictionary<string, object>();

                dic.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_ASSETS, assets);
                dic.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_CHARACTER, character);
                dic.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_DATA, data);
                dic.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_STATS, stats);
                dic.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_TASKS, tasks);

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
                    face = new Dictionary<string, object>();
                    outfits = new Dictionary<string, object>();
                }

                public FSCharacter(int co, FSFace f)
                {
                    currentoutfit = co;
                    face = f.ToDictionary();
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
                        browcolor = "";
                        browid = "";
                        eyesockets = new Dictionary<string, object>();
                        facemarkid = "";
                        makeupcolor = "";
                        makeupid = "";
                        mouth = new Dictionary<string, object>();
                    }

                    public FSFace(string bcol, string bid, FSEyesockets eyes, string fid, string mcol, string mid, FSMouth m)
                    {
                        browcolor = bcol;
                        browid = bid;
                        eyesockets = eyes.ToDictionary();
                        facemarkid = fid;
                        makeupcolor = mcol;
                        makeupid = mid;
                        mouth = m.ToDictionary();
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
                            eyecolor = "";
                            eyeid = "";
                            height = 0d;
                            rotation = 0d;
                            scale = 0d;
                            separation = 0d;
                            squash = 0d;
                        }

                        public FSEyesockets(string eyecol, string eyei, double h, double r, double sc, double sq, double sep)
                        {
                            eyecolor = eyecol;
                            eyeid = eyei;
                            height = h;
                            rotation = r;
                            scale = sc;
                            separation = sep;
                            squash = sq;
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
                            mouthcolor = "";
                            mouthid = "";
                            height = 0d;
                            squash = 0d;
                            scale = 0d;
                        }

                        public FSMouth(double h, string mcol, string mid, double sc, double sq)
                        {
                            mouthcolor = mcol;
                            mouthid = mid;
                            height = h;
                            squash = sq;
                            scale = sc;
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
                public string email;
                public string language;
                public string nickname;
                public string picture;
                public object regdate;
                public string wallpaperid;

                public FSData()
                {
                    birthdate = FieldValue.ServerTimestamp;
                    city = "";
                    country = "";
                    currencies = new Dictionary<string, object>();
                    email = "";
                    language = "";
                    nickname = "";
                    picture = "";
                    regdate = FieldValue.ServerTimestamp;
                    wallpaperid = "";
                }

                public FSData(object bdate, string cit, string count, FSCurrencies curr, string em, string lang, string nick, string pic, string wid)
                {
                    birthdate = bdate;
                    city = cit;
                    country = count;
                    currencies = curr.ToDictionary();
                    email = em;
                    language = lang;
                    nickname = nick;
                    picture = pic;
                    regdate = FieldValue.ServerTimestamp;
                    wallpaperid = wid;
                }

                public Dictionary<string, object> ToDictionary()
                {
                    Dictionary<string, object> dictionary = new Dictionary<string, object>();

                    dictionary.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_DATA_BIRTHDATE, birthdate);
                    dictionary.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_DATA_CITY, city);
                    dictionary.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_DATA_COUNTRY, country);
                    dictionary.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_DATA_CURRENCIES, currencies);
                    dictionary.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_DATA_EMAIL, email);
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

                    public FSCurrencies(int b, int p, int l)
                    {
                        baseC = b;
                        premium = p;
                        lives = l;
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
                public int level;
                public int matchesplayed;
                public int matcheswon;
                public int mmr;

                public FSStats()
                {
                    gamestats = new Dictionary<string, object>();
                    level = 0;
                    matchesplayed = 0;
                    matcheswon = 0;
                    mmr = 0;
                }

                public FSStats(int l, int mp, int mw, int mm)
                {
                    level = l;
                    matchesplayed = mp;
                    matcheswon = mw;
                    mmr = mm;
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
                    dictionary.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_STATS_LEVEL, level);
                    dictionary.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_STATS_MATCHESPLAYED, matchesplayed);
                    dictionary.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_STATS_MATCHESWON, matcheswon);
                    dictionary.Add(Fb_Constants.FIRESTORE_KEY_PLAYER_STATS_MMR, mmr);

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
                testList = new List<string>();
                testString = "";
                testInt = 0;
                testFloat = 0f;
                testDict = new Dictionary<string, object>();
                testMapList = new List<Dictionary<string, object>>();
                testTimestamp = new Timestamp();
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


