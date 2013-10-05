using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Styx;
using Styx.WoWInternals.WoWObjects;

namespace MrItemRemover2.GUI
{
    public partial class MrItemRemover2GUI : Form
    {
        private readonly string GoldImangePathName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                                                                  string.Format(@"Plugins/MrItemRemover2/Gold2.bmp"));

        private readonly string refreshImangePathName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                                                                     string.Format(@"Plugins/MrItemRemover2/ref.bmp"));

        public MrItemRemover2GUI(MrItemRemover2 controller)
        {
            Controller = controller;
            InitializeComponent();
        }

        public MrItemRemover2 Controller { get; private set; }

        public static void slog(string format, params object[] args)
        {
            MrItemRemover2.slog(format, args);
        }

        private void MrItemRemover2GUI_Load(object sender, EventArgs e)
        {
            var refresh = new Bitmap(refreshImangePathName);
            var GoldImg = new Bitmap(GoldImangePathName);
            GoldBox.Image = GoldImg;
            resf.Image = refresh;
            Controller.MIRLoad();
            MrItemRemover2Settings.Instance.Load();
            SellList.Items.Clear();
            RemoveList.Items.Clear();
            ProtectedList.Items.Clear();
            GrayItems.Checked = MrItemRemover2Settings.Instance.DeleteAllGray;
            WhiteItems.Checked = MrItemRemover2Settings.Instance.DeleteAllWhite;
            GreenItems.Checked = MrItemRemover2Settings.Instance.DeleteAllGreen;
            BlueItems.Checked = MrItemRemover2Settings.Instance.DeleteAllBlue;
            SellGray.Checked = MrItemRemover2Settings.Instance.SellGray;
            //ApplyAll.Checked = MrItemRemover2Settings.Instance.ApplyAll;
            SellWhite.Checked = MrItemRemover2Settings.Instance.SellWhite;
            SellGreen.Checked = MrItemRemover2Settings.Instance.SellGreen;
            SellBlue.Checked = MrItemRemover2Settings.Instance.SellBlue;
            SellSoulbound.Checked = MrItemRemover2Settings.Instance.SellSoulbound;
            EnableRemove.Checked = MrItemRemover2Settings.Instance.EnableRemove;
            EnableOpen.Checked = MrItemRemover2Settings.Instance.EnableOpen;
            EnableSell.Checked = MrItemRemover2Settings.Instance.EnableSell;
            RemoveQItems.Checked = MrItemRemover2Settings.Instance.DeleteQuestItems;
            LootEnable.Checked = MrItemRemover2Settings.Instance.LootEnable;
            GoldGrays.Text = MrItemRemover2Settings.Instance.GoldGrays.ToString();
            SilverGrays.Text = MrItemRemover2Settings.Instance.SilverGrays.ToString();
            CopperGrays.Text = MrItemRemover2Settings.Instance.CopperGrays.ToString();
            Time.Value = MrItemRemover2Settings.Instance.Time;
            foreach (string itm in Controller._ItemName)
            {
                RemoveList.Items.Add(itm);
            }
            foreach (string itm in Controller._ItemNameSell)
            {
                SellList.Items.Add(itm);
            }
            foreach (string itm in Controller._KeepList)
            {
                ProtectedList.Items.Add(itm);
            }
            foreach (string itm in Controller._OpnList)
            {
                OpnList.Items.Add(itm);
            }

            foreach (WoWItem BagItem in StyxWoW.Me.BagItems)
            {
                if (BagItem.IsValid && BagItem.BagSlot != -1 && !MyBag.Items.Contains(BagItem.Name))
                {
                    MyBag.Items.Add(BagItem.Name);
                }
            }
        }

        private void AddToBagList_Click(object sender, EventArgs e)
        {
            if (InputAddToBagItem.Text != null)
            {
                MyBag.Items.Add(InputAddToBagItem.Text);
                slog("Added {0} to Inventory List", InputAddToBagItem.Text);
            }
        }

        private void RefreshBagItems_Click(object sender, EventArgs e)
        {
            MyBag.Items.Clear();
            foreach (WoWItem BagItem in StyxWoW.Me.BagItems)
            {
                if (BagItem.BagSlot != -1 && !MyBag.Items.Contains(BagItem.Name))
                {
                    MyBag.Items.Add(BagItem.Name);
                }
            }
        }

        private void AddToSellList_Click(object sender, EventArgs e)
        {
            if (MyBag.SelectedItems[0] != null)
            {
                SellList.Items.Add(MyBag.SelectedItem);
                Controller._ItemNameSell.Add(MyBag.SelectedItem.ToString());
            }
        }

        private void AddToRemoveList_Click(object sender, EventArgs e)
        {
            if (MyBag.SelectedItems[0] != null)
            {
                RemoveList.Items.Add(MyBag.SelectedItem);
                Controller._ItemName.Add(MyBag.SelectedItem.ToString());
            }
        }

        private void AddToProtList_Click(object sender, EventArgs e)
        {
            if (MyBag.SelectedItems[0] != null)
            {
                ProtectedList.Items.Add(MyBag.SelectedItem);
                Controller._KeepList.Add(MyBag.SelectedItem.ToString());
            }
        }

        private void RemoveSellItem_Click(object sender, EventArgs e)
        {
            if (SellList.SelectedItem != null)
            {
                slog("{0} Removed", SellList.SelectedItem.ToString());
                Controller._ItemNameSell.Remove(SellList.SelectedItem.ToString());
                SellList.Items.Remove(SellList.SelectedItem);
            }
        }

        private void RemoveRemoveItem_Click(object sender, EventArgs e)
        {
            if (RemoveList.SelectedItem != null)
            {
                slog("{0} Removed", RemoveList.SelectedItem.ToString());
                Controller._ItemName.Remove(RemoveList.SelectedItem.ToString());
                RemoveList.Items.Remove(RemoveList.SelectedItem);
            }
        }

        private void RemoveProtectedItem_Click(object sender, EventArgs e)
        {
            if (ProtectedList.SelectedItem != null)
            {
                slog("{0} Removed", ProtectedList.SelectedItem.ToString());
                Controller._KeepList.Remove(ProtectedList.SelectedItem.ToString());
                ProtectedList.Items.Remove(ProtectedList.SelectedItem);
            }
        }

        private void Save_Click(object sender, EventArgs e)
        {
            Controller.MIRSave();
            MrItemRemover2Settings.Instance.Save();
        }

        private void GrayItems_CheckedChanged(object sender, EventArgs e)
        {
            MrItemRemover2Settings.Instance.DeleteAllGray = GrayItems.Checked;
        }

        private void WhiteItems_CheckedChanged(object sender, EventArgs e)
        {
            MrItemRemover2Settings.Instance.DeleteAllWhite = WhiteItems.Checked;
        }

        private void GreenItems_CheckedChanged(object sender, EventArgs e)
        {
            MrItemRemover2Settings.Instance.DeleteAllGreen = GreenItems.Checked;
        }

        private void BlueItems_CheckedChanged(object sender, EventArgs e)
        {
            MrItemRemover2Settings.Instance.DeleteAllBlue = BlueItems.Checked;
        }

        private void RemoveQItems_CheckedChanged(object sender, EventArgs e)
        {
            MrItemRemover2Settings.Instance.DeleteQuestItems = RemoveQItems.Checked;
        }

        private void SellGray_CheckedChanged(object sender, EventArgs e)
        {
            MrItemRemover2Settings.Instance.SellGray = SellGray.Checked;
        }

        /* private void ApplyAll_CheckedChanged(object sender, EventArgs e)
        {
            MrItemRemover2Settings.Instance.ApplyAll = ApplyAll.Checked;
        }*/

        private void SellGreen_CheckedChanged(object sender, EventArgs e)
        {
            MrItemRemover2Settings.Instance.SellGreen = SellGreen.Checked;
        }

        private void SellWhite_CheckedChanged(object sender, EventArgs e)
        {
            MrItemRemover2Settings.Instance.SellWhite = SellWhite.Checked;
        }

        private void EnableSell_CheckedChanged(object sender, EventArgs e)
        {
            MrItemRemover2Settings.Instance.EnableSell = EnableSell.Checked;
        }

        private void EnableRemove_CheckedChanged(object sender, EventArgs e)
        {
            MrItemRemover2Settings.Instance.EnableRemove = EnableRemove.Checked;
        }

        private void Time_ValueChanged(object sender, EventArgs e)
        {
            MrItemRemover2Settings.Instance.Time = int.Parse(Time.Value.ToString());
        }

        private void GoldGrays_TextChanged(object sender, EventArgs e)
        {
            MrItemRemover2Settings.Instance.GoldGrays = int.Parse(GoldGrays.Text);
        }

        private void SilverGrays_TextChanged(object sender, EventArgs e)
        {
            MrItemRemover2Settings.Instance.SilverGrays = int.Parse(SilverGrays.Text);
        }

        private void CopperGrays_TextChanged(object sender, EventArgs e)
        {
            MrItemRemover2Settings.Instance.CopperGrays = int.Parse(CopperGrays.Text);
        }

        private void RemoveOpenItem_Click(object sender, EventArgs e)
        {
            if (OpnList.SelectedItem != null)
            {
                slog("{0} Removed", OpnList.SelectedItem.ToString());
                Controller._OpnList.Remove(OpnList.SelectedItem.ToString());
                OpnList.Items.Remove(OpnList.SelectedItem);
            }
        }

        private void EnableOpen_CheckedChanged(object sender, EventArgs e)
        {
            MrItemRemover2Settings.Instance.EnableOpen = EnableOpen.Checked;
        }

        private void resf_Click(object sender, EventArgs e)
        {
            MyBag.Items.Clear();
            foreach (WoWItem BagItem in StyxWoW.Me.BagItems)
            {
                if (BagItem.BagSlot != -1 && !MyBag.Items.Contains(BagItem.Name))
                {
                    MyBag.Items.Add(BagItem.Name);
                }
            }
        }

        private void AddToOpnList_Click(object sender, EventArgs e)
        {
            if (MyBag.SelectedItems[0] != null)
            {
                OpnList.Items.Add(MyBag.SelectedItem);
                Controller._OpnList.Add(MyBag.SelectedItem.ToString());
            }
        }

        private void Run_Click(object sender, EventArgs e)
        {
            Controller.ManualCheckRequested = true;
        }

        private void LootEnable_CheckedChanged(object sender, EventArgs e)
        {
            MrItemRemover2Settings.Instance.LootEnable = LootEnable.Checked;
        }

        private void SellList_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void OpnList_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void groupBox3_Enter(object sender, EventArgs e)
        {
        }

        private void SellBlue_CheckedChanged_1(object sender, EventArgs e)
        {
            MrItemRemover2Settings.Instance.SellBlue = SellBlue.Checked;
        }

        private void label5_Click(object sender, EventArgs e)
        {
        }

        private void SellSoulbound_CheckedChanged(object sender, EventArgs e)
        {
            MrItemRemover2Settings.Instance.SellSoulbound = SellSoulbound.Checked;
        }
    }
}