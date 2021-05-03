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

            //PLAYER ASSETS KEYS
            public const string FIRESTORE_KEY_PLAYER_ASSETS_ID = "assetid";
            public const string FIRESTORE_KEY_PLAYER_ASSETS_TYPE = "assettype";

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
            public const string FIRESTORE_KEY_PLAYER_CHARACTER_CURRENTLOADOUT = "currentloadout";
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
            public const string FIRESTORE_KEY_PLAYER_CHARACTER_FACE_WRINKLES = "wrinklesid";
            public const string FIRESTORE_KEY_PLAYER_CHARACTER_FACE_MAKEUPCOLOR = "makeupcolor";
            public const string FIRESTORE_KEY_PLAYER_CHARACTER_FACE_MAKEUP = "makeupid";
            public const string FIRESTORE_KEY_PLAYER_CHARACTER_FACE_SKINCOLOR = "skincolor";
            public const string FIRESTORE_KEY_PLAYER_CHARACTER_FACE_NOSE = "noseid";
            public const string FIRESTORE_KEY_PLAYER_CHARACTER_FACE_BEAUTYMARKS = "beautymarksid";
            // PLAYER MOUTH KEYS
            public const string FIRESTORE_KEY_PLAYER_CHARACTER_FACE_MOUTH = "mouth";
            public const string FIRESTORE_KEY_PLAYER_CHARACTER_FACE_MOUTH_HEIGHT = "height";
            public const string FIRESTORE_KEY_PLAYER_CHARACTER_FACE_MOUTH_MOUTHCOLOR = "mouthcolor";
            public const string FIRESTORE_KEY_PLAYER_CHARACTER_FACE_MOUTH_MOUTHID = "mouthid";
            public const string FIRESTORE_KEY_PLAYER_CHARACTER_FACE_MOUTH_SCALE = "scale";
            public const string FIRESTORE_KEY_PLAYER_CHARACTER_FACE_MOUTH_SQUASH = "squash";

            // PLAYER LOADOUT KEYS
            public const string FIRESTORE_KEY_PLAYER_CHARACTER_LOADOUTS = "loadouts";
            public const string FIRESTORE_KEY_PLAYER_CHARACTER_LOADOUTS_BACK = "backpieceid";
            public const string FIRESTORE_KEY_PLAYER_CHARACTER_LOADOUTS_EAR = "earid";
            public const string FIRESTORE_KEY_PLAYER_CHARACTER_LOADOUTS_FACIALHAIR = "facialhairid";
            public const string FIRESTORE_KEY_PLAYER_CHARACTER_LOADOUTS_GLASS = "glassid";
            public const string FIRESTORE_KEY_PLAYER_CHARACTER_LOADOUTS_HAIR = "hairid";
            public const string FIRESTORE_KEY_PLAYER_CHARACTER_LOADOUTS_JAW = "jawid";
            public const string FIRESTORE_KEY_PLAYER_CHARACTER_LOADOUTS_SHIRT = "shirtid";
            public const string FIRESTORE_KEY_PLAYER_CHARACTER_LOADOUTS_PANTS = "pantsid";
            public const string FIRESTORE_KEY_PLAYER_CHARACTER_LOADOUTS_SOCKS = "socksid";
            public const string FIRESTORE_KEY_PLAYER_CHARACTER_LOADOUTS_SHOES = "shoesid";
            public const string FIRESTORE_KEY_PLAYER_CHARACTER_LOADOUTS_WALLPAPER = "wallpaperid";
            public const string FIRESTORE_KEY_PLAYER_CHARACTER_LOADOUTS_EMOTEHAPPY = "emotehappyid";
            public const string FIRESTORE_KEY_PLAYER_CHARACTER_LOADOUTS_EMOTESAD = "emotesadid";
            public const string FIRESTORE_KEY_PLAYER_CHARACTER_LOADOUTS_EMOTEANGRY = "emoteangryid";
            public const string FIRESTORE_KEY_PLAYER_CHARACTER_LOADOUTS_EMOTELAUGH = "emotelaughid";
            public const string FIRESTORE_KEY_PLAYER_CHARACTER_LOADOUTS_EMOTESURPRISE = "emotesurpriseid";
            public const string FIRESTORE_KEY_PLAYER_CHARACTER_LOADOUTS_CELEBRATION = "celebrationid";
            public const string FIRESTORE_KEY_PLAYER_CHARACTER_LOADOUTS_TUNE = "tuneid";

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

            // ASSET KEYS
            public const string FIRESTORE_KEY_ASSETS = "assets";
            public const string FIRESTORE_KEY_ASSETS_ACHIEVEMENT = "achievement";
            public const string FIRESTORE_KEY_ASSETS_BASEPRICE = "baseprice";
            public const string FIRESTORE_KEY_ASSETS_PREMIUMPRICE = "premiumprice";
            public const string FIRESTORE_KEY_ASSETS_NAME = "storename";
            public const string FIRESTORE_KEY_ASSETS_TYPE = "type";
            public const string FIRESTORE_KEY_ASSETS_RARITY = "rarity";
            public const string FIRESTORE_KEY_ASSETS_ASSETID = "assetid";

            // TEAM KEYS
            public const string FIRESTORE_KEY_TEAM = "teams";
            public const string FIRESTORE_KEY_TEAM_NAME = "name";
            public const string FIRESTORE_KEY_TEAM_MEMBERS = "members";
            public const string FIRESTORE_KEY_TEAM_TAGLINE = "tagline";
            public const string FIRESTORE_KEY_TEAM_MEMBER_NAME = "name";
            public const string FIRESTORE_KEY_TEAM_MEMBER_UID = "uid";
            public const string FIRESTORE_KEY_TEAM_MEMBER_MMR = "mmr";
            public const string FIRESTORE_KEY_TEAM_MEMBER_ISADMIN = "isadmin";

            // GOAL KEYS
            public const string FIRESTORE_KEY_GOALS = "goals";
            public const string FIRESTORE_KEY_GOALS_DESCRIPTION = "description";
            public const string FIRESTORE_KEY_GOALS_NAME = "name";
            public const string FIRESTORE_KEY_GOALS_SCORENEEDED = "scoreneeded";

            // PLAYER SOCIAL KEYS
            public const string FIRESTORE_KEY_PLAYERSOCIAL = "players-social";
            public const string FIRESTORE_KEY_PLAYERSOCIAL_FRIENDREQUESTS = "friendrequests";
            public const string FIRESTORE_KEY_PLAYERSOCIAL_TEAMREQUESTS = "teamrequests";
            public const string FIRESTORE_KEY_PLAYERSOCIAL_TEAMS = "teams";
            public const string FIRESTORE_KEY_PLAYERSOCIAL_TEAMS_TEAMID = "teamid";
            public const string FIRESTORE_KEY_PLAYERSOCIAL_TEAMS_NAME = "name";
            public const string FIRESTORE_KEY_PLAYERSOCIAL_TEAMS_IMAGEID = "image";
            public const string FIRESTORE_KEY_PLAYERSOCIAL_FRIENDS = "friends";
            public const string FIRESTORE_KEY_PLAYERSOCIAL_FRIENDS_UID = "uid";
            public const string FIRESTORE_KEY_PLAYERSOCIAL_FRIENDS_IMAGEID = "image";
            public const string FIRESTORE_KEY_PLAYERSOCIAL_FRIENDS_NAME = "name";
            public const string FIRESTORE_KEY_PLAYERSOCIAL_CHATSGROUP = "chats-group";
            public const string FIRESTORE_KEY_PLAYERSOCIAL_CHATSGROUP_MEMBERS = "members";
            public const string FIRESTORE_KEY_PLAYERSOCIAL_CHATSGROUP_MEMBERS_NAME = "name";
            public const string FIRESTORE_KEY_PLAYERSOCIAL_CHATSGROUP_MEMBERS_UID = "uid";
            public const string FIRESTORE_KEY_PLAYERSOCIAL_CHATSGROUP_MESSAGES = "messages";
            public const string FIRESTORE_KEY_PLAYERSOCIAL_CHATSGROUP_MESSAGES_ID = "messageid";
            public const string FIRESTORE_KEY_PLAYERSOCIAL_CHATSGROUP_MESSAGES_SENDERID = "senderid";
            public const string FIRESTORE_KEY_PLAYERSOCIAL_CHATSGROUP_MESSAGES_MESSAGE = "message";
            public const string FIRESTORE_KEY_PLAYERSOCIAL_CHATSGROUP_MESSAGES_TIMESTAMP = "timestamp";
            public const string FIRESTORE_KEY_PLAYERSOCIAL_CHATSSING = "chats-singular";
            public const string FIRESTORE_KEY_PLAYERSOCIAL_CHATSSING_MESSAGES = "messages";
            public const string FIRESTORE_KEY_PLAYERSOCIAL_CHATSSING_MESSAGES_MESSAGE = "message";
            public const string FIRESTORE_KEY_PLAYERSOCIAL_CHATSSING_MESSAGES_TIMESTAMP = "timestamp";
            public const string FIRESTORE_KEY_PLAYERSOCIAL_CHATSSING_MESSAGES_MINE = "mine";
        }
    }
        
}


