using System.Collections.Generic;
using System.Linq;
using IaS.Xml;

namespace IaS.Domain.XmlToDomainMapper
{
    public class JunctionMapper
    {
        public Junction MapXmlToDomain(XmlJunction xmlJunction, IList<SplitTrack> subTracks)
        {
            SplitTrack defaultTrack = subTracks.First(subTrack => xmlJunction.BranchDefaultId.Equals(subTrack.Id));
            SplitTrack altTrack = subTracks.First(subTrack => xmlJunction.BranchAlternateId.Equals(subTrack.Id));

            return new Junction(xmlJunction.Id, defaultTrack, altTrack, xmlJunction.Direction);
        }
    }
}
