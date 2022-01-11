using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Revitalize.Framework.World.Objects.InformationFiles;
using Revitalize.Framework.Objects;
using Revitalize.Framework.Utilities;
using StardewValley;
using System.Xml.Serialization;
using Netcode;

namespace Revitalize.Framework.World.Objects.Machines.EnergyGeneration
{
    [XmlType("Mods_Revitalize.Framework.World.Objects.Machines.EnergyGeneration.AdvancedSolarPanel")]
    public class AdvancedSolarPanel:Machine
    {
        public readonly NetInt maxDaysToProduceBattery = new NetInt();
        public readonly NetInt daysRemainingToProduceBattery = new NetInt();

        public AdvancedSolarPanel() { }

        public AdvancedSolarPanel(BasicItemInformation info) : base(info,null)
        {
            this.maxDaysToProduceBattery.Value = 6;
            this.daysRemainingToProduceBattery.Value = this.maxDaysToProduceBattery.Value;
        }

        protected override void initNetFieldsPostConstructor()
        {
            base.initNetFieldsPostConstructor();
            this.NetFields.AddFields(this.maxDaysToProduceBattery, this.daysRemainingToProduceBattery);
        }

        public override Item getOne()
        {
            AdvancedSolarPanel component = new AdvancedSolarPanel(this.basicItemInfo.Copy());
            return component;
        }

        public override void DayUpdate(GameLocation location)
        {
            if (!this.getCurrentLocation().IsOutdoors) return;
            if (Game1.weatherIcon == Game1.weather_snow || Game1.weatherIcon == Game1.weather_rain || Game1.weatherIcon == Game1.weather_lightning) return;
            if (this.heldObject.Value != null) return;

            this.daysRemainingToProduceBattery.Value -= 1;
            if (this.daysRemainingToProduceBattery.Value == 0)
            {
                this.daysRemainingToProduceBattery.Value = this.maxDaysToProduceBattery.Value;
                this.heldObject.Value = ObjectUtilities.getStardewObjectFromEnum(Enums.SDVObject.BatteryPack, 1);
            }

        }

        public override void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1f)
        {
            if (this.AnimationManager == null)
            {
                spriteBatch.Draw(this.CurrentTextureToDisplay, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(x * Game1.tileSize), y * Game1.tileSize)), new Rectangle?(this.AnimationManager.getCurrentAnimationFrameRectangle()), this.getItemInformation().DrawColor * alpha, 0f, Vector2.Zero, (float)Game1.pixelZoom, this.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0f, (float)(y * Game1.tileSize) / 10000f));
            }

            else
            {
                float addedDepth = 0;
                this.AnimationManager.draw(spriteBatch, this.CurrentTextureToDisplay, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(x * Game1.tileSize), y * Game1.tileSize)), new Rectangle?(this.AnimationManager.getCurrentAnimationFrameRectangle()), this.getItemInformation().DrawColor * alpha, 0f, Vector2.Zero, (float)Game1.pixelZoom, this.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0f, (float)((y + addedDepth) * Game1.tileSize) / 10000f) + .00001f);
                try
                {
                    this.AnimationManager.tickAnimation();
                }
                catch (Exception err)
                {
                    ModCore.ModMonitor.Log(err.ToString());
                }
            }

        }

    }
}