namespace IaS.WorldBuilder.XML
{
    public struct XmlParserPosition
    {
        public string ElementPath { get; private set; }
        public string AttributeName { get; set; }

        public XmlParserPosition(string rootXpath) : this()
        {
            ElementPath = rootXpath;
            AttributeName = "";
        }

        public void SetCurrentElement(string elem)
        {
            ElementPath += string.Format("/{0}", elem);
        }

    }
}
