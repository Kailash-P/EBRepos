using ElectricityBoardApi.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using SendGrid;
using SendGrid.Helpers.Mail;
using RestSharp;
using System.Web.Script.Serialization;
using System.Web.Http.Cors;

namespace ElectricityBoardApi.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
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
        public List<tblLogin> ValidateLogin([FromUri]string userName, [FromUri]string password)
        {
            if(userName != string.Empty && password != string.Empty)
            {
                var list = db.tblLogins.Where(x => x.UserName.ToLower() == userName.Trim().ToLower() && x.Password == password).ToList();
                if(list != null && list.Count > 0)
                {
                    Session.Login_ID = list[0].ID;
                    Session.LoginName = list[0].UserName;
                    Session.LoginEmail = list[0].EmailId;
                    return list;
                }
            }
            return new List<tblLogin>();
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

        public List<tblConsumer> LoadConsumer([FromUri]int loginID=0)
        {
            var list = new List<tblConsumer>();
            if(loginID > 0)
            {
                list = db.tblConsumers.Where(x => x.Login_ID == loginID).ToList();
            }
            else
            {
                list = db.tblConsumers.ToList();
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
        public List<Profile> LoadProfileDetails([FromUri] int loginID)
        {
            var list = new List<tblLogin>();

            if(loginID > 0)
            {
                list = db.tblLogins.Where(x => x.ID == loginID).ToList();
            }

            return list.Select(s => new Profile
            {
                ID = s.ID,
                UserName = s.UserName,
                Address = s.Address,
                Email = s.EmailId,
                City = s.City,
                State = s.State,
                ZipCode = s.ZipCode,
                ProfilePicture = Convert.ToBase64String(s.ProfilePicture,0,s.ProfilePicture.Length)
            }).ToList();
        }

        [HttpPost]
        public int SaveProfile([FromBody] Profile obj)
        {
            if(obj != null)
            {
                if (obj.ID > 0)
                {
                    var loginObj = db.tblLogins.Where(x => x.ID == obj.ID).ToList();
                    if (loginObj != null && loginObj.Count > 0)
                    {
                        loginObj[0].Password = obj.NewPassword;
                        loginObj[0].EmailId = !string.IsNullOrEmpty(obj.Email) ? obj.Email : string.Empty;
                        loginObj[0].Address = !string.IsNullOrEmpty(obj.Address) ? obj.Address : string.Empty;
                        loginObj[0].City = !string.IsNullOrEmpty(obj.City) ? obj.City : string.Empty;
                        loginObj[0].ZipCode = obj.ZipCode;
                        loginObj[0].ProfilePicture = ConvertBase64StringToByteArray(obj.ProfilePicture);
                        loginObj[0].State = obj.State;

                        db.Entry(loginObj[0]).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                else
                {
                    tblLogin loginObj = new tblLogin();
                    loginObj.UserName = obj.UserName;
                    loginObj.Password = obj.NewPassword;
                    loginObj.EmailId = obj.Email;
                    loginObj.Address = obj.Address;
                    loginObj.City = obj.City;
                    loginObj.State = obj.State;
                    loginObj.ZipCode = obj.ZipCode;
                    loginObj.IsAdmin = obj.IsAdmin;
                    loginObj.ProfilePicture = ConvertBase64StringToByteArray(obj.ProfilePicture);

                    db.tblLogins.Add(loginObj);
                    int loginID = db.SaveChanges();
                    if(loginID > 0)
                    {
                        tblConsumer consumerObj = new tblConsumer();
                        consumerObj.ConsumerNo = obj.ConsumerNo;
                        consumerObj.ConsumerName = obj.UserName;
                        consumerObj.RegionCode = obj.RegionCode;
                        consumerObj.Login_ID = loginObj.ID;

                        db.tblConsumers.Add(consumerObj);
                        db.SaveChanges();
                    }

                    return loginObj.ID;
                }
            }
            
            return obj.ID;
        }

        [HttpPost]

        public List<tblBillHistory> SearchPreviousBillStatements([FromUri]string startDate, [FromUri]string endDate)
        {
            var sDate = DateTime.ParseExact(startDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            var eDate = DateTime.ParseExact(endDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            var list = db.tblBillHistories.Where(x => x.BillDate >= sDate && x.BillDate <= eDate).ToList();

            return list != null && list.Count > 0 ? list : new List<tblBillHistory>();
        }

        /// <summary>
        /// Load Consumption Chart
        /// </summary>
        /// <param name="consumerID"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Validate Profile
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost]
        public string ValidateProfile([FromBody] Profile obj)
        {
            var errorMessage = string.Empty;
            if(obj != null)
            {
                if (obj.UserName != string.Empty)
                {
                    var loginList = db.tblLogins.Where(x => x.UserName == obj.UserName.Trim()).ToList();
                    if (loginList != null && loginList.Count > 0)
                    {
                        errorMessage = "Login already exists in the database.";
                    }
                }

                if (obj.ConsumerNo > 0)
                {
                    var consumerList = db.tblConsumers.Where(x => x.ConsumerNo == obj.ConsumerNo).ToList();
                    if(consumerList != null && consumerList.Count > 0)
                    {
                        errorMessage = "Consumer No already exists in the database.";
                    }
                }
            }

            return errorMessage;
        }

        public bool AddBill([FromBody]Bill obj)
        {
            tblBillHistory billHistoryObj = new tblBillHistory();
            if (obj != null)
            {                
                var billDate = DateTime.ParseExact(obj.BillDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                billHistoryObj.BillDate = billDate;
                billHistoryObj.UnitsConsumed = obj.Units;
                billHistoryObj.BillAmount = obj.Amount;
                billHistoryObj.Consumer_ID = obj.ConsumerID;
                billHistoryObj.BillPaidDate = Convert.ToDateTime("1900/01/01");
                billHistoryObj.Paid = false;

                db.tblBillHistories.Add(billHistoryObj);

                db.SaveChanges();
                
                //TODO: Need to send email to Consumer linked Login EmailID
            }

            return billHistoryObj.ID > 0 ? true : false;
        }

        [HttpPost]
        public List<tblBillHistory> SearchConsumerBillStatements([FromUri]int consumerID)
        {
            var list = db.tblBillHistories.Where(x => x.Consumer_ID == consumerID).ToList();

            return list != null && list.Count > 0 ? list : new List<tblBillHistory>();
        }

        [HttpPost]
        public bool DeleteConsumerBill([FromUri]int consumerID, [FromUri]int ID)
        {
            var result = db.Database.ExecuteSqlCommand("DELETE FROM tblBillHistory WHERE Consumer_ID = {0} AND ID = {1}", new object[] { consumerID, ID });

            return result > 0 ? true : false;
        }

        [HttpGet]
        public int GetLastInsertedLoginID()
        {
            var list = db.tblLogins.OrderByDescending(o => o.ID).ToList();

            return list != null && list.Count > 0 ? list[0].ID : 0;
        }

        public byte[] ConvertBase64StringToByteArray(string source)
        {
            byte[] data = null;
            try
            {
                string base64 = source.Substring(source.IndexOf(',') + 1);
                base64 = base64.Trim('\0');
                data = Convert.FromBase64String(base64);
            }
            catch
            {
                data = new byte[] { };
            }
            return data;
        }

        [HttpPost]
        public byte[] DownloadImportProfileTemplate()
        {
            using (ExcelPackage pck = new ExcelPackage())
            {
                //Create the worksheet
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("SearchReport");
                ws.Protection.IsProtected = false;
                DataTable dt = CreateEmptyDataTable();
                ws.Cells["A1"].LoadFromDataTable(dt, true);
                //prepare the range for the column headers
                string cellRange = "A1:" + Convert.ToChar('A' + dt.Columns.Count - 1) + 1;
                //Format the header for columns
                using (ExcelRange rng = ws.Cells[cellRange])
                {
                    rng.Style.WrapText = false;
                    rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    rng.Style.Font.Bold = true;
                    rng.Style.Fill.PatternType = ExcelFillStyle.Solid; //Set Pattern for the background to Solid
                    rng.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Gray);
                    rng.Style.Font.Color.SetColor(System.Drawing.Color.White);
                }
                //prepare the range for the rows
                string rowsCellRange = "A2:" + Convert.ToChar('A' + dt.Columns.Count - 1) + dt.Rows.Count * dt.Columns.Count;
                //Format the rows
                for (int i = 1; i <= dt.Columns.Count; i++)
                {
                    ExcelColumn col = ws.Column(i);
                    col.Style.Numberformat.Format = "@";
                }
                Byte[] fileBytes = pck.GetAsByteArray();
                return fileBytes;
            }
        }

        public DataTable CreateEmptyDataTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn() { ColumnName = "UserName", ReadOnly = false });
            dt.Columns.Add(new DataColumn() { ColumnName = "Password", ReadOnly = false });
            dt.Columns.Add(new DataColumn() { ColumnName = "ConsumerNo", ReadOnly = false });
            dt.Columns.Add(new DataColumn() { ColumnName = "RegionCode", ReadOnly = false });            
            dt.Columns.Add(new DataColumn() { ColumnName = "Address", ReadOnly = false });
            dt.Columns.Add(new DataColumn() { ColumnName = "Email", ReadOnly = false });
            dt.Columns.Add(new DataColumn() { ColumnName = "City", ReadOnly = false });
            dt.Columns.Add(new DataColumn() { ColumnName = "State", ReadOnly = false });
            dt.Columns.Add(new DataColumn() { ColumnName = "ZipCode", ReadOnly = false });
            dt.Columns.Add(new DataColumn() { ColumnName = "IsAdmin", ReadOnly = false });

            return dt;
        }

        [HttpPost]
        public string SendConsumerSupportEmail([FromUri]string from, [FromUri]string message)
        {
            var result = string.Empty;
            var consumerObj = LoadConsumer(Session.Login_ID)[0];

            try
            {
                if (consumerObj != null)
                {
                    var complaintID = SaveCustomerComplaint(consumerObj.ID, from, message);
                    if (complaintID > 0)
                    {
                        EmailParameters param = new EmailParameters();
                        param.ID = complaintID;
                        param.fromEmailAddress = from;
                        param.toEmailAddress = "mr.p.kailash@gmail.com";
                        param.consumerName = consumerObj.ConsumerName;
                        param.resolvedMessage = message;

                        SendEmail.TriggerMail(param);

                        result = string.Empty;
                    }
                }
            }
            catch
            {
                result = "Error";
            }


            return result;
        }

        public int SaveCustomerComplaint(int consumer_ID, string email, string concern)
        {
            try
            {
                tblCustomerSupport customerSupportObj = new tblCustomerSupport();
                if (consumer_ID > 0 && email != string.Empty && concern != string.Empty)
                {
                    customerSupportObj.Consumer_ID = consumer_ID;
                    customerSupportObj.Email = email;
                    customerSupportObj.Concern = concern;
                    customerSupportObj.ResolvedMessage = string.Empty;
                    customerSupportObj.IsResolved = false;
                    customerSupportObj.RaisedDate = DateTime.Now;
                    customerSupportObj.ResolvedDate = new DateTime(1900, 01, 01);
                    customerSupportObj.Severity = 0;

                    db.tblCustomerSupports.Add(customerSupportObj);

                    db.SaveChanges();
                }
                return customerSupportObj.ID > 0 ? customerSupportObj.ID : 0;
            }
            catch
            {
                return 0;
            }
        }

        [HttpGet]
        public string GetSessionLoginName()
        {
            return Session.LoginName;
        }

        [HttpGet]
        public string GetSessionEmail()
        {
            return Session.LoginEmail;
        }

        [HttpGet]
        public List<vCustomerSupport> LoadCustomerSupportIssues([FromUri]int isResolved = 0)
        {
            return db.vCustomerSupports.Where(x => x.IsResolved == isResolved > 0 ? true : false).ToList();
        }

        [HttpPost]
        public bool ResolveCustomerSupportIssue([FromUri] int ID, [FromUri] string resolvedMessage, [FromUri] string consumerName, [FromUri] string consumerEmail)
        {
            try
            {
                tblCustomerSupport customerSupportObj = new tblCustomerSupport();
                if (ID > 0 && resolvedMessage != string.Empty)
                {
                    customerSupportObj = db.tblCustomerSupports.Where(x => x.ID == ID).FirstOrDefault();
                    customerSupportObj.ResolvedMessage = resolvedMessage;
                    customerSupportObj.IsResolved = true;
                    customerSupportObj.ResolvedDate = DateTime.Now;
                    customerSupportObj.Severity = 0;

                    db.Entry(customerSupportObj).State = EntityState.Modified;
                    db.SaveChanges();

                    EmailParameters param = new EmailParameters();
                    param.ID = ID;
                    param.fromEmailAddress = "mr.p.kailash@gmail.com";
                    param.toEmailAddress = consumerEmail;
                    param.consumerName = consumerName;
                    param.resolvedMessage = resolvedMessage;

                    SendEmail.TriggerMail(param);

                    return true;
                }
            }
            catch
            {
                return false;
            }

            return false;
        }

        [HttpPost]
        public int SaveFeedBack(FeedBackModel model)
        {
            tblFeedback feedBackObj = new tblFeedback();
            if(model != null)
            {
                feedBackObj.Headline = model.FeedBackHeadline;
                feedBackObj.Feedback = model.FeedBack;
                feedBackObj.ConsumerProfilePicture = ConvertBase64StringToByteArray(model.ProfilePicture);
                feedBackObj.Consumer_ID = db.tblConsumers.Where(x => x.Login_ID == Session.Login_ID).FirstOrDefault().ID;
                feedBackObj.ConsumerName = model.UserName;

                db.tblFeedbacks.Add(feedBackObj);
                db.SaveChanges();                
            }
            return feedBackObj.ID > 0 ? feedBackObj.ID : 0;
        }

        [HttpGet]
        public List<SentimentAnalysis> AnalyseConsumerFeedback()
        {
            var list = new List<SentimentAnalysis>();
            var client = new RestClient("https://brazilsouth.api.cognitive.microsoft.com/text/analytics/v2.0/sentiment");
            var request = new RestRequest(Method.POST);
            request.AddHeader("Postman-Token", "d6b94fb3-8520-48cf-bff8-d983397eb7d9");
            request.AddHeader("Cache-Control", "no-cache");
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Accept", "application/json");
            request.AddHeader("Ocp-Apim-Subscription-Key", "72c321385eac4027b570a73b39c90361");
            var param = GetConsumerFeedBack();
            if (param != string.Empty)
            {
                var res = "{\r\n        \"documents\":  " + param + "  \r\n}";
                request.AddParameter("undefined", res, ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);
                var jsonSerialiser = new JavaScriptSerializer();
                var result = jsonSerialiser.Deserialize<SentimentResponse>(response.Content).documents.ToList();

                if(result != null && result.Count > 0)
                {
                    foreach(var item in result)
                    {
                        var feedBackObj = db.tblFeedbacks.Where(x => x.ID == item.id).FirstOrDefault();
                        if(feedBackObj != null)
                        {
                            var consumerObj = db.tblConsumers.Where(x => x.ID == feedBackObj.Consumer_ID).FirstOrDefault();
                            if(consumerObj != null)
                            {
                                SentimentAnalysis analysisObj = new SentimentAnalysis();
                                analysisObj.ID = item.id;
                                analysisObj.ConsumerName = consumerObj.ConsumerName;
                                analysisObj.FeedBack = feedBackObj.Feedback;
                                analysisObj.Sentiment = item.score;
                                analysisObj.ConsumerProfilePicture = Convert.ToBase64String(feedBackObj.ConsumerProfilePicture, 0, feedBackObj.ConsumerProfilePicture.Length);

                                list.Add(analysisObj);
                            }
                        }
                    }
                }
            }

            return list;
        }

        public string GetConsumerFeedBack()
        {
            var list = db.tblFeedbacks.ToList();
            var tempList = list.Select(s => new FeedBack {
                id=s.ID,
                languate="en",
                text=s.Feedback
            }).ToList();

            var jsonSerialiser = new JavaScriptSerializer();
            return jsonSerialiser.Serialize(tempList);
        }

        #endregion Member Functions
    }
}
 