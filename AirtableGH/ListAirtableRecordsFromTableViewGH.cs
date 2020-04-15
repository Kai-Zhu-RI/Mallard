﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grasshopper.Kernel;
using AirtableApiClient;

namespace AirtableGH
{
    public class ListAirtableRecordsFromTableViewGH : GH_Component
    {

        public ListAirtableRecordsFromTableViewGH() : base("List Airtable Records from Table View", "List",
            "Retrieve a list of Airtable Records from a specific Airtable Base", "Mallard", "Database")
        {

        }


        public override Guid ComponentGuid
        {
            // Don't copy this GUID, make a new one
            get { return new Guid("5ab4f1ec-a99c-40f4-8853-e85089e287d6"); }
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBooleanParameter("Refresh?", "B", "Boolean button to refresh solution", GH_ParamAccess.item);
            pManager.AddTextParameter("Base ID", "ID", "ID of Airtable Base", GH_ParamAccess.item);
            pManager.AddTextParameter("App Key", "K", "App Key for Airtable Base", GH_ParamAccess.item);
            pManager.AddTextParameter("Table Name", "T", "Name of table in Airtable Base", GH_ParamAccess.item);
            pManager.AddTextParameter("View Name", "V", "Name of view in table", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Error Message", "E", "Error Message string", GH_ParamAccess.item);
            pManager.AddGenericParameter("Out Records", "O", "Out Record Result string", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Declare a variable for the input String
            bool data = false;

            // Use the DA object to retrieve the data inside the first input parameter.
            // If the retieval fails (for example if there is no data) we need to abort.
            if (!DA.GetData(0, ref data))
            {
                records.Clear();
                return;
            }
            if (!DA.GetData(1, ref baseID)) { return; }
            if (!DA.GetData(2, ref appKey)) { return; }
            if (!DA.GetData(3, ref tablename)) { return; }
            if (!DA.GetData(4, ref view)) { return; }

            // If the retrieved data is Nothing, we need to abort.
            // We're also going to abort on a zero-length String.
            if (data == false)
            {
                records.Clear();
                return;
            }



            AirtableBase airtableBase = new AirtableBase(appKey, baseID);
            Task OutResponse = ListRecordsMethodAsync(airtableBase);
            var responseString = OutResponse.ToString();
            if (response != null)
            {
                if (response.Records != null)
                {
                    records.AddRange(response.Records.ToList());
                    errorMessageString = "Success!";
                }
            }


            // Use the DA object to assign a new String to the first output parameter.
            DA.SetData(0, errorMessageString);
            DA.SetDataList(1, records);


        }

        //
        public string baseID = "";
        public string appKey = "";
        public string tablename = "";
        public string stringID = "";
        public string errorMessageString = "No response yet, refresh to try again";
        public string attachmentFieldName = "Name";
        public List<Object> records = new List<object>();
        public string offset = null;
        public IEnumerable<string> fieldsArray = null;
        public string filterByFormula = null;
        public int? maxRecords = null;
        public int? pageSize = null;
        public IEnumerable<Sort> sort = null;
        public string view = "";
        public int b = 1;
        public AirtableListRecordsResponse response;

        //

        public async Task ListRecordsMethodAsync(AirtableBase airtableBase)
        {

            Task<AirtableListRecordsResponse> task = airtableBase.ListRecords(
                                   tablename,
                                   offset,
                                   fieldsArray,
                                   filterByFormula,
                                   maxRecords,
                                   pageSize,
                                   sort,
                                   view);

            response = await task;


            if (response.AirtableApiError.ErrorMessage != null)
            {
                // Error reporting
                errorMessageString = response.AirtableApiError.DetailedErrorMessage2;
            }
            else
            {
                // Do something with the retrieved 'records' and the 'offset'
                // for the next page of the record list.

            }



        }




        protected override System.Drawing.Bitmap Icon => Properties.Resources.AirtableList2;





    }
}

