using System;
using System.IO;
using Styx;
using Styx.Helpers;

namespace MrItemRemover2
{
    public class MrItemRemover2Settings : Settings
    {
        public static readonly MrItemRemover2Settings Instance = new MrItemRemover2Settings();

        private MrItemRemover2Settings()
            : base(
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Settings/MrItemRemover2_Settings_{0}.xml",
                             StyxWoW.Me.Name))
        {
        }

        [Setting, DefaultValue(5)]
        public int Time { get; set; }

        [Setting, DefaultValue(false)]
        public bool EnableRemove { get; set; }

        [Setting, DefaultValue(false)]
        public bool EnableOpen { get; set; }

        [Setting, DefaultValue(false)]
        public bool DeleteQuestItems { get; set; }

        [Setting, DefaultValue(false)]
        public bool EnableSell { get; set; }

        [Setting, DefaultValue(true)]
        public bool EnableGray { get; set; }

        [Setting, DefaultValue(false)]
        public bool SellWhite { get; set; }

        [Setting, DefaultValue(false)]
        public bool SellGreen { get; set; }

        [Setting, DefaultValue(false)]
        public bool SellBlue { get; set; }

        [Setting, DefaultValue(false)]
        public bool SellSoulbound { get; set; }

        [Setting, DefaultValue(false)]
        public bool SellGray { get; set; }

        [Setting, DefaultValue(true)]
        public bool ApplyAll { get; set; }

        [Setting, DefaultValue(true)]
        public bool DeleteAllGray { get; set; }

        [Setting, DefaultValue(false)]
        public bool DeleteAllWhite { get; set; }

        [Setting, DefaultValue(false)]
        public bool DeleteAllGreen { get; set; }

        [Setting, DefaultValue(false)]
        public bool DeleteAllBlue { get; set; }

        [Setting, DefaultValue(false)]
        public bool LootEnable { get; set; }

        [Setting, DefaultValue(0)]
        public int GoldGrays { get; set; }

        [Setting, DefaultValue(0)]
        public int SilverGrays { get; set; }

        [Setting, DefaultValue(1)]
        public int CopperGrays { get; set; }
    }
}