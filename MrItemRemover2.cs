/*
 * Mr.ItemRemover2 - Created by CodenameGamma - 1-31-11 - For WoW Version 4.0.3
 * 1.5.2 Update by Bambam922
 * www.thebuddyforum.com
 * This is a free plugin and should not be sold or repackaged.
 * Donations accepted.
 * Version 1.5.2 for WoW Version 5.3 +
 */

using System.Windows.Forms;
using MrItemRemover2.GUI;
using System;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using System.IO;
using Styx.Plugins;
using Styx;
using Styx.Common.Helpers;
using Styx.Common;
using System.Collections.Generic;
using System.Windows.Media;


namespace MrItemRemover2
{

    public partial class MrItemRemover2 : HBPlugin
    {
        const string _name = "Mr.ItemRemover2";

        //Normal Stuff.
        public override string Name { get { return _name; } }
        public override string Author { get { return "CnG & Bambam922"; } }
        public override Version Version { get { return new Version(1, 5, 2); } }
        public override bool WantButton { get { return true; } }
        public override string ButtonText { get { return _name; } }


        public override void OnButtonPress()
        {
            if (!IsInitialized)
            {
                slog("Not finished initializing");
                return;
            }

            var form = new MrItemRemover2GUI(this);
            form.ShowDialog();
        }
    
        public static void slog(string format, params object[] args)
        {
            slog(Colors.Lime, format, args);
        }

        public static void slog(Color color, string format, params object[] args)
        {
            Logging.Write(color, "[" + _name + "]: " + format, args);
        }

        public static void dlog(string format, params object[] args)
        {
            dlog(Colors.Lime, format, args);
        }

        public static void dlog(Color color, string format, params object[] args)
        {
            Logging.WriteDiagnostic(color, "[" + _name + "]: " + format, args);
        }


        //My Crappy Initalise.
        public override void Initialize()
        {
            Lua.Events.AttachEvent("DELETE_ITEM_CONFIRM", DeleteItemConfirmPopup);
            Lua.Events.AttachEvent("MERCHANT_SHOW", SellVenderItems);
            Lua.Events.AttachEvent("LOOT_CLOSED", LootEnded);
            
            slog("Initial Loading of Item names.");
            initialMIRLoad();
            MrItemRemover2Settings.Instance.Load();
            CheckTimer.Reset(); //should start the timer 

            IsInitialized = true;
        }

        public bool ManualCheckRequested { get; set; }
       
        private WaitTimer CheckTimer = new WaitTimer(TimeSpan.FromMinutes(MrItemRemover2Settings.Instance.Time));
        private bool EnableCheck { get; set; }
        private bool IsInitialized { get; set; }
        private static LocalPlayer Me { get { return StyxWoW.Me; } }
   
        public override void Pulse()
        {
            if (ManualCheckRequested)
            {
                EnableCheck = true;
                ManualCheckRequested = false;
                CheckTimer.Reset();

                slog("Checking Bags Manually & Reloading Item Lists.");
            } 
            
            else if (!MrItemRemover2Settings.Instance.LootEnable)
            {
                if (CheckTimer.TimeLeft.Ticks <= 0)
                {
                    if (EnableCheck == false)
                    {
                        EnableCheck = true;
                        CheckTimer.Reset();

                        slog("Enabling Check at {0}", GetTime(DateTime.Now));
                        dlog("Checktimer has Finished its Total wait of {0} Minutes, Enabling Item Check for next Opportunity", MrItemRemover2Settings.Instance.Time.ToString());
                        slog("Will Run Next Check At {0}", GetTime(CheckTimer.EndTime));
                    }
                }
            }

    
            if (!Me.Combat && !Me.IsCasting && !Me.IsDead && !Me.IsGhost && EnableCheck)
            {
                dlog("EnableCheck was Passed!");
                if (MrItemRemover2Settings.Instance.EnableOpen)
                {
                    OpenBagItems();
                }
                if (MrItemRemover2Settings.Instance.EnableRemove)
                {
                    CheckForItems();
                }
                EnableCheck = false;
                dlog("Turning off Check Since Done!");
            }            
        }

        private void LootEnded(object sender, LuaEventArgs args)
        {
            if (MrItemRemover2Settings.Instance.LootEnable)
            {
                if (EnableCheck == false)
                {
                    EnableCheck = true;
                    dlog("Enabling Check because Loot Ended");
                }
            }
        }

        //All items from the TXT Doc are loaded here.
        public List<string> _ItemName = new List<string>
        {

        };

        //Specific items from the TXT Doc are loaded here.
        public List<string> _ItemNameSell = new List<string>
        {

        };

        public List<string> _InventoryList = new List<string>
        {

        };

        public List<string> _KeepList = new List<string>
        {

        };

        public List<string> _OpnList = new List<string>
        {

        };

        //file Path for Saving and Loading. 
        private string RemoveListPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                                              string.Format(@"Plugins/MrItemRemover2/ItemNameRemoveList.txt"));
        private string SellListPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                                           string.Format(@"Plugins/MrItemRemover2/ItemNameSellList.txt"));
        private string KeepListPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                                           string.Format(@"Plugins/MrItemRemover2/ItemNameKeepList.txt"));
        private string OpnListPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                                           string.Format(@"Plugins/MrItemRemover2/ItemNameOpnList.txt"));

        public void initialMIRLoad()
        {
            slog("Initial Loading of Items to Remove List.");
            LoadList(_ItemName, RemoveListPath);
            slog("Initial Loading of Items to Sell List.");
            LoadList(_ItemNameSell, SellListPath);
            slog("Initial Loading of Items to Keep List.");
            LoadList(_KeepList, KeepListPath);
            slog("Initial Loading of Items to Open List.");
            LoadList(_OpnList, OpnListPath);
            slog("Initial Loading Complete!");
        }

        public void MIRLoad()
        {
            LoadList(_ItemName, RemoveListPath);
            LoadList(_ItemNameSell, SellListPath);
            LoadList(_KeepList, KeepListPath);
            LoadList(_OpnList, OpnListPath); 
        }

        public void LoadList(List<string> ListToLoad, string FilePath)
        {
            ListToLoad.Clear();
            try
            {
                StreamReader Read = new StreamReader(Convert.ToString(FilePath));
                while (Read.Peek() >= 0)
                {
                    ListToLoad.Add(Convert.ToString(Read.ReadLine()));
                }

                Read.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(Convert.ToString(ex.Message));
                return;
            }
        }

        public void MIRSave()
        {
            slog("Saving All Lists.");

            writeList(_ItemName, RemoveListPath);

            writeList(_ItemNameSell, SellListPath);

            writeList(_KeepList, KeepListPath);

            writeList(_OpnList, OpnListPath);
        }

        public void writeList(List<string> ListName, string filePath)
        {
            StreamWriter Write;
            try
            {
                Write = new StreamWriter(filePath);
                for (int I = 0; I < ListName.Count; I++)
                {
                    Write.WriteLine(Convert.ToString(ListName[I]));
                }
                Write.Close(); //dakkon for this fix.
            }
            catch (Exception ex)
            {
                MessageBox.Show(Convert.ToString(ex.Message));
                return;
            }
        }
    }
}

