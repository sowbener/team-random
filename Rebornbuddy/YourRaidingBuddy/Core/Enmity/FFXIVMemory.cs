// This file has been cleaned up to use with YRB, taken from https://github.com/xtuaok/ACT_EnmityPlugin

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace YourRaidingBuddy.Helpers
{
    public class FfxivMemory : IDisposable
    {
        private const string CharmapSignature32 = "81FEFFFF0000743581FE58010000732D8B3CB5";
        private const string CharmapSignature64 = "48C1E8033DFFFF0000742B3DA80100007324488D0D";
        private const string TargetSignature32 = "750E85D2750AB9";
        private const string TargetSignature64 = "29017520483935";
        private const string EnmitySignature32 = "E831AA3300B9";
        private const string EnmitySignature64 = "488D0D????????E8D0F23F00488D0D";
        private const int CharmapOffset32 = 0;
        private const int CharmapOffset64 = 0;
        private const int TargetOffset32 = 88;
        private const int TargetOffset64 = 0;
        private const int EnmityOffset32 = 0;
        private const int EnmityOffset64 = 0;

        private readonly FfxivClientMode _mode;
        private readonly Thread _thread;
        private IntPtr _aggroAddress = IntPtr.Zero;
        private IntPtr _charmapAddress = IntPtr.Zero;
        private IntPtr _enmityAddress = IntPtr.Zero;
        private IntPtr _targetAddress = IntPtr.Zero;
        private readonly object _combatantsLock;
        private readonly Process _process;

        public FfxivMemory(Process process)
        {
            _process = process;
            switch (process.ProcessName)
            {
                case "ffxiv":
                    _mode = FfxivClientMode.Ffxiv32;
                    break;
                case "ffxiv_dx11":
                    _mode = FfxivClientMode.Ffxiv64;
                    break;
                default:
                    _mode = FfxivClientMode.Unknown;
                    break;
            }

            GetPointerAddress();

            Combatants = new List<Combatant>();
            _combatantsLock = new object();

            _thread = new Thread(DoScanCombatants) {IsBackground = true};
            _thread.Start();
        }

        

        private List<Combatant> Combatants { get; set; }
        private object CombatantsLock { get { return _combatantsLock; } }
        public Process Process {get { return _process; }}

        public void Dispose()
        {
            _thread.Abort();
        }

        private void DoScanCombatants()
        {
            while (true)
            {
                Thread.Sleep(200);

                if (!ValidateProcess())
                {
                    Thread.Sleep(1000);
                    return;
                }

                var c = _getCombatantList();
                lock (CombatantsLock)
                {
                    Combatants = c;
                }
            }
        }

        private bool ValidateProcess()
        {
            if (Process == null)
            {
                return false;
            }
            if (Process.HasExited)
            {
                return false;
            }
            if (_charmapAddress == IntPtr.Zero ||
                _enmityAddress == IntPtr.Zero ||
                _targetAddress == IntPtr.Zero)
            {
                return GetPointerAddress();
            }
            return true;
        }

        /// <summary>
        ///     各ポインタのアドレスを取得
        /// </summary>
        private bool GetPointerAddress()
        {
            var success = true;
            var charmapSignature = CharmapSignature32;
            var targetSignature = TargetSignature32;
            var enmitySignature = EnmitySignature32;
            var targetOffset = TargetOffset32;
            var charmapOffset = CharmapOffset32;
            var enmityOffset = EnmityOffset32;

            var bRip = false;

            if (_mode == FfxivClientMode.Ffxiv64)
            {
                bRip = true;
                targetOffset = TargetOffset64;
                charmapOffset = CharmapOffset64;
                targetSignature = TargetSignature64;
                charmapSignature = CharmapSignature64;
                enmitySignature = EnmitySignature64;
                enmityOffset = EnmityOffset64;
            }

            // CHARMAP
            var list = SigScan(charmapSignature, 0, bRip);
            if (list == null || list.Count == 0)
            {
                _charmapAddress = IntPtr.Zero;
            }
            if (list != null && list.Count == 1)
            {
                _charmapAddress = list[0] + charmapOffset;
            }
            if (_charmapAddress == IntPtr.Zero)
            {
                success = false;
            }

            // ENMITY
            list = SigScan(enmitySignature, 0, bRip);
            if (list == null || list.Count == 0)
            {
                _enmityAddress = IntPtr.Zero;
            }
            if (list != null && list.Count == 1)
            {
                _enmityAddress = list[0] + enmityOffset;
                _aggroAddress = IntPtr.Add(_enmityAddress, 0x900 + 8);
            }
            if (_enmityAddress == IntPtr.Zero)
            {
                success = false;
            }

            // TARGET
            list = SigScan(targetSignature, 0, bRip);
            if (list == null || list.Count == 0)
            {
                _targetAddress = IntPtr.Zero;
            }
            if (list != null && list.Count == 1)
            {
                _targetAddress = list[0] + targetOffset;
            }
            if (_targetAddress == IntPtr.Zero)
            {
                success = false;
            }

            if (success || _charmapAddress != IntPtr.Zero)
            {
                var c = GetSelfCombatant();
                if (c != null)
                {
                }
            }
            return success;
        }

        /// <summary>
        ///     カレントターゲットの情報を取得
        /// </summary>
        public Combatant GetTargetCombatant()
        {
            IntPtr address;

            var source = GetByteArray(_targetAddress, 128);
            unsafe
            {
                if (_mode == FfxivClientMode.Ffxiv64)
                {
                    fixed (byte* p = source) address = new IntPtr(*(long*) p);
                }
                else
                {
                    fixed (byte* p = source) address = new IntPtr(*(int*) p);
                }
            }
            if (address.ToInt64() <= 0)
            {
                return null;
            }

            source = GetByteArray(address, 0x3F40);
            var target = GetCombatantFromByteArray(source);
            return target;
        }

        /// <summary>
        ///     自キャラの情報を取得
        /// </summary>
        private Combatant GetSelfCombatant()
        {
            Combatant self = null;
            var address = (IntPtr) GetUInt32(_charmapAddress);
            if (address.ToInt64() > 0)
            {
                var source = GetByteArray(address, 0x3F40);
                self = GetCombatantFromByteArray(source);
            }
            return self;
        }

        /// <summary>
        ///     サークルターゲット情報を取得
        /// </summary>
        public Combatant GetAnchorCombatant()
        {
            Combatant self = null;
            var offset = _mode == FfxivClientMode.Ffxiv64 ? 0x08 : 0x04;
            var address = (IntPtr) GetUInt32(_targetAddress + offset);
            if (address.ToInt64() > 0)
            {
                var source = GetByteArray(address, 0x3F40);
                self = GetCombatantFromByteArray(source);
            }
            return self;
        }

        /// <summary>
        ///     フォーカスターゲット情報を取得
        /// </summary>
        public Combatant GetFocusCombatant()
        {
            Combatant self = null;
            var offset = _mode == FfxivClientMode.Ffxiv64 ? 0x70 : 0x48;
            var address = (IntPtr) GetUInt32(_targetAddress + offset);
            if (address.ToInt64() > 0)
            {
                var source = GetByteArray(address, 0x3F40);
                self = GetCombatantFromByteArray(source);
            }
            return self;
        }

        /// <summary>
        ///     ホバーターゲット情報を取得
        /// </summary>
        public Combatant GetHoverCombatant()
        {
            Combatant self = null;
            var offset = _mode == FfxivClientMode.Ffxiv64 ? 0x30 : 0x18;
            var address = (IntPtr) GetUInt32(_targetAddress + offset);
            if (address.ToInt64() > 0)
            {
                var source = GetByteArray(address, 0x3F40);
                self = GetCombatantFromByteArray(source);
            }
            return self;
        }

        /// <summary>
        ///     周辺のキャラ情報を取得
        /// </summary>
        private unsafe List<Combatant> _getCombatantList()
        {
            var num = 344;
            var result = new List<Combatant>();

            var sz = (_mode == FfxivClientMode.Ffxiv64) ? 8 : 4;
            var source = GetByteArray(_charmapAddress, sz*num);
            if (source == null || source.Length == 0)
            {
                return result;
            }

            for (var i = 0; i < num; i++)
            {
                IntPtr p;
                if (_mode == FfxivClientMode.Ffxiv64)
                {
                    fixed (byte* bp = source) p = new IntPtr(*(long*) &bp[i*sz]);
                }
                else
                {
                    fixed (byte* bp = source) p = new IntPtr(*(int*) &bp[i*sz]);
                }

                if (!(p == IntPtr.Zero))
                {
                    var c = GetByteArray(p, 0x3F40);
                    var combatant = GetCombatantFromByteArray(c);
                    if (combatant.Type != ObjectType.Pc && combatant.Type != ObjectType.Monster)
                    {
                        continue;
                    }
                    if (combatant.Id != 0 && combatant.Id != 3758096384u &&
                        !result.Exists(x => x.Id == combatant.Id))
                    {
                        combatant.Order = i;
                        result.Add(combatant);
                    }
                }
            }

            return result;
        }

        /// <summary>
        ///     メモリのバイト配列からキャラ情報に変換
        /// </summary>
        private unsafe Combatant GetCombatantFromByteArray(byte[] source)
        {
            var combatant = new Combatant();
            fixed (byte* p = source)
            {
                combatant.Name = GetStringFromBytes(source, 48);
                combatant.Id = *(uint*) &p[0x74];
                combatant.OwnerId = *(uint*) &p[0x84];
                if (combatant.OwnerId == 3758096384u)
                {
                    combatant.OwnerId = 0u;
                }
                combatant.Type = (ObjectType) p[0x8A];
                combatant.EffectiveDistance = p[0x91];

                var offset = (_mode == FfxivClientMode.Ffxiv64) ? 176 : 160;
                combatant.PosX = *(float*) &p[offset];
                combatant.PosZ = *(float*) &p[offset + 4];
                combatant.PosY = *(float*) &p[offset + 8];

                offset = (_mode == FfxivClientMode.Ffxiv64) ? 448 : 392;
                combatant.TargetId = *(uint*) &p[offset];
                if (combatant.TargetId == 3758096384u)
                {
                    offset = (_mode == FfxivClientMode.Ffxiv64) ? 2448 : 2520;
                    combatant.TargetId = *(uint*) &p[offset];
                }

                if (combatant.Type == ObjectType.Pc || combatant.Type == ObjectType.Monster)
                {
                    offset = (_mode == FfxivClientMode.Ffxiv64) ? 5872 : 5312;
                    combatant.Job = p[offset];
                    combatant.Level = p[offset + 1];
                    combatant.CurrentHp = *(int*) &p[offset + 8];
                    combatant.MaxHp = *(int*) &p[offset + 12];
                    combatant.CurrentMp = *(int*) &p[offset + 16];
                    combatant.MaxMp = *(int*) &p[offset + 20];
                    combatant.CurrentTp = *(short*) &p[offset + 24];
                    combatant.MaxTp = 1000;
                }
                else
                {
                    combatant.CurrentHp =
                        combatant.MaxHp =
                            combatant.CurrentMp =
                                combatant.MaxMp =
                                    combatant.MaxTp =
                                        combatant.CurrentTp = 0;
                }
            }
            return combatant;
        }

        /// <summary>
        ///     カレントターゲットの敵視情報を取得
        /// </summary>
        public unsafe List<EnmityEntry> GetEnmityEntryList()
        {
            short num;
            uint topEnmity = 0;
            var result = new List<EnmityEntry>();
            var combatantList = Combatants;
            var mychar = GetSelfCombatant();

            // 一度に全部読む
            var buffer = GetByteArray(_enmityAddress, 0x900 + 2);
            fixed (byte* p = buffer) num = p[0x900];

            if (num <= 0)
            {
                return result;
            }
            if (num > 32) num = 32;

            for (short i = 0; i < num; i++)
            {
                var p = i*72;
                uint id;
                uint enmity;

                fixed (byte* bp = buffer)
                {
                    id = *(uint*) &bp[p + 0x40];
                    enmity = *(uint*) &bp[p + 0x44];
                }
                var entry = new EnmityEntry
                {
                    Id = id,
                    Enmity = enmity,
                    IsMe = false,
                    Name = "Unknown",
                    Job = 0x00
                };
                if (entry.Id > 0)
                {
                    var c = combatantList.Find(x => x.Id == entry.Id);
                    if (c != null)
                    {
                        entry.Name = c.Name;
                        entry.Job = c.Job;
                        entry.OwnerId = c.OwnerId;
                    }
                    if (entry.Id == mychar.Id)
                    {
                        entry.IsMe = true;
                    }
                    if (topEnmity <= entry.Enmity)
                    {
                        topEnmity = entry.Enmity;
                    }
                    entry.HateRate = (int) ((entry.Enmity/(double) topEnmity)*100);
                    result.Add(entry);
                }
            }
            return result;
        }

        /// <summary>
        ///     敵視リスト情報を取得
        /// </summary>
        public unsafe List<AggroEntry> GetAggroList()
        {
            int num;
            var result = new List<AggroEntry>();
            var combatantList = Combatants;
            var mychar = GetSelfCombatant();

            // 一度に全部読む
            var buffer = GetByteArray(_aggroAddress, 32*72 + 2);

            fixed (byte* p = buffer) num = p[0x900];
            if (num <= 0)
            {
                return result;
            }
            if (num > 32) num = 32;

            // current target
            var currentTargetId = GetUInt32(_aggroAddress, -4);
            if (currentTargetId == 3758096384u) currentTargetId = 0;
            //
            for (var i = 0; i < num; i++)
            {
                var p = i*72;
                uint id;
                short enmity;

                fixed (byte* bp = buffer)
                {
                    id = *(uint*) &bp[p + 64];
                    enmity = bp[p + 68];
                }

                var entry = new AggroEntry
                {
                    Id = id,
                    HateRate = enmity,
                    Name = "Unknown"
                };
                if (entry.Id <= 0) continue;
                var c = combatantList.Find(x => x.Id == entry.Id);
                if (c != null)
                {
                    entry.Id = c.Id;
                    entry.Order = c.Order;
                    entry.IsCurrentTarget = (c.Id == currentTargetId);
                    entry.Name = c.Name;
                    entry.MaxHp = c.MaxHp;
                    entry.CurrentHp = c.CurrentHp;
                    if (c.TargetId > 0)
                    {
                        var t = combatantList.Find(x => x.Id == c.TargetId);
                        if (t != null)
                        {
                            entry.Target = new EnmityEntry
                            {
                                Id = t.Id,
                                Name = t.Name,
                                Job = t.Job,
                                OwnerId = t.OwnerId,
                                IsMe = mychar.Id == t.Id,
                                Enmity = 0,
                                HateRate = 0
                            };
                        }
                    }
                }
                result.Add(entry);
            }
            return result;
        }

        /// <summary>
        ///     バイト配列からUTF-8文字列に変換
        /// </summary>
        private static string GetStringFromBytes(byte[] source, int offset = 0, int size = 256)
        {
            var bytes = new byte[size];
            Array.Copy(source, offset, bytes, 0, size);
            var realSize = 0;
            for (var i = 0; i < size; i++)
            {
                if (bytes[i] != 0)
                {
                    continue;
                }
                realSize = i;
                break;
            }
            Array.Resize(ref bytes, realSize);
            return Encoding.UTF8.GetString(bytes);
        }

        /// <summary>
        ///     バッファの長さだけメモリを読み取ってバッファに格納
        /// </summary>
        private void Peek(IntPtr address, byte[] buffer)
        {
            var zero = IntPtr.Zero;
            var nSize = new IntPtr(buffer.Length);
            NativeMethods.ReadProcessMemory(Process.Handle, address, buffer, nSize, ref zero);
        }

        /// <summary>
        ///     メモリから指定された長さだけ読み取りバイト配列として返す
        /// </summary>
        /// <param name="address">読み取る開始アドレス</param>
        /// <param name="length">読み取る長さ</param>
        /// <returns></returns>
        private byte[] GetByteArray(IntPtr address, int length)
        {
            var data = new byte[length];
            Peek(address, data);
            return data;
        }

        /// <summary>
        ///     メモリから4バイト読み取り32ビットIntegerとして返す
        /// </summary>
        /// <param name="address">読み取る位置</param>
        /// <param name="offset">オフセット</param>
        /// <returns></returns>
        private unsafe int GetInt32(IntPtr address, int offset = 0)
        {
            int ret;
            var value = new byte[4];
            Peek(IntPtr.Add(address, offset), value);
            fixed (byte* p = &value[0]) ret = *(int*) p;
            return ret;
        }

        /// <summary>
        /// </summary>
        /// <param name="address"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        private unsafe uint GetUInt32(IntPtr address, int offset = 0)
        {
            uint ret;
            var value = new byte[4];
            Peek(IntPtr.Add(address, offset), value);
            fixed (byte* p = &value[0]) ret = *(uint*) p;
            return ret;
        }

        /// <summary>
        ///     Signature scan.
        ///     Read data at address which follow matched with the pattern and return it as a pointer.
        /// </summary>
        /// <param name="pattern">byte pattern signature</param>
        /// <param name="offset">offset to read</param>
        /// <param name="bRip">x64 rip relative addressing mode if true</param>
        /// <returns>the pointer addresses</returns>
        private List<IntPtr> SigScan(string pattern, int offset = 0, bool bRip = false)
        {
            if (pattern == null || pattern.Length%2 != 0)
            {
                return new List<IntPtr>();
            }

            var array = new byte?[pattern.Length/2];
            for (var i = 0; i < pattern.Length/2; i++)
            {
                var text = pattern.Substring(i*2, 2);
                if (text == "??")
                {
                    array[i] = null;
                }
                else
                {
                    array[i] = Convert.ToByte(text, 16);
                }
            }

            const int num = 4096;

            var moduleMemorySize = Process.MainModule.ModuleMemorySize;
            var baseAddress = Process.MainModule.BaseAddress;
            var intPtr = IntPtr.Add(baseAddress, moduleMemorySize);
            var intPtr2 = baseAddress;
            var array2 = new byte[num];
            var list = new List<IntPtr>();
            while (intPtr2.ToInt64() < intPtr.ToInt64())
            {
                var zero = IntPtr.Zero;
                var nSize = new IntPtr(num);
                if (IntPtr.Add(intPtr2, num).ToInt64() > intPtr.ToInt64())
                {
                    nSize = (IntPtr) (intPtr.ToInt64() - intPtr2.ToInt64());
                }
                if (NativeMethods.ReadProcessMemory(Process.Handle, intPtr2, array2, nSize, ref zero))
                {
                    var num2 = 0;
                    while (num2 < zero.ToInt64() - array.Length - offset - 4L + 1L)
                    {
                        var num3 = 0;
                        for (var j = 0; j < array.Length; j++)
                        {
                            if (!array[j].HasValue)
                            {
                                num3++;
                            }
                            else
                            {
                                if (array[j].Value != array2[num2 + j])
                                {
                                    break;
                                }
                                num3++;
                            }
                        }
                        if (num3 == array.Length)
                        {
                            IntPtr item;
                            if (bRip)
                            {
                                item = new IntPtr(BitConverter.ToInt32(array2, num2 + array.Length + offset));
                                item = new IntPtr(intPtr2.ToInt64() + num2 + array.Length + 4L + item.ToInt64());
                            }
                            else if (_mode == FfxivClientMode.Ffxiv64)
                            {
                                item = new IntPtr(BitConverter.ToInt64(array2, num2 + array.Length + offset));
                                item = new IntPtr(item.ToInt64());
                            }
                            else
                            {
                                item = new IntPtr(BitConverter.ToInt32(array2, num2 + array.Length + offset));
                                item = new IntPtr(item.ToInt64());
                            }
                            list.Add(item);
                        }
                        num2++;
                    }
                }
                intPtr2 = IntPtr.Add(intPtr2, num);
            }
            return list;
        }

        private enum FfxivClientMode
        {
            Unknown = 0,
            Ffxiv32 = 1,
            Ffxiv64 = 2
        }
    }
}