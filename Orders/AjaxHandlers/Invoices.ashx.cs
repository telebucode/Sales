﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OrdersManagement.Core;
using OrdersManagement.Exceptions;
using OrdersManagement.Model;

using Newtonsoft.Json.Linq;
using System.Web.SessionState;

namespace Orders.AjaxHandlers
{
    /// <summary>
    /// Summary description for Invoices
    /// </summary>
    public class Invoices : IHttpHandler, IRequiresSessionState
    {
        private JObject errorJSon = new JObject(new JProperty("Success", false), new JProperty("Message", ""));
        public void ProcessRequest(HttpContext context)
        {
            if (HttpContext.Current.Session["AdminId"] == null || HttpContext.Current.Session["AdminId"].ToString() == string.Empty)
            {
                context.Response.StatusCode = 401;
                context.Response.StatusDescription = "Invalid Session";
                context.Response.End();
            }
            if (context.Request["Action"] == null)
            {
                context.Response.StatusCode = 400;
                errorJSon["Message"] = "Action parameter is mandatory";
                context.Response.Write(errorJSon);
                context.Response.End();
            }
            try
            {
                switch (context.Request["Action"].ToString())
                {
                    case "GetStatuses":
                        GetStatuses(context);
                        break;
                    case "Search":
                        Search(context);
                        break;
                    case "Create":
                        CreateInvoice(context);
                        break;
                    case "View":
                        View(context);
                        break;
                    case "Download":
                        Download(context);
                        break;
                    case "Cancel":
                        CancelInvoice(context);
                        break;
                    case "downloadInvoices":
                        downloadInvoices(context);
                        break;
                    case "GetInvoiceAccountDetails":
                        GetInvoiceAccountDetails(context);
                        break;
                    case "UpdateInvoice":
                        UpdateInvoice(context);
                        break;
                    default:
                        GenerateErrorResponse(400, string.Format("Invalid Action ({0})", context.Request["Action"].ToString()));
                        break;
                }
            }
            catch (System.Threading.ThreadAbortException e)
            { }
            catch (OrdersManagement.Exceptions.QuotationException e)
            {
                GenerateErrorResponse(500, e.Message);
            }
            catch (Exception e)
            {
                GenerateErrorResponse(500, e.Message);
            }
        }

        private void downloadInvoices(HttpContext context)
        {
            byte productId = 0;
            int invoiceId = 0;
            string quotationNumber = string.Empty;
            string accountName = string.Empty;
            string mobile = string.Empty;
            string email = string.Empty;
            string ipAddress = string.Empty;
            int accountId = 0;
            int employeeId = 0;
            int ownerShipId = 0;
            byte statusId = 0;
            sbyte channelId = 0;
            byte billingModeId = 1;
            bool isdownload = true;
            DateTime fromDateTime = DateTime.Now.Date;
            DateTime toDateTime = DateTime.Now.AddDays(1).AddTicks(-1);
            int pageNumber = 1;
            byte limit = 20;

            if (context.Request["ProductId"] != null && !byte.TryParse(context.Request["productId"].ToString(), out productId))
                GenerateErrorResponse(400, string.Format("ProductId must be a number"));
            //if (context.Request["InvoiceId"] != null && !int.TryParse(context.Request["InvoiceId"].ToString(), out invoiceId))
            //    GenerateErrorResponse(400, string.Format("InvoiceId must be a number"));
            if (context.Request["QuotationNumber"] != null)
                quotationNumber = context.Request["QuotationNumber"].ToString();
            if (context.Request["AccountName"] != null)
                accountName = context.Request["AccountName"].ToString();
            //if (context.Request["AccountId"] != null && !int.TryParse(context.Request["AccountId"].ToString(), out accountId))
            //    GenerateErrorResponse(400, string.Format("AccountId must be a number"));
            //if (context.Request["EmployeeId"] != null && !int.TryParse(context.Request["EmployeeId"].ToString(), out employeeId))
            //    GenerateErrorResponse(400, string.Format("EmployeeId must be a number"));
            //if (context.Request["OwnerShipId"] != null && !int.TryParse(context.Request["OwnerShipId"].ToString(), out ownerShipId))
            //    GenerateErrorResponse(400, string.Format("OwnerShipId must be a number"));
            if (context.Request["StatusId"] != null && !byte.TryParse(context.Request["StatusId"].ToString(), out statusId))
                GenerateErrorResponse(400, string.Format("StatusId must be a number"));
            //if (context.Request["ChannelId"] != null && !sbyte.TryParse(context.Request["ChannelId"].ToString(), out channelId))
            //    GenerateErrorResponse(400, string.Format("ChannelId must be a number"));
            if (context.Request["BillingModeId"] != null && !byte.TryParse(context.Request["BillingModeId"].ToString(), out billingModeId))
                GenerateErrorResponse(400, string.Format("BillingModeId must be a number"));
            if(context.Request["mobile"] != null)
                mobile = Convert.ToString(context.Request["mobile"]);
            if (context.Request["email"] != null)
                email = Convert.ToString(context.Request["email"]);
            if (context.Request["FromDateTime"] != null)                
                fromDateTime = Convert.ToDateTime(context.Request["fromDateTime"]);
            if (context.Request["ToDateTime"] != null)
                toDateTime = Convert.ToDateTime(context.Request["ToDateTime"]);

            TablePreferences invoiceTablePreferences = new TablePreferences("", "", true, false);
            Dictionary<string, TablePreferences> invoiceDictionary = new Dictionary<string, TablePreferences>();
            invoiceDictionary.Add("Invoices", invoiceTablePreferences);
            OrdersManagement.Core.Client client = new OrdersManagement.Core.Client(responseFormat: OrdersManagement.ResponseFormat.JSON);
            //JObject jobj = new JObject();
            context.Response.Write(client.GetInvoices(productId: productId, invoiceId: invoiceId, quotationNumber: quotationNumber, accountId: accountId,
                employeeId: employeeId, ownerShipId: ownerShipId, statusId: statusId, channelId: channelId, ipAddress: ipAddress,
                billingModeId: billingModeId, fromDateTime: fromDateTime, toDateTime: toDateTime, pageNumber: pageNumber, limit: limit,
                mobile: mobile, email: email, accountName: accountName, tablePreferences: invoiceDictionary, isdownload: isdownload));
            //string resp = jobj.ToString();
            //if (isdownload == false)
              //  context.Response.Write(jobj);
        }

        private void GetStatuses(HttpContext context)
        {
            bool onlyActive = true;
            if (context.Request["OnlyActive"] != null && !bool.TryParse(context.Request["OnlyActive"].ToString(), out onlyActive))
                GenerateErrorResponse(400, string.Format("OnlyActive value ({0}) is not a valid boolean value", context.Request["OnlyActive"].ToString()));
            OrdersManagement.Core.Client client = new OrdersManagement.Core.Client(responseFormat: OrdersManagement.ResponseFormat.JSON);
            context.Response.Write(client.GetInvoiceStatuses(onlyActive: onlyActive));
        }
        private void CreateInvoice(HttpContext context)
        {
            int quotationId = 0;
            byte billingModeId = 0;
            int employeeId = 0;
            if (context.Request["QuotationId"] != null && !int.TryParse(context.Request["QuotationId"].ToString(), out quotationId))
                GenerateErrorResponse(400, string.Format("QuotationId Must be a number"));
            if (context.Request["BillingModeId"] != null && !byte.TryParse(context.Request["BillingModeId"].ToString(), out billingModeId))
                GenerateErrorResponse(400, string.Format("BillingModeId Must be a number"));
            //if (context.Request["EmployeeId"] != null && !int.TryParse(context.Request["EmployeeId"].ToString(), out employeeId))
            //    GenerateErrorResponse(400, string.Format("EmployeeId Must be a number"));
            employeeId = Convert.ToInt32(context.Session["AdminId"]);
            OrdersManagement.Core.Client client = new OrdersManagement.Core.Client(responseFormat: OrdersManagement.ResponseFormat.JSON);
            context.Response.Write(client.CreateInvoice(quotationId, billingModeId, employeeId));
        }

        //cancel Invoice
        private void CancelInvoice(HttpContext context)
        {
            int quotationId = 0;
            int adminId = 0;
            if (context.Request["QuotationId"] != null && !int.TryParse(context.Request["QuotationId"].ToString(), out quotationId))
                GenerateErrorResponse(400, string.Format("QuotationId Must be a number"));
            if (context.Request["AdminId"] != null && !int.TryParse(context.Request["AdminId"].ToString(), out adminId))
                GenerateErrorResponse(400, string.Format("AdminId Must be a number"));

            OrdersManagement.Core.Client client = new OrdersManagement.Core.Client(responseFormat: OrdersManagement.ResponseFormat.JSON);
            context.Response.Write(client.CancelInvoice(quotationId, adminId));
        }

        private void UpdateInvoice(HttpContext context)
        {
            
            JObject InvoiceDetails = JObject.Parse(context.Request["payload"]);

            string mobile = string.Empty;
            string email = string.Empty;
            string address = string.Empty;
            string GSTIN = string.Empty;
            string companyName = string.Empty;
            int stateId = 0;
            int invoiceId = 0;

            if (InvoiceDetails.SelectToken("Mobile") != null)
                mobile = Convert.ToString(InvoiceDetails.SelectToken("Mobile"));
            if (InvoiceDetails.SelectToken("BusinessMailID") != null)
                email = Convert.ToString(InvoiceDetails.SelectToken("BusinessMailID"));
            if (InvoiceDetails.SelectToken("ContactAddress") != null)
                address = Convert.ToString(InvoiceDetails.SelectToken("ContactAddress"));
            if (InvoiceDetails.SelectToken("GSTIN") != null)
                GSTIN = Convert.ToString(InvoiceDetails.SelectToken("GSTIN"));
            if (InvoiceDetails.SelectToken("CompanyName") != null)
                companyName = Convert.ToString(InvoiceDetails.SelectToken("CompanyName"));
            if (InvoiceDetails.SelectToken("States") != null)
                stateId = Convert.ToInt32(InvoiceDetails.SelectToken("States"));
            if (InvoiceDetails.SelectToken("InvoiceId") != null)
                invoiceId = Convert.ToInt32(InvoiceDetails.SelectToken("InvoiceId"));

            OrdersManagement.Core.Client client = new OrdersManagement.Core.Client(responseFormat: OrdersManagement.ResponseFormat.JSON);
            context.Response.Write(client.UpdateInvoice(invoiceId:invoiceId,mobile: mobile, email: email, address: address, GSTIN: GSTIN, companyName: companyName, stateId: stateId));                     
           
        }

        private void GetInvoiceAccountDetails(HttpContext context)
        {
            int invoiceId = 0;

            if (context.Request["invoiceId"] != null && !int.TryParse(context.Request["invoiceId"].ToString(), out invoiceId))
                GenerateErrorResponse(400, string.Format("InvoiceId Must be a number"));


            OrdersManagement.Core.Client client = new OrdersManagement.Core.Client(responseFormat: OrdersManagement.ResponseFormat.JSON);
            context.Response.Write(client.GetInvoiceAccountDetails(invoiceId: invoiceId));
        }
        private void Search(HttpContext context)
        {
            byte productId = 0;
            int invoiceId = 0;
            string quotationNumber = string.Empty;
            string accountName = string.Empty;
            string mobile = string.Empty;
            string email = string.Empty;
            int accountId = 0;
            int employeeId = 0;
            int ownerShipId = 0;
            byte statusId = 0;
            sbyte channelId = 0;
            string ipAddress = string.Empty;
            byte billingModeId = 1;
            bool isdownload = false;
            DateTime fromDateTime = DateTime.Now.Date;
            DateTime toDateTime = DateTime.Now.AddDays(1).AddTicks(-1);
            int pageNumber = 1;
            byte limit = 20;
            JObject searchData = new JObject();
            searchData = JObject.Parse(context.Request["SearchData"]);

            if (searchData.SelectToken("ProductId") != null && !byte.TryParse(searchData.SelectToken("ProductId").ToString(), out productId))
                GenerateErrorResponse(400, string.Format("ProductId must be a number"));
            if (searchData.SelectToken("InvoiceId") != null && !int.TryParse(searchData.SelectToken("InvoiceId").ToString(), out invoiceId))
                GenerateErrorResponse(400, string.Format("QuotationId must be a number"));
            if (searchData.SelectToken("QuotationNumber") != null)
                quotationNumber = searchData.SelectToken("QuotationNumber").ToString();
            if (searchData.SelectToken("AccountName") != null)
                accountName = searchData.SelectToken("AccountName").ToString();
            if (searchData.SelectToken("AccountId") != null && !int.TryParse(searchData.SelectToken("AccountId").ToString(), out accountId))
                GenerateErrorResponse(400, string.Format("AccountId must be a number"));
            if (searchData.SelectToken("EmployeeId") != null && !int.TryParse(searchData.SelectToken("EmployeeId").ToString(), out employeeId))
                GenerateErrorResponse(400, string.Format("EmployeeId must be a number"));
            if (searchData.SelectToken("OwnerShipId") != null && !int.TryParse(searchData.SelectToken("OwnerShipId").ToString(), out ownerShipId))
                GenerateErrorResponse(400, string.Format("OwnerShipId must be a number"));
            if (searchData.SelectToken("StatusId") != null && !byte.TryParse(searchData.SelectToken("StatusId").ToString(), out statusId))
                GenerateErrorResponse(400, string.Format("StatusId must be a number"));
            if (searchData.SelectToken("ChannelId") != null && !sbyte.TryParse(searchData.SelectToken("ChannelId").ToString(), out channelId))
                GenerateErrorResponse(400, string.Format("ChannelId must be a number"));
            if (searchData.SelectToken("BillingModeId") != null && !byte.TryParse(searchData.SelectToken("BillingModeId").ToString(), out billingModeId))
                GenerateErrorResponse(400, string.Format("BillingModeId must be a number"));
            if (searchData.SelectToken("FromDateTime") != null && !DateTime.TryParse(searchData.SelectToken("FromDateTime").ToString(), out fromDateTime))
                GenerateErrorResponse(400, string.Format("FromDateTime is not a valid datetime"));
            if (searchData.SelectToken("ToDateTime") != null && !DateTime.TryParse(searchData.SelectToken("ToDateTime").ToString(), out toDateTime))
                GenerateErrorResponse(400, string.Format("ToDateTime is not a valid datetime"));
            if (searchData.SelectToken("PageNumber") != null && !int.TryParse(searchData.SelectToken("PageNumber").ToString(), out pageNumber))
                GenerateErrorResponse(400, string.Format("PageNumber must be a number"));
            if (searchData.SelectToken("Limit") != null && !byte.TryParse(searchData.SelectToken("Limit").ToString(), out limit))
                GenerateErrorResponse(400, string.Format("Limit must be a number"));
            if (searchData.SelectToken("Mobile") != null)
                mobile = searchData.SelectToken("Mobile").ToString();
            if (searchData.SelectToken("Email") != null)
                email = searchData.SelectToken("Email").ToString();
            TablePreferences invoiceTablePreferences = new TablePreferences("", "", true, false);
            Dictionary<string, TablePreferences> invoiceDictionary = new Dictionary<string, TablePreferences>();
            invoiceDictionary.Add("Invoices", invoiceTablePreferences);
            OrdersManagement.Core.Client client = new OrdersManagement.Core.Client(responseFormat: OrdersManagement.ResponseFormat.JSON);
            context.Response.Write(client.GetInvoices(productId: productId, invoiceId: invoiceId, quotationNumber: quotationNumber, accountId: accountId,
                employeeId: employeeId, ownerShipId: ownerShipId, statusId: statusId, channelId: channelId, ipAddress: ipAddress,
                billingModeId: billingModeId, fromDateTime: fromDateTime, toDateTime: toDateTime, pageNumber: pageNumber, limit: limit,
                mobile: mobile, email: email, accountName: accountName, tablePreferences: invoiceDictionary, isdownload: isdownload));
        }
        private void View(HttpContext context)
        {
            int quotationId = 0;
            bool isPostPaidQuotation = false;
            if (context.Request["QuotationId"] != null && !int.TryParse(context.Request["QuotationId"].ToString(), out quotationId))
                GenerateErrorResponse(400, string.Format("OnlyActive value ({0}) is not a valid boolean value", context.Request["OnlyActive"].ToString()));
            if (quotationId <= 0)
                GenerateErrorResponse(400, string.Format("QuoationId must be greater than 0"));
            if (context.Request["IsPostPaidQuotation"] != null && !bool.TryParse(context.Request["IsPostPaidQuotation"].ToString(), out isPostPaidQuotation))
                GenerateErrorResponse(400, string.Format("IsPostPaidQuotation must be a boolean value"));
            OrdersManagement.Core.Client client = new OrdersManagement.Core.Client(responseFormat: OrdersManagement.ResponseFormat.JSON);
            context.Response.Write(client.ViewInvoice(quotationId, isPostPaidQuotation));
        }
        private void Download(HttpContext context)
        {
            int quotationId = 0;
            bool isPostPaidQuotation = false;
            if (context.Request["QuotationId"] != null && !int.TryParse(context.Request["QuotationId"].ToString(), out quotationId))
                GenerateErrorResponse(400, string.Format("OnlyActive value ({0}) is not a valid boolean value", context.Request["OnlyActive"].ToString()));
            if (quotationId <= 0)
                GenerateErrorResponse(400, string.Format("QuoationId must be greater than 0"));
            if (context.Request["IsPostPaidQuotation"] != null && !bool.TryParse(context.Request["IsPostPaidQuotation"].ToString(), out isPostPaidQuotation))
                GenerateErrorResponse(400, string.Format("IsPostPaidQuotation must be a boolean value"));
            OrdersManagement.Core.Client client = new OrdersManagement.Core.Client(responseFormat: OrdersManagement.ResponseFormat.JSON);
            context.Response.Write(client.DownloadInvoice(quotationId, isPostPaidQuotation));
        }
        private void GenerateErrorResponse(int statusCode, string message)
        {
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.StatusCode = statusCode;
            errorJSon["Message"] = message;
            HttpContext.Current.Response.Write(errorJSon);
            //HttpContext.Current.ApplicationInstance.CompleteRequest();
            try
            {
                HttpContext.Current.Response.End();
            }
            catch (System.Threading.ThreadAbortException e)
            { }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}