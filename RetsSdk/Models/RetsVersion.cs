namespace CrestApps.RetsSdk.Models
{
    using System;
    using CrestApps.RetsSdk.Helpers;
    using CrestApps.RetsSdk.Models.Enums;

    public class RetsVersion : Version
    {
        private SupportedRetsVersion RetVersion;

        public RetsVersion(SupportedRetsVersion retsVersion)
        {
            this.RetVersion = retsVersion;
            string version = ExtractVersionNumber(retsVersion);

            this.Load(version);
        }

        public string AsHeader()
        {
            return $"RETS/{this.ToString()}";
        }

        public override string ToString()
        {
            string version = string.Empty;

            if (!this.Major.HasValue)
            {
                throw new NullReferenceException($"The {this.Major} value cannot be null.");
            }

            if (!this.Minor.HasValue)
            {
                throw new NullReferenceException($"The {this.Major} value cannot be null.");
            }

            if (!this.Patch.HasValue)
            {
                return $"{this.Major}.{this.Minor}";
            }

            return base.ToString();
        }

        private static string ExtractVersionNumber(SupportedRetsVersion retsVersion)
        {
            return Str.TrimStart(retsVersion.ToString(), "Version_").Replace('_', '.');
        }

        public static SupportedRetsVersion Make(string version)
        {
            var v = Str.TrimStart(version, "RETS/").Replace('.', '_');

            var castable = Str.PrependOnce(v, "Version_");

            return Enum.Parse<SupportedRetsVersion>(castable);
        }
    }
}
