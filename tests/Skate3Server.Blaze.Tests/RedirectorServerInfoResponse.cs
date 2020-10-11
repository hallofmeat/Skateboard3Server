using Skate3Server.Blaze.Serializer.Attributes;

namespace Skate3Server.Blaze.Responses
{
    public class DummyResponse
    {
        [TdfField("TEST")]
        public DummyStruct SomeStruct { get; set; }

    }

    public class DummyStruct
    {
        [TdfField("DUMB")]
        public string Dumb { get; set; }
    }
}
