using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.IO;
using System.Data;
using CommonClass;

namespace APILib
{
    public class APIClass
    {
        public enum HttpVerb
        {
            GET,
            POST,
            PUT,
            PATCH,
            DELETE
        }

        public enum AuthenticationType
        {
            NONE,
            APIKEY,
            BEARERTOKEN,
            BASIC,
            OAUTH2
        }



        
        const double TOKENEXPIRYMINUTES = 5;

        public string APIKey { get; set; }

        public string EndPoint { get; set; }
        public HttpVerb Method { get; set; }

        public List<KeyValuePair<string, string>> Parameters { get; set; }
        public string ContentType { get; set; }
        public string PostData { get; set; }

        public string LastStep { get; set; }

        public string LastEndpoint { get; set; }

        public string LastJSON { get; set; }

        public string LastStatus { get; set; }

        public string LastId { get; set; }

        public string LastError { get; set; }

        public bool CheckProcessBusy()
        {
            return ProcessBusy;
        }



        bool ProcessBusy = false;


        public APIClass()
        {
            ContentType = "application/json"; //"text/xml";

        }

        //public API(string endpoint, HttpVerb method, string postData, List<KeyValuePair<string, string>> AuthParams)
        //{
        //    EndPoint = endpoint;
        //    Method = method;
        //    ContentType = "application/json";
        //    PostData = postData;

        //}

        public string WriteAPI(string url, string endpoint, string filter, string apikey, HttpVerb MethodType, string OutputJson)
        {
            string ReturnValue = "";
            LastError = "";


            try
            {
                    


                EndPoint = url;
                    
                if (!(url.LastIndexOf("/") >= url.Length - 1))
                    EndPoint += "/";
                EndPoint += endpoint + filter;
                    

                Method = MethodType;



                if (OutputJson == null || OutputJson == "")
                    ReturnValue = "ERROR: No body available";
                if (EndPoint == "")
                    ReturnValue = "ERROR: Incorrect parameters suppplid";

                if (ReturnValue == "")
                {

                    ReturnValue = MakeRequest(EndPoint,apikey,Method,OutputJson);
                }

            }
            catch (Exception ex)
            {
                ReturnValue = "ERROR: " + ex.Message;
            }
            LastEndpoint = EndPoint;
               



            return ReturnValue;// + " Endpoint: " + EndPoint + "      JSON: " + json;
        }


        public DataTable ReadAPI(string url, string endpoint, string filter, string apikey, HttpVerb MethodType = HttpVerb.GET, int PageSize = 0, int MaxPageCount = 0)
        {

            DataTable dtReturn = new DataTable();
            LastError = "";

            try
            {
                string Response = "";
                string PageFilter = "";
                string LastPageFilter = "";
                string LastNextPage = "";
                string BaseEndPoint = url;
                int LastSkip = 0;


                if (PageSize > 0 && !filter.ToUpper().Contains("$TOP"))
                    PageFilter = "$top=" + PageSize.ToString();


                if (endpoint.Length > 0)
                {
                    if (!BaseEndPoint.EndsWith("/"))
                        BaseEndPoint += "/";
                    BaseEndPoint += endpoint;
                }
                Console.WriteLine("Endpoint: " + BaseEndPoint + ". Filter = " + filter);

                Method = MethodType;


                string NextPagelink = "";
                int PageLinkIndex = 0;
                int PageCount = 0;
                int PageError = 0;
                bool NextPageAvailable = true;
                while (NextPageAvailable)
                {
                    try
                    {
                        EndPoint = BaseEndPoint;
                        if (filter.Length > 1)
                        {
                            EndPoint += "?" + filter;
                            if (!NextPagelink.ToUpper().Contains("FILTER") && NextPagelink != "")
                                NextPagelink += "&" + filter;
                        }

                        if (PageFilter != "" && LastPageFilter != PageFilter)
                        {
                            if (!EndPoint.Contains("?"))
                                EndPoint += "?" + PageFilter;
                            else
                                EndPoint += "&" + PageFilter;

                            LastPageFilter = PageFilter;
                        }



                        // Request next page
                        // Note: this is where a delay can be added to prevent a rate limit
                        //Response = MakeRequest(endpoint, API key name, APIkey, TypeOfAuthentication, APIKey);

                        Response = MakeRequest(EndPoint,apikey,MethodType,"");


                        DataTable dtResponseTable = new DataTable();
                        if (Response.Contains("{"))
                            dtResponseTable = TabulateJSON(Response);

                        PageFilter = "";

                        //merge data
                        if (dtResponseTable != null && dtResponseTable.Rows.Count > 0)
                        {
                            PageCount++;
                            if (dtReturn == null || dtReturn.Rows.Count == 0)
                                dtReturn = dtResponseTable;
                            else
                                dtReturn.Merge(dtResponseTable);

                            // page filters
                            PageFilter = "";
                            if (PageSize > 1 && dtResponseTable.Rows.Count > 0 && !filter.ToUpper().Contains("SKIP") && !filter.ToUpper().Contains("LIMIT") && (MaxPageCount == 0 || MaxPageCount > PageCount))
                            {
                                int Skip = PageCount * PageSize;
                                PageFilter = "limit=" + PageSize.ToString() + "&skip=" + Skip.ToString();
                            }
                        }
                        else
                        {
                            if (Response.ToUpper().Contains("ERROR") || PageCount == 0)
                                PageError++;
                            NextPageAvailable = false;
                        }

                        PageLinkIndex = 0;
                        NextPagelink = "";

                        if ((LastNextPage == NextPagelink && NextPagelink != "") || (NextPagelink == "" && LastPageFilter == PageFilter))
                            NextPageAvailable = false;

                    }
                    catch (Exception ex1)
                    {
                        NextPageAvailable = false;
                        PageError++;
                        LastError += " Response handler error: " + ex1.Message;
                    }
                    //Check max pages
                    if (MaxPageCount > 0 && MaxPageCount <= PageCount)
                        NextPageAvailable = false;
                }

                if (PageError > 0)
                {
                    string PageErrorCode = Response;
                    if (PageErrorCode.Length > 200)
                        PageErrorCode = PageErrorCode.Substring(0, 200);
                    LastError += " Page errors encountered " + PageErrorCode;
                }


            }
            catch (Exception ex)
            {
                LastError += ex.Message;
            }
            return dtReturn;

        }


        public string MakeRequest(string endpoint, string apikey, HttpVerb Method, string postdata)
        {

            LastStep = "Start";
            var responseValue = ""; // string.Empty;
            try
            {
                string url = @"" + endpoint ;

                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = Method.ToString();
                //request.Method = HttpVerb.GET.ToString();

                //request.ContentLength = 0;
                request.ContentType = ContentType;


                // Authentication is using API key                    
                request.Headers["x-api-key"] = apikey;





                //request.Headers["Accept"] = ContentType;

                LastStep = "Checking for PostMethod";


                if (!string.IsNullOrEmpty(postdata) && (Method == HttpVerb.POST || Method == HttpVerb.PATCH))
                {

                    try
                    {
                        LastStep = "Writing response";
                        //var encoding = new UTF8Encoding();
                        //var bytes = Encoding.GetEncoding("iso-8859-1").GetBytes(PostData);
                        //request.ContentLength = bytes.Length;
                        request.ContentLength = postdata.Length;

                        StreamWriter requestWriter = new StreamWriter(request.GetRequestStream(), System.Text.Encoding.ASCII);
                        requestWriter.Write(postdata);
                        requestWriter.Close();
                    }
                    catch (Exception apierror)
                    {
                        if (apierror == null)
                            responseValue = "ERROR: WriteError";
                        else
                            responseValue = "ERROR: WriteError - " + apierror.Message;
                        LastError = responseValue;
                        return responseValue;
                    }

                }

                try
                {
                    LastStep = "Get response";

                    HttpWebResponse response = null;
                    try
                    {
                        response = (HttpWebResponse)request.GetResponse();
                    }
                    catch (Exception ex)
                    {
                        responseValue = "ERROR: " + ex.Message;
                        LastStatus = "ERROR: " + ex.Message;

                    }

                    if (!responseValue.StartsWith("ERROR: "))
                    {

                        LastStep = "Reading response stream";
                        Stream webStream = response.GetResponseStream();
                        StreamReader responseReader = new StreamReader(webStream);
                        responseValue += responseReader.ReadToEnd();



                        LastStatus = response.StatusCode.ToString();
                        responseReader.Close();

                    }
                }
                catch (WebException webex1)
                {

                    LastStep = "Get error response";
                    responseValue = "Error Message = " + webex1.Message;
                    HttpWebResponse errorresponse = (HttpWebResponse)webex1.Response;
                    Stream webStreamerror = errorresponse.GetResponseStream();
                    StreamReader errorresponseReader = new StreamReader(webStreamerror);
                    string errorresponseValue = "";
                    errorresponseValue += errorresponseReader.ReadToEnd();
                    if (errorresponseValue.Length > 0)
                        responseValue += " Message = " + errorresponseValue;

                    LastStatus = errorresponse.StatusCode.ToString();
                    LastError = responseValue;
                    errorresponseReader.Close();
                }

            }
            catch (Exception ex)
            {
                responseValue = "ERROR: " + ex.Message + " Step: " + LastStep + responseValue;
            }

            return responseValue;

        }



        public DataTable TabulateJSON(string json)
        {
            DataTable dtResult = new DataTable();
            try
            {
                JsonHandler _clsJson = new JsonHandler();
                dtResult = _clsJson.TabulateJSON(json);

            }
            catch (Exception ex)
            {
                LastError = ex.Message;
            }
            return dtResult;
        }
        
    }
}
