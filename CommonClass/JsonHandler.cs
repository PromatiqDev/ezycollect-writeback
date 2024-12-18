using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data;

namespace CommonClass
{
    public class JsonHandler
    {

        public string SerialiseJSON(DataTable InputData, DataTable DefinitionData, string AllowArray, string IDField, string FilterURL, string parentName = "")
        {
            string jsonresult = "";
            string LastId = "";
            try
            {
                Dictionary<string, object> parentRow;
                Dictionary<string, object> childRow;

                string jsonarray = "";
                string jsonheader = "";
                string LastPrimaryKey = "";
                string arrayname = "";
                int ParentRowCount = 0;
                int ParentCount = 0;
                parentRow = new Dictionary<string, object>();
                foreach (DataRow dr in InputData.Rows)
                {
                    if (AllowArray.ToUpper() != "TRUE" && AllowArray != "1" && jsonresult.Contains("{"))
                        return jsonresult;
                    string RowPrimaryKey = "";
                    parentRow.Clear();
                    childRow = new Dictionary<string, object>();
                    arrayname = "";
                    foreach (DataColumn col in InputData.Columns)
                    {
                        Object ColValue = dr[col];



                        if (DefinitionData != null & DefinitionData.Rows.Count > 0)
                        {
                            string searchExpression = "Destination = '" + col.ColumnName + "'";
                            DataRow[] foundRows = DefinitionData.Select(searchExpression);
                            if (foundRows.Length > 0)
                            {
                                //convert type
                                try
                                {
                                    if (foundRows[0]["Type"].ToString().ToUpper().Contains("INT"))
                                        ColValue = int.Parse(dr[col].ToString());
                                    if (foundRows[0]["Type"].ToString().ToUpper().Contains("DECIMAL"))
                                        ColValue = decimal.Parse(dr[col].ToString());
                                    if (foundRows[0]["Type"].ToString().ToUpper().Contains("FLOAT"))
                                        ColValue = float.Parse(dr[col].ToString());
                                    if (foundRows[0]["Type"].ToString().ToUpper().Contains("SINGLE"))
                                        ColValue = Single.Parse(dr[col].ToString());
                                    if (foundRows[0]["Type"].ToString().ToUpper().Contains("DOUBLE"))
                                        ColValue = double.Parse(dr[col].ToString());
                                }
                                catch
                                {
                                }

                                if (foundRows[0]["PrimaryKey"].ToString() == "1")
                                    RowPrimaryKey += dr[col].ToString();
                                if (foundRows[0]["ArrayName"].ToString().Length > 0)
                                {
                                    childRow.Add(col.ColumnName, ColValue);
                                    if (arrayname == "")
                                        arrayname = foundRows[0]["ArrayName"].ToString();
                                }
                                else
                                {
                                    if (col.ColumnName.ToUpper() != IDField.ToUpper() || FilterURL == "0" || FilterURL.ToUpper() == "FALSE")
                                    {
                                        parentRow.Add(col.ColumnName, ColValue);
                                        ParentCount++;
                                    }

                                    if (col.ColumnName.ToUpper() == IDField.ToUpper())
                                        LastId = ColValue.ToString();
                                }
                            }
                        }
                        else
                            parentRow.Add(col.ColumnName, dr[col]);
                    }

                    if (parentName != "" && LastPrimaryKey != RowPrimaryKey)
                    {
                        if (jsonheader != "")
                        {
                            if (jsonresult != "")
                                jsonresult += ",";
                            jsonresult += jsonheader;
                            if (arrayname != "" && jsonarray != "")
                            {
                                if (jsonresult.EndsWith("}") || jsonresult.LastIndexOf("}") > jsonresult.Length - 2)
                                    jsonresult = jsonresult.Substring(0, jsonresult.LastIndexOf("}"));
                                jsonresult += ",\"" + arrayname + "\":[" + jsonarray + "]}";

                            }

                        }

                        jsonheader = JsonConvert.SerializeObject(parentRow, Formatting.Indented);
                        jsonarray = "";
                        ParentRowCount++;
                    }
                    if (arrayname != "" && childRow.Count > 0)
                    {
                        if (jsonarray != "")
                            jsonarray += ",";
                        jsonarray += JsonConvert.SerializeObject(childRow, Formatting.Indented);
                    }


                    LastPrimaryKey = RowPrimaryKey;
                    RowPrimaryKey = "";
                }

                if (jsonheader == "" && ParentCount > 0)
                {
                    jsonheader = JsonConvert.SerializeObject(parentRow, Formatting.Indented);

                }

                if (jsonheader != "")
                {
                    if (jsonresult != "")
                        jsonresult += ",";
                    jsonresult += jsonheader;

                }



                if (arrayname != "" && jsonarray != "")
                {
                    if (jsonresult == "")
                        jsonresult = "{";
                    else
                    {
                        if (jsonresult.EndsWith("}") || jsonresult.LastIndexOf("}") > jsonresult.Length - 2)
                            jsonresult = jsonresult.Substring(0, jsonresult.LastIndexOf("}"));
                        if (jsonresult.Length > 5)
                            jsonresult += ",";
                    }
                    jsonresult += "\"" + arrayname + "\":[" + jsonarray + "]}";

                }


                if (ParentRowCount > 1 && parentName != "")
                    jsonresult = "{\"" + parentName + "\":[" + jsonresult + "]}";


            }
            catch (Exception ex)
            {
                jsonresult = "{\"Error\":\"" + ex.Message + "\"}";
            }
            return jsonresult;
        }


        public DataTable TabulateJSON(string json)
        {
            DataTable dtResult = new DataTable();
            try
            {


                string flattenedjson = "";
                string flattenedjson1 = "";
                string flattenedjson2 = "";
                string flattenedjson3 = "";
                string flattenedjson4 = "";
                //Level 1
                List<string> jsonList1 = GetJsonList(json, 1);
                if (jsonList1.Count > 0)
                {
                    foreach (string Level1 in jsonList1)
                    {
                        flattenedjson1 = "";
                        if (Level1.Contains("<#>"))
                        {
                            int Index1Start = Level1.IndexOf("<#>");
                            string Level1Prefix = Level1.Substring(0, Index1Start);
                            string Level1Suffix = Level1.Substring(Index1Start + 3);
                            int Index1End = Level1Suffix.IndexOf("<#>");
                            string Replace1 = Level1Suffix.Substring(0, Index1End);
                            Level1Suffix = Level1Suffix.Substring(Index1End + 3);
                            List<string> jsonList2 = GetJsonList(Replace1, 2);
                            if (jsonList2.Count > 0)
                            {
                                foreach (string Level2 in jsonList2)
                                {
                                    flattenedjson2 = "";
                                    if (Level2.Contains("<#>"))
                                    {
                                        int Index2Start = Level2.IndexOf("<#>");
                                        string Level2Prefix = Level2.Substring(0, Index2Start);
                                        string Level2Suffix = Level2.Substring(Index2Start + 3);
                                        int Index2End = Level2Suffix.IndexOf("<#>");
                                        string Replac2 = Level2Suffix.Substring(0, Index2End);
                                        Level2Suffix = Level2Suffix.Substring(Index2End + 3);
                                        List<string> jsonList3 = GetJsonList(Replac2, 3);
                                        if (jsonList3.Count > 0)
                                        {
                                            foreach (string Level3 in jsonList3)
                                            {
                                                flattenedjson3 = "";
                                                if (Level3.Contains("<#>"))
                                                {
                                                    int Index3Start = Level3.IndexOf("<#>");
                                                    string Level3Prefix = Level3.Substring(0, Index3Start);
                                                    string Level3Suffix = Level2.Substring(Index3Start + 3);
                                                    int Index3End = Level3Suffix.IndexOf("<#>");
                                                    string Replace3 = Level3Suffix.Substring(0, Index3End);
                                                    Level3Suffix = Level3Suffix.Substring(Index3End + 3);
                                                    List<string> jsonList4 = GetJsonList(Replace3, 4);
                                                    if (jsonList4.Count > 0)
                                                    {
                                                        foreach (string Level4 in jsonList4)
                                                        {
                                                            flattenedjson4 = "";
                                                            if (!Level4.Contains("<#>"))
                                                            {
                                                                flattenedjson4 = Level4;
                                                                if (Level1Prefix != "" && !Level1Prefix.Trim().EndsWith(",") && !Level2Prefix.Trim().StartsWith(","))
                                                                    Level1Prefix += ",";
                                                                if (Level2Prefix != "" && !Level2Prefix.Trim().EndsWith(",") && !Level3Prefix.Trim().StartsWith(","))
                                                                    Level2Prefix += ",";
                                                                if (Level3Prefix != "" && !Level3Prefix.Trim().EndsWith(",") && !flattenedjson4.Trim().StartsWith(","))
                                                                    Level3Prefix += ",";
                                                                if (flattenedjson4 != "" && !flattenedjson4.Trim().EndsWith(",") && !Level3Suffix.StartsWith(",") && Level3Suffix + Level2Suffix + Level1Suffix != "")
                                                                    flattenedjson4 += ",";
                                                                if (Level3Suffix != "" && !Level3Suffix.Trim().EndsWith(",") && !Level2Suffix.Trim().StartsWith(",") && Level2Suffix + Level1Suffix != "")
                                                                    Level3Suffix += ",";
                                                                if (Level2Suffix != "" && !Level2Suffix.Trim().EndsWith(",") && !Level1Suffix.Trim().StartsWith(",") && Level1Suffix != "")
                                                                    Level2Suffix += ",";
                                                                flattenedjson3 += Level1Prefix + Level2Prefix + Level3Prefix + flattenedjson4 + Level3Suffix + Level2Suffix + Level1Suffix;
                                                                if (flattenedjson != "" && !flattenedjson.Trim().EndsWith(","))
                                                                    flattenedjson += ",";
                                                                if (flattenedjson3.Trim().StartsWith(","))
                                                                    flattenedjson3 = flattenedjson3.Trim().Substring(1);
                                                                flattenedjson += "{" + flattenedjson3 + "}";
                                                                flattenedjson3 = "";
                                                            }

                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    flattenedjson3 = Level3;
                                                    if (Level1Prefix != "" && !Level1Prefix.Trim().EndsWith(",") && !Level2Prefix.Trim().StartsWith(","))
                                                        Level1Prefix += ",";
                                                    if (Level2Prefix != "" && !Level2Prefix.Trim().EndsWith(",") && !flattenedjson3.Trim().StartsWith(","))
                                                        Level2Prefix += ",";
                                                    if (flattenedjson3 != "" && !flattenedjson3.EndsWith(",") && Level2Suffix != "" && !Level2Suffix.Trim().StartsWith(",") && Level2Suffix + Level1Suffix != "")
                                                        flattenedjson3 += ",";
                                                    if (Level2Suffix != "" && !Level2Suffix.EndsWith(",") && Level1Suffix != "" && !Level1Suffix.Trim().StartsWith(","))
                                                        Level2Suffix += ",";
                                                    flattenedjson2 += Level1Prefix + Level2Prefix + flattenedjson3 + Level2Suffix + Level1Suffix;
                                                    if (flattenedjson != "" && !flattenedjson.Trim().EndsWith(","))
                                                        flattenedjson += ",";
                                                    if (flattenedjson2.Trim().StartsWith(","))
                                                        flattenedjson2 = flattenedjson2.Trim().Substring(1);
                                                    flattenedjson += "{" + flattenedjson2 + "}";
                                                    flattenedjson2 = "";
                                                }

                                            }
                                        }

                                    }
                                    else
                                    {
                                        flattenedjson2 = Level2;
                                        if (Level1Prefix != "" && !Level1Prefix.Trim().EndsWith(",") && !flattenedjson2.Trim().StartsWith(","))
                                            Level1Prefix += ",";
                                        if (flattenedjson2 != "" && !flattenedjson2.EndsWith(",") && Level1Suffix != "" && !Level1Suffix.Trim().StartsWith(","))
                                            flattenedjson2 += ",";
                                        flattenedjson1 += Level1Prefix + flattenedjson2 + Level1Suffix;
                                        if (flattenedjson != "" && !flattenedjson.Trim().EndsWith(","))
                                            flattenedjson += ",";
                                        if (flattenedjson1.Trim().StartsWith(","))
                                            flattenedjson1 = flattenedjson1.Trim().Substring(1);
                                        flattenedjson += "{" + flattenedjson1 + "}";
                                        flattenedjson1 = "";
                                    }

                                }
                            }

                        }
                        else
                        {
                            flattenedjson1 = Level1;
                            if (flattenedjson != "" && !flattenedjson.Trim().EndsWith(","))
                                flattenedjson += ",";
                            if (flattenedjson1.Trim().StartsWith(","))
                                flattenedjson1 = flattenedjson1.Trim().Substring(1);
                            flattenedjson += "{" + flattenedjson1 + "}";
                            flattenedjson1 = "";
                        }


                    }
                    flattenedjson = "[" + flattenedjson + "]";

                    dtResult = JsonConvert.DeserializeObject<DataTable>(flattenedjson);
                }


            }
            catch (Exception ex)
            {

            }
            return dtResult;
        }


        //Flatten into a list
        public List<string> GetJsonList(string json, int Level)
        {
            List<string> JsonResult = new List<string>();
            try
            {

                string HeaderPrefix = "";
                try
                {
                    if (json.Trim().StartsWith("["))
                        json = "{\"Items\":" + json + "}";
                    else
                    {
                        //ensure we have an array at the top level
                        int Index1 = json.IndexOf(":");
                        try
                        {
                            if (json.Substring(Index1 + 1, 1) == "{")
                            {
                                json = json.Substring(0, Index1 + 1) + '[' + json.Substring(Index1 + 1, json.Length - Index1 - 2) + "]}";

                            }

                        }
                        catch { }



                        if (Level > 1)
                        {
                            // get nested header
                            try
                            {
                                int IndexStart = json.IndexOf(":");
                                string Prefix = json.Substring(0, IndexStart);
                                IndexStart = Prefix.IndexOf("\"");
                                Prefix = Prefix.Substring(IndexStart);
                                Prefix = Prefix.Replace("\"", "");
                                if (Prefix != "")
                                    HeaderPrefix = Prefix + "_";
                            }
                            catch { }
                        }



                    }
                    //handle single item as an array with 1 line
                    if (!json.Contains("["))
                        json = "{\"Items\":[" + json + "]}";
                }
                catch
                {

                }
                var jsonLinq = JObject.Parse(json);


                // Find the first array using Linq
                var srcArray = jsonLinq.Descendants().Where(d => d is JArray).First();


                foreach (JObject row in srcArray.Children<JObject>())
                {
                    string headerjson = "";
                    foreach (JProperty column in row.Properties())
                    {
                        // Only include JValue types
                        if (column.Value is JValue)
                        {
                            if (headerjson != "")
                                headerjson += ",";
                            headerjson += column.ToString().Replace(column.Name.ToString(), HeaderPrefix + column.Name.ToString());

                        }
                        else
                        {
                            try
                            {
                                string HeaderColumn = column.Name.ToString();
                                if (!column.Value.ToString().Contains("[") && column.Value.ToString().Contains("{"))
                                {
                                    string jsonproperty = "";
                                    var sub_jsonLinq = JObject.Parse(column.Value.ToString());
                                    foreach (JProperty sub_column in sub_jsonLinq.Properties())
                                    {

                                        // Only include JValue types
                                        if (sub_column.Value is JValue)
                                        {
                                            if (jsonproperty != "")
                                                jsonproperty += ",";
                                            jsonproperty += sub_column.ToString().Replace(sub_column.Name.ToString(), HeaderPrefix + HeaderColumn + "_" + sub_column.Name.ToString());
                                        }
                                        else
                                        {
                                            if (!sub_column.Value.ToString().Contains("[") && sub_column.Value.ToString().Contains("{"))
                                            {
                                                string jsonsubproperty = "";
                                                var sub_sub_jsonLinq = JObject.Parse(sub_column.Value.ToString());
                                                //sub-property
                                                foreach (JProperty sub_sub_column in sub_sub_jsonLinq.Properties())
                                                {
                                                    string SubHeaderColumn = sub_column.Name.ToString();
                                                    // Only include JValue types
                                                    if (sub_sub_column.Value is JValue)
                                                    {
                                                        if (jsonsubproperty != "" && !jsonsubproperty.Trim().EndsWith(","))
                                                            jsonsubproperty += ",";
                                                        jsonsubproperty += sub_sub_column.ToString().Replace(sub_sub_column.Name.ToString(), HeaderPrefix + HeaderColumn + "_" + SubHeaderColumn + "_" + sub_sub_column.Name.ToString());
                                                    }
                                                    else
                                                    {
                                                        //Add left over
                                                        if (column.Value.ToString().Contains("["))
                                                        {
                                                            if (jsonsubproperty != "" && !jsonsubproperty.Trim().EndsWith(","))
                                                                jsonsubproperty += ",";
                                                            jsonsubproperty += "<#>" + "{\"" + HeaderColumn + "_" + SubHeaderColumn + sub_sub_column.Name.ToString() + "\":" + sub_sub_column.Value.ToString() + "}<#>";
                                                        }
                                                    }

                                                }
                                                if (jsonsubproperty != "")
                                                {
                                                    if (jsonproperty != "" && !jsonproperty.Trim().EndsWith(","))
                                                        jsonproperty += ",";
                                                    jsonproperty += jsonsubproperty;
                                                }



                                            }
                                            else
                                            {
                                                if (sub_column.Value.ToString().Contains("["))
                                                {
                                                    if (jsonproperty != "" && !jsonproperty.Trim().EndsWith(","))
                                                        jsonproperty += ",";
                                                    jsonproperty += "<#>" + "{\"" + HeaderColumn + "_" + sub_column.Name.ToString() + "\":" + sub_column.Value.ToString() + "}<#>";
                                                }
                                            }
                                        }
                                    }
                                    //Add property and left over property/ 
                                    if (jsonproperty != "")
                                    {
                                        if (headerjson != "" && !headerjson.Trim().EndsWith(","))
                                            headerjson += ",";
                                        headerjson += jsonproperty;
                                    }
                                }
                                else
                                {
                                    if (column.Value.ToString().Contains("["))
                                    {
                                        if (headerjson != "" && !headerjson.Trim().EndsWith(","))
                                            headerjson += ",";
                                        headerjson += "<#>" + "{\"" + HeaderColumn + "\":" + column.Value.ToString() + "}<#>";
                                    }
                                }
                            }
                            catch (Exception ex) { }
                        }

                    }
                    if (headerjson != "")
                        JsonResult.Add(headerjson);
                    headerjson = "";
                }
            }
            catch (Exception ex)
            {

            }
            return JsonResult;
        }
    }
}
