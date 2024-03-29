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
    [XmlTypeAttribute("relation", Namespace="", AnonymousType=true)]
    [DebuggerStepThroughAttribute()]
    [DesignerCategoryAttribute("code")]
    [XmlRootAttribute("relation", Namespace="")]
    public partial class Relation : IAttlistRelation
    {
        
        [XmlIgnoreAttribute()]
        private List<Infon> _infon;
        
        [XmlElementAttribute("infon")]
        public List<Infon> Infon
        {
            get
            {
                return _infon;
            }
            init
            {
                _infon = value;
            }
        }
        
        /// <summary>
        /// <para xml:lang="en">Gets a value indicating whether the Infon collection is empty.</para>
        /// </summary>
        [XmlIgnoreAttribute()]
        public bool InfonSpecified
        {
            get
            {
                return ((this.Infon != null) 
                            && (this.Infon.Count != 0));
            }
        }
        
        /// <summary>
        /// <para xml:lang="en">Initializes a new instance of the <see cref="Relation" /> class.</para>
        /// </summary>
        public Relation()
        {
            this._infon = new List<Infon>();
            this._node = new List<Node>();
        }
        
        [XmlIgnoreAttribute()]
        private List<Node> _node;
        
        [XmlElementAttribute("node")]
        public List<Node> Node
        {
            get
            {
                return _node;
            }
            init
            {
                _node = value;
            }
        }
        
        /// <summary>
        /// <para xml:lang="en">Gets a value indicating whether the Node collection is empty.</para>
        /// </summary>
        [XmlIgnoreAttribute()]
        public bool NodeSpecified
        {
            get
            {
                return ((this.Node != null) 
                            && (this.Node.Count != 0));
            }
        }
        
        [AllowNullAttribute()]
        [MaybeNullAttribute()]
        [XmlAttributeAttribute("id")]
        public string Id { get; set; }
    }
}
