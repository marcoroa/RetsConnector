namespace CrestApps.RetsSdk.Models
{
    public class PhotoId
    {
        public PhotoId()
        {
        }

        public PhotoId(string id, int? objectId = null)
        {
            this.Id = id;
            this.ObjectId = objectId;
        }

        public string Id { get; set; }
        public int? ObjectId { get; set; }

        public override string ToString()
        {
            if (!this.ObjectId.HasValue)
            {
                return string.Format("{0}:*", this.Id);
            }

            return string.Format("{0}:{1}", this.Id, this.ObjectId.Value);
        }
    }
}
