using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using VoreMod.Buffs;

namespace VoreMod.Items.VoreMod.Charms
{
	public class CharmSoulPlatinum : CharmSoulBase<CharmSoulGoldBuff>
	{
		public override ItemTier Tier => ItemTier.GoldPlatinum;
		public override int Metal => ItemID.PlatinumBar;
	}
}
