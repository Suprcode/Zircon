using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MirDB;

namespace Server.DBModels
{
    [UserObject]
    public sealed class GameGoldPayment : DBObject
    {
        public string RawMessage
        {
            get { return _RawMessage; }
            set
            {
                if (_RawMessage == value) return;

                var oldValue = _RawMessage;
                _RawMessage = value;

                OnChanged(oldValue, value, "RawMessage");
            }
        }
        private string _RawMessage;

        public string CharacterName
        {
            get { return _CharacterName; }
            set
            {
                if (_CharacterName == value) return;

                var oldValue = _CharacterName;
                _CharacterName = value;

                OnChanged(oldValue, value, "CharacterName");
            }
        }
        private string _CharacterName;

        public string Name
        {
            get { return _Name; }
            set
            {
                if (_Name == value) return;

                var oldValue = _Name;
                _Name = value;

                OnChanged(oldValue, value, "Name");
            }
        }
        private string _Name;

        public string PaymentDate
        {
            get { return _PaymentDate; }
            set
            {
                if (_PaymentDate == value) return;

                var oldValue = _PaymentDate;
                _PaymentDate = value;

                OnChanged(oldValue, value, "PaymentDate");
            }
        }
        private string _PaymentDate;
        
        [Association("Payments")]
        public AccountInfo Account
        {
            get { return _Account; }
            set
            {
                if (_Account == value) return;

                var oldValue = _Account;
                _Account = value;

                OnChanged(oldValue, value, "Account");
            }
        }
        private AccountInfo _Account;

        public string TransactionID
        {
            get { return _TransactionID; }
            set
            {
                if (_TransactionID == value) return;

                var oldValue = _TransactionID;
                _TransactionID = value;

                OnChanged(oldValue, value, "TransactionID");
            }
        }
        private string _TransactionID;

        public string TransactionType
        {
            get { return _TransactionType; }
            set
            {
                if (_TransactionType == value) return;

                var oldValue = _TransactionType;
                _TransactionType = value;

                OnChanged(oldValue, value, "TransactionType");
            }
        }
        private string _TransactionType;

        public string Status
        {
            get { return _Status; }
            set
            {
                if (_Status == value) return;

                var oldValue = _Status;
                _Status = value;

                OnChanged(oldValue, value, "Status");
            }
        }
        private string _Status;

        public long GameGoldAmount
        {
            get { return _GameGoldAmount; }
            set
            {
                if (_GameGoldAmount == value) return;

                var oldValue = _GameGoldAmount;
                _GameGoldAmount = value;

                OnChanged(oldValue, value, "GameGoldAmount");
            }
        }
        private long _GameGoldAmount;

        public string Receiver_EMail
        {
            get { return _Receiver_EMail; }
            set
            {
                if (_Receiver_EMail == value) return;

                var oldValue = _Receiver_EMail;
                _Receiver_EMail = value;

                OnChanged(oldValue, value, "Receiver_EMail");
            }
        }
        private string _Receiver_EMail;

        public string Payer_EMail
        {
            get { return _Payer_EMail; }
            set
            {
                if (_Payer_EMail == value) return;

                var oldValue = _Payer_EMail;
                _Payer_EMail = value;

                OnChanged(oldValue, value, "Payer_EMail");
            }
        }
        private string _Payer_EMail;
        

        public string Payer_ID
        {
            get { return _Payer_ID; }
            set
            {
                if (_Payer_ID == value) return;

                var oldValue = _Payer_ID;
                _Payer_ID = value;

                OnChanged(oldValue, value, "Payer_ID");
            }
        }
        private string _Payer_ID;

        public decimal Price
        {
            get { return _Price; }
            set
            {
                if (_Price == value) return;

                var oldValue = _Price;
                _Price = value;

                OnChanged(oldValue, value, "Price");
            }
        }
        private decimal _Price;

        public string Currency
        {
            get { return _Currency; }
            set
            {
                if (_Currency == value) return;

                var oldValue = _Currency;
                _Currency = value;

                OnChanged(oldValue, value, "Currency");
            }
        }
        private string _Currency;

        public decimal Fee
        {
            get { return _Fee; }
            set
            {
                if (_Fee == value) return;

                var oldValue = _Fee;
                _Fee = value;

                OnChanged(oldValue, value, "Fee");
            }
        }
        private decimal _Fee;

        public bool Error
        {
            get { return _Error; }
            set
            {
                if (_Error == value) return;

                var oldValue = _Error;
                _Error = value;

                OnChanged(oldValue, value, "Error");
            }
        }
        private bool _Error;


        protected override void OnDeleted()
        {
            Account = null;
            
            base.OnDeleted();
        }

    }
}
