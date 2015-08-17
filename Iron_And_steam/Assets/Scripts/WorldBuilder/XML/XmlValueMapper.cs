using System;
using System.Collections.Generic;
using System.Xml.Linq;
using IaS.Domain;
using IaS.Helpers;
using IaS.WorldBuilder.XML.mappers;
using UnityEngine;

namespace IaS.WorldBuilder.XML
{
    public class XmlValueMapper
    {
        public static XmlValueMappedResult<string> FromAttribute(XElement element, string attributeName)
        {
            string elementName = element.Name.LocalName;
            XAttribute attribute = element.Attribute(attributeName);
            return attribute != null
                ? new XmlValueMappedResult<string>(attribute.Value, attributeName, elementName, new SimpleMapper(), true)
                : new XmlValueMappedResult<string>("", attributeName, elementName, null, false);
        }

        public static XmlValueMappedResult<string> FromElementValue(XElement parent, string elementName)
        {
            XElement element = parent.Element(elementName);
            return element != null
                ? new XmlValueMappedResult<string>(element.Value, "value", elementName, new SimpleMapper(), true)
                : new XmlValueMappedResult<string>("", "value", elementName, new SimpleMapper(), false);
        }

        public struct XmlValueMappedResult<T>
        {
            private readonly string _value;
            private readonly bool _present;
            private readonly string _attribName;
            private readonly string _elementName;

            private readonly IXmlValueMapper<T> _mapper;

            internal XmlValueMappedResult(string value, string attribName, string elementName, IXmlValueMapper<T> mapper,
                bool present)
            {
                _mapper = mapper;
                _value = value;
                _present = present;
                _attribName = attribName;
                _elementName = elementName;
            }


            public XmlValueMappedResult<string> AsText()
            {
                return new XmlValueMappedResult<string>(_value, _attribName, _elementName, new SimpleMapper(), _present);
            }

            public XmlValueMappedResult<float> AsFloat()
            {
                return new XmlValueMappedResult<float>(_value, _attribName, _elementName, new FloatMapper(), _present);
            }

            public XmlValueMappedResult<Vector3> AsVector3()
            {
                return new XmlValueMappedResult<Vector3>(_value, _attribName, _elementName, new Vector3Mapper(),
                    _present);
            }

            public XmlValueMappedResult<Vector3> AsDirection()
            {
                return new XmlValueMappedResult<Vector3>(_value, _attribName, _elementName, new DirectionMapper(),
                    _present);
            }

            public XmlValueMappedResult<Vector3> AsAxis()
            {
                return new XmlValueMappedResult<Vector3>(_value, _attribName, _elementName, new AxisMapper(), _present);
            }

            public XmlValueMappedResult<TE> AsEnum<TE>() where TE : struct
            {
                return new XmlValueMappedResult<TE>(_value, _attribName, _elementName, new EnumMapper<TE>(), _present);
            }

            public XmlValueMappedResult<TR> AsReference<TR>(IEnumerable<TR> references) where TR : IXmlReferenceable
            {
                return new XmlValueMappedResult<TR>(_value, _attribName, _elementName, new ReferenceMapper<TR>(references), _present);
            }

            public XmlValueMappedResult<SplitAttachment> AsSplitAttachment(Split[] splits)
            {
                return new XmlValueMappedResult<SplitAttachment>(_value, _attribName, _elementName, new SplitAttachMapper(splits), _present);
            }

            public XmlValueMappedResult<string> AsMultiChoice(params string[] choices)
            {
                return new XmlValueMappedResult<string>(_value, _attribName, _elementName,
                    new MultiChoiceMapper(choices), _present);
            }

            public string AsIdValue(string type, Dictionary<string, int> counts)
            {
                return IdMapper.Map(_value, type, counts, _present);
            }



            public T OptionalValue(T defaultValue)
            {
                try
                {
                    return _present ? _mapper.Map(_value) : defaultValue;
                }
                catch (Exception e)
                {
                    throw NewMappingException(e);
                }
            }

            public T MandatoryValue()
            {
                if (!_present)
                {
                    throw new Exception(string.Format("Required attribute '{0}' from element '{1}' could not be found",
                        _attribName, _elementName));
                }
                try
                {
                    return _mapper.Map(_value);
                }
                catch (Exception e)
                {
                    throw NewMappingException(e);
                }
            }

            public TS? OptionalStruct<TS>() where TS : struct
            {
                if (!_present)
                    return default(TS);

                try
                {
                    return _mapper.Map(_value) as TS?;
                }
                catch (Exception e)
                {
                    throw NewMappingException(e);
                }
            }

            private Exception NewMappingException(Exception e)
            {
                return
                    new Exception(
                        string.Format("Could not map attribute {0} from element {1}.", _attribName, _elementName), e);
            }
        }
    }
}