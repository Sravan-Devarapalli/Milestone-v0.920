using System.Collections.Generic;

namespace PraticeManagement.Controls.Generic.Buttons
{
    public class IsCorrectImageButton : MultistateImageButton<bool>
    {
        private static readonly Dictionary<bool, KeyValuePair<string, string>> Filenames;

        static IsCorrectImageButton()
        {
            Filenames = new Dictionary<bool, KeyValuePair<string, string>>
                            {
                                {true, new KeyValuePair<string, string>(
                                            "~\\Images\\add_16.png", "Correct")},
                                {false, new KeyValuePair<string, string>(
                                            "~\\Images\\warning_16.png", "Incorrect")}
                            };
        }

        #region Overrides of MultistateImageButton<bool>

        protected override Dictionary<bool, KeyValuePair<string, string>> FilenameMapping
        {
            get { return Filenames; }
        }

        public override string EntityName
        {
            get { return Constants.EntityNames.IsCorrectEntity; }
        }

        #endregion
    }
}
