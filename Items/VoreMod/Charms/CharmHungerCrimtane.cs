using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using VoreMod.Buffs;

namespace VoreMod.Items.VoreMod.Charms
{
	public class CharmHungerCrimtane : CharmHungerBase<CharmHungerDemoniteBuff>
	{
		public override ItemTier Tier => ItemTier.DemoniteCrimtane;
		public override int Metal => ItemID.CrimtaneBar;
	}
}
