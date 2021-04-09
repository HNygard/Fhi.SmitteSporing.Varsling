﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by xsd, Version=4.8.3928.0.
// 
namespace Fhi.Smittesporing.Helsenorge.Api.Models {
    using System.Xml.Serialization;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.ehelse.no/helsenorge/innsyn/2016-12-02")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="http://www.ehelse.no/helsenorge/innsyn/2016-12-02", IsNullable=false)]
    public partial class Innsyn {
        
        private Section[] sectionField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Section")]
        public Section[] Section {
            get {
                return this.sectionField;
            }
            set {
                this.sectionField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.ehelse.no/helsenorge/innsyn/2016-12-02")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="http://www.ehelse.no/helsenorge/innsyn/2016-12-02", IsNullable=false)]
    public partial class Section {
        
        private object[] itemsField;
        
        private string titleField;
        
        private bool collapsedField;
        
        public Section() {
            this.collapsedField = false;
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("FormattedText", typeof(FormattedText))]
        [System.Xml.Serialization.XmlElementAttribute("Image", typeof(Image))]
        [System.Xml.Serialization.XmlElementAttribute("NameValue", typeof(NameValue))]
        [System.Xml.Serialization.XmlElementAttribute("OrderedList", typeof(OrderedList))]
        [System.Xml.Serialization.XmlElementAttribute("Section", typeof(Section))]
        [System.Xml.Serialization.XmlElementAttribute("Table", typeof(Table))]
        [System.Xml.Serialization.XmlElementAttribute("Text", typeof(Text))]
        [System.Xml.Serialization.XmlElementAttribute("UnorderedList", typeof(UnorderedList))]
        [System.Xml.Serialization.XmlElementAttribute("XmlContent", typeof(XmlContent))]
        public object[] Items {
            get {
                return this.itemsField;
            }
            set {
                this.itemsField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string title {
            get {
                return this.titleField;
            }
            set {
                this.titleField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool collapsed {
            get {
                return this.collapsedField;
            }
            set {
                this.collapsedField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.ehelse.no/helsenorge/innsyn/2016-12-02")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="http://www.ehelse.no/helsenorge/innsyn/2016-12-02", IsNullable=false)]
    public partial class FormattedText {
        
        private string itemField;
        
        private ItemChoiceType itemElementNameField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Lead", typeof(string))]
        [System.Xml.Serialization.XmlElementAttribute("Subtitle", typeof(string))]
        [System.Xml.Serialization.XmlElementAttribute("Title", typeof(string))]
        [System.Xml.Serialization.XmlChoiceIdentifierAttribute("ItemElementName")]
        public string Item {
            get {
                return this.itemField;
            }
            set {
                this.itemField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public ItemChoiceType ItemElementName {
            get {
                return this.itemElementNameField;
            }
            set {
                this.itemElementNameField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.ehelse.no/helsenorge/innsyn/2016-12-02", IncludeInSchema=false)]
    public enum ItemChoiceType {
        
        /// <remarks/>
        Lead,
        
        /// <remarks/>
        Subtitle,
        
        /// <remarks/>
        Title,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.ehelse.no/helsenorge/innsyn/2016-12-02")]
    public abstract partial class InnsynList {
        
        private Text[] listItemField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("Text", typeof(Text), IsNullable=false)]
        public Text[] ListItem {
            get {
                return this.listItemField;
            }
            set {
                this.listItemField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.ehelse.no/helsenorge/innsyn/2016-12-02")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="http://www.ehelse.no/helsenorge/innsyn/2016-12-02", IsNullable=false)]
    public partial class Text {
        
        private bool paragraphField;
        
        private bool italicField;
        
        private bool emphasizedField;
        
        private bool asideField;
        
        private bool breakField;
        
        private string[] text1Field;
        
        public Text() {
            this.paragraphField = false;
            this.italicField = false;
            this.emphasizedField = false;
            this.asideField = false;
            this.breakField = false;
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool paragraph {
            get {
                return this.paragraphField;
            }
            set {
                this.paragraphField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool italic {
            get {
                return this.italicField;
            }
            set {
                this.italicField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool emphasized {
            get {
                return this.emphasizedField;
            }
            set {
                this.emphasizedField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool aside {
            get {
                return this.asideField;
            }
            set {
                this.asideField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool @break {
            get {
                return this.breakField;
            }
            set {
                this.breakField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string[] Text1 {
            get {
                return this.text1Field;
            }
            set {
                this.text1Field = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.ehelse.no/helsenorge/innsyn/2016-12-02")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="http://www.ehelse.no/helsenorge/innsyn/2016-12-02", IsNullable=false)]
    public partial class Image {
        
        private string titleField;
        
        private byte[] imageField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string title {
            get {
                return this.titleField;
            }
            set {
                this.titleField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType="base64Binary")]
        public byte[] image {
            get {
                return this.imageField;
            }
            set {
                this.imageField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.ehelse.no/helsenorge/innsyn/2016-12-02")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="http://www.ehelse.no/helsenorge/innsyn/2016-12-02", IsNullable=false)]
    public partial class NameValue {
        
        private object[] itemsField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("EmptyText", typeof(string))]
        [System.Xml.Serialization.XmlElementAttribute("Pair", typeof(NameValuePair))]
        public object[] Items {
            get {
                return this.itemsField;
            }
            set {
                this.itemsField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.ehelse.no/helsenorge/innsyn/2016-12-02")]
    public partial class NameValuePair {
        
        private string nameField;
        
        private string valueField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name {
            get {
                return this.nameField;
            }
            set {
                this.nameField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string value {
            get {
                return this.valueField;
            }
            set {
                this.valueField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.ehelse.no/helsenorge/innsyn/2016-12-02")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="http://www.ehelse.no/helsenorge/innsyn/2016-12-02", IsNullable=false)]
    public partial class OrderedList : InnsynList {
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.ehelse.no/helsenorge/innsyn/2016-12-02")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="http://www.ehelse.no/helsenorge/innsyn/2016-12-02", IsNullable=false)]
    public partial class Table {
        
        private string[] headerField;
        
        private string[] rowField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("Cell", IsNullable=false)]
        public string[] Header {
            get {
                return this.headerField;
            }
            set {
                this.headerField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("Cell", typeof(string), IsNullable=false)]
        public string[] Row {
            get {
                return this.rowField;
            }
            set {
                this.rowField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.ehelse.no/helsenorge/innsyn/2016-12-02")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="http://www.ehelse.no/helsenorge/innsyn/2016-12-02", IsNullable=false)]
    public partial class UnorderedList : InnsynList {
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.ehelse.no/helsenorge/innsyn/2016-12-02")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="http://www.ehelse.no/helsenorge/innsyn/2016-12-02", IsNullable=false)]
    public partial class XmlContent {
        
        private object xmlContent1Field;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("XmlContent")]
        public object XmlContent1 {
            get {
                return this.xmlContent1Field;
            }
            set {
                this.xmlContent1Field = value;
            }
        }
    }
}
