﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.225
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System.Xml.Serialization;

// 
// This source code was auto-generated by xsd, Version=4.0.30319.1.
// 


/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://schemas.microsoft.com/office/infopath/2003/myXSD/2011-04-29T06:14:18")]
[System.Xml.Serialization.XmlRootAttribute(Namespace="http://schemas.microsoft.com/office/infopath/2003/myXSD/2011-04-29T06:14:18", IsNullable=false)]
public partial class SPWFiAExpenseReport {
    
    private Expense[] expensesField;
    
    private System.Nullable<double> totalAmountField;
    
    private bool totalAmountFieldSpecified;
    
    private string expenseTypeField;
    
    private System.Xml.XmlAttribute[] anyAttrField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlArrayItemAttribute("Expense", IsNullable=false)]
    public Expense[] Expenses {
        get {
            return this.expensesField;
        }
        set {
            this.expensesField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
    public System.Nullable<double> TotalAmount {
        get {
            return this.totalAmountField;
        }
        set {
            this.totalAmountField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlIgnoreAttribute()]
    public bool TotalAmountSpecified {
        get {
            return this.totalAmountFieldSpecified;
        }
        set {
            this.totalAmountFieldSpecified = value;
        }
    }
    
    /// <remarks/>
    public string ExpenseType {
        get {
            return this.expenseTypeField;
        }
        set {
            this.expenseTypeField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAnyAttributeAttribute()]
    public System.Xml.XmlAttribute[] AnyAttr {
        get {
            return this.anyAttrField;
        }
        set {
            this.anyAttrField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://schemas.microsoft.com/office/infopath/2003/myXSD/2011-04-29T06:14:18")]
[System.Xml.Serialization.XmlRootAttribute(Namespace="http://schemas.microsoft.com/office/infopath/2003/myXSD/2011-04-29T06:14:18", IsNullable=false)]
public partial class Expense {
    
    private System.Nullable<System.DateTime> expenseDateField;
    
    private bool expenseDateFieldSpecified;
    
    private System.Nullable<double> amountField;
    
    private bool amountFieldSpecified;
    
    private string descriptionField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
    public System.Nullable<System.DateTime> ExpenseDate {
        get {
            return this.expenseDateField;
        }
        set {
            this.expenseDateField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlIgnoreAttribute()]
    public bool ExpenseDateSpecified {
        get {
            return this.expenseDateFieldSpecified;
        }
        set {
            this.expenseDateFieldSpecified = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
    public System.Nullable<double> Amount {
        get {
            return this.amountField;
        }
        set {
            this.amountField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlIgnoreAttribute()]
    public bool AmountSpecified {
        get {
            return this.amountFieldSpecified;
        }
        set {
            this.amountFieldSpecified = value;
        }
    }
    
    /// <remarks/>
    public string Description {
        get {
            return this.descriptionField;
        }
        set {
            this.descriptionField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://schemas.microsoft.com/office/infopath/2003/myXSD/2011-04-29T06:14:18")]
[System.Xml.Serialization.XmlRootAttribute(Namespace="http://schemas.microsoft.com/office/infopath/2003/myXSD/2011-04-29T06:14:18", IsNullable=false)]
public partial class Expenses {
    
    private Expense[] expenseField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("Expense")]
    public Expense[] Expense {
        get {
            return this.expenseField;
        }
        set {
            this.expenseField = value;
        }
    }
}
