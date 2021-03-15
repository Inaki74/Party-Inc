using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PartyInc
{
    namespace PartyFirebase.Firestore
    {
        public static class Fb_Constants
        {
            // Keys and collections must be the EXACT name as in Firestore.

            public const string FIRESTORE_COLLECTION_TEST = "test-collection";

            public const string FIRESTORE_KEY_PLAYERS = "players";

            // PLAYER DATA KEYS
            public const string FIRESTORE_KEY_PLAYER_DATA = "data";
            public const string FIRESTORE_KEY_PLAYER_DATA_BIRTHDATE = "birthdate";
            public const string FIRESTORE_KEY_PLAYER_DATA_CITY = "city";
            public const string FIRESTORE_KEY_PLAYER_DATA_COUNTRY = "country";
            // PLAYER CURRENCIES KEYS
            public const string FIRESTORE_KEY_PLAYER_DATA_CURRENCIES = "currencies";
            public const string FIRESTORE_KEY_PLAYER_DATA_CURRENCIES_BASE = "base";
            public const string FIRESTORE_KEY_PLAYER_DATA_CURRENCIES_PREMIUM = "premium";
            public const string FIRESTORE_KEY_PLAYER_DATA_CURRENCIES_LIVES = "lives";
            public const string FIRESTORE_KEY_PLAYER_DATA_EMAIL = "email";
            public const string FIRESTORE_KEY_PLAYER_DATA_LANGUAGE = "language";
            public const string FIRESTORE_KEY_PLAYER_DATA_NICKNAME = "nickname";
            public const string FIRESTORE_KEY_PLAYER_DATA_PICTURE = "picture";
            public const string FIRESTORE_KEY_PLAYER_DATA_REGDATE = "regdate";
            public const string FIRESTORE_KEY_PLAYER_DATA_WALLPAPER = "wallpaper";

            // PLAYER STATS KEYS
            public const string FIRESTORE_KEY_PLAYER_STATS = "stats";
            // PLAYER GAMESTATS KEYS
            public const string FIRESTORE_KEY_PLAYER_STATS_GAMESTATS = "gamestats";
            public const string FIRESTORE_KEY_PLAYER_STATS_GAMESTATS_MATCHESPLAYED = "matchesplayed";
            public const string FIRESTORE_KEY_PLAYER_STATS_GAMESTATS_MATCHESWON = "matcheswon";
            public const string FIRESTORE_KEY_PLAYER_STATS_GAMESTATS_RECORDSCORE = "recordscore";
            public const string FIRESTORE_KEY_PLAYER_STATS_GAMESTATS_RECORDDATE = "recorddate";
            public const string FIRESTORE_KEY_PLAYER_STATS_XP = "experience";
            public const string FIRESTORE_KEY_PLAYER_STATS_LEVEL = "level";
            public const string FIRESTORE_KEY_PLAYER_STATS_MATCHESPLAYED = "matchesplayed";
            public const string FIRESTORE_KEY_PLAYER_STATS_MATCHESWON = "matcheswon";
            public const string FIRESTORE_KEY_PLAYER_STATS_MMR = "mmr";

            // PLAYER CHARACTER KEYS
            public const string FIRESTORE_KEY_PLAYER_CHARACTER = "character";
            public const string FIRESTORE_KEY_PLAYER_CHARACTER_CURRENTOUTFIT = "currentoutfit";
            // PLAYER FACE KEYS
            public const string FIRESTORE_KEY_PLAYER_CHARACTER_FACE = "face";
            public const string FIRESTORE_KEY_PLAYER_CHARACTER_FACE_BROWCOLOR = "browcolor";
            public const string FIRESTORE_KEY_PLAYER_CHARACTER_FACE_BROW = "browid";
            // PLAYER EYESOCKET KEYS
            public const string FIRESTORE_KEY_PLAYER_CHARACTER_FACE_EYESOCKETS = "eyesockets";
            public const string FIRESTORE_KEY_PLAYER_CHARACTER_FACE_EYESOCKETS_EYECOLOR = "eyecolor";
            public const string FIRESTORE_KEY_PLAYER_CHARACTER_FACE_EYESOCKETS_EYE = "eyeid";
            public const string FIRESTORE_KEY_PLAYER_CHARACTER_FACE_EYESOCKETS_HEIGHT = "height";
            public const string FIRESTORE_KEY_PLAYER_CHARACTER_FACE_EYESOCKETS_ROTATION = "rotation";
            public const string FIRESTORE_KEY_PLAYER_CHARACTER_FACE_EYESOCKETS_SCALE = "scale";
            public const string FIRESTORE_KEY_PLAYER_CHARACTER_FACE_EYESOCKETS_SQUASH = "squash";
            public const string FIRESTORE_KEY_PLAYER_CHARACTER_FACE_EYESOCKETS_SEPARATION = "separation";
            public const string FIRESTORE_KEY_PLAYER_CHARACTER_FACE_FACEMARK = "facemarkid";
            public const string FIRESTORE_KEY_PLAYER_CHARACTER_FACE_MAKEUPCOLOR = "makeupcolor";
            public const string FIRESTORE_KEY_PLAYER_CHARACTER_FACE_MAKEUP = "makeupid";
            // PLAYER MOUTH KEYS
            public const string FIRESTORE_KEY_PLAYER_CHARACTER_FACE_MOUTH = "mouth";
            public const string FIRESTORE_KEY_PLAYER_CHARACTER_FACE_MOUTH_HEIGHT = "height";
            public const string FIRESTORE_KEY_PLAYER_CHARACTER_FACE_MOUTH_MOUTHCOLOR = "mouthcolor";
            public const string FIRESTORE_KEY_PLAYER_CHARACTER_FACE_MOUTH_MOUTHID = "mouthid";
            public const string FIRESTORE_KEY_PLAYER_CHARACTER_FACE_MOUTH_SCALE = "scale";
            public const string FIRESTORE_KEY_PLAYER_CHARACTER_FACE_MOUTH_SQUASH = "squash";

            // PLAYER OUTFIT KEYS
            public const string FIRESTORE_KEY_PLAYER_CHARACTER_OUTFITS = "outfits";
            public const string FIRESTORE_KEY_PLAYER_CHARACTER_OUTFITS_BACK = "backpieceid";
            public const string FIRESTORE_KEY_PLAYER_CHARACTER_OUTFITS_EAR = "earid";
            public const string FIRESTORE_KEY_PLAYER_CHARACTER_OUTFITS_FACIALHAIR = "facialhairid";
            public const string FIRESTORE_KEY_PLAYER_CHARACTER_OUTFITS_GLASS = "glassid";
            public const string FIRESTORE_KEY_PLAYER_CHARACTER_OUTFITS_HAIR = "hairid";
            public const string FIRESTORE_KEY_PLAYER_CHARACTER_OUTFITS_JAW = "jawid";
            public const string FIRESTORE_KEY_PLAYER_CHARACTER_OUTFITS_SHIRT = "shirtid";
            public const string FIRESTORE_KEY_PLAYER_CHARACTER_OUTFITS_PANTS = "pantsid";
            public const string FIRESTORE_KEY_PLAYER_CHARACTER_OUTFITS_SOCKS = "socksid";

            // PLAYER ASSETS KEY
            public const string FIRESTORE_KEY_PLAYER_ASSETS = "assets";

            // PLAYER TASKS KEY
            public const string FIRESTORE_KEY_PLAYER_TASKS = "goals";

            // PLAYER SESSIONS
            public const string FIRESTORE_KEY_PLAYER_SESSIONS = "sessions";
            public const string FIRESTORE_KEY_PLAYER_SESSIONS_DATE = "date";
            public const string FIRESTORE_KEY_PLAYER_SESSIONS_ISRANKED = "isranked";
            public const string FIRESTORE_KEY_PLAYER_SESSIONS_PLAYERS = "players";
            public const string FIRESTORE_KEY_PLAYER_SESSIONS_PLAYERS_ID = "uid";
            public const string FIRESTORE_KEY_PLAYER_SESSIONS_PLAYERS_MMR = "mmr";
            public const string FIRESTORE_KEY_PLAYER_SESSIONS_PLAYERS_NAME = "name";
            public const string FIRESTORE_KEY_PLAYER_SESSIONS_MATCHES = "matches";
            public const string FIRESTORE_KEY_PLAYER_SESSIONS_MATCHES_GAMENAME = "game";
            public const string FIRESTORE_KEY_PLAYER_SESSIONS_MATCHES_PLAYERS = "players";
            public const string FIRESTORE_KEY_PLAYER_SESSIONS_MATCHES_PLAYERS_NICKNAME = "nickname";
            public const string FIRESTORE_KEY_PLAYER_SESSIONS_MATCHES_PLAYERS_SCORE = "score";
            public const string FIRESTORE_KEY_PLAYER_SESSIONS_MATCHES_PLAYERS_UID = "uid";
        }
    }
        
}


