//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// This code was generated by XmlSchemaClassGenerator version 2.1.1092.0 using the following command:
// xscgen --netCore --nullable --namespace=BioCXml --collectionType=System.Collections.Generic.List`1 --collectionSettersMode=Init --complexTypesForCollections=False --compactTypeNames --nullableReferenceAttributes --separateFiles --interface=False --codeTypeReferenceOptions=GenericTypeParameter -o .. -v BioC.xsd
namespace BioCXml
{
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Xml;
    using System.Xml.Schema;
    using System.Xml.Serialization;
    
    
    [GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.1.1092.0")]
    [SerializableAttribute()]
    [XmlTypeAttribute("node", Namespace="", AnonymousType=true)]
    [DebuggerStepThroughAttribute()]
    [DesignerCategoryAttribute("code")]
    [XmlRootAttribute("node", Namespace="")]
    public partial class Node : IAttlistNode
    {
        
        [RequiredAttribute()]
        [XmlAttributeAttribute("refid")]
        public string Refid { get; set; }
        
        [XmlIgnoreAttribute()]
        private string _role = "";
        
        [DefaultValueAttribute("")]
        [XmlAttributeAttribute("role")]
        public string Role
        {
            get
            {
                return _role;
            }
            set
            {
                _role = value;
            }
        }
    }
}
