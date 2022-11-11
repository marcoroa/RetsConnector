namespace CrestApps.RetsSdk.Models
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Xml.Linq;

    [Description("METADATA-CLASS")]
    public class RetsClassCollection : RetsCollection<RetsClass>
    {
        public string Resource { get; set; }

        public override RetsClass Get(object value)
        {
            RetsClass item = this.Get().FirstOrDefault(x => x.ClassName.Equals(value.ToString(), StringComparison.CurrentCultureIgnoreCase));

            return item;
        }

        public override void Load(XElement xElement)
        {
            this.Load(this.GetType(), xElement);
        }
    }
}
