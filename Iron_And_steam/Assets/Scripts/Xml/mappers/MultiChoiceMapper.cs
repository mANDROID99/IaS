using System;
using System.Linq;

namespace IaS.Domain.XML.mappers
{
    public class MultiChoiceMapper : IXmlValueMapper<string>
    {
        private readonly string[] _choices;

        public MultiChoiceMapper(string[] choices)
        {
            _choices = choices;
        }

        public string Map(string from)
        {
            if (!_choices.Contains(from))
                throw new Exception(string.Format("Value '{0}'. Option not found in multi-choice.", from));
            return from;
        }
    }
}
