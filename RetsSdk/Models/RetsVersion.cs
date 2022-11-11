namespace CrestApps.RetsSdk.Models
{
    using System;
    using CrestApps.RetsSdk.Helpers;
    using CrestApps.RetsSdk.Models.Enums;

    public class RetsVersion : Version
    {
        private readonly SupportedRetsVersion retVersion;

        public RetsVersion(SupportedRetsVersion retsVersion)
        {
            this.retVersion = retsVersion;
            string version = ExtractVersionNumber(this.retVersion);

            this.Load(version);
        }

        public static SupportedRetsVersion Make(string version)
        {
            var supportedVersion = Str.TrimStart(version, "RETS/").Replace('.', '_');

            var castable = Str.PrependOnce(supportedVersion, "Version_");

            return Enum.Parse<SupportedRetsVersion>(castable);
        }

        public string AsHeader()
        {
            return $"RETS/{this}";
        }

        public override string ToString()
        {
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
    }
}
