using System.Collections.Generic;
using DataTransferObjects.TimeEntry;

namespace PraticeManagement.Controls.Generic.Buttons
{
    public class ReviewStatusImageButton : MultistateImageButton<ReviewStatus>
    {
        private static readonly Dictionary<ReviewStatus, KeyValuePair<string, string>> Filenames;

        static ReviewStatusImageButton()
        {
            Filenames = new Dictionary<ReviewStatus, KeyValuePair<string, string>>
                            {
                                {ReviewStatus.Pending, new KeyValuePair<string, string>(
                                                        "~\\Images\\pending_16.png", "Pending")},
                                {ReviewStatus.Declined, new KeyValuePair<string, string>(
                                                        "~\\Images\\declined_16.png", "Declined")},
                                {ReviewStatus.Approved, new KeyValuePair<string, string>(
                                                        "~\\Images\\accepted_16.png", "Approved")}
                            };
        }

        #region Overrides of MultistateImageButton<ReviewStatus>

        protected override Dictionary<ReviewStatus, KeyValuePair<string, string>> FilenameMapping
        {
            get { return Filenames; }
        }

        public override string EntityName
        {
            get { return Constants.EntityNames.ReviewStatusEntity; }
        }

        #endregion
    }
}



