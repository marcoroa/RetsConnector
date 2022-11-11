namespace CrestApps.RetsSdk.Models
{
    using System;
    using CrestApps.RetsSdk.Exceptions;

    public class Version
    {
        public Version()
        {
        }

        public Version(int? major)
        {
            this.Major = major;
        }

        public Version(int? major, int? minor)
            : this(major)
        {
            this.Minor = minor;
        }

        public Version(int? major, int? minor, int? patch)
            : this(major, minor)
        {
            this.Patch = patch;
        }

        public Version(string version, char wildCard = '*')
        {
            this.WildCard = wildCard;
            this.Load(version);
        }

        public int? Major { get; set; }
        public int? Minor { get; set; }
        public int? Patch { get; set; }
        public char WildCard { get; set; } = '*';

        public void Load(string version)
        {
            string[] parts = this.GetVersionParts(version);
            int totalParts = parts.Length;

            this.Major = this.ParseValue(parts[0]);
            this.Minor = this.ParseValue(parts[1]);

            if (totalParts == 3)
            {
                this.Patch = this.ParseValue(parts[2]);
            }
        }

        public override string ToString()
        {
            return $"{this.Major ?? this.WildCard}.{this.Minor ?? this.WildCard}.{this.Patch ?? this.WildCard}";
        }

        protected virtual string[] GetVersionParts(string version)
        {
            if (string.IsNullOrWhiteSpace(version))
            {
                throw new ArgumentNullException(nameof(version), $"'{nameof(version)}' cannot be null or empty.");
            }

            string[] parts = version.Replace('_', '.').Split('.');
            int totalParts = parts.Length;
            if (totalParts == 0 || totalParts > 3)
            {
                throw new VersionParsingException();
            }

            return parts;
        }

        protected virtual int? ParseValue(string part)
        {
            if (string.IsNullOrWhiteSpace(part) || part.Equals(this.WildCard.ToString()))
            {
                return default;
            }

            if (!int.TryParse(part, out int value))
            {
                throw new VersionParsingException();
            }

            return value;
        }
    }
}
