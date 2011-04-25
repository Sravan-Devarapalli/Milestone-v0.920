using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Xml;

namespace testExport
{
    public class testExport
    {
        public static void exportExcelData(String SProc)
        {

            try
            {
                SqlConnection conn = new SqlConnection("Integrated Security=yes;Initial Catalog=UAPracticeManagement_Copy;Data Source=(local)");
                conn.Open();
                
                SqlCommand command = new SqlCommand(SProc, conn);
                command.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataSet dataset = new DataSet();
                adapter.Fill(dataset);
                Microsoft.Office.Interop.Excel.ApplicationClass excel = new Microsoft.Office.Interop.Excel.ApplicationClass();
                excel.Application.Workbooks.Add(true);
                System.Data.DataTable dataTable = dataset.Tables[0];
                int ColumnIndex = 0;
                foreach (System.Data.DataColumn col in dataTable.Columns)
                {
                    ColumnIndex++;
                    excel.Cells[1, ColumnIndex] = col.ColumnName;
                }
                int rowIndex = 0;
                foreach (DataRow row in dataTable.Rows)
                {
                    rowIndex++;
                    ColumnIndex = 0;
                    foreach (DataColumn col in dataTable.Columns)
                    {
                        ColumnIndex++;
                        excel.Cells[rowIndex + 1, ColumnIndex] = row[col.ColumnName];
                    }
                }
                excel.Visible = true;
                Microsoft.Office.Interop.Excel.Worksheet worksheet = (Microsoft.Office.Interop.Excel.Worksheet)excel.ActiveSheet;
                worksheet.Activate();
            }
            catch (XmlException exml)			
            {				
                // catch an xmlexception errors				
                Console.WriteLine(exml.Message);
            }
          
     
        }
    }
}

