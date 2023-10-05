using System;
using System.Collections.Generic;

namespace TibiantisLauncher.Profiles
{
    [Serializable]
    public class ProfilesData
    {
        public List<Profile> Profiles { get; set; } = new List<Profile>();
    }
}