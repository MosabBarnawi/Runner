namespace BarnoGames.Runner2020
{
    [System.Serializable]
    public struct LivingEntityHealth
    {
        public float StartingHealth;
        public float Health { get; set; }
        public bool isDead { get; set; }
    }
}
