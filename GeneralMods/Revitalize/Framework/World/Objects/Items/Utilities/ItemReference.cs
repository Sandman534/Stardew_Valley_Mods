using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Netcode;
using Omegasis.Revitalize.Framework.Constants;
using Omegasis.Revitalize.Framework.World.Objects.Interfaces;
using StardewValley;

namespace Omegasis.Revitalize.Framework.World.Objects.Items.Utilities
{
    /// <summary>
    /// Used to reference the many types of items that can be used.
    /// </summary>
    [XmlType("Mods_Revitalize.Framework.World.Objects.Items.Utilities.ItemReference")]
    public class ItemReference : StardustCore.Networking.NetObject
    {
        /// <summary>
        /// The default stack size for getting the item when using <see cref="getItem()"/>
        /// </summary>
        public readonly NetInt stackSize = new NetInt(1);
        public readonly NetString objectManagerId = new NetString("");
        public readonly NetEnum<Enums.SDVObject> sdvObjectId = new NetEnum<Enums.SDVObject>(Enums.SDVObject.NULL);
        public readonly NetEnum<Enums.SDVBigCraftable> sdvBigCraftableId = new NetEnum<Enums.SDVBigCraftable>(Enums.SDVBigCraftable.NULL);

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

        protected override void initializeNetFields()
        {
            base.initializeNetFields();
            this.NetFields.AddFields(this.stackSize, this.objectManagerId, this.sdvObjectId, this.sdvBigCraftableId);
        }

        public virtual void setItemReference(string ObjectId, int StackSize = 1)
        {
            this.objectManagerId.Value = ObjectId;
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
            if (item is IBasicItemInfoProvider)
            {
                string id = (item as IBasicItemInfoProvider).Id;
                this.objectManagerId.Value = id;
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
            else
            {
                throw new Exception("Item can not be cleanly converted to an item reference!");
            }

            this.stackSize.Value = item.Stack;
        }

        /// <summary>
        /// Checks to see if this item reference is null or not.
        /// </summary>
        /// <returns></returns>
        public virtual bool isNotNull()
        {
            return this.sdvObjectId.Value != Enums.SDVObject.NULL || this.sdvBigCraftableId.Value != Enums.SDVBigCraftable.NULL || !string.IsNullOrEmpty(this.objectManagerId.Value);
        }

        /// <summary>
        /// Clears the fields for this item reference.
        /// </summary>
        public virtual void clearItemReference()
        {
            this.sdvObjectId.Value = Enums.SDVObject.NULL;
            this.sdvBigCraftableId.Value = Enums.SDVBigCraftable.NULL;
            this.objectManagerId.Value = "";
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
            if(self is IBasicItemInfoProvider && (other is IBasicItemInfoProvider))
            {
                return (self as IBasicItemInfoProvider).Id.Equals((other as IBasicItemInfoProvider).Id);
            }

            if(self is StardewValley.Object && other is StardewValley.Object)
            {
                StardewValley.Object sObj = (self as StardewValley.Object);
                StardewValley.Object oObj = (other as StardewValley.Object);
                return sObj.bigCraftable == oObj.bigCraftable && sObj.ParentSheetIndex == oObj.ParentSheetIndex;
            }
            return false;

        }


        public virtual Item getItem(int StackSize = 1)
        {
            if (this.sdvObjectId.Value != Enums.SDVObject.NULL)
            {
                return RevitalizeModCore.ModContentManager.objectManager.getItem(this.sdvObjectId.Value, StackSize);
            }
            if (this.sdvBigCraftableId.Value != Enums.SDVBigCraftable.NULL)
            {
                return RevitalizeModCore.ModContentManager.objectManager.getItem(this.sdvBigCraftableId.Value, StackSize);
            }
            if (!string.IsNullOrEmpty(this.objectManagerId.Value))
            {
                return RevitalizeModCore.ModContentManager.objectManager.getItem(this.objectManagerId.Value, StackSize);
            }
            throw new InvalidObjectManagerItemException("An ItemReference must have one of the id fields set to be valid.");
        }

        public virtual Item getItem()
        {
            return this.getItem(this.stackSize.Value);
        }

        public virtual List<INetSerializable> getNetFields()
        {
            return new List<INetSerializable>()
            {
                this.stackSize,
                this.objectManagerId,
                this.sdvObjectId,
                this.sdvBigCraftableId
            };
        }

        public virtual ItemReference readItemReference(BinaryReader reader)
        {
            this.stackSize.Value = reader.ReadInt32();
            this.objectManagerId.Value = reader.ReadString();
            this.sdvObjectId.Value = reader.ReadEnum<Enums.SDVObject>();
            this.sdvBigCraftableId.Value = reader.ReadEnum<Enums.SDVBigCraftable>();
            return this;
        }

        public virtual void writeItemReference(BinaryWriter writer)
        {
            writer.Write(this.stackSize.Value);
            writer.Write(this.objectManagerId.Value);
            writer.WriteEnum<Enums.SDVObject>(this.sdvObjectId.Value);
            writer.WriteEnum<Enums.SDVBigCraftable>(this.sdvBigCraftableId.Value);

        }
    }
}
