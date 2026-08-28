using TerrariaModder.Core.Config;

namespace TrueBestiary
{
    public class TrueBestiaryConfig : ModConfig
    {
        public override int Version => 1;

        [Client]
        [Label("Reveal Drops After 1 Kill")]
        [Description("Show enemy drops in the Bestiary after killing an enemy once.")]
        public bool RevealDropsAfterOneKill { get; set; } = true;
    }
}