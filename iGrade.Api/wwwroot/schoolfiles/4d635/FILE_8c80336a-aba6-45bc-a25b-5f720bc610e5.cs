using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Payment24.BOL
{
    public class Card
    {
        #region Private Variables

        private CardType myCardType;
        private CardStatus myCardStatus;
        private string myCardNumber;
        private string myPIN;
        private string myPan;
        private DateTime myOrderDate;
        private DateTime myActivationDate;
        private DateTime myExpiryDate;
        private bool myEnableFleet;
        private bool myEnablePrepaid;
        private bool myEnableLoyalty;
        private bool myEnableMembership;
        private MerchantGroup myMerchantGroup;
        private string myCeatedBy;
        private int myCardId;
        private int myCustomerIssuedToId;
        private int myCustomerGroupId;
        private string myISONumber;
        private int myDriverId;

        #endregion

        #region Constructors

        public Card(int cardId)
        {
            InitialiseVariables();
            Load(cardId);
        }

        public Card(string cardNumber, string pan)
        {
            InitialiseVariables();
            Load(cardNumber, pan);
        }
        public Card(System.Data.DataRow row)
        {
            InitialiseVariables();
            Populate(row);
        }

        public Card()
        {
            InitialiseVariables();
        }

        #endregion

        #region Properties

        public string ISONumber
        {
            get { return myISONumber; }
            set { myISONumber = value; }
        }

        public int CustomerGroupId
        {
            get { return myCustomerGroupId; }
            set { myCustomerGroupId = value; }
        }

        public int CustomerIssuedToId
        {
            get { return myCustomerIssuedToId; }
            set { myCustomerIssuedToId = value; }
        }

        public int CardId
        {
            get { return myCardId; }
            set { myCardId = value; }
        }

        public CardType CardType
        {
            get { return myCardType; }
            set { myCardType = value; }
        }

        public CardStatus CardStatus
        {
            get { return myCardStatus; }
            set { myCardStatus = value; }
        }

        public string CardNumber
        {
            get { return myCardNumber; }
            set { myCardNumber = value; }
        }

        public string PIN
        {
            get { return myPIN; }
            set { myPIN = value; }
        }

        public string PAN
        {
            get { return myPan; }
            set { myPan = value; }
        }

        public DateTime OrderDate
        {
            get { return myOrderDate; }
            set { myOrderDate = value; }
        }

        public DateTime ActivationDate
        {
            get { return myActivationDate; }
            set { myActivationDate = value; }
        }

        public DateTime ExpiryDate
        {
            get { return myExpiryDate; }
            set { myExpiryDate = value; }
        }

        public bool EnableFleet
        {
            get { return myEnableFleet; }
            set { myEnableFleet = value; }
        }

        public bool EnablePrepaid
        {
            get { return myEnablePrepaid; }
            set { myEnablePrepaid = value; }
        }

        public bool EnableLoyalty
        {
            get { return myEnableLoyalty; }
            set { myEnableLoyalty = value; }
        }

        public bool EnableMembership
        {
            get { return myEnableMembership; }
            set { myEnableMembership = value; }
        }

        public MerchantGroup MerchantGroup
        {
            get { return myMerchantGroup; }
            set { myMerchantGroup = value; }
        }

        public string CeatedBy
        {
            get { return myCeatedBy; }
            set { myCeatedBy = value; }
        }

        public int DriverId
        {
            get { return myDriverId; }
            set { myDriverId = value; }
        }

        #endregion

        #region Methods

        public bool Save()
        {
            bool isSuccess = false;
            int id = Convert.ToInt32(this.myCardId);
            if (id > 0)
            {
                // Update an existing Draw
                DAL.Card.Update(
                                myCardType.CardTypeId,
                                myCardStatus.CardStatusId,
                                myCardNumber,
                                myPIN,
                                myPan,
                                myOrderDate,
                                myActivationDate,
                                myExpiryDate,
                                myEnableFleet,
                                myEnablePrepaid,
                                myEnableLoyalty,
                                myEnableMembership,
                                myMerchantGroup.MerchantGroupId,
                                myCustomerIssuedToId,
                                myCustomerGroupId,
                                myISONumber,
                                myCeatedBy,
                                myCardId,
                                myDriverId);
                isSuccess = true;


            }
            else
            {
                // insert new record
                id = DAL.Card.Insert(myCardType.CardTypeId,
                                    myCardStatus.CardStatusId,
                                    myCardNumber,
                                    myPIN,
                                    myPan,
                                    myOrderDate,
                                    myActivationDate,
                                    myExpiryDate,
                                    myEnableFleet,
                                    myEnablePrepaid,
                                    myEnableLoyalty,
                                    myEnableMembership,
                                    myMerchantGroup.MerchantGroupId,
                                    myCustomerIssuedToId,
                                    myCustomerGroupId,
                                    myISONumber,
                                    myCeatedBy,
                                    myDriverId);

                myCardId = id;
                if (myCardId > 0)
                {
                    isSuccess = true;
                }
            }

            return isSuccess;
        }

        public static bool SaveFleetCard(int cardId, int fleetCardId, string createdBy)
        {
            return DAL.Card.SaveFleetCard(cardId, fleetCardId, createdBy);
        }
        public static bool Delete(int cardId)
        {
            return DAL.Card.Delete(cardId);
        }

        public static IList<Card> LoadCardAll()
        {
            DataTable dt = DAL.Card.GetList(0);
            IList<Card> list = new System.Collections.Generic.List<Card>();
            foreach (DataRow row in dt.Rows)
                list.Add(new Card(row));
            return list;
        }

        public static IList<Card> LoadByMerchantGroup(int merchantGroupId)
        {
            DataTable dt = DAL.Card.GetList(merchantGroupId);
            IList<Card> list = new System.Collections.Generic.List<Card>();
            foreach (DataRow row in dt.Rows)
                list.Add(new Card(row));
            return list;
        }

        public static IList<Card> GetAvailable(int merchantGroupId, string cardTypeCode)
        {
            DataTable dt = DAL.Card.GetAvailable(merchantGroupId, cardTypeCode);
            IList<Card> list = new System.Collections.Generic.List<Card>();
            foreach (DataRow row in dt.Rows)
                list.Add(new Card(row));
            return list;
        }

        public static IList<Card> GetListAvailableByCustomerGroup(int merchantGroupId, int customerGroupId, string cardTypeCode)
        {
            DataTable dt = DAL.Card.GetAvailableByCustomerGroup(merchantGroupId, customerGroupId, cardTypeCode);
            IList<Card> list = new System.Collections.Generic.List<Card>();
            foreach (DataRow row in dt.Rows)
                list.Add(new Card(row));
            return list;
        }

        public static IList<Card> VeirfyBeforeEnrolling(string cardNumber, string pan)
        {
            DataTable dt = DAL.Card.VeirfyBeforeEnrolling(cardNumber, pan);
            IList<Card> list = new System.Collections.Generic.List<Card>();
            foreach (DataRow row in dt.Rows)
                list.Add(new Card(row));
            return list;
        }

        public static DataTable GetAvailable(string merchantGroupId, string cardTypeCode)
        {
            DataTable dt = DAL.Card.GetAvailable(int.Parse(merchantGroupId), cardTypeCode);

            return dt;
        }

        public static DataTable GetAvailableByCustomerGroup(int merchantGroupId, int customerGroupId, string cardTypeCode)
        {
            DataTable dt = DAL.Card.GetAvailableByCustomerGroup(merchantGroupId, customerGroupId, cardTypeCode);

            return dt;
        }

        public static DataTable GetAvailableByCustomer(string merchantGroupId, int customerIssuedTo, string cardTypeCode)
        {
            DataTable dt = DAL.Card.GetAvailableByCustomer(int.Parse(merchantGroupId), customerIssuedTo, cardTypeCode);

            return dt;
        }

        public static DataTable GetAvailableAndNotAssignedToCustomer(string merchantGroupId)
        {
            DataTable dt = DAL.Card.GetAvailableAndNotAssignedToCustomer(int.Parse(merchantGroupId));

            return dt;
        }

        public static DataTable GetByMerchantGroupId(int merchantGroupId)
        {
            DataTable dt = DAL.Card.GetByMerchantGroupId(merchantGroupId);

            return dt;
        }

        public static DataTable GetList()
        {
            DataTable dt = DAL.Card.GetList(0);
            return dt;
        }

        public static DataTable Get(int cardId)
        {
            DataTable dt = DAL.Card.Get(cardId, 0);
            return dt;
        }

        public static DataTable GetCardsByPrefix(string prefix, int merchangGroupId)
        {
            DataTable dt = DAL.Card.GetCardsByPrefix(prefix, merchangGroupId);
            return dt;
        }

        #endregion

        #region Private Methods

        private void InitialiseVariables()
        {
            myCardType = new CardType();
            myCardStatus = new CardStatus();
            myCardNumber = string.Empty;
            myPIN = string.Empty;
            myPan = string.Empty;
            myOrderDate = DateTime.Now;
            myActivationDate = DateTime.Now;
            myExpiryDate = DateTime.Now;
            myEnableFleet = false;
            myEnablePrepaid = false;
            myEnableLoyalty = false;
            myEnableMembership = false;
            myCeatedBy = string.Empty;
            myCardId = 0;
            myMerchantGroup = new MerchantGroup();
            myCustomerIssuedToId = 0;
            myCustomerGroupId = 0;
            myISONumber = string.Empty;
            myDriverId = 0;
        }

        private void Load(int cardId)
        {
            DataTable dt = DAL.Card.Get(cardId, 0);

            if (dt.Rows.Count > 0)
            {
                Populate(dt.Rows[0]);
            }
        }

        private void Load(string cardNumber, string pan)
        {
            DataTable dt = DAL.Card.Get(cardNumber, pan);

            if (dt.Rows.Count > 0)
            {
                Populate(dt.Rows[0]);
            }
        }

        private void Populate(DataRow row)
        {
            myCardType = new CardType(int.Parse(row["CardTypeId"].ToString()));
            myCardStatus = new CardStatus(int.Parse(row["CardStatusId"].ToString()));
            myCardNumber = row["CardNumber"].ToString();
            myPIN = row["PIN"].ToString();
            myPan = row["PAN"].ToString();
            if (row["OrderDate"].ToString().Length > 0)
                myOrderDate = DateTime.Parse(row["OrderDate"].ToString());
            if (row["ActivationDate"].ToString().Length > 0)
                myActivationDate = DateTime.Parse(row["ActivationDate"].ToString());
            if (row["ExpiryDate"].ToString().Length > 0)
                myExpiryDate = DateTime.Parse(row["ExpiryDate"].ToString());

            myEnableFleet = bool.Parse(row["EnableFleet"].ToString());
            myEnablePrepaid = bool.Parse(row["EnablePrepaid"].ToString());
            myEnableLoyalty = bool.Parse(row["EnableLoyalty"].ToString());
            myEnableMembership = bool.Parse(row["EnableMembership"].ToString());
            myCeatedBy = row["CardTypeId"].ToString();
            myCardId = int.Parse(row["CardId"].ToString());
            if (row["MerchantGroupId"].ToString().Length > 0)
                myMerchantGroup = new MerchantGroup(int.Parse(row["MerchantGroupId"].ToString()));
            if (row["CustomerIssuedToId"].ToString().Length > 0)
                myCustomerIssuedToId = int.Parse(row["CustomerIssuedToId"].ToString());
            if (row["CustomerGroupId"].ToString().Length > 0)
                myCustomerGroupId = int.Parse(row["CustomerGroupId"].ToString());
            if (row["ISONumber"].ToString().Length > 0)
                myISONumber = row["ISONumber"].ToString();
           
        }

        #endregion
    }
}
