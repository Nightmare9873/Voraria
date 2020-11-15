using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace VoreMod.Items.VoreMod.Amulets
{
	public class AmuletThroatPlatinum : AmuletThroatBase
	{
		public override ItemTier Tier => ItemTier.GoldPlatinum;
		public override int Metal => ItemID.PlatinumBar;
		public override int Capacity => 2;
		public override int EscapeLimit => 60;
	}
}
