using System.Collections.Generic;

namespace PraticeManagement.Controls.Generic.Buttons
{
    public class IsChargeableImageButton : MultistateImageButton<bool>
    {
        private static readonly Dictionary<bool, KeyValuePair<string, string>> Filenames;

        static IsChargeableImageButton()
        {
            Filenames = new Dictionary<bool, KeyValuePair<string, string>>
                            {
                                {true, new KeyValuePair<string, string>(
                                    "~\\Images\\yes.png", 
                                    Resources.Controls.ChargeableTooltip)},
                                {false, new KeyValuePair<string, string>(
                                    "~\\Images\\no.png", 
                                    Resources.Controls.NotChargeableTooltip)}
                            };
        }

        #region Overrides of MultistateImageButton<bool>

        protected override Dictionary<bool, KeyValuePair<string, string>> FilenameMapping
        {
            get { return Filenames; }
        }

        public override string EntityName
        {
            get { return Constants.EntityNames.IsChargeableEntity; }
        }

        #endregion
    }
}
