namespace IaS.Domain.Tracks
{
    public class TrackBuilderConfiguration
    {
        private const float DEFAULT_CURVE_OFFSET = 0f;

        public float curveOffset { get; set; }


        public static TrackBuilderConfiguration DefaultConfig
        {
            get { return DefaultConfigWithCurveOffset(DEFAULT_CURVE_OFFSET); }
        }

        public static TrackBuilderConfiguration DefaultConfigWithCurveOffset(float curveOffset)
        {
            return new TrackBuilderConfiguration { curveOffset = curveOffset };
        }

    }
}
