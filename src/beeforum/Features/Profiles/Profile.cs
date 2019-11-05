using Newtonsoft.Json;

namespace beeforum.Features.Profiles
{
    public class Profile
    {
        public string Username { get; set; }

        public string Bio { get; set; }

        public string Image { get; set; }

        [JsonProperty("following")]
        public bool IsFollowed { get; set; }
    }

    public class ProfileEnvelope
    {
        public ProfileEnvelope(Profile profile)
        {
            Profile = profile;
        }

        public Profile Profile { get; set; }
    }
}