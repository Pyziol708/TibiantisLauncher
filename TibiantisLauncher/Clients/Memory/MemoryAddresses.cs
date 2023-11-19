namespace TibiantisLauncher.Clients.Memory
{
    internal static class MemoryAddresses
    {
        public const long BattleListStart = 0x1C68B0;
        public const long BattleListCreatureSize = 0x9C;
        public const long BattleListMaxCreatures = 100;
        public const long BattleListEnd = BattleListStart + BattleListCreatureSize * BattleListMaxCreatures;
        public const long BattleListOffsetPositionX = 0x24;
        public const long BattleListOffsetPositionY = 0x28;
        public const long BattleListOffsetPositionZ = 0x2C;
        public const long CfgPath = 0x15A4BB;
        public const long ConnectionStatus = 0x1CCF98;
        public const long Experience = 0x1C6840;
        public const long Level = 0x1C683C;
    }
}
