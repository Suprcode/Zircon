using System.Collections.Generic;
using System.Drawing;
using Library;
using MirDB;

namespace Server.DBModels
{
    public sealed class MapInfo : DBObject
    {
        public string FileName
        {
            get { return _FileName; }
            set
            {
                if (_FileName == value) return;

                var oldValue = _FileName;
                _FileName = value;

                OnChanged(oldValue, value, "FileName");
            }
        }
        private string _FileName;

        public string Description
        {
            get { return _Description; }
            set
            {
                if (_Description == value) return;

                var oldValue = _Description;
                _Description = value;

                OnChanged(oldValue, value, "Description");
            }
        }
        private string _Description;

        public int MiniMap
        {
            get { return _MiniMap; }
            set
            {
                if (_MiniMap == value) return;

                var oldValue = _MiniMap;
                _MiniMap = value;

                OnChanged(oldValue, value, "MiniMap");
            }
        }
        private int _MiniMap;

        public LightSetting Light
        {
            get { return _Light; }
            set
            {
                if (_Light == value) return;

                var oldValue = _Light;
                _Light = value;

                OnChanged(oldValue, value, "Light");
            }
        }
        private LightSetting _Light;

        public bool AllowRT
        {
            get { return _AllowRT; }
            set
            {
                if (_AllowRT == value) return;

                var oldValue = _AllowRT;
                _AllowRT = value;

                OnChanged(oldValue, value, "AllowRT");
            }
        }
        private bool _AllowRT;

        [Association("SafeZones", true)]
        public DBBindingList<SafeZoneInfo> SafeZones { get; set; }

        [Association("Movements", true)]
        public DBBindingList<MovementInfo> Movements { get; set; }

        [Association("Respawns", true)]
        public DBBindingList<RespawnInfo> Respawns { get; set; }

        [Association("NPCs", true)]
        public DBBindingList<NPCInfo> NPCs { get; set; }

        [Association("Guards", true)]
        public DBBindingList<GuardInfo> Guards { get; set; }

        protected internal override void OnCreated()
        {
            base.OnCreated();

            AllowRT = true;
            //AllowRecall
        }
    }
}
