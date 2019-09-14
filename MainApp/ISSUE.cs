using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace MainApp
{
    public class LA_LANGUAGE
    {
        [XmlAttribute(AttributeName = "seq")]
        public string Seq { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    public class KEYWORD
    {
        [XmlElement]
        public string AUTHOR_KEYWORD { get; set; }
        [XmlAttribute(AttributeName = "seq")]
        public string Seq { get; set; }
    }

    public class TITLES
    {
        [XmlElement]
        public string TI_TITLE { get; set; }
    }
        
    public class ABSTRACT
    {
        [XmlAttribute(AttributeName = "seq")]
        public string Seq { get; set; }
        [XmlText]
        public string Text { get; set; }
    }
    
    public class ITEM_CONTENT
    {
        [XmlElement]
        public string DT_DOCUMENTTYPE { get; set; }
        [XmlElement]
        public string PG_PAGESPAN { get; set; }
        [XmlElement]
        public LA_LANGUAGE LA_LANGUAGE { get; set; }
        [XmlElement]
        public List<KEYWORD> KEYWORD { get; set; }
        [XmlElement]
        public TITLES TITLES { get; set; }
        [XmlElement]
        public ABSTRACT ABSTRACT { get; set; }
        [XmlElement]
        public string BIOSIS_DATA { get; set; }
    }
    
    public class ITEM
    {
        [XmlElement]
        public ITEM_CONTENT ITEM_CONTENT { get; set; }
        [XmlAttribute]
        public string ITEMNO { get; set; }
    }

    [XmlRoot(ElementName = "ISSUE", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
    public class ISSUE
    {
        [XmlElement]
        public string ID_ACCESSION { get; set; }
        [XmlElement]
        public string JS_JOURNALSEQ { get; set; }
        [XmlElement]
        public string YR_PUBLYEAR { get; set; }
        [XmlElement]
        public ITEM ITEM { get; set; }
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
        [XmlAttribute]
        public string Ns0 { get; set; }
    }

}
