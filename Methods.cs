using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Styx;
using Styx.CommonBot.Frames;
using Styx.Helpers;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;

namespace MrItemRemover2
{
    public partial class MrItemRemover2
    {
        public void SellVenderItems(object sender, LuaEventArgs args)
        {
            if (MerchantFrame.Instance.IsVisible && MrItemRemover2Settings.Instance.EnableSell)
            {
                foreach (WoWItem item in Me.BagItems)
                {
                    if (!MrItemRemover2Settings.Instance.SellSoulbound)
                    {
                        if (!item.IsSoulbound && !_KeepList.Contains(item.Name))
                        {
                            if (item.Quality == WoWItemQuality.Poor && MrItemRemover2Settings.Instance.SellGray)
                            {
                                slog("Selling Gray Item {0}.", item.Name);
                                item.UseContainerItem();
                            }
                            if (item.Quality == WoWItemQuality.Common && MrItemRemover2Settings.Instance.SellWhite)
                            {
                                slog("Selling White Item {0}.", item.Name);
                                item.UseContainerItem();
                            }
                            if (item.Quality == WoWItemQuality.Uncommon && MrItemRemover2Settings.Instance.SellGreen)
                            {
                                slog("Selling Green Item {0}.", item.Name);
                                item.UseContainerItem();
                            }
                            if (item.Quality == WoWItemQuality.Rare && MrItemRemover2Settings.Instance.SellBlue)
                            {
                                slog("Selling Blue Item {0}.", item.Name);
                                item.UseContainerItem();
                            }
                            if (_ItemNameSell.Contains(item.Name))
                            {
                                slog("{0} Matched Sell List.", item.Name);
                                item.UseContainerItem();
                            }                            
                        }
                    }

                    if (MrItemRemover2Settings.Instance.SellSoulbound)
                    {
                        if (!_KeepList.Contains(item.Name))
                        {
                            if (item.Quality == WoWItemQuality.Poor && MrItemRemover2Settings.Instance.SellGray)
                            {
                                slog("Selling Gray Item {0}.", item.Name);
                                item.UseContainerItem();
                            }
                            if (item.Quality == WoWItemQuality.Common && MrItemRemover2Settings.Instance.SellWhite)
                            {
                                slog("Selling White Item {0}.", item.Name);
                                item.UseContainerItem();
                            }
                            if (item.Quality == WoWItemQuality.Uncommon && MrItemRemover2Settings.Instance.SellGreen)
                            {
                                slog("Selling Green Item {0}.", item.Name);
                                item.UseContainerItem();
                            }
                            if (item.Quality == WoWItemQuality.Rare && MrItemRemover2Settings.Instance.SellBlue)
                            {
                                slog("Selling Blue Item {0}.", item.Name);
                                item.UseContainerItem();
                            }
                            if (_ItemNameSell.Contains(item.Name))
                            {
                                slog("{0} Matched Sell List.", item.Name);
                                item.UseContainerItem();
                            }                            
                        }
                    }
                }
            }
        }

        private static void DeleteItemConfirmPopup(object sender, LuaEventArgs args)
        {
            string ItemNamePopUp = args.Args[0].ToString();

            if (Me.CurrentTarget != null)
            {
                slog("Clicking Yes to Comfirm {0}'s Removal From Inventory.", ItemNamePopUp);
                Lua.DoString("RunMacroText(\"/click StaticPopup1Button1\");");
            }
        }

        public void OpenBagItems()
        {
            MIRSave();
            MIRLoad();
            LoadList(_OpnList, OpnListPath);

            // NB: Since we will be modifying the Me.BagItems list indirectly through WoWclient directives,
            // we can't use it as our iterator--we must make a copy, instead.
            List<WoWItem> itemsToVisit = Me.BagItems.ToList();
            foreach (WoWItem item in itemsToVisit)
            {
                if ((item == null) || !item.IsValid)
                {
                    continue;
                }

                if (_OpnList.Contains(item.Name))
                {
                    //probally not needed, but still user could be messing with thier inventory.
                    slog("Found Item to Open - {0}.", item.Name);
                    item.Interact();
                    Thread.Sleep(600);
                }
            }
        }

        public void CheckForItems()
        {
            //Reload item lists
            MIRSave();
            MIRLoad();

            //Added to Make sure our list matches what we are looking for. 
            LoadList(_ItemName, RemoveListPath);

            // NB: Since we will be modifying the Me.BagItems list indirectly through WoWclient directives,
            // we can't use it as our iterator--we must make a copy, instead.
            List<WoWItem> itemsToVisit = Me.BagItems.ToList();
            foreach (WoWItem item in itemsToVisit)
            {
                if ((item == null) || !item.IsValid)
                {
                    continue;
                }

                bool isQuestItem = IsQuestItem(item);

                /* Uncomment this to have quest items printed to log. DIAGNOSTIC.
                Logging.Write("{0} is quest item: {1}", item.Name, isQuestItem);
                */

                //if item name Matches whats in the text file / the internal list (after load)
                if (_ItemName.Contains(item.Name) && !_KeepList.Contains(item.Name))
                {
                    //probally not needed, but still user could be messing with thier inventory.
                    //Printing to the log, and Deleting the Item.
                    slog("Remove List Item Found - {0}.", item.Name);
                    item.PickUp();
                    Lua.DoString("DeleteCursorItem()");
                    //a small Sleep, might not be needed. 
                    Thread.Sleep(600);
                }

                if (MrItemRemover2Settings.Instance.DeleteQuestItems && item.ItemInfo.BeginQuestId != 0 &&
                    !_KeepList.Contains(item.Name))
                {
                    slog("{0} Began a Quest. Removing.", item.Name);
                    item.PickUp();
                    Lua.DoString("DeleteCursorItem()");
                }

                //Process all Gray Items if enabled. 
                if (MrItemRemover2Settings.Instance.DeleteAllGray && item.Quality == WoWItemQuality.Poor && !_KeepList.Contains(item.Name))
                {
                    //Gold Format, goes in GXX SXX CXX 
                    string goldString = MrItemRemover2Settings.Instance.GoldGrays.ToString();
                    int goldValue = goldString.ToInt32() * 10000;
                    string silverString = MrItemRemover2Settings.Instance.SilverGrays.ToString();
                    int silverValue = silverString.ToInt32() * 100;
                    string copperString = MrItemRemover2Settings.Instance.CopperGrays.ToString();
                    int copperValue = copperString.ToInt32();

                    if (item.BagSlot != -1 && !isQuestItem && item.ItemInfo.SellPrice <= (goldValue + silverValue + copperValue))
                    {
                        slog("Removing Gray Item {0}.", item.Name);
                        Lua.DoString("ClearCursor()");
                        item.PickUp();
                        Lua.DoString("DeleteCursorItem()");
                        Thread.Sleep(300);
                    }
                }

                //Process all White Items if enabled.
                if (MrItemRemover2Settings.Instance.DeleteAllWhite && item.Quality == WoWItemQuality.Common && !_KeepList.Contains(item.Name))
                {
                    if (item.BagSlot != -1 && !isQuestItem)
                    {
                        slog("Removing White Item {0}..", item.Name);
                        Lua.DoString("ClearCursor()");
                        item.PickUp();
                        Lua.DoString("DeleteCursorItem()");
                        Thread.Sleep(300);
                    }
                }

                //Process all Green Items if enabled.
                if (MrItemRemover2Settings.Instance.DeleteAllGreen && item.Quality == WoWItemQuality.Uncommon && !_KeepList.Contains(item.Name))
                {
                    if (item.BagSlot != -1 && !isQuestItem)
                    {
                        slog("Removing Green Item {0}.", item.Name);
                        Lua.DoString("ClearCursor()");
                        item.PickUp();
                        Lua.DoString("DeleteCursorItem()");
                        Thread.Sleep(300);
                    }
                }

                //Process all Blue Items if enabled.
                if (MrItemRemover2Settings.Instance.DeleteAllBlue && item.Quality == WoWItemQuality.Rare && !_KeepList.Contains(item.Name))
                {
                    if (item.BagSlot != -1 && !isQuestItem)
                    {
                        slog("Removing Blue Item {0}.", item.Name);
                        Lua.DoString("ClearCursor()");
                        item.PickUp();
                        Lua.DoString("DeleteCursorItem()");
                        Thread.Sleep(300);
                    }
                }
            }
        }

        public string GetTime(DateTime Input)
        {
            string TimeInString = "";
            int hour = Input.Hour;
            int min = Input.Minute;
            int sec = Input.Second;

            TimeInString = (hour < 10) ? "0" + hour.ToString() : hour.ToString();
            TimeInString += ":" + ((min < 10) ? "0" + min.ToString() : min.ToString());
            TimeInString += ":" + ((sec < 10) ? "0" + sec.ToString() : sec.ToString());
            return TimeInString;
        }

        private bool IsQuestItem(WoWItem item)
        {
            if ((item == null) || !item.IsValid)
            {
                return false;
            }

            string luaCommand = string.Format("return GetContainerItemQuestInfo({0},{1});", item.BagIndex + 1,
                                              item.BagSlot + 1);
            bool isQuestItem =
                Lua.GetReturnVal<bool>(luaCommand, 0) // item is quest item?
                || (Lua.GetReturnVal<int>(luaCommand, 1) > 0); // item begins a quest?

            return isQuestItem;
        }
    }
}