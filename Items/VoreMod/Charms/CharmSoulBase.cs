using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using VoreMod.Buffs;
using System.Linq;
using System.Collections.Generic;

namespace VoreMod.Items.VoreMod.Charms
{
	public abstract class CharmSoulBase : CharmBase
	{
		public override CharmEffect Effect => CharmEffect.Soul;
		public override int Material => ItemID.SoulofNight;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Soul Charm");
			Tooltip.SetDefault("Controls soul drops on digestion");
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			if (!tooltips.Any(t => t.Name == "Soul Charm"))
			{
				tooltips.Insert(tooltips.FindIndex(t => t.Name == "ItemName") + 1, new TooltipLine(Mod, "Soul Charm", "" + ((int)Tier * 10) + "% soul drop chance"));
			}
		}
	}
}
