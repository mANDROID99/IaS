using System.Collections.Generic;
using System.Linq;
using IaS.Xml;

namespace IaS.Domain.XmlToDomainMapper
{
    public class LevelMapper
    {
        private readonly GroupMapper groupMapper = new GroupMapper();

        public Level MapXmlToDomain(XmlLevel xmlLevel)
        {
            List<SplitAttachment> splitAttachments = new List<SplitAttachment>();

            Group[] groups = xmlLevel.Groups.Select(
                xmlGroup => groupMapper.MapXmlToDomain(xmlGroup, splitAttachments)).ToArray();

            SplitTrack startTrack = GetTrackRef(groups, xmlLevel.StartTrackId);
            SplitTrack endTrack = GetTrackRef(groups, xmlLevel.EndTrackId);

            ResolveSplitAttachments(splitAttachments, groups);

            Level level = new Level(xmlLevel.LevelId, endTrack, startTrack);
            level.Groups.AddRange(groups);
            return level;
        }



        private void ResolveSplitAttachments(List<SplitAttachment> splitAttachments, Group[] groups)
        {
            foreach (SplitAttachment attachment in splitAttachments)
            {
                Split split = attachment.Split.Resolve(groups.SelectMany(g => g.Splits));
                split.AttachedGroups.Add(attachment);
            }
        }

        private SplitTrack GetTrackRef(Group[] groups, string reference)
        {
            return groups.SelectMany(g => g.Tracks).First(track => ("@" + track.Id).Equals(reference));
        }
    }
}
