namespace FiestaTime
{
    public struct PlayerResults<T>
    {
        public T scoring;
        public int playerId;
        public bool reachedEnd;

        public override bool Equals(object obj)
        {
            PlayerResults<T> a = (PlayerResults<T>)obj;
            return a.scoring.Equals(scoring);
        }

        public override int GetHashCode()
        {
            return playerId * 17;
        }

        public override string ToString()
        {
            return "PlayerID: " + playerId + " , scoring: " + scoring;
        }
    }
}


