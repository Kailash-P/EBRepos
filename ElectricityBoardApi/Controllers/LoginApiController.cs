using ElectricityBoardApi.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ElectricityBoardApi.Controllers
{
    public class LoginApiController : ApiController
    {
        #region Member Variables

        ElectricityBoardDatabaseEntities db = new ElectricityBoardDatabaseEntities();

        #endregion Member Variables


        #region Member Functions

        /// <summary>
        /// Validate Login
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [HttpPost]
        public int ValidateLogin([FromUri]string userName, [FromUri]string password)
        {
            if(userName != string.Empty && password != string.Empty)
            {
                var list = db.tblLogins.Where(x => x.UserName.ToLower() == userName.Trim().ToLower() && x.Password == password).ToList();
                if(list != null && list.Count > 0)
                {
                    return list[0].ID;
                }
            }
            return 0;
        }

        /// <summary>
        /// Validate Consumer No
        /// </summary>
        /// <param name="consumerNo"></param>
        /// <returns></returns>
        [HttpPost]
        public bool ValidateConsumerNo([FromUri] int consumerNo, [FromUri] int loginID)
        {
            if(consumerNo > 0)
            {
                var consumerObj = db.tblConsumers.Where(x => x.ConsumerNo == consumerNo && x.Login_ID == loginID).ToList();

                if (consumerObj != null && consumerObj.Count > 0)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Quick Pay
        /// </summary>
        /// <param name="consumerNo"></param>
        /// <returns></returns>
        [HttpPost]
        public bool QuickPay([FromUri] int consumerNo)
        {
            if(consumerNo > 0)
            {
                var consumerObj = db.tblConsumers.Where(x => x.ConsumerNo == consumerNo).ToList();

                if(consumerObj != null && consumerObj.Count > 0)
                {
                    int consumerID = consumerObj[0].ID;
                    var billObj = db.tblBillHistories.Where(x => x.Consumer_ID == consumerID && !x.Paid).ToList();

                    if(billObj != null && billObj.Count > 0)
                    {
                        billObj[0].Paid = true;
                        billObj[0].BillPaidDate = DateTime.Now;

                        db.Entry(billObj[0]).State = EntityState.Modified;
                        db.SaveChanges();

                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Get Bill Amount
        /// </summary>
        /// <param name="consumerNo"></param>
        /// <returns></returns>
        [HttpPost]
        public int GetBillAmount([FromUri] int consumerNo)
        {
            if (consumerNo > 0)
            {
                var consumerObj = db.tblConsumers.Where(x => x.ConsumerNo == consumerNo).ToList();

                if (consumerObj != null && consumerObj.Count > 0)
                {
                    int consumerID = consumerObj[0].ID;
                    var billObj = db.tblBillHistories.Where(x => x.Consumer_ID == consumerID && !x.Paid).ToList();

                    if (billObj != null && billObj.Count > 0)
                    {
                        return billObj[0].BillAmount;
                    }
                    else
                    {
                        return 0;
                    }
                }
            }

            return 0;
        }

        public List<tblConsumer> LoadConsumer([FromUri]int loginID)
        {
            var list = new List<tblConsumer>();
            if(loginID > 0)
            {
                list = db.tblConsumers.Where(x => x.Login_ID == loginID).ToList();
            }
            return list;
        }

        public List<vPaymentDetail> LoadPaymentDetails([FromUri]int consumerID, [FromUri] int loginID)
        {
            var list = new List<vPaymentDetail>();

            if(consumerID > 0 && loginID > 0)
            {
                list = db.vPaymentDetails.Where(x => x.Consumer_ID == consumerID && x.Login_ID == loginID).ToList();
            }

            return list;
        }

        [HttpPost]
        public List<tblLogin> LoadProfileDetails([FromUri] int loginID)
        {
            var list = new List<tblLogin>();

            if(loginID > 0)
            {
                list = db.tblLogins.Where(x => x.ID == loginID).ToList();
            }

            return list;
        }

        [HttpPost]
        public bool SaveProfile([FromBody] Profile obj)
        {
            if(obj != null && obj.ID > 0)
            {
                var loginObj = db.tblLogins.Where(x => x.ID == obj.ID).ToList();
                if(loginObj != null && loginObj.Count > 0)
                {
                    loginObj[0].Password = obj.NewPassword;
                    loginObj[0].EmailId = !string.IsNullOrEmpty(obj.Email) ? obj.Email : string.Empty;

                    db.Entry(loginObj[0]).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            return true;
        }

        [HttpPost]

        public List<tblBillHistory> SearchPreviousBillStatements([FromUri]string startDate, [FromUri]string endDate)
        {
            var sDate = DateTime.ParseExact(startDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            var eDate = DateTime.ParseExact(endDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            var list = db.tblBillHistories.Where(x => x.BillDate >= sDate && x.BillDate <= eDate).ToList();

            return list != null && list.Count > 0 ? list : new List<tblBillHistory>();
        }

        [HttpPost]
        public List<Chart> LoadConsumptionChart(int consumerID)
        {
            var list = new List<Chart>();

            if (consumerID > 0)
            {
                var billHistoryList = db.vUnitsConsumptions.Where(x => x.Consumer_ID == consumerID).ToList();

                if (billHistoryList != null && billHistoryList.Count > 0)
                {
                    foreach (var item in billHistoryList)
                    {
                        var obj = new Chart();
                        obj.y = item.UnitsConsumed.HasValue ? item.UnitsConsumed.Value : 0;
                        obj.label = item.Year;
                        
                        list.Add(obj);
                    }
                }
            }

            return list;
        }

        #endregion Member Functions
    }
}
