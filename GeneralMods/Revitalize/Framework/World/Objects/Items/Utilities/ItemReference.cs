using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Netcode;
using Newtonsoft.Json;
using Omegasis.Revitalize.Framework.Constants;
using Omegasis.Revitalize.Framework.World.Objects.Interfaces;
using StardewValley;

namespace Omegasis.Revitalize.Framework.World.Objects.Items.Utilities
{
    /// <summary>
    /// Used to reference the many types of items that can be used.
    ///
    /// TODO: Add an enum value for vanilla tools?
    /// TODO: Add an enum value for vanilla furniture?
    /// </summary>
    [XmlType("Mods_Revitalize.Framework.World.Objects.Items.Utilities.ItemReference")]
    public class ItemReference : StardustCore.Networking.NetObject
    {
        [JsonIgnore]
        /// <summary>
        /// The default stack size for getting the item when using <see cref="getItem()"/>
        /// </summary>
        public readonly NetInt stackSize = new NetInt(1);
        [JsonIgnore]
        public readonly NetString registeredObjectId = new NetString("");
        [JsonIgnore]
        public readonly NetEnum<Enums.SDVObject> sdvObjectId = new NetEnum<Enums.SDVObject>(Enums.SDVObject.NULL);
        [JsonIgnore]
        public readonly NetEnum<Enums.SDVBigCraftable> sdvBigCraftableId = new NetEnum<Enums.SDVBigCraftable>(Enums.SDVBigCraftable.NULL);

        [JsonProperty("amount")]
        public virtual int StackSize
        {
            get
            {
                return this.stackSize.Value;
            }
            set
            {
                this.stackSize.Value = value;
            }
        }

        [JsonProperty("registeredObjectId")]
        public virtual string RegisteredObjectId
        {
            get
            {
                if (string.IsNullOrEmpty(this.registeredObjectId.Value))
                {
                    if(this.StardewValleyItemId!= Enums.SDVObject.NULL)
                    {
                        return RevitalizeModCore.ModContentManager.objectManager.getItemId(this.StardewValleyItemId);
                    }
                    if (this.StardewValleyBigCraftableId != Enums.SDVBigCraftable.NULL)
                    {
                        return RevitalizeModCore.ModContentManager.objectManager.getItemId(this.StardewValleyBigCraftableId);
                    }
                }

                return this.registeredObjectId.Value;
            }
            set
            {
                this.registeredObjectId.Value = value;
            }
        }

        [JsonProperty("stardewValleyItemId")]
        public virtual Enums.SDVObject StardewValleyItemId
        {
            get
            {
                return this.sdvObjectId.Value;
            }
            set
            {
                this.sdvObjectId.Value = value;
            }
        }

        [JsonProperty("stardewValleyBigCraftableId")]
        public virtual Enums.SDVBigCraftable StardewValleyBigCraftableId
        {
            get
            {
                return this.sdvBigCraftableId.Value;
            }
            set
            {
                this.sdvBigCraftableId.Value = value;
            }
        }

        


        public ItemReference()
        {

        }

        public ItemReference(string ObjectId, int StackSize = 1)
        {
            this.setItemReference(ObjectId, StackSize);
        }

        public ItemReference(Enums.SDVObject ObjectId, int StackSize = 1)
        {
            this.setItemReference(ObjectId, StackSize);
        }

        public ItemReference(Enums.SDVBigCraftable ObjectId, int StackSize = 1)
        {
            this.setItemReference(ObjectId, StackSize);
        }

        /// <summary>
        /// Attempts to convert an item into an item reference!
        /// </summary>
        /// <param name="item"></param>
        public ItemReference(Item item)
        {
            this.setItemReference(item);
        }

        /// <summary>
        /// Attempts to convert an item into an item reference!
        /// </summary>
        /// <param name="item"></param>
        public ItemReference(Item item, int StackSize)
        {
            this.setItemReference(item);
            this.StackSize = StackSize;
        }

        /// <summary>
        /// Attempts to convert an item that is being sold into an item reference.
        /// </summary>
        /// <param name="salableItem"></param>
        public ItemReference(ISalable salableItem)
        {
            if(salableItem is StardewValley.Item)
            {
                this.setItemReference((StardewValley.Item)salableItem);
            }
        }

        protected override void initializeNetFields()
        {
            base.initializeNetFields();
            this.NetFields.AddFields(this.stackSize, this.registeredObjectId, this.sdvObjectId, this.sdvBigCraftableId);
        }

        public virtual void setItemReference(string ObjectId, int StackSize = 1)
        {
            this.registeredObjectId.Value = ObjectId;
            this.stackSize.Value = StackSize;
        }

        public virtual void setItemReference(Enums.SDVObject ObjectId, int StackSize = 1)
        {
            this.sdvObjectId.Value = ObjectId;
            this.stackSize.Value = StackSize;
        }

        public virtual void setItemReference(Enums.SDVBigCraftable ObjectId, int StackSize = 1)
        {
            this.sdvBigCraftableId.Value = ObjectId;
            this.stackSize.Value = StackSize;
        }

        public virtual void setItemReference(Item item)
        {
            if (item == null)
            {
                this.clearItemReference();
                return;
            }

            if (item is IBasicItemInfoProvider)
            {
                string id = (item as IBasicItemInfoProvider).Id;
                this.registeredObjectId.Value = id;
            }

            //TODO: Replace the following section with just using the Item's Id field once SDV 1.6 is released.
            else if (item.GetType().Equals(typeof(StardewValley.Objects.Furniture)) || item.GetType().Equals(typeof(StardewValley.Objects.BedFurniture)) || item.GetType().Equals(typeof(StardewValley.Objects.FishTankFurniture)) || item.GetType().Equals(typeof(StardewValley.Objects.StorageFurniture)))
            {
                //TODO: Maybe add in references for furniture types?
                return;
            }
            else if (item is StardewValley.Object)
            {


                if ((item as StardewValley.Object).bigCraftable)
                {
                    this.sdvBigCraftableId.Value = (Enums.SDVBigCraftable)item.ParentSheetIndex;
                }
                else
                {
                    this.sdvObjectId.Value = (Enums.SDVObject)item.ParentSheetIndex;
                }
            }
            else if (item is StardewValley.Tool)
            {
                //Don't really need item references for tools as far as I'm aware of. Skipping it.
                return;
            }
            else
            {
                throw new Exception("Item can not be cleanly converted to an item reference!");
            }
            //End TODO;

            this.stackSize.Value = item.Stack;
        }

        /// <summary>
        /// Checks to see if this item reference is null or not.
        /// </summary>
        /// <returns></returns>
        public virtual bool isNotNull()
        {
            return !string.IsNullOrEmpty(this.RegisteredObjectId);
        }

        /// <summary>
        /// Clears the fields for this item reference.
        /// </summary>
        public virtual void clearItemReference()
        {
            this.sdvObjectId.Value = Enums.SDVObject.NULL;
            this.sdvBigCraftableId.Value = Enums.SDVBigCraftable.NULL;
            this.registeredObjectId.Value = "";
            this.stackSize.Value = 1;
        }

        public virtual bool itemEquals(Item other)
        {
            Item self = this.getItem();

            if (self == null && other == null) return true;
            if (self == null && other != null) return false;
            if (self != null && other == null) return false;

            if (!self.GetType().Equals(other.GetType())) return false;

            //Custom mod objects should have the same id.
            if (self is IBasicItemInfoProvider && (other is IBasicItemInfoProvider))
            {
                return (self as IBasicItemInfoProvider).Id.Equals((other as IBasicItemInfoProvider).Id);
            }

            if (self is StardewValley.Object && other is StardewValley.Object)
            {
                StardewValley.Object sObj = (self as StardewValley.Object);
                StardewValley.Object oObj = (other as StardewValley.Object);
                return sObj.bigCraftable == oObj.bigCraftable && sObj.ParentSheetIndex == oObj.ParentSheetIndex;
            }
            return false;

        }


        public virtual Item getItem(int StackSize = 1)
        {
            if (this.isNotNull())
            {
                return RevitalizeModCore.ModContentManager.objectManager.getItem(this.RegisteredObjectId, StackSize);
            }
            throw new InvalidObjectManagerItemException("An ItemReference must have one of the id fields set to be valid.");
        }

        public virtual Item getItem()
        {
            return this.getItem(this.stackSize.Value);
        }

        public virtual ItemReference readItemReference(BinaryReader reader)
        {
            this.stackSize.Value = reader.ReadInt32();
            this.registeredObjectId.Value = reader.ReadString();
            this.sdvObjectId.Value = reader.ReadEnum<Enums.SDVObject>();
            this.sdvBigCraftableId.Value = reader.ReadEnum<Enums.SDVBigCraftable>();
            return this;
        }

        public virtual void writeItemReference(BinaryWriter writer)
        {
            writer.Write(this.stackSize.Value);
            writer.Write(this.registeredObjectId.Value);
            writer.WriteEnum<Enums.SDVObject>(this.sdvObjectId.Value);
            writer.WriteEnum<Enums.SDVBigCraftable>(this.sdvBigCraftableId.Value);

        }
    }
}
