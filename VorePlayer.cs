using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using VoreMod.Items;

namespace VoreMod
{
    public class VorePlayer : ModPlayer
    {
        public VoreEntity entity;

		public static PlayerLayer BellyLayer = null;
		public static PlayerLayer ClothingBellyLayer = null;

		public override void ModifyDrawLayers(List<PlayerLayer> layers)
		{
			if (BellyLayer == null)
			{
				BellyLayer = new PlayerLayer(nameof(VoreMod), nameof(BellyLayer), PlayerLayer.Body, delegate (PlayerDrawInfo drawInfo)
				{
					Player drawPlayer = drawInfo.drawPlayer;
					if (!drawPlayer.GetEntity().HasBelly()) return;
					Texture2D texture = drawPlayer.GetEntity().GetBellyTexture();
					Rectangle bellyRect = drawPlayer.GetEntity().GetBellyRect();

					Vector2 pos = drawInfo.position - Main.screenPosition;
					Vector2 offset = drawPlayer.GetEntity().GetBellyOffset();
					Color color = drawInfo.bodyColor.MultiplyRGB(drawPlayer.GetEntity().GetBellyColor());

					if (drawPlayer.wereWolf)
					{
						color = Lighting.GetColor((int)((drawInfo.position.X + drawPlayer.width / 2f) / 16f), (int)((drawInfo.position.Y + drawPlayer.height / 2f) / 16f)).MultiplyRGB(new Color(166, 124, 82));
					}
					if (drawPlayer.merman)
					{
						color = Lighting.GetColor((int)((drawInfo.position.X + drawPlayer.width / 2f) / 16f), (int)((drawInfo.position.Y + drawPlayer.height / 2f) / 16f)).MultiplyRGB(new Color(62, 165, 101));
					}

					Vector2 drawPos = pos + offset;
					drawPos.X = drawPlayer.direction == -1 ? (int)Math.Floor(drawPos.X) : (int)Math.Ceiling(drawPos.X);
					drawPos.Y = (int)Math.Ceiling(drawPos.Y);
					DrawData data = new DrawData(texture, drawPos, bellyRect, color, 0f, new Vector2(0, 0), 1f, drawPlayer.direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
					Main.playerDrawData.Add(data);
				});
			}
			BellyLayer.visible = true;
			layers.Insert(layers.IndexOf(PlayerLayer.Arms), BellyLayer);
			if (CheckForClothingBellyTexture())
			{
				if (ClothingBellyLayer == null)
				{
					ClothingBellyLayer = new PlayerLayer(nameof(VoreMod), nameof(ClothingBellyLayer), PlayerLayer.Body, delegate (PlayerDrawInfo drawInfo)
					{
						Player drawPlayer = drawInfo.drawPlayer;
						if (!drawPlayer.GetEntity().HasBelly()) return;
						Texture2D texture = GetClothingBellyTexture();
						Rectangle bellyRect = drawPlayer.GetEntity().GetBellyRect();

						Vector2 pos = drawInfo.position - Main.screenPosition;
						Vector2 offset = drawPlayer.GetEntity().GetBellyOffset();
						Color color = drawInfo.bodyColor.MultiplyRGB(drawPlayer.GetEntity().GetBellyColor());

						Vector2 drawPos = pos + offset;
						drawPos.X = drawPlayer.direction == -1 ? (int)Math.Floor(drawPos.X) : (int)Math.Ceiling(drawPos.X);
						drawPos.Y = drawPlayer.direction == -1 ? (int)Math.Floor(drawPos.Y) : (int)Math.Ceiling(drawPos.Y);
						DrawData data = new DrawData(texture, drawPos, bellyRect, color, 0f, new Vector2(0, 0), 1f, drawPlayer.direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
						Main.playerDrawData.Add(data);
					});
				}
				ClothingBellyLayer.visible = true;
				layers.Insert(layers.IndexOf(PlayerLayer.Arms), ClothingBellyLayer);
			}
		}

        public override void ModifyDrawInfo(ref PlayerDrawInfo drawInfo)
        {
            VoreEntity ent = drawInfo.drawPlayer.GetEntity();
            if (ent.IsSwallowed() && !ent.ShouldShowWhileSwallowed())
            {
                drawInfo.position = new Vector2(-100f, -100f);
            }
        }

		private bool CheckForClothingBellyTexture()
		{
			switch (player.body)
			{
				case ArmorIDs.Body.PrincessCostume:
				case ArmorIDs.Body.WeddingDress:
				case ArmorIDs.Body.RedRidingHuntress:
					return true;
			}
			return false;
		}

		private Texture2D GetClothingBellyTexture()
		{
			switch (player.body)
			{
				case ArmorIDs.Body.PrincessCostume: return ModContent.GetTexture(nameof(VoreMod) + "/Items/Clothing/PrincessDressClothier_Belly_Overlay");
				case ArmorIDs.Body.WeddingDress: return ModContent.GetTexture(nameof(VoreMod) + "/Items/Clothing/WeddingDress_Belly_Overlay");
				case ArmorIDs.Body.RedRidingHuntress: return ModContent.GetTexture(nameof(VoreMod) + "/Items/Clothing/RedRidingDress_Belly_Overlay");
			}
			return ModContent.GetTexture(nameof(VoreMod) + "/Belly2");
		}

		public override void ResetEffects()
        {
            base.ResetEffects();
            player.GetEntity().ResetTick();
        }

        public override void PostUpdateBuffs()
        {
            if (player.GetEntity().IsSwallowed()) player.velocity = Vector2.Zero;
            player.GetEntity().UpdateTick();
        }

		public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
		{
			if (player.GetEntity().IsSwallowed())
				genGore = false;
			return base.PreKill(damage, hitDirection, pvp, ref playSound, ref genGore, ref damageSource);
		}

		public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
        {
            player.GetEntity().Death();
        }

        public override bool CanBeHitByNPC(NPC npc, ref int cooldownSlot)
        {
            if (!npc.GetEntity().CanDamage(player.GetEntity())) return false;
            return base.CanBeHitByNPC(npc, ref cooldownSlot);
        }

        public override bool CanBeHitByProjectile(Projectile proj)
        {
            if (!player.GetEntity().CanBeDamaged()) return false;
            return base.CanBeHitByProjectile(proj);
        }

        public override bool? CanHitNPC(Item item, NPC target)
        {
            if (!player.GetEntity().CanDamage(target)) return false;
            if (item.modItem is AmuletBase) return player.GetEntity().CanSwallow(target);
            if (item.modItem is TalismanBase) return target.GetEntity().CanSwallow(player);
            return base.CanHitNPC(item, target);
        }

        public override bool CanHitPvp(Item item, Player target)
        {
            if (!player.GetEntity().CanDamage(target)) return false;
            if (item.modItem is AmuletBase) return player.GetEntity().CanSwallow(target);
            if (item.modItem is TalismanBase) return target.GetEntity().CanSwallow(player);
            return base.CanHitPvp(item, target);
        }

        public override bool PreItemCheck()
        {
            if (player.GetEntity().IsSwallowed()) return false;
            return base.PreItemCheck();
        }

        public override void NaturalLifeRegen(ref float regen)
        {
            if (player.GetEntity().IsSwallowed())
            {
                regen = 0f;
            }
        }
    }
}
