using Microsoft.Office.InfoPath;
using System;
using System.Xml;
using System.Xml.XPath;

namespace Expense_Report_With_Code
{
    public partial class FormCode
    {
        // Member variables are not supported in browser-enabled forms.
        // Instead, write and read these values from the FormState
        // dictionary using code such as the following:
        //
        // private object _memberVariable
        // {
        //     get
        //     {
        //         return FormState["_memberVariable"];
        //     }
        //     set
        //     {
        //         FormState["_memberVariable"] = value;
        //     }
        // }

        // NOTE: The following procedure is required by Microsoft InfoPath.
        // It can be modified using Microsoft InfoPath.
        public void InternalStartup()
        {
            EventManager.XmlEvents["/my:SPWFiAExpenseReport/my:Expenses/my:Expense/my:Amount"].Changed += new XmlChangedEventHandler(Amount_Changed);
        }

        public void Amount_Changed(object sender, XmlEventArgs e)
        {
            // Write your code here to change the main data source.
            XPathNavigator formData = this.CreateNavigator();
            XPathNavigator expenseGroup = formData.SelectSingleNode("/my:SPWFiAExpenseReport/my:Expenses",NamespaceManager);
            XPathNodeIterator expenses = expenseGroup.SelectDescendants("Expense",expenseGroup.NamespaceURI,false);
            double total = 0;
            foreach (XPathNavigator expense in expenses)
            {
                string value = expense.SelectSingleNode("my:Amount", NamespaceManager).Value;
                if (value == string.Empty)
                {
                    value = "0";
                }
                double amount = double.Parse(value);
                total += amount;
            }
            XPathNavigator xnfield1 = formData.SelectSingleNode("/my:SPWFiAExpenseReport/my:TotalAmount", NamespaceManager);
            DeleteNil(xnfield1);
            xnfield1.SetValue(total.ToString());
        }

        public void DeleteNil(XPathNavigator node)
        {
            if (node.MoveToAttribute("nil", "http://www.w3.org/2001/XMLSchema-instance"))
            {
                node.DeleteSelf();
            }
        }
    }
}
