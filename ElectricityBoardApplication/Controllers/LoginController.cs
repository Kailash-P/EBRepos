using ElectricityBoardApplication.Models;
using ExcelDataReader;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Serialization;

namespace ElectricityBoardApplication.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult LoginPage()
        {
            return View();
        }

        public ActionResult AdminHomePage(int loginID, bool IsAdmin = false)
        {
            ViewBag.LoginID = loginID;
            ViewBag.IsAdmin = IsAdmin;
            return View();
        }

        public ViewResult HomePage(int loginID, bool IsAdmin = false)
        {
            ViewBag.LoginID = loginID;
            ViewBag.IsAdmin = IsAdmin;
            return View();
        }

        public ViewResult QuickPayPage(int loginID, bool IsAdmin = false)
        {
            ViewBag.LoginID = loginID;
            ViewBag.IsAdmin = IsAdmin;
            return View();
        }

        public ViewResult PaymentDetailPage(int loginID, bool IsAdmin = false)
        {
            ViewBag.LoginID = loginID;
            ViewBag.IsAdmin = IsAdmin;
            return View();
        }

        public ViewResult EditProfilePage(int loginID, bool IsAdmin = false)
        {
            ViewBag.LoginID = loginID;
            ViewBag.IsAdmin = IsAdmin;
            return View();
        }

        public ViewResult PreviousBillStatementPage(int loginID, bool IsAdmin = false)
        {
            ViewBag.LoginID = loginID;
            ViewBag.IsAdmin = IsAdmin;
            return View();
        }

        public ViewResult AccountSummaryPage(int loginID, bool IsAdmin = false)
        {
            ViewBag.LoginID = loginID;
            ViewBag.IsAdmin = IsAdmin;
            return View();
        }

        public ViewResult AddProfilePage(int loginID, bool IsAdmin = false)
        {
            ViewBag.LoginID = loginID;
            ViewBag.IsAdmin = IsAdmin;
            return View();
        }

        public ViewResult AddBillPage(int loginID, bool IsAdmin = false)
        {
            ViewBag.LoginID = loginID;
            ViewBag.IsAdmin = IsAdmin;
            return View();
        }

        public ViewResult CustomerBillStatementPage(int loginID, bool IsAdmin = false)
        {
            ViewBag.LoginID = loginID;
            ViewBag.IsAdmin = IsAdmin;

            return View();
        }

        public ViewResult CustomerSupportPage(int loginID, bool isAdmin = false)
        {
            ViewBag.LoginID = loginID;
            ViewBag.IsAdmin = isAdmin;

            return View();
        }

        public ViewResult YourFeedbackPage(int loginID, bool isAdmin = false)
        {
            ViewBag.LoginID = loginID;
            ViewBag.IsAdmin = isAdmin;

            return View();
        }

        [HttpPost]
        public void SaveImageToAppData()
        {
            HttpPostedFileBase postedFileBase = Request.Files[0];
            if (postedFileBase != null && postedFileBase.ContentLength > 0)
            {
                var fileExtension = postedFileBase.FileName.Split('.')[1];

                postedFileBase.SaveAs(Server.MapPath("/ElectricityBoardApplication/App_Data/Images/" + "Login_ID_" + Request.Form["ID"] + "." + fileExtension));
            }
        }

        public string GetImageFromAppData(int loginID)
        {
            var result = "~/Images/img_avatar.png";
            var imagePath = "/ElectricityBoardApplication/App_Data/Images/" + "Login_ID_" + loginID + ".jpg";
            if (loginID > 0 && System.IO.File.Exists(Server.MapPath(imagePath)))
            {
                FileStream fs = new FileStream(Server.MapPath(imagePath), FileMode.Open, FileAccess.Read);
                byte[] filebytes = new byte[fs.Length];
                fs.Read(filebytes, 0, Convert.ToInt32(fs.Length));
                result= "data:image/png;base64," + Convert.ToBase64String(filebytes, Base64FormattingOptions.None);
                //result = imagePath;
            }
            return result;
        }

        [HttpPost]
        public JsonResult ImportExcel()
        {
            var list = new List<ImportError>();
            if (Request.Files.Count > 0)
            {
                HttpPostedFileBase postedFileBase = Request.Files[0];
                string fileName = postedFileBase.FileName;

                byte[] buffer = new byte[postedFileBase.ContentLength];

                postedFileBase.InputStream.Read(buffer, 0, postedFileBase.ContentLength);

                DataTable dt = ConvertExcelToDataTable(buffer, fileName);

                var importList = ConvertDataTableToGenericList(dt);
                string xmlString = ConvertToXmlString(importList);

                if (xmlString != string.Empty)
                {
                    var errorMessage = ImportProfile(xmlString);
                    if (errorMessage != string.Empty)
                    {
                        UTF8Encoding encoding = new UTF8Encoding();
                        XmlSerializer xs = new XmlSerializer(typeof(List<ImportError>), new XmlRootAttribute("ArrayOfImportError"));
                        MemoryStream memoryStream = new MemoryStream(encoding.GetBytes(errorMessage));
                        list = (List<ImportError>)xs.Deserialize(memoryStream);
                    }
                }
            }
            return Json(list.OrderBy(o => o.RowNumber), JsonRequestBehavior.AllowGet);
        }

        public DataTable ConvertExcelToDataTable(byte[] buffer, string fileName)
        {

            Stream fileStream = new MemoryStream(buffer);
            IExcelDataReader excelReader = (fileName.Substring(fileName.LastIndexOf('.') + 1) == "xls") ?
                                                ExcelReaderFactory.CreateBinaryReader(fileStream) :
                                                 ExcelReaderFactory.CreateOpenXmlReader(fileStream);
            //Using Excel Reader
            DataTable dt = excelReader.AsDataSet() != null ? excelReader.AsDataSet().Tables[0] : null;
            fileStream.Close();
            return dt;

        }

        public List<ImportProfile> ConvertDataTableToGenericList(DataTable dt)
        {
            List<ImportProfile> list = new List<ImportProfile>();

            if (dt != null && dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    bool flag = false;
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        if (Convert.ToString(dt.Rows[i][j]) != string.Empty)
                        {
                            flag = true;
                        }
                    }
                    if (flag)
                    {
                        list.Add(ConvertEnquiryToTableRow(dt.Rows[i]));
                    }
                }
            }

            if (list != null && list.Count > 0)
            {
                list.RemoveAt(0);
            }

            return list;
        }

        public ImportProfile ConvertEnquiryToTableRow(DataRow dr)
        {
            ImportProfile importProfile = new ImportProfile();
            importProfile.UserName = Convert.ToString(dr[0]);
            importProfile.Password = Convert.ToString(dr[1]);
            importProfile.ConsumerNo = Convert.ToString(dr[2]);
            importProfile.RegionCode = Convert.ToString(dr[3]);
            importProfile.Address = Convert.ToString(dr[4]);
            importProfile.Email = Convert.ToString(dr[5]);
            importProfile.City = Convert.ToString(dr[6]);
            importProfile.State = Convert.ToString(dr[7]);
            importProfile.ZipCode = Convert.ToString(dr[8]);
            importProfile.IsAdmin = Convert.ToString(dr[9]);

            return importProfile;
        }

        // By using this method we can convert datatable to xml
        public string ConvertDatatableToXML(DataTable dt)
        {
            MemoryStream str = new MemoryStream();
            dt.WriteXml(str, true);
            str.Seek(0, SeekOrigin.Begin);
            StreamReader sr = new StreamReader(str);
            string xmlstr;
            xmlstr = sr.ReadToEnd();
            return (xmlstr);
        }

        public string ConvertToXmlString(IList data)
        {
            if (data.Count > 0)
            {
                PropertyDescriptorCollection props = TypeDescriptor.GetProperties(data[0].GetType());
                DataTable table = new DataTable(data[0].GetType().Name);
                for (int i = 0; i < props.Count; i++)
                {
                    PropertyDescriptor prop = props[i];
                    table.Columns.Add(prop.Name, prop.PropertyType);
                }
                object[] values = new object[props.Count];
                foreach (object item in data)
                {
                    for (int i = 0; i < values.Length; i++)
                    {
                        values[i] = props[i].GetValue(item);
                    }
                    table.Rows.Add(values);
                }
                StringBuilder strXml = new StringBuilder();
                table.WriteXml(XmlWriter.Create(strXml));
                return strXml.ToString();
            }
            return string.Empty;
        }

        public string ImportProfile(string xmlString)
        {
            var parameters = new List<SqlParameter>();

            parameters.Add(new SqlParameter()
            {
                SqlDbType = SqlDbType.NVarChar,
                Value = xmlString,
                ParameterName = "@XMLString"
            });

            SqlConnection con = new SqlConnection("Server=tcp:301chennai.database.windows.net,1433;Initial Catalog=ElectricityBoardDatabase;Persist Security Info=False;User ID=chennai;Password=Mindtree@123;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
            con.Open();
            SqlCommand cmd = new SqlCommand("EXEC dbo.spImportProfile @xmlString", con);
            cmd.Parameters.Add(parameters[0]);
            SqlDataReader reader;

            reader = cmd.ExecuteReader();

            return reader != null && reader.Read() ? reader.GetString(0) : string.Empty;
        }

        public static T DeserializeObject<T>(string pXmlizedString)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            XmlSerializer xs = new XmlSerializer(typeof(T));
            MemoryStream memoryStream = new MemoryStream(encoding.GetBytes(pXmlizedString));
            System.Xml.XmlTextWriter xmlTextWriter = new System.Xml.XmlTextWriter(memoryStream, Encoding.UTF8);
            return (T)xs.Deserialize(memoryStream);
        }
    }
}