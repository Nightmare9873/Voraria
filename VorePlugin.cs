
using Terraria;
using Terraria.ModLoader;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria.ID;

namespace VoreMod
{
    public abstract class VorePlugin
    {
        public abstract string Name { get; }
        public List<NPC> npcs = new List<NPC>();
        public List<Item> items = new List<Item>();

        public abstract Builder Build(Builder builder);
        public virtual void Load() { }
        public virtual void Unload() { }

        public Mod Mod => ModLoader.GetMod(Name);

        public bool IsValid() => loaded && built && (Mod != null || Name == "Terraria");

        private bool loaded;
        private bool built;

        public void Register()
        {
            if (loaded) return;
            if (!built)
            {
                Build(new Builder(this));
                built = true;
            }
            Load();
            loaded = true;
        }

        public void Unregister()
        {
            if (!loaded) return;
            Unload();
            loaded = false;
        }

        public EntityTags? GetTags(Terraria.NPC npc)
        {
            NPC modNPC = npcs.Find(n => n.Is(npc));
            if (modNPC != null) return modNPC.GetTags();
            return null;
        }

        public CharmEffects? GetCharmEffects(Terraria.NPC npc)
        {
            NPC modNPC = npcs.Find(n => n.Is(npc));
            if (modNPC != null) return modNPC.GetCharmEffects();
            return null;
        }

        public List<VoreSprite> GetSprites(SpriteType type, Terraria.NPC npc)
        {
            NPC modNPC = npcs.Find(n => n.Is(npc));
            if (modNPC != null) return modNPC.GetSprites(type);
            return null;
        }

        public List<VoreSprite> GetSprites(SpriteType type, Terraria.Item item)
        {
            Item modItem = items.Find(n => n.Is(item));
            if (modItem != null) return modItem.GetSprites(type);
            return null;
        }

        public class Builder
        {
            VorePlugin plugin;

            public Builder(VorePlugin plugin)
            {
                this.plugin = plugin;
            }

            public Builder NPC(string name, System.Func<NPC.Builder, NPC> build)
            {
                plugin.npcs.Add(build(new NPC.Builder(new NPC(this, name, null))));
                return this;
            }

            public Builder NPC(int type, string name, System.Func<NPC.Builder, NPC> build)
            {
                plugin.npcs.Add(build(new NPC.Builder(new NPC(this, name, type))));
                return this;
            }

            public Builder Item(string name, System.Func<Item.Builder, Item> build)
            {
                plugin.items.Add(build(new Item.Builder(new Item(this, name, null))));
                return this;
            }

            public Builder Item(int type, string name, System.Func<Item.Builder, Item> build)
            {
                plugin.items.Add(build(new Item.Builder(new Item(this, name, type))));
                return this;
            }

            public static implicit operator VorePlugin(Builder builder) => builder.plugin;
        }

        public class NPC
        {
            VorePlugin plugin;
            string name;
            int? vanillaType = null;
            EntityTags? tags;
            CharmEffects? charmEffects;

            Dictionary<SpriteType, List<VoreSprite>> sprites = new Dictionary<SpriteType, List<VoreSprite>>();

            public NPC(VorePlugin plugin, string name, int? vanillaType)
            {
                this.plugin = plugin;
                this.name = name;
                this.vanillaType = vanillaType;
            }

            public bool Is(Terraria.NPC npc)
            {
                if (FindType(out int type))
                {
                    return npc.type == type;
                }
                return false;
            }

            public bool FindType(out int type)
            {
                if (vanillaType.HasValue)
                {
                    type = vanillaType.Value;
                    return true;
                }
                type = plugin.Mod.NPCType(name);
                if (type == 0)
                {
                    VoreMod.Instance.Logger.WarnFormat("Unable to find NPC type \"{0}/{1}\"", plugin.Name, name);
                    return false;
                }
                return true;
            }

            public EntityTags? GetTags() => tags;

            public CharmEffects? GetCharmEffects() => charmEffects;

            public List<VoreSprite> GetSprites(SpriteType type) => sprites.ContainsKey(type) ? sprites[type] : null;

            public class Builder
            {
                NPC npc;

                public Builder(NPC npc)
                {
                    this.npc = npc;
                }

                public Builder Tags(EntityTags tags)
                {
                    npc.tags = tags;
                    return this;
                }

                public Builder CharmEffects(CharmEffects charmEffects)
                {
                    npc.charmEffects = charmEffects;
                    return this;
                }

                public Builder Sprite(SpriteType type, string filename, System.Func<VoreSprite.Builder, VoreSprite> build)
                {
                    string path = nameof(VoreMod) + "/NPCs/" + npc.plugin.Name + "/" + filename;
                    List<VoreSprite> list = npc.sprites.ContainsKey(type) ? npc.sprites[type] : null;
                    if (list == null)
                    {
                        list = new List<VoreSprite>();
                        npc.sprites[type] = list;
                    }
                    list.Add(build(new VoreSprite.Builder(new VoreSprite(path, type))));
                    return this;
                }

                public static implicit operator NPC(Builder builder) => builder.npc;
            }
        }

        public class Item
        {
            VorePlugin plugin;
            string name;
            int? vanillaType = null;

            Dictionary<SpriteType, List<VoreSprite>> sprites = new Dictionary<SpriteType, List<VoreSprite>>();

            public Item(VorePlugin plugin, string name, int? vanillaType)
            {
                this.plugin = plugin;
                this.name = name;
                this.vanillaType = vanillaType;
            }

            public bool Is(Terraria.Item item)
            {
                if (FindType(out int type))
                {
                    return item.type == type;
                }
                return false;
            }

            public bool FindType(out int type)
            {
                if (vanillaType.HasValue)
                {
                    type = vanillaType.Value;
                    return true;
                }
                type = plugin.Mod.ItemType(name);
                if (type == 0)
                {
                    VoreMod.Instance.Logger.WarnFormat("Unable to find Item type \"{0}/{1}\"", plugin.Name, name);
                    return false;
                }
                return true;
            }

            public List<VoreSprite> GetSprites(SpriteType type) => sprites.ContainsKey(type) ? sprites[type] : null;

            public class Builder
            {
                Item item;

                public Builder(Item item)
                {
                    this.item = item;
                }

                public Builder Sprite(SpriteType type, string filename, System.Func<VoreSprite.Builder, VoreSprite> build)
                {
                    string path = nameof(VoreMod) + "/Items/" + item.plugin.Name + "/" + filename;
                    List<VoreSprite> list = item.sprites.ContainsKey(type) ? item.sprites[type] : null;
                    if (list == null)
                    {
                        list = new List<VoreSprite>();
                        item.sprites[type] = list;
                    }
                    list.Add(build(new VoreSprite.Builder(new VoreSprite(path, type))));
                    return this;
                }

                public static implicit operator Item(Builder builder) => builder.item;
            }
        }
    }
}