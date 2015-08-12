// This file has been cleaned up to use with YRB, taken from https://github.com/xtuaok/ACT_EnmityPlugin

using System;

namespace YourRaidingBuddy.Helpers
{
    public enum ObjectType : byte
    {
        Unknown    = 0x00,
        Pc         = 0x01,
        Monster    = 0x02,
        Npc        = 0x03,
        Aetheryte  = 0x05,
        Gathering  = 0x06,
        Minion     = 0x09
    }

    public class Combatant
    {
        public uint Id;
        public uint OwnerId;
        public int Order;
        public ObjectType Type;
        public uint TargetId;

        public byte Job;
        public byte Level;
        public string Name;

        public int CurrentHp;
        public int MaxHp;
        public int CurrentMp;
        public int MaxMp;
        public short MaxTp;
        public short CurrentTp;

        public float PosX;
        public float PosY;
        public float PosZ;
        public byte EffectiveDistance;
        public string Distance;
        public string HorizontalDistance;

        public string HpPercent
        {
            get
            {
                try
                {
                    return MaxHp == 0 ? "0.00" : (Convert.ToDouble(CurrentHp) / Convert.ToDouble(MaxHp) * 100).ToString("0.00");
                }
                catch
                {
                    return "0.00";
                }
            }
        }

        public float GetDistanceTo(Combatant target)
        {
            var distanceX = Math.Abs(PosX - target.PosX);
            var distanceY = Math.Abs(PosY - target.PosY);
            var distanceZ = Math.Abs(PosZ - target.PosZ);
            return (float)Math.Sqrt((distanceX * distanceX) + (distanceY * distanceY) + (distanceZ * distanceZ));
        }

        public float GetHorizontalDistanceTo(Combatant target)
        {
            var distanceX = Math.Abs(PosX - target.PosX);
            var distanceY = Math.Abs(PosY - target.PosY);
            return (float)Math.Sqrt((distanceX * distanceX) + (distanceY * distanceY));
        }
    }

    ///
    /// Job enum
    ///
    public enum JobEnum : byte
    {
        Unknown,
        Gld, // 1
        Pgl, // 2
        Mrd, // 3
        Lnc, // 4
        Arc, // 5
        Cnj, // 6
        Thm, // 7
        Crp, // 8
        Bsm, // 9
        Arm, // 10
        Gsm, // 11
        Ltw, // 12
        Wvr, // 13
        Alc, // 14
        Cul, // 15
        Min, // 15
        Btn, // 17
        Fsh, // 18
        Pld, // 19
        Mnk, // 20
        War, // 21
        Drg, // 22
        Brd, // 23
        Whm, // 24
        Blm, // 25
        Acn, // 26
        Smn, // 27
        Sch, // 28
        Rog, // 29
        Nin, // 30
        Mch, // 31
        Drk, // 32
        Ast  // 33
    }

    //// 敵視されてるキャラエントリ
    public class EnmityEntry
    {
        public uint Id;
        public uint OwnerId;
        public string Name;
        public uint Enmity;
        public bool IsMe;
        public int HateRate;
        public byte Job;
        public string JobName
        {
            get
            {
                return Enum.GetName(typeof(JobEnum), Job);
            }
        }
        public string EnmityString
        {
            get
            {
                return Enmity.ToString("##,#");
            }
        }
        public bool IsPet
        {
            get
            {
                return (OwnerId != 0);
            }
        }
    }
  
    //// 敵視リストエントリ
    public class AggroEntry
    {
        public uint Id;
        public string Name;
        public int HateRate;
        public int Order;
        public bool IsCurrentTarget;

        public int CurrentHp;
        public int MaxHp;
        public string HpPercent
        {
            get
            {
                try
                {
                    return MaxHp == 0 ? "0.00" : (Convert.ToDouble(CurrentHp) / Convert.ToDouble(MaxHp) * 100).ToString("0.00");
                }
                catch
                {
                    return "0.00";
                }
            }
        }

        // Target of Enemy
        public EnmityEntry Target;
    }
}
